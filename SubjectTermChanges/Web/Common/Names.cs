using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web.Common
{
	public class Names
	{
		//ITAT-wide Application Security Resources
		public const string _ASR_SystemAdmin = "ITAT System Admin";
        public const string _ASR_SystemSuperAdmin = "ITAT System Super Admin";

		//User Controls
		public const string _UC_CommentControl = "~/Controls/CommentControl.ascx";
		public const string _UC_AttachmentControl = "~/Controls/AttachmentControl.ascx";
		public const string _UC_DateTermControl = "~/Controls/DateTermControl.ascx";
		public const string _UC_ExternalTermControl = "~/Controls/ExternalTermControl.ascx";
		public const string _UC_RenewalTermControl = "~/Controls/RenewalTermControl.ascx";
		public const string _UC_PickListTermControl = "~/Controls/PickListTermControl.ascx";
		public const string _UC_TextTermControl = "~/Controls/TextTermControl.ascx";
		public const string _UC_LinkTermControl = "~/Controls/LinkTermControl.ascx";
		public const string _UC_MSOTermControl = "~/Controls/MSOTermControl.ascx";
		public const string _UC_FacilityTermControl = "~/Controls/FacilityTermControl.ascx";
		public const string _UC_NameValueListTermControl = "~/Controls/NameValueListTermControl.ascx";

		public const string _UC_ManagedItemProfilePanel = "~/Controls/ManagedItemProfilePanel.ascx";


		//Form Fields
		public const string _FF_Comments = "Comments";
		public const string _FF_Attachments = "Attachments";

		//Context
		public const string _CNTXT_ActionIndex = "ActionIndex";
        public const string _CNTXT_BaseTransfer = "BaseTransfer";

		public const string _CNTXT_EditMode = "EditMode";
        public const string _CNTXT_FromActionEdit = "FromActionEdit";
        public const string _CNTXT_FromWorkflowStateEdit = "FromWorkflowStateEdit";
        public const string _CNTXT_IsChanged = "IsChanged";
        public const string _CNTXT_ITATDocumentID = "ITATDocumentID";
        public const string _CNTXT_ManagedItem = "ManagedItem";
        public const string _CNTXT_OrgStates = "OrgStates";
        public const string _CNTXT_Report = "Report";
		public const string _CNTXT_ReportManagedItemSort = "ReportManagedItemSort";

        public const string _CNTXT_ScheduledEventUpdated = "ScheduledEventUpdated";
        public const string _CNTXT_SearchCriteria = "_kh_ci_SearchCriteria";
        public const string _CNTXT_SearchResultColumns = "SearchResultColumns";
        public const string _CNTXT_SearchResults = "SearchResults";
        public const string _CNTXT_SelectedTermGroupIndex = "SelectedTermGroupIndex";
        public const string _CNTXT_StateIndex = "StateIndex";
		
        public const string _CNTXT_Template = "Template";
        public const string _CNTXT_TemplateId = "TemplateId";
        public const string _CNTXT_TemplateName = "TemplateName";
		public const string _CNTXT_TermDependency = "TermDependency";
        public const string _CNTXT_TermDependencyIndex = "TermDependencyIndex";
        public const string _CNTXT_TermEdit = "TermEdit";
        public const string _CNTXT_TermGroup = "TermGroup";
		public const string _CNTXT_TermGroupId = "TermGroupId";
        public const string _CNTXT_TermGroupIndex = "TermGroupIndex";
		
        public const string _CNTXT_Workflow = "Workflow";
        public const string _CNTXT_WorkflowEditMode = "WorkflowEditMode";
        public const string _CNTXT_WorkflowId = "WorkflowId";


		//QueryString
        public const string _QS_COMPLEXLIST_INDEX = "index";
        public const string _QS_COMPLEXLIST_ITEM_INDEX = "itemindex";
        public const string _QS_COMPLEXLIST_NAME = "list";
        public const string _QS_DEFINITION_ID = "DefinitionID";
        public const string _QS_DOC_DLG_ACTION = "action";
        public const string _QS_DOC_DLG_ACTION_LOOKUP = "lookup";
        public const string _QS_DOC_DLG_ACTION_OPEN = "open";
        public const string _QS_DOC_DLG_ACTION_PREVIEW = "preview";
        public const string _QS_DOC_DLG_ACTION_SUMMARY = "summary";
        public const string _QS_DOC_DLG_ACTION_VIEW = "view";

        public const string _QS_DOCNAME_ID = "documentname";
        public const string _QS_DOCUMENT_ID = "documentid";
        public const string _QS_FILENAME = "filename";
        public const string _QS_ITAT_DOCUMENT_ID = "itatdocumentid";
        public const string _QS_ITAT_SYSTEM_ID = "system";
        public const string _QS_ITEMS_ID = "ItemsID";
        public const string _QS_MANAGED_ITEM_AUDIT_ID = "itemauditid";
        public const string _QS_MANAGED_ITEM_ID = "item";
        public const string _QS_MANAGED_ITEM_NAME = "itemname";
        public const string _QS_MANAGED_ITEM_TERMGROUP = "group";
        public const string _QS_REPORT_ID = "report";
        public const string _QS_SAVED_SEARCH = "SavedSearch";
        public const string _QS_SELECTED_ROW = "selectedrow";
        public const string _QS_SHOW_DEFAULT_DOCUMENT = "showdefaultdocument";
        public const string _QS_TEMPLATE_ID = "template";
        public const string _QS_TERM_NAME = "termname";
        


		//Style
		public const string _STYLE_CSSCLASS_EDIT = "ProfileEdit";
		public const string _STYLE_CSSCLASS_EDITREADONLY = "ProfileEditReadOnly";
		public const string _STYLE_CSSCLASS_CAPTION = "ProfileCaption";
		public const string _STYLE_CSSCLASS_CAPTION_ALIGNLEFT = "ProfileCaption AlignLeft";
		public const string _STYLE_CSSCLASS_PROFILE_VALIDATIONERROR = "ProfileValidationError";
		public const string _STYLE_CSSCLASS_EXTTERM_CONTROLCELL = "ExternalTermControlCell";
		public const string _STYLE_CSSCLASS_EXTTERM_CONTROLPANEL = "ExternalTermControlPanel";
		public const string _STYLE_CSSCLASS_EXTTERM_CAPTIONCELL = "ExternalTermCaptionCell";
		public const string _STYLE_CSSCLASS_EXTTERM_TABLEROW = "ExternalTermTableRow";

		//Miscellaneous
		public const string _IDENTIFIER_StartDate = "_StartDate";
		public const string _IDENTIFIER_EndDate = "_EndDate";
        public const string _STATUS_Active = "Active";
        public const string _STATUS_Inactive = "Inactive";


		//Header Events
		public const string _HEADER_EVENT_Preview = "Preview";
		public const string _HEADER_EVENT_Save = "Save";
		public const string _HEADER_EVENT_Reset = "Reset";

		//Grid Commands
		public const string _GRID_COMMAND_EditRow = "EditRow";
		public const string _GRID_COMMAND_DeleteRow = "DeleteRow";
        public const string _GRID_COMMAND_CopyRow = "CopyRow";
		public const string _GRID_COMMAND_SingleClick = "singleClick";
		public const string _GRID_COMMAND_DoubleClick = "doubleClick";
		public const string _GRID_COMMAND_HeaderClick = "headerClick";
		public const string _GRID_COMMAND_Copy = "Copy";
		public const string _GRID_COMMAND_Edit = "Edit";
		public const string _GRID_COMMAND_Download = "Download";


	}
}
