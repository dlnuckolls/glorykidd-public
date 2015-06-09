using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Xml;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.IO;
using Kindred.Knect.ITAT.Business;


namespace Kindred.Knect.ITAT.Web
{
    public partial class ManagedItemLoad : BasePage
	{
        #region private const

        private const string _COL_ACTION = "Action";
        private const string _COL_MI_NUMBER = "ManagedItemNumber";
        private const string _COL_ACTIVE_WF = "ActiveWorkflow";
        private const string _COL_ACTIVE_WF_STATE = "ActiveWorkflowState";
        private const string _COL_OWNING_FAC_ID = "OwningFacilityID";
        private const string _COL_ERRORS = "*Errors*";
        private const string _COL_DO_NOT_OUTPUT = "*DoNotOutput*";
        private const string _COL_EVENT = "Event=";

        private const string _ROW_COMPLEX_LIST = "ComplexList=";

        private const string _ROW_COL_NO_CHANGE = "";
        private const string _ROW_TERM_NULLED_OUT = "null";

        private const int _ROW_MAIN_COLUMN_HEADINGS = 0;
        private const int _ROW_SECONDARY_COLUMN_HEADINGS = 1;
        private const int _ROW_FIRST_DATA = 2;

        private const string _ACTION_INSERT = "I";
        private const string _ACTION_UPDATE = "U";
        private const string _ACTION_UPDATE_COMPLEX_LIST = "UCL";
        private const string _ACTION_DELETE = "D";
        private const char _TERM_DELIMITER = '|';
        //private const char _ROW_DELIMITER = '^';
        private const char _FIELD_DELIMITER = '~';
        private const string VSKEY_LOAD_TABLE = "_vskey_LoadTable";

        DataTable _dtExcelData = null;

        #endregion private const

        #region protected override

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			ClientScript.RegisterClientScriptInclude(this.GetType(), "_kh_jsITAT", "../Scripts/itat.js");
		}

        internal override HtmlGenericControl HTMLBody()
        {
            return htmlBody;
        }

        internal override Control ResizablePanel()
        {
            return null;
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            Server.ScriptTimeout = 900;     //seconds, or 15 minutes

			if (IsPostBack)
			{
			}
			else
			{
                if (!AllowAccess(Common.Names._ASR_SystemSuperAdmin))
                {
                    UnauthorizedPageAccess();
                }
				InitializeForm();
			}

            btnHelp.OnClientClick = "javascript:window.open('BulkLoaderHelp.htm','_blank','resizable=1,width=720,height=430,scrollbars=1'); return false;";
            
		}

        #endregion protected override

        #region private

        private void InitializeForm()
        {
            using (DataSet ds = Business.ITATSystem.GetSystemList())
            {
                ddlSystem.DataSource = ds;
                ddlSystem.DataTextField = "ITATSystemName";
                ddlSystem.DataValueField = "ITATSystemID";
                ddlSystem.DataBind();
            }
            ddlSystem.Items.Insert(0, new ListItem("(Please select a system)", string.Empty));
        }

        #region ManagedItem Processing

        private List<string> InsertManagedItem(bool updateDatabase, Guid templateID, int? primaryFacilityID, DataTable dtExcelData, DataRow row, out bool success)
        {
            success = false;
            Business.ManagedItem managedItem = Business.ManagedItem.Create(updateDatabase, templateID, primaryFacilityID);
            List<string> feedback = SetValues(managedItem, dtExcelData, row);
            try
            {
                if (feedback.Count == 0)
                    managedItem.FirstSave(updateDatabase, false);
            }
            catch (Exception ex)
            {
                feedback.Add(string.Format("FirstSave returned error: {0}. ", ex.Message));
            }
            if (feedback.Count == 0)
            {
                success = true;
                row[_COL_MI_NUMBER] = managedItem.ItemNumber;
                feedback.Add("Success");
            }
            return feedback;
        }

        private List<string> UpdateManagedItem(bool updateDatabase, Business.ITATSystem itatSystem, Guid sourceTemplateID, Business.Template destinationTemplate, string systemName, DataTable dtExcelData, DataRow row)
        {
            List<string> errors = new List<string>();
            Guid managedItemID = Guid.Empty;
            try
            {
                DataTable dtMI = Business.ManagedItem.GetManagedItemByNumber(row[_COL_MI_NUMBER].ToString());
                managedItemID = new Guid(dtMI.Rows[0][Data.DataNames._C_ManagedItemID].ToString());
            }
            catch (Exception ex)
            {
                errors.Add(string.Format("Unable to retrieve ManagedItem {0} - Error: {1}. ", row[_COL_MI_NUMBER].ToString(), ex.Message));
                return errors;
            }

            if (destinationTemplate != null && sourceTemplateID == destinationTemplate.ID)
            {
                errors.Add("The source and destination templates should not be the same");
                return errors;
            }

            Business.ManagedItem managedItem = Business.ManagedItem.Get(managedItemID, true);
            Business.Retro.AuditType auditType = Business.Retro.AuditType.Saved;

            //This was changed 4/27/2011 - the ManagedItem should always belong to the selected template, retro or not.
            if (!managedItem.TemplateID.Equals(sourceTemplateID))
            {
                errors.Add(string.Format("ManagedItem {0} does not belong to the selected source template", row[_COL_MI_NUMBER].ToString()));
                return errors;
            }

            if (destinationTemplate != null)
            {
                bool revertToDraft;
                managedItem = Business.ManagedItem.CreateRetro(destinationTemplate, true, managedItemID, itatSystem.HasOwningFacility.Value, itatSystem.DocumentStorageType, out revertToDraft);
                managedItem.UpdateManagedItemTemplate(destinationTemplate.ID);

                switch (destinationTemplate.RetroModel)
                {
                    case Business.Retro.RetroModel.OnWithEditLanguage:
                        auditType = Business.Retro.AuditType.RetroWithEditLanguage;
                        break;

                    case Business.Retro.RetroModel.OnWithoutEditLanguage:
                        auditType = Business.Retro.AuditType.RetroWithoutEditLanguage;
                        break;
                }
            }

            errors = SetValues(managedItem, dtExcelData, row);
            if (updateDatabase && errors.Count == 0)
                managedItem.Update(false, auditType);

            return errors;
        }

        private List<string> DeleteManagedItem(bool updateDatabase, Business.ITATSystem itatSystem, DataRow row)
        {
            List<string> errors = new List<string>();
            Guid managedItemID = Guid.Empty;
            try
            {
                DataTable dtMI = Business.ManagedItem.GetManagedItemByNumber(row[_COL_MI_NUMBER].ToString());
                managedItemID = new Guid(dtMI.Rows[0][Data.DataNames._C_ManagedItemID].ToString());
            }
            catch (Exception ex)
            {
                errors.Add(string.Format("Unable to retrieve ManagedItem {0} - Error: {1}. ", row[_COL_MI_NUMBER].ToString(), ex.Message));
                return errors;
            }

            DataTable dtDocuments = Business.ManagedItem.GetManagedItemDocuments(managedItemID);
            if (dtDocuments.Rows.Count > 0)
            {
                Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(itatSystem.DocumentStorageType);
                documentStorageObject.RootPath = itatSystem.DocumentStorageRootPath;
                foreach (DataRow rowDocument in dtDocuments.Rows)
                {
                    string documentStoreId = rowDocument[Data.DataNames._C_DocumentStoreID].ToString();
                    if (updateDatabase && documentStorageObject.DeleteDocument(documentStoreId))
                    {
                        //if record exists in CachedDocuments table, delete it
                        Data.Document.RemoveCachedDocument(documentStoreId);
                    }
                }
            }
            //Determine if we need to delete a GeneratedDocument.
            Business.ManagedItem managedItem = Business.ManagedItem.Get(managedItemID, false);

            //Changed for Multiple Documents
            foreach (ITATDocument doc in managedItem.Documents)
            {
                if (!string.IsNullOrEmpty(doc.GeneratedDocumentID))
                {
                    if (updateDatabase)
                        Business.ManagedItem.RemoveGeneratedDocument(managedItem, doc.GeneratedDocumentID, itatSystem.DocumentStorageType);
                }
            }

            if (updateDatabase)
                Business.ManagedItem.DeleteManagedItem(managedItemID);
            return errors;
        }

        private bool SkipColumn(string colName)
        {
            switch (colName)
            {
                case _COL_ACTION:
                case _COL_MI_NUMBER:
                case _COL_ACTIVE_WF:
                case _COL_ACTIVE_WF_STATE:
                case _COL_OWNING_FAC_ID:
                case _COL_ERRORS:
                case _COL_DO_NOT_OUTPUT:
                    return true;
            }
            return false;
        }

        private List<string> SetValues(Business.ManagedItem managedItem, DataTable dtExcelData, DataRow row)
        {
            DataRow rowPattern = dtExcelData.Rows[_ROW_SECONDARY_COLUMN_HEADINGS];
            List<string> errors = new List<string>();

            string activeWorkflow = row[_COL_ACTIVE_WF].ToString();
            string activeWorkflowState = row[_COL_ACTIVE_WF_STATE].ToString();
            if (!(string.IsNullOrEmpty(activeWorkflow) && string.IsNullOrEmpty(activeWorkflowState)))
            {
                if (string.IsNullOrEmpty(activeWorkflow) || string.IsNullOrEmpty(activeWorkflowState))
                    errors.Add("Both the ActiveWorkflow and the ActiveWorkflowState must be provided. ");

                else
                {
                    Business.Workflow workflow = managedItem.FindWorkflow(activeWorkflow);
                    if (workflow == null)
                        errors.Add(string.Format("The provided workflow '{0}' is not defined. ", activeWorkflow));
                    else
                    {
                        Business.State state = workflow.FindState(activeWorkflowState);
                        if (state == null)
                            errors.Add(string.Format("The provided ActiveWorkflowState '{0}' is not defined. ", activeWorkflowState));
                        else
                        {
                            managedItem.ActiveWorkflowID = workflow.ID;
                            managedItem.State = state;
                        }
                    }
                }
            }

            List<string> undefinedColumns = new List<string>();
            foreach (DataColumn col in dtExcelData.Columns)
            {
                if (SkipColumn(col.ColumnName))
                    continue;

                string rowColumnValue = row[col.ColumnName].ToString();

                if (rowColumnValue.Trim().Equals(_ROW_COL_NO_CHANGE))
                    continue;

                if (IsEventColumn(col.ColumnName))
                {
                    SetEvent(managedItem, rowPattern, errors, col, rowColumnValue);
                }
                else
                {
                    Business.Term term = managedItem.FindBasicTerm(col.ColumnName);

                    if (term != null)
                    {
                        if (rowColumnValue.Trim().Equals(_ROW_TERM_NULLED_OUT))
                        {
                            NullOutTerm(managedItem, undefinedColumns, col, term);
                        }
                        else
                            SetTermValue(rowPattern, errors, col, term, rowColumnValue, row);
                    }
                    else
                    {
                        errors.Add(string.Format("Term {0} not found", col.ColumnName));
                    }
                }
            }

            if (undefinedColumns.Count > 0)
                errors.Add(string.Format("The following columns are not defined: {0}. ", string.Join(",", undefinedColumns.ToArray()).TrimEnd(',')));
            return errors;
        }

        private static void SetTermValue(DataRow rowPattern, List<string> errors, DataColumn col, Business.Term term, string rowColumnValue, DataRow row)
        {
            string value = string.Empty;
            char delimiter = _FIELD_DELIMITER;
            switch (term.TermType)
            {
                case Business.TermType.Text:
                case Business.TermType.Date:
                case Business.TermType.PickList:    //e.g. '1,2,3,4'
                case Business.TermType.Renewal:     //e.g. 'EffectiveDate,InitialCount,RenewalCount'
                    value = rowColumnValue;
                    break;
                case Business.TermType.Facility:    //e.g. 'OwningFacilityID|2,3,4,5'
                    value = string.Format("{0}{1}{2}", row[_COL_OWNING_FAC_ID].ToString(), _TERM_DELIMITER, rowColumnValue);
                    delimiter = _TERM_DELIMITER;
                    break;
                //Not loaded
                case Business.TermType.MSO:
                case Business.TermType.Link:
                case Business.TermType.External:
                case Business.TermType.PlaceHolderAttachments:
                case Business.TermType.PlaceHolderComments:
                default:
                    return;
            }
            try
            {
                term.Load(value, rowPattern[col.ColumnName].ToString(), delimiter);
            }
            catch (Exception ex)
            {
                errors.Add(string.Format("Load of term {0} threw exception: {1}.", term.Name, ex.Message));
            }
        }

        private static void NullOutTerm(Business.ManagedItem managedItem, List<string> undefinedColumns, DataColumn col, Business.Term term)
        {
            if (term != null)
            {
                term.Clear();
            }
            else
            {
                Business.Term complexList = managedItem.FindComplexList(col.ColumnName);
                if (complexList != null)
                {
                    complexList.Clear();
                }
                else
                {
                    undefinedColumns.Add(string.Format("'{0}'", col.ColumnName));
                }
            }
        }

        private List<string> SetComplexListValues(Business.ManagedItem managedItem, string complexListName, DataTable dtExcelData, DataRow row, bool firstRow)
        {
            List<string> errors = new List<string>();

            Business.ComplexList complexList = managedItem.FindComplexList(complexListName) as Business.ComplexList;
            if (complexList != null)
            {
                try
                {
                    if (firstRow)
                        complexList.Clear();

                    Dictionary<string /*FieldName*/, string /*FieldValue*/> fieldValues = new Dictionary<string, string>();

                    foreach (DataColumn col in dtExcelData.Columns)
                    {
                        if (SkipColumn(col.ColumnName))
                            continue;

                        if (fieldValues.ContainsKey(col.ColumnName))
                            throw new Exception(string.Format("Field {0} is repeated", col.ColumnName));
                        fieldValues.Add(col.ColumnName, row[col.ColumnName].ToString());
                    }

                    complexList.Load(fieldValues);
                }
                catch (Exception ex)
                {
                    errors.Add(string.Format("Load of complex list {0} threw exception: {1}.", complexList.Name, ex.Message));
                }
            }
            else
            {
                errors.Add(string.Format("Unable to find complex list {0}.", complexListName));
            }

            if (errors.Count == 0)
                managedItem.Update(false, Business.Retro.AuditType.Saved);

            return errors;
        }

        private void SetEvent(Business.ManagedItem managedItem, DataRow rowPattern, List<string> errors, DataColumn col, string rowColumnValue)
        {
            string eventName = GetEventNameFromColumn(col.ColumnName);

            Business.Event miEvent = managedItem.FindEvent(eventName);
            if (miEvent.EventType == Business.EventType.Renewal)
            {
                string errorMessage = string.Format("Event '{0}' is of type Renewal and is handled via the Renewal Term", eventName);
                if (!errors.Contains(errorMessage))
                    errors.Add(errorMessage);
            }
            else
            {
                try
                {
                    miEvent.Load(managedItem, rowColumnValue, rowPattern[col.ColumnName].ToString(), _FIELD_DELIMITER);
                }
                catch (Exception ex)
                {
                    errors.Add(string.Format("Column {0} had error: {1}", col.ColumnName, ex.Message));
                }
            }
        }

        #endregion ManagedItem Processing

        #region Excel Processing

        //Requirement here is that the excelColumns are contained within the templateColumns
        private bool CompareLists(List<string> templateColumns, List<string> excelColumns, List<string> skipColumns)
        {
            if (Utility.ListHelper.ListsMatch(templateColumns, excelColumns))
                return true;

            templateColumns.Sort();
            excelColumns.Sort();

            for (int j = 0; j < excelColumns.Count; j++)
            {
                if (templateColumns.Contains(excelColumns[j]) || skipColumns.Contains(excelColumns[j]))
                {
                    excelColumns.RemoveAt(j);
                    j--;
                }
            }

            return excelColumns.Count == 0;
        }

        private DataTable GetTemplateTable(Business.Template template)
        {
            DataTable dtTemplate = new DataTable();
            dtTemplate.Columns.Add(_COL_ACTION);
            dtTemplate.Columns.Add(_COL_MI_NUMBER);
            dtTemplate.Columns.Add(_COL_ACTIVE_WF);
            dtTemplate.Columns.Add(_COL_ACTIVE_WF_STATE);
            dtTemplate.Columns.Add(_COL_OWNING_FAC_ID);

            string renewalTermColumn = null;
            foreach (Business.Term term in template.BasicTerms)
            {
                switch (term.TermType)
                {
                    case Business.TermType.Renewal:
                        renewalTermColumn = term.Name;
                        break;
                    case Business.TermType.PlaceHolderAttachments:
                    case Business.TermType.PlaceHolderComments:
                        continue;
                    default:
                        break;
                }
                dtTemplate.Columns.Add(term.Name);
            }

            foreach (Business.ComplexList term in template.ComplexLists)
            {
                dtTemplate.Columns.Add(term.Name);
            }

            foreach (Business.Event miEvent in template.Events)
            {
                if (miEvent.EventType != Business.EventType.Renewal)
                    dtTemplate.Columns.Add(GetEventColumnName(miEvent.Name));
            }

            DataRow rowPatterns = dtTemplate.NewRow();

            //For Renewal Term:
            if (!string.IsNullOrEmpty(renewalTermColumn))
                rowPatterns[renewalTermColumn] = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}",
                    _FIELD_DELIMITER,
                    Business.RenewalTerm._LABEL_EFFECTIVE_DATE,
                    Business.RenewalTerm._LABEL_INTIAL_DURATION_COUNT,
                    Business.RenewalTerm._LABEL_RENEWAL_DURATION_COUNT,
                    Business.RenewalTerm._LABEL_RENEWAL_COUNT,
                    Business.Event._LABEL_OFFSET_TERM_NAME,
                    Business.Event._LABEL_OFFSET_DEFAULT_VALUE,
                    Business.Event._LABEL_BASE_DATE_OFFSET);

            foreach (Business.ComplexList term in template.ComplexLists)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(string.Empty);
                foreach (Business.ComplexListField field in term.Fields)
                {
                    sb.AppendFormat("{0}{1}", field.Name, _FIELD_DELIMITER);
                }
                rowPatterns[term.Name] = sb.ToString().TrimEnd(_FIELD_DELIMITER);
            }

            foreach (Business.Event miEvent in template.Events)
            {
                if (miEvent.EventType != Business.EventType.Renewal)
                {
                    rowPatterns[GetEventColumnName(miEvent.Name)] = string.Format("{1}{0}{2}{0}{3}{0}{4}",
                    _FIELD_DELIMITER,
                    Business.Event._LABEL_BASE_DATE_TERM_NAME,
                    Business.Event._LABEL_OFFSET_TERM_NAME,
                    Business.Event._LABEL_OFFSET_DEFAULT_VALUE,
                    Business.Event._LABEL_BASE_DATE_OFFSET);
                }
            }

            dtTemplate.Rows.Add(rowPatterns);
            return dtTemplate;
        }

        private string GetEventColumnName(string eventName)
        {
            return string.Format("{0}{1}", _COL_EVENT, eventName);
        }

        private string GetEventNameFromColumn(string columnName)
        {
            return columnName.Replace(_COL_EVENT, string.Empty);
        }

        private bool IsEventColumn(string columnName)
        {
            return columnName.IndexOf(_COL_EVENT) == 0;
        }

        private bool IsComplexListLoad(string rowColumnValue)
        {
            return rowColumnValue.IndexOf(_ROW_COMPLEX_LIST) == 0;
        }

        private string GetComplexListName(string rowColumnValue)
        {
            return rowColumnValue.Replace(_ROW_COMPLEX_LIST, string.Empty).Trim();
        }

        private void ExportToExcel(string reportName, string contents)
        {
            Response.Clear();
            Response.ContentType = Utility.WebServiceHelper.ExtensionToMimeType("xls");
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.xls", reportName));
            Response.Write(contents);
            Response.End();
        }

        private string ConvertTableToExcelString(DataTable dtSource, bool useHeader, List<string> skipColumn, string skipRowIfColumnNonEmpty)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder("<TABLE><TBODY>");

            if (useHeader)
            {
                //Add the header
                sb.Append("<TR>");
                foreach (DataColumn col in dtSource.Columns)
                {
                    if (skipColumn == null || (skipColumn != null && !skipColumn.Contains(col.ColumnName)))
                        sb.AppendFormat("<TH>{0}</TH>", col.ColumnName);
                }
                sb.Append("</TR>");
            }

            //Add the rows
            foreach (DataRow row in dtSource.Rows)
            {
                if (skipRowIfColumnNonEmpty == null || (skipRowIfColumnNonEmpty != null && string.IsNullOrEmpty(row[skipRowIfColumnNonEmpty].ToString())))
                {
                    sb.Append("<TR>");
                    foreach (DataColumn col in dtSource.Columns)
                    {
                        if (skipColumn == null || (skipColumn != null && !skipColumn.Contains(col.ColumnName)))
                            sb.AppendFormat("<TD>{0}</TD>", row[col.ColumnName].ToString());
                    }
                    sb.Append("</TR>");
                }
            }
            sb.Append("</TBODY></TABLE>");

            return sb.ToString();
        }
        #endregion Excel Processing

        #endregion private

        #region Event Handlers

        protected void ddlSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSystem.SelectedIndex > 0)
            {
                using (DataTable dt = Business.Template.GetActiveTemplates(new Guid(ddlSystem.SelectedValue)))
                {
                    ddlSourceTemplate.DataSource = dt;
                    ddlSourceTemplate.DataTextField = "TemplateName";
                    ddlSourceTemplate.DataValueField = "TemplateID";
                    ddlSourceTemplate.DataBind();
                }
                ddlSourceTemplate.Items.Insert(0, new ListItem("(Please select a template)", string.Empty));
                //Check to see if the system has Retro enabled
                divDestinationTemplate.Visible = false;
                Business.ITATSystem itatSystem = Business.ITATSystem.Get(new Guid(ddlSystem.SelectedValue));
                if (itatSystem.AllowRetro)
                {
                    divDestinationTemplate.Visible = true;
                    using (DataTable dtDB = Business.Template.GetActiveTemplates(new Guid(ddlSystem.SelectedValue)))
                    {
                        DataTable dtSource = new DataTable();
                        dtSource.Columns.Add(Data.DataNames._C_TemplateName);
                        dtSource.Columns.Add(Data.DataNames._C_TemplateID);
                        foreach (DataRow dr in dtDB.Rows)
                        {
                            Business.Template template = new Business.Template(new Guid(dr[Data.DataNames._C_TemplateID].ToString()), Business.DefType.Final);
                            if (template.RetroModel != Business.Retro.RetroModel.Off)
                            {
                                DataRow drSource = dtSource.NewRow();
                                drSource[Data.DataNames._C_TemplateName] = dr[Data.DataNames._C_TemplateName];
                                drSource[Data.DataNames._C_TemplateID] = dr[Data.DataNames._C_TemplateID];
                                dtSource.Rows.Add(drSource);
                            }
                        }
                        ddlDestinationTemplate.DataSource = dtSource;
                        ddlDestinationTemplate.DataTextField = Data.DataNames._C_TemplateName;
                        ddlDestinationTemplate.DataValueField = Data.DataNames._C_TemplateID;
                        ddlDestinationTemplate.DataBind();
                    }
                    ddlDestinationTemplate.Items.Insert(0, new ListItem("(Please select a template)", string.Empty));
                }
                else
                {
                    ddlDestinationTemplate.Items.Clear();
                }
            }
            else
            {
                ddlSourceTemplate.Items.Clear();
                ddlDestinationTemplate.Items.Clear();
            }
        }

        private bool IsEndOfLine(ref int index, string allContent)
        {
            if (index <= allContent.Length - 3)
            {
                if (allContent[index + 1] == '\r' && allContent[index + 2] == '\n')
                {
                    index += 2;     //The index will point to the last carriage return character upon returning.
                    return true;
                }
            }
            return false;
        }

        private void LoadManagedItems(bool updateDatabase)
        {
            List<string> errors = new List<string>();
            if (ddlSystem.SelectedIndex == 0)
            {
                RegisterAlert("A valid system must be selected.");
                return;
            }

            if (ddlSourceTemplate.SelectedIndex == 0)
            {
                RegisterAlert("A valid template must be selected.");
                return;
            }

            if (filUpload.PostedFile == null || filUpload.PostedFile.ContentLength == 0)
            {
                RegisterAlert("A valid load file must be selected.");
                return;
            }

            List<string> fileRows = new List<string>();

            if (filUpload.HasFile)
            {
                string extension = filUpload.FileName.Substring(filUpload.FileName.Length - 4, 4);
                if (extension.ToUpper().Equals(".TXT") || extension.ToUpper().Equals(".CSV"))
                {
                    using (StreamReader sr = new StreamReader(filUpload.PostedFile.InputStream))
                    {
                        string allContent = sr.ReadToEnd();
                        string current = string.Empty;
                        for (int index = 0; index < allContent.Length; index++)
                        {
                            current += allContent[index];
                            if (IsEndOfLine(ref index, allContent))
                            {
                                fileRows.Add(current);
                                current = string.Empty;
                            }
                        }
                        if (current.Length > 0)
                            fileRows.Add(current);
                    }
                }
                else
                {
                    RegisterAlert("You must upload a file with a '.txt' or a '.csv' extension");
                    return;
                }
            }
            else
            {
                RegisterAlert("No uploaded file was found");
                return;
            }

            string exception = string.Empty;
            try
            {
                _dtExcelData = Business.ManagedItem.QueryFile(fileRows.ToArray(), _ROW_MAIN_COLUMN_HEADINGS, _ROW_SECONDARY_COLUMN_HEADINGS, _TERM_DELIMITER, out exception);
            }
            catch (Exception ex)
            {
                exception = ex.Message;
            }

            if (_dtExcelData == null || !string.IsNullOrEmpty(exception))
            {
                RegisterAlert(string.Format("Failed to extract from supplied file '{0}'. Error: '{1}'", HttpUtility.HtmlDecode(filUpload.PostedFile.FileName), exception));
                return;
            }

            int? primaryFacilityID = null;
            int nRowsTotal = 0;
            int nRowsUnsuccessful = 0;
            List<string> confirmationMessage = new List<string>();
            if (!updateDatabase)
            {
                confirmationMessage.Add("**************************************************************************");
                confirmationMessage.Add("***************************   THIS IS A TEST   ***************************");
                confirmationMessage.Add("**************************************************************************");
                confirmationMessage.Add("");
                confirmationMessage.Add("");
            }
            confirmationMessage.Add("Results for Load of");
            confirmationMessage.Add(string.Format("\tSystem = {0}", ddlSystem.SelectedItem.Text));
            confirmationMessage.Add(string.Format("\tTemplate = {0}", ddlSourceTemplate.SelectedItem.Text));
            string fileName = filUpload.PostedFile.FileName;
            int nPos = fileName.LastIndexOf('\\') + 1;
            fileName = fileName.Substring(nPos, fileName.Length - nPos);
            confirmationMessage.Add(string.Format("\tSource File = {0}", fileName));

            List<string> feedBack = new List<string>();
            bool feedBackExists = false;

            divConfirmationMessage.Visible = false;
            btnExportFeedbackExcel.Visible = false;

            _dtExcelData.Columns.Add(_COL_ERRORS);
            _dtExcelData.Columns.Add(_COL_DO_NOT_OUTPUT);
            List<string> skipColumns = new List<string>();
            skipColumns.Add(_COL_ERRORS);
            skipColumns.Add(_COL_DO_NOT_OUTPUT);

            //See if this is a load of a complex list
            if (IsComplexListLoad(_dtExcelData.Rows[_ROW_FIRST_DATA][0].ToString()))
            {
                string complexListName = GetComplexListName(_dtExcelData.Rows[_ROW_FIRST_DATA][0].ToString());

                if (string.IsNullOrEmpty(complexListName))
                {
                    RegisterAlert("No complex list name was found");
                    return;
                }

                //Process
                bool firstRow = true;
                List<Guid> managedItemTracker = new List<Guid>();
                Business.ManagedItem managedItem = null;

                for (int index = _ROW_FIRST_DATA + 1; index < _dtExcelData.Rows.Count; index++)
                {
                    //Determine MI here, pass in to call
                    //Detect when a new one is encountered....
                    DataRow row = _dtExcelData.Rows[index];

                    switch (row[_COL_ACTION].ToString())
                    {
                        case _ACTION_INSERT:
                        case _ACTION_UPDATE:
                        case _ACTION_DELETE:
                            feedBack.Add(string.Format("Action '{0}' was requested, but only '{1}' will be processed", row[_COL_ACTION].ToString(), _ACTION_UPDATE_COMPLEX_LIST));
                            continue;

                        case _ACTION_UPDATE_COMPLEX_LIST:
                            break;

                        default:
                            feedBack.Add(string.Format("Unknown action '{0}' - no processing performed", row[_COL_ACTION].ToString()));
                            continue;
                    }

                    bool managedItemDefined = false;
                    Guid managedItemID = Guid.Empty;
                    try
                    {
                        DataTable dtMI = Business.ManagedItem.GetManagedItemByNumber(row[_COL_MI_NUMBER].ToString());
                        managedItemID = new Guid(dtMI.Rows[0][Data.DataNames._C_ManagedItemID].ToString());
                        managedItemDefined = !managedItemID.Equals(Guid.Empty);
                    }
                    catch (Exception ex)
                    {
                        feedBack.Add(string.Format("Unable to retrieve ManagedItem {0} - Error: {1}. ", row[_COL_MI_NUMBER].ToString(), ex.Message));
                    }

                    if (managedItemDefined)
                    {
                        //TODO: If requested, input the config info - Default,xx,xx,xx,etc.... as separate columns
                        //TODO: Need to change the BW Interface to output the config info for the upload as separate columns
                        firstRow = !managedItemTracker.Contains(managedItemID);
                        if (firstRow)
                        {
                            managedItemTracker.Add(managedItemID);
                            managedItem = Business.ManagedItem.Get(managedItemID, true);
                        }

                        feedBack.AddRange(SetComplexListValues(managedItem, complexListName, _dtExcelData, row, firstRow));
                    }

                    if (feedBack.Count == 0)
                    {
                        row[_COL_DO_NOT_OUTPUT] = "*";
                        row[_COL_ERRORS] = string.Empty;
                    }
                    else
                    {
                        row[_COL_DO_NOT_OUTPUT] = string.Empty;
                        row[_COL_ERRORS] = string.Join("\n", feedBack.ToArray());
                        nRowsUnsuccessful++;
                        feedBack = new List<string>();
                        feedBackExists = true;
                    }
                    nRowsTotal++;
                }
            }
            else
            {
                //Create a list of expected column headings for this template
                Guid sourceTemplateID = new Guid(ddlSourceTemplate.SelectedValue.ToString());
                Business.Template template = new Business.Template(sourceTemplateID, Business.DefType.Final);
                DataTable dtTemplate = GetTemplateTable(template);

                List<string> templateColumns = new List<string>(dtTemplate.Rows.Count);
                foreach (DataColumn col in dtTemplate.Columns)
                    templateColumns.Add(col.ColumnName);

                List<string> excelColumns = new List<string>(_dtExcelData.Rows.Count);
                foreach (DataColumn col in _dtExcelData.Columns)
                    excelColumns.Add(col.ColumnName);

                //Ensure that the Excel contains the 'required' columns...
                List<string> requiredColumns = new List<string>();
                if (!excelColumns.Contains(_COL_ACTION))
                    requiredColumns.Add(_COL_ACTION);
                if (!excelColumns.Contains(_COL_MI_NUMBER))
                    requiredColumns.Add(_COL_MI_NUMBER);
                if (!excelColumns.Contains(_COL_ACTIVE_WF))
                    requiredColumns.Add(_COL_ACTIVE_WF);
                if (!excelColumns.Contains(_COL_ACTIVE_WF_STATE))
                    requiredColumns.Add(_COL_ACTIVE_WF_STATE);
                if (!excelColumns.Contains(_COL_OWNING_FAC_ID))
                    requiredColumns.Add(_COL_OWNING_FAC_ID);

                if (requiredColumns.Count > 0)
                    errors.Add(string.Format("The following required columns were not found in the excel file: {0}. ", string.Join(",", requiredColumns.ToArray()).TrimEnd(',')));

                if (!CompareLists(templateColumns, excelColumns, skipColumns))
                {
                    errors.Add("There was a mismatch between the excel file and the template...");
                    string columnListing = string.Empty;
                    for (int index = 0; index < excelColumns.Count; index++)
                    {
                        if (index == excelColumns.Count - 1)
                            columnListing += string.Format("'{0}'", excelColumns[index]);
                        else
                            columnListing += string.Format("'{0}',\\n", excelColumns[index]);
                    }
                    errors.Add(string.Format("The following excel columns were not found in the template:\\n{0}. ", columnListing).TrimEnd(new char[]{'\\','n'}));
                }

                if (errors.Count > 0)
                {
                    RegisterAlert(string.Join("\\n", errors.ToArray()));
                    return;
                }

                //Process
                Business.ITATSystem itatSystem = null;
                Business.Template destinationTemplate = null;
                if (ddlDestinationTemplate.SelectedIndex > 0)
                {
                    Guid destinationTemplateID = new Guid(ddlDestinationTemplate.SelectedValue);
                    if (!sourceTemplateID.Equals(destinationTemplateID))
                    {
                        destinationTemplate = new Business.Template(destinationTemplateID, Business.DefType.Final);
                    }
                }

                for (int index = _ROW_FIRST_DATA; index < _dtExcelData.Rows.Count; index++)
                {
                    bool success = false;
                    DataRow row = _dtExcelData.Rows[index];

                    primaryFacilityID = null;
                    string owningFacilityID = row[_COL_OWNING_FAC_ID].ToString();
                    if (!string.IsNullOrEmpty(owningFacilityID))
                        primaryFacilityID = int.Parse(owningFacilityID);

                    switch (row[_COL_ACTION].ToString())
                    {
                        case _ACTION_INSERT:
                            feedBack.AddRange(InsertManagedItem(updateDatabase, sourceTemplateID, primaryFacilityID, _dtExcelData, row, out success));
                            break;

                        case _ACTION_UPDATE:
                            if (itatSystem == null)
                                itatSystem = Business.ITATSystem.Get(new Guid(ddlSystem.SelectedValue));
                            feedBack.AddRange(UpdateManagedItem(updateDatabase, itatSystem, sourceTemplateID, destinationTemplate, ddlSystem.SelectedItem.Text, _dtExcelData, row));
                            break;

                        case _ACTION_UPDATE_COMPLEX_LIST:
                            feedBack.Add(string.Format("Action '{0}' was requested, but will not be performed", row[_COL_ACTION].ToString()));
                            break;

                        case _ACTION_DELETE:
                            if (itatSystem == null)
                                itatSystem = Business.ITATSystem.Get(new Guid(ddlSystem.SelectedValue));
                            feedBack.AddRange(DeleteManagedItem(updateDatabase, itatSystem, row));
                            break;

                        default:
                            feedBack.Add(string.Format("Unknown action '{0}' - no processing performed", row[_COL_ACTION].ToString()));
                            break;
                    }

                    if (feedBack.Count == 0)
                    {
                        row[_COL_DO_NOT_OUTPUT] = "*";
                        row[_COL_ERRORS] = string.Empty;
                    }
                    else
                    {
                        row[_COL_DO_NOT_OUTPUT] = string.Empty;
                        row[_COL_ERRORS] = string.Join("\n", feedBack.ToArray());
                        if (!success)
                            nRowsUnsuccessful++;
                        feedBack = new List<string>();
                        feedBackExists = true;
                    }
                    nRowsTotal++;
                }
            }

            confirmationMessage.Add(string.Format("\t{0:D5}\t\tRow{1} Successful", nRowsTotal - nRowsUnsuccessful, nRowsTotal - nRowsUnsuccessful == 1 ? string.Empty : "s"));
            confirmationMessage.Add(string.Format("\t{0:D5}\t\tRow{1} Unsuccessful", nRowsUnsuccessful, nRowsUnsuccessful == 1 ? string.Empty : "s"));
            confirmationMessage.Add("\t---------------------");
            confirmationMessage.Add(string.Format("\t{0:D5}\t\tRow{1} Total", nRowsTotal, nRowsTotal == 1 ? string.Empty : "s"));

            if (feedBackExists)
            {
                confirmationMessage.Add(string.Empty);
                confirmationMessage.Add("\tPlease click button 'Export Feedback Excel' for more information");
            }

            divConfirmationMessage.Visible = true;
            txtConfirmationMessage.Text = string.Join("\r\n", confirmationMessage.ToArray());
            btnExportFeedbackExcel.Visible = feedBackExists;
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            LoadManagedItems(true);
        }

        protected void btnLoadTest_Click(object sender, EventArgs e)
        {
            LoadManagedItems(false);
        }

        protected void btnExportTemplateExcel_Click(object sender, EventArgs e)
        {
            if (ddlSystem.SelectedIndex == 0)
            {
                RegisterAlert("A valid system must be selected.");
                return;
            }

            string templateGuid = null;
            string templateName = string.Empty;
            if (divDestinationTemplate.Visible && ddlDestinationTemplate.SelectedIndex > 0)
            {
                templateGuid = ddlDestinationTemplate.SelectedValue.ToString();
                templateName = ddlDestinationTemplate.SelectedItem.Text;
            }

            if (string.IsNullOrEmpty(templateGuid))
            {
                if (ddlSourceTemplate.SelectedIndex == 0)
                {
                    if (divDestinationTemplate.Visible)
                        RegisterAlert("A valid source or destination template must be selected.");
                    else
                        RegisterAlert("A valid source template must be selected.");
                    return;
                }
                templateGuid = ddlSourceTemplate.SelectedValue.ToString();
                templateName = ddlSourceTemplate.SelectedItem.Text;
            }

            Business.Template template = new Business.Template(new Guid(templateGuid), Business.DefType.Final);
            DataTable dtTemplate = GetTemplateTable(template);

            string excelFileName = string.Format("[{0}]{1}", ddlSystem.SelectedItem.Text, templateName);
            ExportToExcel(excelFileName, ConvertTableToExcelString(dtTemplate, true, null, null));
        }

        protected void btnExportFeedbackExcel_Click(object sender, EventArgs e)
        {
            divConfirmationMessage.Visible = false;
            string excelFileName = string.Format("[{0}]{1} Feedback", ddlSystem.SelectedItem.Text, ddlSourceTemplate.SelectedItem.Text);
            ExportToExcel(excelFileName, ConvertTableToExcelString(_dtExcelData, true, new List<string>(new string[] { _COL_DO_NOT_OUTPUT }), _COL_DO_NOT_OUTPUT));
        }

        protected override object SaveViewState()
        {
            ViewState[VSKEY_LOAD_TABLE] = _dtExcelData;
            return base.SaveViewState();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _dtExcelData = (DataTable)ViewState[VSKEY_LOAD_TABLE];
        }

        #endregion Event Handlers

     }
}
