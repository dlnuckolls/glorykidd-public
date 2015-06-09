using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Kindred.Knect.ITAT.Utility;

namespace Kindred.Knect.ITAT.Business
{
    public class ManagedItemStore
    {
        #region Structs

        private struct standardColumnData
        {
            public Guid templateID;
            public string templateName;
            public string managedItemNumber;
            public string activeWorkflow;
            public string status;
            public string state;
        }

        public struct dataRow
        {
            public string rowData;
            public List<rowError> rowErrors;
        }

        public struct rowError
        {
            public string templateName;
            public string managedItemNumber;
            public string termName;
            public string fieldName;
            public string message;
        }

        #endregion Structs

        #region Private Members

        private standardColumnData _columnData;
        private List<TermStore> _terms;
        private Dictionary<string, TermStore> _termsList;

        #endregion Private Members

        #region Public Properties

        public Guid TemplateID
        {
            get { return _columnData.templateID; }
        }

        public string ManagedItemNumber
        {
            get { return _columnData.managedItemNumber; }
        }

        public string ActiveWorkflow
        {
            get { return _columnData.activeWorkflow; }
            set { _columnData.activeWorkflow = value; }
        }

        #endregion Public Properties

        #region Constructor

        public ManagedItemStore(Guid templateID, string templateName, string managedItemNumber, string status, string state)
        {
            _columnData = new standardColumnData();
            _columnData.templateID = templateID;
            _columnData.managedItemNumber = managedItemNumber;
            _columnData.status = status;
            _columnData.state = state;
            _columnData.templateName = templateName;
            _terms = new List<TermStore>();
        }

        #endregion Constructor

        #region Public Methods

        public int AddTerm(TermStore termStore)
        {
            if (_terms == null)
                _terms = new List<TermStore>();
            if (termStore != null)
                _terms.Add(termStore);
            return _terms.Count;
        }

        public int GetRowCount(Guid templateID, List<string> reportedTermNames)
        {
            if (_terms == null)
                throw new Exception(string.Format("TermStore does not exist for template '{0}'", templateID.ToString()));
            int maxRowCount = 1;
            foreach (TermStore termStore in _terms)
            {
                if (reportedTermNames.Contains(termStore.Name))
                {
                    int rowCount = termStore.GetRowCount();
                    if (rowCount > maxRowCount)
                        maxRowCount = rowCount;
                }
            }
            return maxRowCount;
        }

        //This call will examine the termAliasOrder list of columns to report, look at multiplexing to determine which term to
        //report in these cases, and return back the final listing of DSF term names, in order reported.
        //public int GetRowCountMultiplex(Guid templateID, List<string /*DSF Alias Name*/> orderedAliasNames, Dictionary<string /*DSF Alias Name*/, List<string /*DSF Name*/>> termAliasMapping, List<string> orderedDSFNames)
        //{
        //    if (_terms == null)
        //        throw new Exception(string.Format("TermStore does not exist for template '{0}'", templateID.ToString()));
        //    orderedDSFNames = new List<string>();     //This listing is defined during this call.
        //    int maxRowCount = 1;

        //    foreach (string termAlias in orderedAliasNames)
        //    {
        //        if (termAliasMapping.ContainsKey(termAlias))
        //        {
        //            if (termAliasMapping[termAlias].Count == 0)
        //            {
        //                throw new Exception(string.Format("The term alias '{0}' does not have any terms assigned for template '{1}'", termAlias, templateID.ToString()));
        //            }

        //            if (termAliasMapping[termAlias].Count == 1)
        //            {
        //                orderedDSFNames.Add(termAliasMapping[termAlias][0]);
        //                string termName = DataStoreField.GetTermName(termAliasMapping[termAlias][0]);
        //                TermStore termStore = _terms.Find(term => term.Name.Equals(termName));
        //                if (termStore != null)
        //                {
        //                    int rowCount = termStore.GetRowCount();
        //                    if (rowCount > maxRowCount)
        //                        maxRowCount = rowCount;
        //                }
        //            }
        //            else
        //            {
        //                //Need to determine which term to use
        //                int termAliasMaxRowCount = 0;
        //                string orderedDSFName = string.Empty;
        //                //Note: Need to ensure that the first valid term found is selected by default.
        //                string validTermName = string.Empty;
        //                //Note: Need to ensure that the first term encountered with a valid value is selected by default.
        //                bool validValue = false;
        //                foreach (string dsfName in termAliasMapping[termAlias])
        //                {
        //                    TermStore termStore = _terms.Find(term => term.Name.Equals(DataStoreField.GetTermName(dsfName)));
        //                    if (termStore != null)
        //                    {
        //                        if (string.IsNullOrEmpty(validTermName))
        //                            validTermName = termStore.Name;
        //                        int rowCount = termStore.GetRowCount();
        //                        bool validTerm = rowCount > 1 || (rowCount == 1 && termStore.GetIndexedValue(0, DataStoreField.GetTermField(dsfName)).Length > 0);
        //                        if (validTerm)
        //                        {
        //                            if (!validValue)
        //                            {
        //                                orderedDSFName = dsfName;
        //                                validValue = true;
        //                            }
        //                            if (rowCount > termAliasMaxRowCount)
        //                            {
        //                                termAliasMaxRowCount = rowCount;
        //                                orderedDSFName = dsfName;
        //                            }
        //                        }
        //                    }
        //                }
        //                if (string.IsNullOrEmpty(orderedDSFName))
        //                    orderedDSFName = validTermName;

        //                if (termAliasMaxRowCount > maxRowCount)
        //                    maxRowCount = termAliasMaxRowCount;
        //                orderedDSFNames.Add(orderedDSFName);
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception(string.Format("The term alias mapping object did not contain the ordered Alias Name '{0}' for template '{1}'", termAlias, templateID.ToString()));
        //        }
        //    }
        //    return maxRowCount;
        //}

        public dataRow GetDataRow(bool isHeader, TemplateStore templateStore, int index, List<string /*DSF Name*/> orderedDSFNames)
        {
            GetTermsList();
            dataRow row = new dataRow();
            row.rowErrors = new List<rowError>();
            StringBuilder sb = new StringBuilder();

            for (int columnIndex = 0; columnIndex < orderedDSFNames.Count; columnIndex++)
            {
                string orderedDSFName = orderedDSFNames[columnIndex];
                string value = string.Empty;
                int? maxLength = null;

                if (isHeader)
                {
                    if (SystemStore.IsStandardColumn(orderedDSFName))
                        value = orderedDSFName;
                    else
                        value = DataStoreField.GetHeaderName(orderedDSFName);
                }
                else
                {
                    if (SystemStore.IsStandardColumn(orderedDSFName))
                    {
                        if (index == 0)
                        {
                            switch (SystemStore.GetStandardColumn(orderedDSFName))
                            {
                                case SystemStore.StandardColumn.ManagedItemNumber:
                                    value = _columnData.managedItemNumber;
                                    break;

                                case SystemStore.StandardColumn.TemplateName:
                                    value = _columnData.templateName;
                                    break;

                                case SystemStore.StandardColumn.WorkflowName:
                                    value = _columnData.activeWorkflow;
                                    break;

                                case SystemStore.StandardColumn.WorkflowStatus:
                                    value = _columnData.status;
                                    break;

                                case SystemStore.StandardColumn.WorkflowState:
                                    value = _columnData.state;
                                    break;
                            }
                        }
                        else
                        {
                            switch (SystemStore.GetStandardColumn(orderedDSFName))
                            {
                                //NOTE - Are all of these values repeated for multi-value terms?
                                case SystemStore.StandardColumn.ManagedItemNumber:
                                    value = _columnData.managedItemNumber;
                                    break;

                                case SystemStore.StandardColumn.TemplateName:
                                    value = _columnData.templateName;
                                    break;

                                case SystemStore.StandardColumn.WorkflowName:
                                    value = _columnData.activeWorkflow;
                                    break;

                                case SystemStore.StandardColumn.WorkflowStatus:
                                    value = _columnData.status;
                                    break;

                                case SystemStore.StandardColumn.WorkflowState:
                                    value = _columnData.state;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        string termName = DataStoreField.GetTermName(orderedDSFName);
                        string fieldName = DataStoreField.GetTermField(orderedDSFName);
                        //A term could be missing from the _termsList if it is from an older manageditem (not the same as the current template).
                        if (_termsList.ContainsKey(termName))
                        {
                            string error = string.Empty;
                            value = _termsList[termName].GetIndexedValue(index, fieldName);

                            //Note - the DataStoreDefinition term (if found) is used here to determine how the manageditem term will be validated.
                            DataStoreField dsf = templateStore.DataStoreFields.First(kvp => kvp.Key.Equals(orderedDSFName)).Value;
                            if (dsf != null)
                            {
                                maxLength = dsf.Length;
                                //If a term does not pass validation, wipe the value and report the error.
                                //If this is an 'assigned' term type, then process it as such.
                                if (TermStore.RequiresValidation(dsf.TextTermFormat))
                                {
                                    error = TermStore.ValidateTermType(value, dsf.TermType, dsf.TextTermFormat);
                                    if (!string.IsNullOrEmpty(error))
                                        value = string.Empty;
                                    else
                                        value = TermStore.GetFormattedValue(value, dsf.TermType, dsf.TextTermFormat, dsf.DateFormat, templateStore.DefaultDateFormat);
                                }
                                else
                                {
                                    error = TermStore.ValidateTermType(value, dsf.TermType, TextTermFormat.Plain);
                                    if (!string.IsNullOrEmpty(error))
                                        value = string.Empty;
                                    else
                                        value = TermStore.GetFormattedValue(value, dsf.TermType, TextTermFormat.Plain, dsf.DateFormat, templateStore.DefaultDateFormat);
                                }
                            }
                            else
                            {
                                //This should never happen since the initial dsf listing is built based on the DataStoreConfiguration from the database.
                                error = string.Format("DSF term '{0}' not found in DataStoreFields for templateStore {1}", orderedDSFName, templateStore.Template.Name);
                            }
                            if (!string.IsNullOrEmpty(error))
                                AddError(row, _columnData.templateName, _columnData.managedItemNumber, termName, fieldName, error);
                        }
                        //20110113 - Per discussion, do not report the case of terms missing from the ManagedItem.
                        //else
                        //{
                        //    //Only need to record this error once per ManagedItem.  Also, do not report this for the special case of 'Attachments'.
                        //    if (index == 0 && !termName.Equals(XMLNames._A_Attachments))
                        //    {
                        //        string error = "Term not found in ManagedItem";
                        //        AddError(row, columnData.templateName, columnData.managedItemNumber, termName, fieldName, error);
                        //    }
                        //}
                    }
                }
                sb = sb.Append(TextHelper.CSVFormat(maxLength, TextHelper.ReplaceWhiteSpace(value), columnIndex == orderedDSFNames.Count - 1));
            }
            row.rowData = sb.ToString();
            return row;
        }

        #endregion Public Methods

        #region Private Methods

        private void GetTermsList()
        {
            if (_termsList == null)
            {
                _termsList = new Dictionary<string, TermStore>();
                foreach (TermStore term in _terms)
                    _termsList.Add(term.Name, term);
            }
        }

        private void AddError(dataRow row, string templateName, string managedItemNumber, string termName, string fieldName, string message)
        {
            if (row.rowErrors == null)
            {
                row.rowErrors = new List<rowError>();
            }

            rowError error = new rowError();
            error.templateName = templateName;
            error.managedItemNumber = managedItemNumber;
            error.termName = termName;
            error.fieldName = fieldName;
            error.message = message;
            row.rowErrors.Add(error);
        }

        #endregion Private Methods

    }
}
