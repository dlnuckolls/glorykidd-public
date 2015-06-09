using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Kindred.Common.Data;
using System.IO;
using Kindred.Common.Logging;
using Kindred.Knect.ITAT.Utility;
using System.Diagnostics;

namespace Kindred.Knect.ITAT.Business
{
    public class SystemStore
    {
        private const int _HI_MANAGEDITEM_COUNT = 1000000;
        #region Private Members

        private ILog _log;

        #endregion Private Members

        #region Structs

        public struct metaStoreDefinition
        {
            public DataStoreDefinition dataStoreDefinition;

            //External to DataStoreDefinition class:
            public int outputRowCount;
            public List<Guid> managedItemIDs;
            public List<Guid> skippedManagedItemIDs;
            public int managedItemIDCount;      //Used for reporting
            public string emailAttachmentFileName;
            public string emailAttachmentExtension;
            public StreamWriter swOutput;
            public bool isCompleted;
            public bool isMultiplexed;      //True if the same Alias is reused
            public bool requiresHeaderRow;
            public bool headerRowWritten;
            public bool closedOut;

            public List<ManagedItemStore.dataRow> dataRows;
            public Dictionary<Guid /*TemplateID*/, TemplateStore> templateStores;

            public List<string> orderedDSFNames;
        }

        //This structure is used to gather a listing of all unique term names across all system DSD's for the same template.
        //The purpose of this collection is to possibly reduce the amount of XML parsing required by only parsing
        //those terms which are needed.
        public struct templateDefinition
        {
            public List<string> basicTermNames;
            public List<string> complexListTermNames;
        }

        #endregion Structs

        #region Enums

        public enum StandardColumn
        {
            TemplateName,
            ManagedItemNumber,
            WorkflowName,
            WorkflowStatus,
            WorkflowState,
        };

        #endregion Enums

        #region Public Static Methods

        public static bool IsStandardColumn(string columnName)
        {
            if (DataStoreField.HasTermNameParts(columnName))
                return false;
            return Enum.IsDefined(typeof(StandardColumn), columnName);
        }

        public static StandardColumn GetStandardColumn(string columnName)
        {
            return (StandardColumn)Enum.Parse(typeof(StandardColumn), columnName);
        }

        #endregion Public Static Methods

        #region Constructor

        public SystemStore(ILog log)
        {
            _log = log;
        }

        #endregion Constructor

        #region Public Methods

        public static List<string> GetTermReferences(Guid systemID, Guid templateID, Guid termID)
        {
            List<string> termReferences = new List<string>();
            DataTable dtActiveDSDs = DataStoreDefinition.GetDataStoreDefinitionsBySystemID(systemID, true);
            foreach (DataRow rowDSD in dtActiveDSDs.Rows)
            {
                string configXML = rowDSD[StoreNames._C_DEFINITION].ToString();
                string dsdName = rowDSD[StoreNames._C_DEFINITION_NAME].ToString();

                Dictionary<Guid, string> templates = DataStoreConfig.GetSelectedTemplateListByID(configXML, systemID);
                if (templates.ContainsKey(templateID))
                {
                    if (DataStoreConfig.GetTermInUseFromDef(configXML, termID))
                    {
                        termReferences.Add(string.Format("Data Store definition \"{0}\" (Active) uses this term.", dsdName));
                    }
                }
            }
            DataTable dtInActiveDSDs = DataStoreDefinition.GetDataStoreDefinitionsBySystemID(systemID, false);
            foreach (DataRow rowDSD in dtInActiveDSDs.Rows)
            {
                string configXML = rowDSD[StoreNames._C_DEFINITION].ToString();
                string dsdName = rowDSD[StoreNames._C_DEFINITION_NAME].ToString();

                Dictionary<Guid, string> templates = DataStoreConfig.GetSelectedTemplateListByID(configXML, systemID);
                if (templates.ContainsKey(templateID))
                {
                    if (DataStoreConfig.GetTermInUseFromDef(configXML, termID))
                    {
                        termReferences.Add(string.Format("Data Store definition \"{0}\" (InActive) uses this term.", dsdName));
                    }
                }
            }
            return termReferences;
        }

        public int ProcessSystem(Guid systemID, string systemName)
        {
            int errorCount = 0;
            DateTime runDate = DateTime.Now;
            DataTable dtDSDs = DataStoreDefinition.GetDataStoreDefinitionsBySystemID(systemID, true);
            List<metaStoreDefinition> metaStoreDefinitions = new List<metaStoreDefinition>();

            Dictionary<Guid, Dictionary<Guid, string>> termNameLookup = null;
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<Guid /*FieldID*/, string /*FieldName*/>>> fieldNameLookup = null;
            GetTermAndFieldNameLookup(systemID, ref termNameLookup, ref fieldNameLookup);

            foreach (DataRow rowDSD in dtDSDs.Rows)
            {
                metaStoreDefinitions.Add(InitializeMetaStoreDefinition((Guid)rowDSD[Data.DataNames._C_DataStoreDefinitionID], runDate, systemID, termNameLookup, fieldNameLookup));
            }

            if (metaStoreDefinitions.Count > 0)
            {
                List<Guid> systemManagedItemIDs = new List<Guid>();
                foreach (metaStoreDefinition msd in metaStoreDefinitions)
                {
                    foreach (Guid managedItemID in msd.managedItemIDs)
                        if (!systemManagedItemIDs.Contains(managedItemID))
                            systemManagedItemIDs.Add(managedItemID);
                }
                _log.Info(string.Format("For system '{0}', found {1:D} active DataStoreDefinition(s) and {2:D} unique ManagedItems from the search(es).", systemName, metaStoreDefinitions.Count, systemManagedItemIDs.Count));

                List<Guid> skippedManagedItemIDs = new List<Guid>();
                //Create a collection of all the basicTerms, Complex List Terms, that will be needed for all the DSD's.
                Dictionary<Guid, templateDefinition> systemTemplateDefinitions = new Dictionary<Guid, templateDefinition>();
                foreach (metaStoreDefinition msd in metaStoreDefinitions)
                {
                    foreach (KeyValuePair<Guid /*TemplateID*/, TemplateStore> templateStoreDict in msd.templateStores)
                    {
                        templateDefinition systemTemplateDefinition;
                        if (!systemTemplateDefinitions.ContainsKey(templateStoreDict.Key))
                        {
                            systemTemplateDefinition = new templateDefinition();
                            systemTemplateDefinition.basicTermNames = new List<string>();
                            systemTemplateDefinition.complexListTermNames = new List<string>();
                            systemTemplateDefinitions.Add(templateStoreDict.Key, systemTemplateDefinition);
                        }
                        else
                        {
                            systemTemplateDefinition = systemTemplateDefinitions[templateStoreDict.Key];
                        }

                        //Add unique term names here for templateDefinition.
                        foreach (KeyValuePair<string /*Term Name*/, Term> termDict in templateStoreDict.Value.TemplateTerms)
                        {
                            DataStoreField dsf = templateStoreDict.Value.DataStoreFields.FirstOrDefault(kvp => kvp.Value.TermName.Equals(termDict.Key)).Value;
                            if (dsf != null)
                            {
                                switch (termDict.Value.TermType)
                                {
                                    case TermType.ComplexList:
                                        if (!systemTemplateDefinition.complexListTermNames.Contains(termDict.Key))
                                            systemTemplateDefinition.complexListTermNames.Add(termDict.Key);
                                        break;

                                    default:
                                        if (!systemTemplateDefinition.basicTermNames.Contains(termDict.Key))
                                            systemTemplateDefinition.basicTermNames.Add(termDict.Key);
                                        break;
                                }
                            }
                        }
                    }
                }

                //Note - Temporary metrics info here...
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                int miCount = 0;
                int basicTermsCount = 0;
                int complexListTermsCount = 0;

                while (true)
                {
                    Guid? managedItemID = GetNextManagedItemID(metaStoreDefinitions);
                    if (managedItemID.HasValue)
                    {
                        try
                        {
                            DataTable dtManagedItemInfo = Business.ManagedItem.GetRawManagedItem(managedItemID.Value);
                            if (dtManagedItemInfo.Rows.Count > 0)
                            {
                                DataRow managedItemInfo = dtManagedItemInfo.Rows[0];
                                Guid templateID = (Guid)managedItemInfo[Data.DataNames._C_TemplateID];
                                if (systemTemplateDefinitions.ContainsKey(templateID))
                                {
                                    basicTermsCount = systemTemplateDefinitions[templateID].basicTermNames.Count;
                                    complexListTermsCount = systemTemplateDefinitions[templateID].complexListTermNames.Count;
                                    errorCount += ProcessManagedItemID(metaStoreDefinitions, managedItemID.Value, systemName, managedItemInfo, systemTemplateDefinitions[templateID]);
                                    miCount++;
                                }
                                else
                                {
                                    SkipManagedItemID(metaStoreDefinitions, managedItemID.Value, skippedManagedItemIDs);
                                    _log.Error(string.Format("When processing managedItemID {0}, systemTemplateDefinitions has {1:D} members and did not contain data for templateID '{2}'", managedItemID.Value.ToString(), systemTemplateDefinitions.Count, templateID.ToString()));
                                    errorCount++;
                                }
                            }
                            else
                            {
                                SkipManagedItemID(metaStoreDefinitions, managedItemID.Value, skippedManagedItemIDs);
                                _log.Error(string.Format("GetRawManagedItem call failed for ManagedItemID '{0}'.  The item was not processed.", managedItemID.Value));
                                errorCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            SkipManagedItemID(metaStoreDefinitions, managedItemID.Value, skippedManagedItemIDs);
                            _log.Error(string.Format("ProcessManagedItemID halted for '{0}'.  Exception: '{1}'.  Stack Trace: '{2}'.", managedItemID.Value, ex.Message, ex.StackTrace));
                            errorCount++;
                            continue;
                        }
                        if (miCount > _HI_MANAGEDITEM_COUNT)
                        {
                            _log.Error(string.Format("ProcessSystem halted for system '{0}' due to a high ManagedItem count of '{1:D}' with ManagedItemID = '{2}'.", systemName, miCount, managedItemID.Value));
                            errorCount++;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                stopWatch.Stop();
                double tsAll = (double)stopWatch.ElapsedMilliseconds;
                double avgMS = 0;
                if (miCount > 0)
                    avgMS = tsAll / miCount;
                _log.Info(string.Format("{0} ManagedItems were processed in {1} milliseconds ({2:D2}:{3:D2} minutes) - average = {4:F4} ms.  {5} Basic Terms, {6} ComplexList Terms",
                    miCount.ToString(),
                    tsAll.ToString(),
                    stopWatch.Elapsed.Minutes,
                    stopWatch.Elapsed.Seconds,
                    avgMS,
                    basicTermsCount.ToString(),
                    complexListTermsCount.ToString()));
                if (skippedManagedItemIDs.Count > 0)
                {
                    _log.Info("The following ManagedItems were skipped:");
                    List<string> managedItemIds = new List<string>();
                    foreach (Guid managedItemID in skippedManagedItemIDs)
                    {
                        if (managedItemIds.Count < 50)
                            managedItemIds.Add(managedItemID.ToString());
                        else
                        {
                            _log.Info(string.Join(",", managedItemIds.ToArray()));
                            managedItemIds = new List<string>();
                        }
                    }
                    if (managedItemIds.Count > 0)
                        _log.Info(string.Join(",", managedItemIds.ToArray()));
                }

                //Ensure that all of the metaStoreDefinitions are closed out.
                for (int index = 0; index < metaStoreDefinitions.Count; index++)
                {
                    metaStoreDefinition msd = metaStoreDefinitions[index];
                    CloseOutMetaStoreDefinition(msd, systemName);
                    msd.closedOut = true;
                    metaStoreDefinitions[index] = msd;
                }
            }
            else
            {
                _log.Info(string.Format("For system '{0}', no active DataStoreDefinitions found.", systemName));
            }
            return errorCount;
        }

        #endregion Public Methods

        #region Private Methods

        private int ProcessManagedItemID(List<metaStoreDefinition> metaStoreDefinitions, Guid managedItemID, string systemName, DataRow managedItemInfo, templateDefinition templateDefinition)
        {
            int errorCount = 0;
            ManagedItemStore managedItemStore = null;
            DateTime dateOfChange = (DateTime)managedItemInfo[Data.DataNames._C_DateOfChange];
            Guid templateID = (Guid)managedItemInfo[Data.DataNames._C_TemplateID];
            List<string> basicTermsToProcess = new List<string>();
            foreach (string termName in templateDefinition.basicTermNames)
                basicTermsToProcess.Add(termName);
            List<string> complexListTermsToProcess = new List<string>();
            foreach (string termName in templateDefinition.complexListTermNames)
                complexListTermsToProcess.Add(termName);

            for (int dsdIndex = 0; dsdIndex < metaStoreDefinitions.Count; dsdIndex++)
            {
                metaStoreDefinition metaStoreDefinition = metaStoreDefinitions[dsdIndex];
                if (metaStoreDefinition.managedItemIDs.Contains(managedItemID))
                {
                    bool newData = true;
                    if (metaStoreDefinition.dataStoreDefinition.DataStoreConfig.LoadType != LoadType.Full)
                        if (metaStoreDefinition.dataStoreDefinition.LastRunDate.HasValue)
                            newData = dateOfChange >= metaStoreDefinition.dataStoreDefinition.LastRunDate.Value.AddDays(-1 * metaStoreDefinition.dataStoreDefinition.DataStoreConfig.NoOfDays);
                    if (newData)
                    {
                        if (metaStoreDefinition.templateStores.ContainsKey(templateID))
                        {
                            TemplateStore templateStore = metaStoreDefinition.templateStores[templateID];
                            //Process the one ManagedItemID
                            if (managedItemStore == null)
                            {
                                try
                                {
                                    managedItemStore = ManagedItem.CreateStore(_log, templateStore.Template,
                                        (string)managedItemInfo[Data.DataNames._C_TemplateDef],
                                        managedItemID,
                                        ((string)managedItemInfo[Data.DataNames._C_ManagedItemNumber]).Trim(),
                                        (string)managedItemInfo[Data.DataNames._C_Status],
                                        (string)managedItemInfo[Data.DataNames._C_State],
                                        templateStore.TemplateTerms,
                                        basicTermsToProcess,
                                        complexListTermsToProcess);
                                }
                                catch (Exception ex)
                                {
                                    _log.Error(string.Format("ManagedItem.CreateStore halted for '{0}'.  Exception: '{1}'.  Stack Trace: '{2}'.", managedItemStore.ManagedItemNumber, ex.Message, ex.StackTrace));
                                    //Mark this managedItemID as completed.
                                    metaStoreDefinition.managedItemIDs.Remove(managedItemID);
                                    errorCount++;
                                    continue;
                                }
                            }

                            //Perform data processing here.
                            int maxRowCount = managedItemStore.GetRowCount(templateID, templateStore.ReportedTermNames);
                            List<string> orderedDSFNames = new List<string>();

                            if (metaStoreDefinition.requiresHeaderRow && !metaStoreDefinition.headerRowWritten)
                            {
                                ManagedItemStore.dataRow dataRow = managedItemStore.GetDataRow(true, templateStore, 0, metaStoreDefinition.orderedDSFNames);
                                metaStoreDefinition.swOutput.WriteLine(dataRow.rowData);
                                metaStoreDefinition.headerRowWritten = true;
                            }

                            for (int rowIndex = 0; rowIndex < maxRowCount; rowIndex++)
                            {
                                ManagedItemStore.dataRow dataRow = managedItemStore.GetDataRow(false, templateStore, rowIndex, metaStoreDefinition.orderedDSFNames);
                                metaStoreDefinition.swOutput.WriteLine(dataRow.rowData);
                                metaStoreDefinition.outputRowCount = metaStoreDefinition.outputRowCount + 1;
                                if (dataRow.rowErrors.Count > 0)
                                    metaStoreDefinition.dataRows.Add(dataRow);
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("systemDataStoreDefinition.templateStores is missing templateID {0}", templateID.ToString()));
                        }
                    }

                    //Mark this managedItemID as completed.
                    metaStoreDefinition.managedItemIDs.Remove(managedItemID);
                    //If the last one, close out the file.
                    if (metaStoreDefinition.managedItemIDs.Count == 0)
                    {
                        CloseOutMetaStoreDefinition(metaStoreDefinition, systemName);
                        metaStoreDefinition.closedOut = true;
                    }
                }
                metaStoreDefinitions[dsdIndex] = metaStoreDefinition;
            }
            return errorCount;
        }

        private void CloseOutMetaStoreDefinition(metaStoreDefinition metaStoreDefinition, string systemName)
        {
            if (!metaStoreDefinition.closedOut)
            {
                metaStoreDefinition.swOutput.Close();

                _log.Info(string.Format("For System '{0}', Definition '{1}': Processed file \"{2}\", excel file row count = {3:D}, ManagedItem count = {4:D}, ManagedItem's with errors count = {5:D}, total error count = {6:D}",
                    systemName,
                    metaStoreDefinition.dataStoreDefinition.Name,
                    metaStoreDefinition.dataStoreDefinition.DefinitionFilePath,
                    metaStoreDefinition.outputRowCount,
                    metaStoreDefinition.managedItemIDCount,
                    GetCountOfManagedItemsWithErrors(metaStoreDefinition),
                    GetErrorCount(metaStoreDefinition)));

                if (metaStoreDefinition.dataRows != null && metaStoreDefinition.dataRows.Count > 0)
                {
                    ProcessDefinitionErrors(metaStoreDefinition, systemName);
                }
                //Note - if we wanted to add a 'status message', this would be the place to do it.
                DataStoreDefinition.UpdateDataStoreDefinitionOnRun(metaStoreDefinition.dataStoreDefinition);
            }
        }

        private void SkipManagedItemID(List<metaStoreDefinition> metaStoreDefinitions, Guid managedItemID, List<Guid> skippedManagedItemIDs)
        {
            if (!skippedManagedItemIDs.Contains(managedItemID))
                skippedManagedItemIDs.Add(managedItemID);
            for (int dsdIndex = 0; dsdIndex < metaStoreDefinitions.Count; dsdIndex++)
            {
                metaStoreDefinition metaStoreDefinition = metaStoreDefinitions[dsdIndex];
                if (metaStoreDefinition.managedItemIDs.Contains(managedItemID))
                {
                    metaStoreDefinition.managedItemIDs.Remove(managedItemID);
                    if (!metaStoreDefinition.skippedManagedItemIDs.Contains(managedItemID))
                        metaStoreDefinition.skippedManagedItemIDs.Add(managedItemID);
                }
            }
        }

        private int GetErrorCount(metaStoreDefinition definition)
        {
            int nErrorCount = 0;
            foreach (ManagedItemStore.dataRow dataRow in definition.dataRows)
            {
                nErrorCount += dataRow.rowErrors.Count;
            }
            return nErrorCount;
        }

        private int GetCountOfManagedItemsWithErrors(metaStoreDefinition definition)
        {
            List<string> managedItemNumbers = new List<string>();
            foreach (ManagedItemStore.dataRow dataRow in definition.dataRows)
            {
                if (!managedItemNumbers.Contains(dataRow.rowErrors[0].managedItemNumber))
                {
                    managedItemNumbers.Add(dataRow.rowErrors[0].managedItemNumber);
                }
            }
            return managedItemNumbers.Count;
        }

        private void ProcessDefinitionErrors(metaStoreDefinition definition, string systemName)
        {
            string errorReport = string.Format("There were a total of {0:D} error(s) reported for {1:D} manageditem(s) out of a total of {2:D} manageditem(s) processed.",
                GetErrorCount(definition),
                definition.dataRows.Count,
                definition.managedItemIDCount - definition.skippedManagedItemIDs.Count);

            if (definition.skippedManagedItemIDs.Count > 0)
                errorReport += string.Format("  {0:D} manageditem(s) were not processed.", definition.skippedManagedItemIDs.Count);

            if (definition.dataStoreDefinition.DataStoreConfig.ErrorLogRecepientEmailList.Count == 0)
            {
                _log.Error(string.Format("Email Not Sent - For System '{0}', Definition '{1}' ... {2}. No email recipents were listed.", systemName, definition.dataStoreDefinition.Name, errorReport));
            }
            else
            {
                _log.Error(string.Format("For System '{0}', Definition '{1}' ... {2}", systemName, definition.dataStoreDefinition.Name, errorReport));
                byte[] output = null;
                using (StringWriter sw = new StringWriter())
                {
                    sw.WriteLine("Template Name,ManagedItem Number,Term Name,Field Name,Error Message");
                    foreach (ManagedItemStore.dataRow dataRow in definition.dataRows)
                    {
                        foreach (ManagedItemStore.rowError rowError in dataRow.rowErrors)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb = sb.Append(TextHelper.CSVFormat(null, rowError.templateName, false));
                            sb = sb.Append(TextHelper.CSVFormat(null, rowError.managedItemNumber, false));
                            sb = sb.Append(TextHelper.CSVFormat(null, rowError.termName, false));
                            sb = sb.Append(TextHelper.CSVFormat(null, rowError.fieldName, false));
                            sb = sb.Append(TextHelper.CSVFormat(null, rowError.message, true));
                            sw.WriteLine(sb.ToString());
                        }
                    }
                    output = Encoding.UTF8.GetBytes(sw.ToString());
                }
                Kindred.Common.Email.Email email = new Kindred.Common.Email.Email();
                email.Subject = string.Format("ITAT System {0} - Validation Errors for Interface '{1}'", systemName, definition.dataStoreDefinition.Name);
                email.Format = Common.Email.EmailFormat.Html;
                email.From = XMLNames._M_EmailFrom;
                foreach (string recipient in definition.dataStoreDefinition.DataStoreConfig.ErrorLogRecepientEmailList)
                    email.AddTo(recipient);
                email.Format = Kindred.Common.Email.EmailFormat.Html;
                email.Body = string.Format("Please find the list of validation errors attached. {0}", errorReport);
                email.ApplicationName = Business.EmailHelper.EmailName(systemName);
                email.Attachments.Add(new Common.Email.EmailAttachment(output, definition.emailAttachmentFileName, definition.emailAttachmentExtension));
                email.Send();
            }
        }

        private Guid? GetNextManagedItemID(List<metaStoreDefinition> metaStoreDefinitions)
        {
            if (metaStoreDefinitions != null)
            {
                foreach (metaStoreDefinition metaStoreDefinition in metaStoreDefinitions)
                {
                    if (metaStoreDefinition.isCompleted)
                        continue;
                    if (metaStoreDefinition.managedItemIDs == null || metaStoreDefinition.managedItemIDs.Count == 0)
                        continue;
                    return metaStoreDefinition.managedItemIDs[0];
                }
            }
            return null;
        }

        private metaStoreDefinition InitializeMetaStoreDefinition(
            Guid dsdID, 
            DateTime runDate, 
            Guid systemID, 
            Dictionary<Guid, Dictionary<Guid, string>> termNameLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<Guid /*FieldID*/, string /*FieldName*/>>> fieldNameLookup)
        {
            metaStoreDefinition msd = new metaStoreDefinition();
            ITATSystem system = null;
            try
            {
                system = ITATSystem.Get(systemID);
                msd.dataStoreDefinition = DataStoreDefinition.GetDataStoreDefinitionByID(dsdID, system, termNameLookup, fieldNameLookup, true);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("DataStoreDefinition.GetDataStoreDefinitionByID threw exception for DSDID = '{0}'.  Exception: '{1}'.   Stack Trace: {2}.", dsdID.ToString(), ex.Message, ex.StackTrace));
                throw new Exception(string.Format("DataStoreDefinition.GetDataStoreDefinitionByID halted for DSDID = '{0}'", dsdID.ToString()));
            }

            msd.orderedDSFNames = new List<string>();
            foreach (DataStoreField dsf in msd.dataStoreDefinition.DataStoreConfig.Terms)
            {
                msd.orderedDSFNames.Add(dsf.Name);
            }
            msd.requiresHeaderRow = true;
            msd.headerRowWritten = false;
            msd.dataStoreDefinition.LastRunDate = runDate;

            msd.isCompleted = false;
            msd.isMultiplexed = false;
            msd.dataRows = new List<ManagedItemStore.dataRow>();

            msd.skippedManagedItemIDs = new List<Guid>();

            //Create a new Templates Dictionary from the DataStoreConfig.Templates with the values switched, for easy Name access.
            Dictionary<string /*Template Name*/, Guid> templates = new Dictionary<string /*Template Name*/, Guid>();
            List<Guid> searchTemplateIDs = new List<Guid>();

            if (msd.dataStoreDefinition.DataStoreConfig.Templates == null || msd.dataStoreDefinition.DataStoreConfig.Templates.Count == 0)
            {
                _log.Error(string.Format("DataStoreDefinitionID = '{0}' does not contain any Templates.", dsdID.ToString()));
                throw new Exception(string.Format("InitializeMetaStoreDefinition halted for DSDID = '{0}'", dsdID.ToString()));
            }
            foreach (KeyValuePair<Guid, string> kvp in msd.dataStoreDefinition.DataStoreConfig.Templates)
            {
                templates.Add(kvp.Value, kvp.Key);
            }

            try
            {
                msd.templateStores = TemplateStore.GetTemplateStoreCollection(templates, msd.dataStoreDefinition.DataStoreConfig.Terms, msd.dataStoreDefinition.DataStoreConfig.DefaultDateFormat);
                //The returned templateStores dictionary contains the templates actually referenced within the dsfTerms.
                foreach (KeyValuePair<Guid, TemplateStore> kvp in msd.templateStores)
                {
                    searchTemplateIDs.Add(kvp.Key);
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetTemplateStoreCollection threw exception for DSDID = '{0}'.  Exception: '{1}'.   Stack Trace: '{2}'.", dsdID.ToString(), ex.Message, ex.StackTrace));
                throw new Exception(string.Format("InitializeMetaStoreDefinition halted for DSDID = '{0}'", dsdID.ToString()));
            }

            try
            {
                msd.managedItemIDs = GetManagedItems(msd.dataStoreDefinition.DataStoreConfig.SearchCriteria, systemID, searchTemplateIDs);
                msd.managedItemIDCount = msd.managedItemIDs.Count;
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetManagedItems threw exception for DSDID = '{0}'.  Exception: '{1}'.   Stack Trace: '{2}'.", dsdID.ToString(), ex.Message, ex.StackTrace));
                throw new Exception(string.Format("InitializeMetaStoreDefinition halted for DSDID = '{0}'", dsdID.ToString()));
            }

            if (string.IsNullOrEmpty(msd.dataStoreDefinition.DataStoreConfig.Path))
            {
                _log.Error(string.Format("Path not defined for DSDID = '{0}'.", dsdID.ToString()));
                throw new Exception(string.Format("InitializeMetaStoreDefinition halted for DSDID = '{0}'", dsdID.ToString()));
            }
            string filePath = msd.dataStoreDefinition.DataStoreConfig.Path;
            if (!filePath.EndsWith(@"\") && !filePath.EndsWith(@"/"))
                filePath = string.Format(@"{0}\", filePath);
            string fileNameNoExtension = string.Format("{0}_{1}", msd.dataStoreDefinition.Name, msd.dataStoreDefinition.LastRunDate.Value.ToString("yyyyMMdd_HHmmss_ffff"));
            msd.emailAttachmentExtension = "csv";
            msd.dataStoreDefinition.DefinitionFilePath = string.Format("{0}{1}.{2}", filePath, fileNameNoExtension, msd.emailAttachmentExtension);
            msd.emailAttachmentFileName = string.Format("{0}_Validation_Errors", fileNameNoExtension);

            try
            {
                msd.swOutput = new StreamWriter(msd.dataStoreDefinition.DefinitionFilePath);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("StreamWriter threw exception for DSDID = '{0}', path = '{1}'.  Exception: '{2}'.   Stack Trace: '{3}'.", dsdID.ToString(), msd.dataStoreDefinition.DefinitionFilePath, ex.Message, ex.StackTrace));
                throw new Exception(string.Format("InitializeMetaStoreDefinition halted for DSDID = '{0}'", dsdID.ToString()));
            }

            msd.outputRowCount = 0;
            msd.closedOut = false;

            return msd;
        }

        private List<Guid> GetManagedItems(SearchCriteria searchCriteria, Guid systemID, List<Guid> templateIDs)
        {
            DataTable managedItems = new ManagedItemSearch().FindStore(systemID, templateIDs, searchCriteria.Statuses, searchCriteria.FacilityIds, searchCriteria.TextTerm1, searchCriteria.TextTerm2, searchCriteria.DateTerm3Range, searchCriteria.TextTerm4, searchCriteria.TextTerm5, searchCriteria.DateTerm6Range, searchCriteria.DateTerm7Range);

            List<Guid> managedItemIDs = new List<Guid>();
            foreach (DataRow managedItem in managedItems.Rows) 
            {
                managedItemIDs.Add((Guid)managedItem[Data.DataNames._C_ManagedItemID]);
            }

            return managedItemIDs;
        }

        public static void GetTermAndFieldIDLookup(Guid systemID, ref Dictionary<Guid, Dictionary<string, Guid>> termIDLookup, ref Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<string /*FieldName*/, Guid /*FieldID*/>>> fieldIDLookup)
        {
            termIDLookup = new Dictionary<Guid, Dictionary<string, Guid>>();
            fieldIDLookup = new Dictionary<Guid, Dictionary<Guid, Dictionary<string, Guid>>>();
            DataSet ds = GetActiveSearchOnlySystemTemplateList(systemID);
            foreach (DataRow rowTemplate in ds.Tables[0].Rows)
            {
                List<string> repeatedTermNames = new List<string>();
                Guid templateID = (Guid)rowTemplate["TemplateID"];
                string templateName = (string)rowTemplate["TemplateName"];
                Business.Template template = new Business.Template(templateID, DefType.Final);
                termIDLookup.Add(templateID, new Dictionary<string,Guid>());
                fieldIDLookup.Add(templateID, new Dictionary<Guid,Dictionary<string,Guid>>());
                foreach (Business.Term term in template.BasicTerms)
                {
                    if (termIDLookup[templateID].ContainsKey(term.Name))
                    {
                        if (!repeatedTermNames.Contains(term.Name))
                            repeatedTermNames.Add(term.Name);
                    }
                    else
                        termIDLookup[templateID].Add(term.Name, term.ID);
                }

                foreach (Business.Term complexList in template.ComplexLists)
                {
                    if (termIDLookup[templateID].ContainsKey(complexList.Name))
                    {
                        if (!repeatedTermNames.Contains(complexList.Name))
                            repeatedTermNames.Add(complexList.Name);
                    }
                    else
                    {
                        termIDLookup[templateID].Add(complexList.Name, complexList.ID);
                        fieldIDLookup[templateID].Add(complexList.ID, new Dictionary<string, Guid>());
                        foreach (Business.ComplexListField clf in (complexList as Business.ComplexList).Fields)
                        {
                            fieldIDLookup[templateID][complexList.ID].Add(clf.Name, clf.ID);
                        }
                    }
                }
                if (repeatedTermNames.Count > 1)
                    throw new Exception(string.Format("For Template {0} the following term names were repeated: {1}", templateName, string.Join(",", repeatedTermNames.ToArray()).TrimEnd(',')));
            }
        }

        public static void GetTermAndFieldNameLookup(Guid systemID, ref Dictionary<Guid, Dictionary<Guid, string>> termNameLookup, ref Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<Guid /*FieldID*/, string /*FieldName*/>>> fieldNameLookup)
        {
            termNameLookup = new Dictionary<Guid, Dictionary<Guid, string>>();
            fieldNameLookup = new Dictionary<Guid, Dictionary<Guid, Dictionary<Guid, string>>>();
            DataSet ds = GetActiveSearchOnlySystemTemplateList(systemID);
            foreach (DataRow rowTemplate in ds.Tables[0].Rows)
            {
                Guid templateID = (Guid)rowTemplate["TemplateID"];
                string templateName = (string)rowTemplate["TemplateName"];
                Business.Template template = new Business.Template(templateID, DefType.Final);
                termNameLookup.Add(templateID, new Dictionary<Guid, string>());
                fieldNameLookup.Add(templateID, new Dictionary<Guid,Dictionary<Guid,string>>());
                foreach (Business.Term term in template.BasicTerms)
                {
                    termNameLookup[templateID].Add(term.ID, term.Name);
                }

                foreach (Business.Term complexList in template.ComplexLists)
                {
                    termNameLookup[templateID].Add(complexList.ID, complexList.Name);
                    fieldNameLookup[templateID].Add(complexList.ID, new Dictionary<Guid,string>());
                    foreach (Business.ComplexListField clf in (complexList as Business.ComplexList).Fields)
                    {
                        fieldNameLookup[templateID][complexList.ID].Add(clf.ID, clf.Name);
                    }
                }
            }
        }

        public static DataSet GetActiveSearchOnlySystemTemplateList(Guid systemID)
        {
            List<short> statuses = new List<short>();
            statuses.Add((short)Business.TemplateStatusType.Active);
            statuses.Add((short)Business.TemplateStatusType.SearchOnly);
            return Business.Template.GetTemplateListWithStatus(systemID, statuses, null);
        }

        #endregion Private Methods
    }
}
