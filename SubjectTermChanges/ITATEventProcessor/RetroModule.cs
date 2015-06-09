using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Kindred.Common.Logging;
using System.Diagnostics;


namespace Kindred.Knect.ITAT
{
    class TemplateCounter
    {
        public string TemplateName { get; set; }
        public List<string> RetroedManagedItemNumbers { get; set; }
        public List<string> RevertedManagedItemNumbers { get; set; }
        public List<string> OrphanedManagedItemNumbers { get; set; }
        public List<string> FailedManagedItemIDs { get; set; }
        public List<string> FailedManagedItemNumbers { get; set; }

        public TemplateCounter(string templateName)
        {
            TemplateName = templateName;
            RetroedManagedItemNumbers = new List<string>();
            RevertedManagedItemNumbers = new List<string>();
            OrphanedManagedItemNumbers = new List<string>();
            FailedManagedItemIDs = new List<string>();
            FailedManagedItemNumbers = new List<string>();
        }
    }
   
    class RetroModule
    {

        #region private

        private const int _MAX_LOG_SIZE = 2000;

        private List<string> GetManagedItemLoggingInfo(List<string> managedItemNumbers, bool useMaxSize, int lineLength)
        {
            List<string> logInfo = new List<string>();
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < managedItemNumbers.Count; index++)
            {
                sb.AppendFormat("{0},", managedItemNumbers[index].Trim());
                if (index % lineLength == 0)
                {
                    if (!useMaxSize || sb.ToString().Length >= _MAX_LOG_SIZE)
                    {
                        logInfo.Add(sb.ToString());
                        sb = new StringBuilder();
                    }
                }
            }
            logInfo.Add(sb.ToString().TrimEnd(','));
            return logInfo;
        }

        private void LogMetrics(ILog log, DateTime startTime, int initialManagedItemsProcessed, TimeSpan initialElapsedTime, int managedItemsToBeProcessed)
        {
            if (initialManagedItemsProcessed <= 0 || managedItemsToBeProcessed <= 0)
                return;
            double averageProcessingTime = initialElapsedTime.TotalSeconds / initialManagedItemsProcessed;
            DateTime estimatedStopTime = startTime.AddSeconds(averageProcessingTime * managedItemsToBeProcessed);
            log.Debug(string.Format("Metrics: Elapsed Time = {0}; MI Count = {1:D}; AVG = {2}; Projected Finish = {3}; Actual Finish = {4}", initialElapsedTime.ToString(), initialManagedItemsProcessed, averageProcessingTime.ToString(), estimatedStopTime.ToString("HH:mm:ss"), DateTime.Now.ToString("HH:mm:ss")));
        }

        private List<Business.Template> GetSystemTemplates(Guid gSystemID)
        {
            Dictionary<string, Business.Template> dictTemplates = new Dictionary<string, Business.Template>();

            using (DataSet dsTemplates = Data.Template.GetRetroTemplateList(gSystemID))
            {
                foreach (DataRow drTemplate in dsTemplates.Tables[0].Rows)
                {
                    Guid templateID = (Guid)drTemplate[Data.DataNames._C_TemplateID];
                    Business.Template template = new Business.Template(templateID, Business.DefType.Final);
                    if (template.RetroModel != Business.Retro.RetroModel.Off)
                    {
                        //Determine how many ManagedItems would require retro applied 
                        int managedItemCount = GetManagedItems(templateID, template.RetroDate).Count;
                        if (managedItemCount > 0)
                        {
                            string templateKey = string.Format("{0:D10}|{1}", managedItemCount, templateID.ToString());
                            dictTemplates.Add(templateKey, template);
                        }
                    }
                }
            }
            //Sort the templates according to number of ManagedItems, in decreasing order.
            List<KeyValuePair<string, Business.Template>> dictTemplatesSorted = new List<KeyValuePair<string, Business.Template>>(dictTemplates);
            dictTemplatesSorted.Sort(
                  delegate(KeyValuePair<string, Business.Template> firstPair, KeyValuePair<string, Business.Template> secondPair)
                  {
                      return secondPair.Key.CompareTo(firstPair.Key);
                  }
            );

            //Create a list for returning to the caller
            List<Business.Template> templates = new List<Business.Template>(dictTemplatesSorted.Count);
            foreach (KeyValuePair<string, Business.Template> kvp in dictTemplatesSorted)
            {
                templates.Add(kvp.Value);
            }
            return templates;
        }

        private List<Guid> GetManagedItems(Guid gTemplateID, DateTime? retroDate)
        {
            List<Guid> managedItems = new List<Guid>();
            DataTable dtManagedItems = Data.ManagedItem.GetRetroManagedItems(gTemplateID, retroDate);
            foreach (DataRow drManagedItem in dtManagedItems.Rows)
            {
                managedItems.Add((Guid)drManagedItem[Data.DataNames._C_ManagedItemID]);
            }
            return managedItems;
        }

        private List<string> GetOrphanedManagedItems(Guid gTemplateID)
        {
            List<string> managedItems = new List<string>();
            DataTable dtManagedItems = Data.ManagedItem.GetOrphanedManagedItems(gTemplateID);
            foreach (DataRow drManagedItem in dtManagedItems.Rows)
            {
                managedItems.Add(drManagedItem[Data.DataNames._C_ManagedItemNumber].ToString());
            }
            return managedItems;
        }

        #endregion

        #region private processing
        
        private List<string> ProcessManagedItem(
                                ILog log, 
                                Business.ITATSystem itatSystem, 
                                Business.Template template, 
                                Stopwatch stopWatch, 
                                TemplateCounter templateCounter,
                                ref int managedItemsProcessed,
                                Guid managedItemID,
                                Business.Retro.AuditType auditType,
                                List<string> processedManagedItemNumbers,
                                List<string> failedManagedItemIDs,
                                List<string> failedManagedItemNumbers)
        {
            List<string> sErrors = new List<string>();

            stopWatch.Start();
            string managedItemNumber = null;
            try
            {
                bool returnToDraft;
                Business.ManagedItem newManagedItem = Business.ManagedItem.CreateRetro(template, false, managedItemID, itatSystem.HasOwningFacility.Value, itatSystem.DocumentStorageType, out returnToDraft);
                managedItemNumber = newManagedItem.ItemNumber;
                if (returnToDraft)
                {
                    templateCounter.RevertedManagedItemNumbers.Add(newManagedItem.ItemNumber);
                }
                newManagedItem.Update(false, auditType, true);    //Also calls UpdateManagedItemStateRole
                newManagedItem.UpdateScheduledEvents(Business.ScheduledEvent.ExecutedCalculationType.UseGracePeriod);
                processedManagedItemNumbers.Add(newManagedItem.ItemNumber);
                templateCounter.RetroedManagedItemNumbers.Add(newManagedItem.ItemNumber);
                newManagedItem = null;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error Processing ManagedItem '{0}' ({1}) - {2}  Trace:{3}", string.IsNullOrEmpty(managedItemNumber) ? "" : managedItemNumber, managedItemID.ToString(), ex.Message, ex.StackTrace));
                templateCounter.FailedManagedItemIDs.Add(managedItemID.ToString());
                failedManagedItemIDs.Add(managedItemID.ToString());
                if (!string.IsNullOrEmpty(managedItemNumber))
                {
                    templateCounter.FailedManagedItemNumbers.Add(managedItemNumber);
                    failedManagedItemNumbers.Add(managedItemNumber);
                }
            }
            if (processedManagedItemNumbers.Count % 100 == 0)
            {
                log.Info(string.Format("Processed {0:D} manageditems...", processedManagedItemNumbers.Count));
            }
            stopWatch.Stop();
            managedItemsProcessed++;

            return sErrors;
        }

        private List<string> ProcessTemplate(ILog log, Business.ITATSystem itatSystem, Business.Template template, List<TemplateCounter> templateCounters)
        {
            List<string> errors = new List<string>();
            int managedItemsProcessed = 0;
            Stopwatch stopWatch = new Stopwatch();

            Business.Retro.AuditType auditType = Business.Retro.AuditType.None;
            switch (template.RetroModel)
            {
                case Business.Retro.RetroModel.OnWithEditLanguage:
                    auditType = Business.Retro.AuditType.RetroWithEditLanguage;
                    break;

                case Business.Retro.RetroModel.OnWithoutEditLanguage:
                    auditType = Business.Retro.AuditType.RetroWithoutEditLanguage;
                    break;
            }

            TemplateCounter templateCounter = new TemplateCounter(template.Name);
            List<Guid> managedItemIDs = GetManagedItems(template.ID, template.RetroDate);
            List<string> orphanedManagedItems = GetOrphanedManagedItems(template.ID);
            templateCounter.OrphanedManagedItemNumbers.AddRange(orphanedManagedItems);
            if (managedItemIDs.Count > 0)
            {
                List<string> processedManagedItemNumbers = new List<string>();
                List<string> failedManagedItemIDs = new List<string>();
                List<string> failedManagedItemNumbers = new List<string>();
                //Used temporarily for metrics
                DateTime startTime = DateTime.Now;
                int initialManagedItemsProcessed = managedItemsProcessed;
                TimeSpan initialElapsedTime = stopWatch.Elapsed;
                int managedItemsToBeProcessed = managedItemIDs.Count;
                log.Info(string.Format("Processing system {0} template {1} - {2:D} manageditems", itatSystem.Name, template.Name, managedItemIDs.Count));
                DateTime dtTemplateRetroDate = DateTime.Now;
                foreach (Guid managedItemID in managedItemIDs)
                {
                    errors.AddRange(ProcessManagedItem(log, itatSystem, template, stopWatch, templateCounter,
                                            ref managedItemsProcessed, managedItemID, auditType, processedManagedItemNumbers,
                                            failedManagedItemIDs, failedManagedItemNumbers));
                }
                Data.Template.UpdateTemplateSummary(template.ID, template.Name, (short)template.Status, dtTemplateRetroDate);
                if (processedManagedItemNumbers.Count > 0)
                {
                    processedManagedItemNumbers.Sort();
                    log.Info(string.Format("For System {0}, Template {1}, Retro applied to the following ManagedItems:", itatSystem.Name, template.Name));
                    List<string> loggingInfo = GetManagedItemLoggingInfo(processedManagedItemNumbers, true, 20);
                    foreach (string logInfo in loggingInfo)
                        log.Info(logInfo);
                }
                else
                {
                    log.Info(string.Format("For System {0}, Template {1}, Retro not applied to any ManagedItems", itatSystem.Name, template.Name));
                }
                //Here - log the ones that are orphaned
                if (orphanedManagedItems.Count > 0)
                {
                    orphanedManagedItems.Sort();
                    log.Info(string.Format("For System {0}, Template {1}, The following ManagedItems are orphaned:", itatSystem.Name, template.Name));
                    List<string> loggingInfo = GetManagedItemLoggingInfo(orphanedManagedItems, true, 20);
                    foreach (string logInfo in loggingInfo)
                        log.Info(logInfo);
                }
                else
                {
                    log.Info(string.Format("For System {0}, Template {1}, There were no orphaned ManagedItems", itatSystem.Name, template.Name));
                }
                //Here - log the ones that failed
                if (failedManagedItemIDs.Count > 0)
                {
                    failedManagedItemIDs.Sort();
                    log.Error(string.Format("For System {0}, Template {1}, Retro errors encountered for the following ManagedItemIDs:", itatSystem.Name, template.Name));
                    List<string> loggingError = GetManagedItemLoggingInfo(failedManagedItemIDs, true, 20);
                    foreach (string logInfo in loggingError)
                        log.Error(logInfo);
                    if (failedManagedItemNumbers.Count > 0)
                    {
                        failedManagedItemNumbers.Sort();
                        log.Error(string.Format("For System {0}, Template {1}, Retro errors encountered for the following captured ManagedItemNumbers:", itatSystem.Name, template.Name));
                        loggingError = GetManagedItemLoggingInfo(failedManagedItemNumbers, true, 20);
                        foreach (string logInfo in loggingError)
                            log.Error(logInfo);
                    }
                }
                else
                {
                    log.Info(string.Format("For System {0}, Template {1}, There were no ManagedItem errors", itatSystem.Name, template.Name));
                }
                templateCounters.Add(templateCounter);
                LogMetrics(log, startTime, initialManagedItemsProcessed, initialElapsedTime, managedItemsToBeProcessed);
                if (templateCounter.RevertedManagedItemNumbers.Count > 0)
                {
                    templateCounter.RevertedManagedItemNumbers.Sort();
                    StringBuilder sbRevertMessage = new StringBuilder();
                    sbRevertMessage.AppendFormat("{0}<br />", template.Workflow.RetroRevertMessage.Text);
                    List<string> loggingInfo = GetManagedItemLoggingInfo(templateCounter.RevertedManagedItemNumbers, false, 10);
                    foreach (string logInfo in loggingInfo)
                        sbRevertMessage.AppendFormat("{0}<br />", logInfo);
                    sbRevertMessage.Append("<br /><br />End of list<br /><br />");

                    if (template.Workflow.RetroRevertMessage.Recipients.Count == 0)
                    {
                        log.Debug(string.Format("No message sent for template '{0}' as no recipients defined.  Message text = {1}", template.Name, sbRevertMessage.ToString()));
                    }
                    else
                    {
                        Business.EmailHelper.SendNotificationToRoles(itatSystem, template.Workflow.RetroRevertMessage.Subject, sbRevertMessage.ToString(), template.Workflow.RetroRevertMessage.Recipients);
                    }
                }
            }
            else
            {
                log.Info(string.Format("For system {0} template {1} - no manageditems to be processed, {2:D} orphaned manageditem(s)", itatSystem.Name, template.Name, orphanedManagedItems.Count));
                if (orphanedManagedItems.Count > 0)
                {
                    orphanedManagedItems.Sort();
                    List<string> loggingInfo = GetManagedItemLoggingInfo(orphanedManagedItems, true, 20);
                    foreach (string logInfo in loggingInfo)
                        log.Info(logInfo);
                }
            }

            return errors;
        }

        private List<string> ProcessSystem(string systemName, Guid itatSystemID)
        {
            ILog log = Kindred.Common.Logging.LogManager.GetLogger(this.GetType());
            List<string> errors = new List<string>();

            StringBuilder sbSystemText = new StringBuilder();
            log.Info(string.Format("Examining system {0}", systemName));
            Business.ITATSystem itatSystem = Business.ITATSystem.Get(itatSystemID);
            if (itatSystem.AllowRetro)
            {
                sbSystemText.AppendFormat("For System {0}<br /><br />", systemName);
                List<Business.Template> templates = GetSystemTemplates(itatSystemID);
                if (templates.Count == 0)
                {
                    log.Info(string.Format("For System {0}, No Templates found requiring Retro", systemName));
                    sbSystemText.Append("No Templates found requiring Retro<br />");
                }
                else
                {
                    List<TemplateCounter> templateCounters = new List<TemplateCounter>();
                    foreach (Business.Template template in templates)
                    {
                        errors.AddRange(ProcessTemplate(log, itatSystem, template, templateCounters));
                    }
                    if (templateCounters.Count == 0)
                    {
                        sbSystemText.Append("No Templates found requiring Retro<br /><br />");
                    }
                    else
                    {
                        foreach (TemplateCounter templateCounter in templateCounters)
                        {
                            sbSystemText.AppendFormat("For Template '{0}':<br /><br />", templateCounter.TemplateName);
                            sbSystemText.AppendFormat("&nbsp;&nbsp;&nbsp;Success: {0:D}<br />", templateCounter.RetroedManagedItemNumbers.Count);
                            sbSystemText.AppendFormat("&nbsp;&nbsp;&nbsp;Failed: {0:D}<br />", templateCounter.FailedManagedItemIDs.Count);
                            sbSystemText.AppendFormat("&nbsp;&nbsp;&nbsp;Orphaned: {0:D}<br />", templateCounter.OrphanedManagedItemNumbers.Count);
                            sbSystemText.AppendFormat("&nbsp;&nbsp;&nbsp;RevertToDraft: {0:D}<br /><br /><br /><br />", templateCounter.RevertedManagedItemNumbers.Count);
                        }
                    }
                }
                if (itatSystem.AllowedRoles(Business.XMLNames._AF_AdminRecipient).Count == 0)
                {
                    log.Debug(string.Format("No message sent for system '{0}' as no '{1}' roles defined", itatSystem.Name, Business.XMLNames._AF_AdminRecipient));
                    log.Debug(string.Format("Message contents for system {0}: {1}", itatSystem.Name, sbSystemText.ToString()));
                }
                else
                    Business.EmailHelper.SendNotificationToRoles(itatSystem, string.Format("ITAT - Retro Update of system {0}", itatSystem.Name), sbSystemText.ToString(), itatSystem.AllowedRoles(Business.XMLNames._AF_AdminRecipient));
            }
            else
                log.Info(string.Format("System {0} does not allow Retro", systemName));

            return errors;
        }

        #endregion

        #region public

        public List<string> Process(string[] systemNames)
        {
            List<string> errors = new List<string>();

            using (DataSet dsSystem = Business.ITATSystem.GetSystemList())
            {
                if (systemNames == null)
                {
                    foreach (DataRow drSystem in dsSystem.Tables[0].Rows)
                    {
                        errors.AddRange(ProcessSystem((string)drSystem[Data.DataNames._C_ITATSystemName], (Guid)drSystem[Data.DataNames._C_ITATSystemID]));
                    }
                }
                else
                {
                    foreach (string systemName in systemNames)
                    {
                        foreach (DataRow drSystem in dsSystem.Tables[0].Rows)
                        {
                            if ((string)drSystem[Data.DataNames._C_ITATSystemName] == systemName.Trim())
                            {
                                errors.AddRange(ProcessSystem(systemName.Trim(), (Guid)drSystem[Data.DataNames._C_ITATSystemID]));
                                break;
                            }
                        }
                    }
                }
            }
            return errors;
        }

        #endregion

    }
}
