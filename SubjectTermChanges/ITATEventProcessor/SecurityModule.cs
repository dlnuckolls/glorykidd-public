using System;
using System.Collections.Generic;
using System.Data;
using Kindred.Common.Logging;
using System.Diagnostics;

namespace Kindred.Knect.ITAT
{
    class SecurityModule
    {
        public List<string> Process(string[] systemNames)
        {
            List<string> sErrors = new List<string>();
            int allTemplateDraftCount = 0;
            int allTemplateFinalCount = 0;
            int allManagedItemCount = 0;
            using (DataSet dsSystem = Business.ITATSystem.GetSystemList())
            {
                ILog log = Kindred.Common.Logging.LogManager.GetLogger(this.GetType());
                int systemCount = 0;
                Stopwatch stopWatchBatch = new Stopwatch();
                stopWatchBatch.Start();
                if (systemNames == null)
                {
                    int templateDraftCount = 0;
                    int templateFinalCount = 0;
                    int managedItemCount = 0;

                    foreach (DataRow drSystem in dsSystem.Tables[0].Rows)
                    {
                        string systemName = (string)drSystem[Data.DataNames._C_ITATSystemName];
                        systemCount++;
                        log.Info(string.Format("Processing System {0} ...", systemName));
                        Stopwatch stopWatchSystem = new Stopwatch();
                        stopWatchSystem.Start();
                        ProcessSystem((Guid)drSystem[Data.DataNames._C_ITATSystemID], systemName, ref templateDraftCount, ref templateFinalCount, ref managedItemCount);
                        stopWatchSystem.Stop();
                        LogMetrics(log, string.Format("System {0} required an elapsed time of ", systemName), stopWatchSystem.Elapsed);
                    }
                    allTemplateDraftCount += templateDraftCount;
                    allTemplateFinalCount += templateFinalCount;
                    allManagedItemCount += managedItemCount;
                }
                else
                {
                    foreach (string systemName in systemNames)
                    {
                        int templateDraftCount = 0;
                        int templateFinalCount = 0;
                        int managedItemCount = 0;

                        foreach (DataRow drSystem in dsSystem.Tables[0].Rows)
                        {
                            if ((string)drSystem[Data.DataNames._C_ITATSystemName] == systemName.Trim())
                            {
                                systemCount++;
                                log.Info(string.Format("Processing System {0} ...", systemName));
                                Stopwatch stopWatchSystem = new Stopwatch();
                                stopWatchSystem.Start();
                                ProcessSystem((Guid)drSystem[Data.DataNames._C_ITATSystemID], systemName, ref templateDraftCount, ref templateFinalCount, ref managedItemCount);
                                stopWatchSystem.Stop();
                                LogMetrics(log, string.Format("System {0} required an elapsed time of ", systemName), stopWatchSystem.Elapsed);
                                break;
                            }
                        }
                        allTemplateDraftCount += templateDraftCount;
                        allTemplateFinalCount += templateFinalCount;
                        allManagedItemCount += managedItemCount;
                    }
                }

                stopWatchBatch.Stop();
                int sumCount = allTemplateDraftCount + allTemplateFinalCount + allManagedItemCount;
                log.Info(string.Format("Batch summary - Processed {0:D} Systems, {1:D} Draft Templates, {2:D} Final Templates, {3:D} Managed Items (Total = {4:D})",
                    systemCount, allTemplateDraftCount, allTemplateFinalCount, allManagedItemCount, sumCount));
                log.Info(string.Format("Batch summary - Elapsed Time = {0}  Average Time = {1} seconds", stopWatchBatch.Elapsed, stopWatchBatch.Elapsed.TotalSeconds / sumCount));
            }
            return sErrors;
        }

        public List<string> ProcessSystem(Guid gSystemID, string systemName, ref int batchTemplateDraftCount, ref int batchTemplateFinalCount, ref int batchManagedItemCount)
        {
            batchTemplateDraftCount = 0;
            batchTemplateFinalCount = 0;
            batchManagedItemCount = 0;
            Stopwatch stopWatchTemplate = new Stopwatch();
            Stopwatch stopWatchManagedItem = new Stopwatch();
            ILog log = Kindred.Common.Logging.LogManager.GetLogger(this.GetType());
            List<string> badDraftTemplateDefGuids = new List<string>();
            using (DataSet dsTemplates = Business.Template.GetTemplateList(gSystemID, null))
            {
                log.Info(string.Format("Processing {0:D} Templates", dsTemplates.Tables[0].Rows.Count));
                foreach (DataRow drTemplate in dsTemplates.Tables[0].Rows)
                {
                    Guid templateID = (Guid)drTemplate[Data.DataNames._C_TemplateID];

                    if (Business.Template.DraftTemplateDefValid(templateID))
                    {
                        batchTemplateDraftCount++;
                        string templateName = "UNDEFINED";
                        stopWatchTemplate.Start();
                        try
                        {
                            Business.Template template = new Business.Template(templateID, Business.DefType.Draft);
                            //Ensure that the SecurityModel is loaded.
                            Business.SecurityModel securityModel = template.SecurityModel;
                            //Ensure that the Terms, ComplexLists, Workflows are updated.
                            int nBasicTermsCount = template.BasicTerms.Count;
                            int nComplexListsCount = template.ComplexLists.Count;
                            int nWorkflowsCount = template.Workflows.Count;
                            templateName = template.Name;
                            template.SaveLoaded(false, true, null);
                            log.Info(string.Format("Processed Template '{0}'", templateName));
                        }
                        catch (Exception ex)
                        {
                            log.Error(string.Format("Failed to process DraftTemplateDef for System '{0}', Template '{1}' - Error Message = '{2}'", systemName, templateName, ex.Message), ex);
                        }
                        stopWatchTemplate.Stop();

                        if (Business.Template.FinalTemplateDefValid(templateID))
                        {
                            batchTemplateFinalCount++;
                            stopWatchTemplate.Start();
                            try
                            {
                                Business.Template template = new Business.Template(templateID, Business.DefType.Final);
                                //Ensure that the Terms, ComplexLists, Workflows are updated.
                                int nBasicTermsCount = template.BasicTerms.Count;
                                int nComplexListsCount = template.ComplexLists.Count;
                                int nWorkflowsCount = template.Workflows.Count;
                                template.SaveLoaded(false, true, null);
                                Data.Template.UpdateTemplateBaseStateRole(template.ID, Utility.ListHelper.EliminateDuplicates(template.Workflow.FindBaseState().EditorRoleNames));
                            }
                            catch (Exception ex)
                            {
                                log.Error(string.Format("Failed to process FinalTemplateDef for System '{0}', Template '{1}' - Error Message = '{2}'", systemName, templateName, ex.Message), ex);
                            }
                            stopWatchTemplate.Stop();

                            List<Guid> managedItemIDs = GetManagedItems(gSystemID, templateID);
                            if (managedItemIDs.Count > 0)
                            {
                                log.Info(string.Format("Processing {0:D} ManagedItems", managedItemIDs.Count));
                                int templateManagedItemCount = 0;
                                int nLog = 200;
                                foreach (Guid managedItemID in managedItemIDs)
                                {
                                    batchManagedItemCount++;
                                    templateManagedItemCount++;

                                    stopWatchManagedItem.Start();
                                    try
                                    {
                                        Business.ManagedItem managedItem = Business.ManagedItem.Get(managedItemID, true);
                                        //Ensure that the Terms, ComplexLists, Workflows are updated.
                                        int nBasicTermsCount = managedItem.BasicTerms.Count;
                                        int nComplexListsCount = managedItem.ComplexLists.Count;
                                        int nWorkflowsCount = managedItem.Workflows.Count;
										managedItem.Update(false, Kindred.Knect.ITAT.Business.Retro.AuditType.Saved);   //Update also calls UpdateManagedItemStateRole
										managedItem.UpdateScheduledEvents(Business.ScheduledEvent.ExecutedCalculationType.UseGracePeriod);
                                        if (templateManagedItemCount % nLog == 0)
                                            log.Info(string.Format("Processed {0:D} ManagedItems", templateManagedItemCount));
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(string.Format("Failed to process ManagedItem '{0}' for System '{1}', Template '{2}' - Error Message = '{3}'", managedItemID.ToString(), systemName, templateName, ex.Message), ex);
                                    }
                                    stopWatchManagedItem.Stop();
                                }
                                log.Info(string.Format("Processed a total of {0:D} ManagedItems", templateManagedItemCount));
                            }
                        }
                        else
                            log.Debug(string.Format("FinalTemplateDef invalid for System '{0}', Template '{1}'", systemName, templateName));
                    }
                    else
                    {
                        badDraftTemplateDefGuids.Add(templateID.ToString());
                        if (Business.Template.FinalTemplateDefValid(templateID))
                        {
                            log.Error(string.Format("For System '{0}', TemplateID '{1}' - DraftTemplateDef invalid BUT FinalTemplateDef valid!", systemName, templateID.ToString()));
                        }
                    }
                }
            }
            if (badDraftTemplateDefGuids.Count > 0)
                log.Error(string.Format("DraftTemplateDef invalid for the following {0:D} templates: '{1}'", badDraftTemplateDefGuids.Count, string.Join(",", badDraftTemplateDefGuids.ToArray())));
            log.Debug(string.Format("System {0} - Total Template elapsed time = {1};  Average Template elapsed time = {2} sec", systemName, stopWatchTemplate.Elapsed, stopWatchTemplate.Elapsed.TotalSeconds / (batchTemplateDraftCount + batchTemplateFinalCount)));
            if (batchManagedItemCount > 0)
                log.Debug(string.Format("System {0} - Total ManagedItem elapsed time = {1};  Average ManagedItem elapsed time = {2} sec", systemName, stopWatchManagedItem.Elapsed, stopWatchManagedItem.Elapsed.TotalSeconds / batchManagedItemCount));
            else
                log.Debug(string.Format("System {0} - Total ManagedItem elapsed time = 0;  Average ManagedItem elapsed time = 0 sec", systemName));
            return null;
        }

        private List<Guid> GetManagedItems(Guid gSystemID, Guid gTemplateID)
        {
            List<Guid> managedItems = new List<Guid>();
            DataTable dtManagedItems = Data.ManagedItem.GetSystemTemplateManagedItems(gSystemID, gTemplateID);
            foreach (DataRow drManagedItem in dtManagedItems.Rows)
            {
                managedItems.Add((Guid)drManagedItem[Data.DataNames._C_ManagedItemID]);
            }
            return managedItems;
        }

        private void LogMetrics(ILog log, string message, TimeSpan elapsedTime)
        {
            //System X required an elapsed time of X
            log.Debug(string.Format("Metrics: {0} {1}", message, elapsedTime.ToString()));
        }

    }
}
