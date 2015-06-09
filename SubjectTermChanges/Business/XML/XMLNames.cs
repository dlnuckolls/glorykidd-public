using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{
	public class XMLNames
	{
		//Examples of places that contain hard-coded text:
		//  row["...
		//	case "...
		//	Attributes["...
		//	["...
		//	Response.Redirect("...
		//	Server.Transfer("...
		//	.CssClass = "...
		//	new SqlParameter("...
		//	Request.QueryString["...

		//System Attributes
		public const string _A_AllowActionConfirmation = "AllowActionConfirmation";
		public const string _A_AllowAttachments = "AllowAttachments";
        public const string _A_AllowRetro = "AllowRetro";
        public const string _A_AllowNotificationFilterFacility = "AllowNotificationFilterFacility";
        public const string _A_ApplicationFunctionName = "Name";
		public const string _A_ApplicationSecurityName = "ApplicationSecurityName";
		public const string _A_ApplicationSecurityResourceName = "ApplicationSecurityResourceName";
		public const string _A_ManagedItemNumberSystemType = "ManagedItemNumberSystemType";
		public const string _A_DefaultDateFormat = "DefaultDateFormat";
		public const string _A_HasContent = "HasContent";
		public const string _A_HasOwningFacility = "HasOwningFacility";
		public const string _A_HeaderRowSize = "HeaderRowSize";
		public const string _A_ManagedItemName = "ManagedItemName";
		public const string _A_ManagedItemNumberSystemID = "ManagedItemNumberSystemId";
		public const string _A_OverrideEmail = "OverrideEmail";
		public const string _A_OwnerEmail = "OwnerEmail";
        public const string _A_RetroEventGracePeriodDays = "RetroEventGracePeriodDays";
        public const string _A_RoleLevels = "RoleLevels";
		public const string _A_RoleLevelType = "RoleLevelType";
		public const string _A_RoleType = "RoleType";
		public const string _A_RootPath = "RootPath";
        public const string _A_SearchResultsHeaderRowSize = "SearchResultsHeaderRowSize";
        public const string _A_TrackAudit = "TrackAudit";
        public const string _A_ViewersAddComments = "ViewersAddComments";
        public const string _A_ViewersEditAttachments = "ViewersEditAttachments";
		public const string _A_ViewersCannotSeeComplexLists = "ViewersCannotSeeComplexLists";
        public const string _A_XMLVersion = "XMLVersion";
        public const string _A_SupportMultipleDocuments = "SupportMultipleDocuments";


		//Template Attributes
        public const string _A_Action = "Action";
        public const string _A_ActionEnabled = "Enabled";
		public const string _A_ActionVisible = "Visible";
		public const string _A_ActionRequired = "Required";
		public const string _A_ActiveWorkflowID = "ActiveWorkflowID";
		public const string _A_Address1 = "Address1";
		public const string _A_Address1Name = "Address1Name";
		public const string _A_Address1NameDisplayed = "Address1NameDisplayed";
		public const string _A_Address1Value = "Address1Value";
		public const string _A_Address2 = "Address2";
		public const string _A_Address2Name = "Address2Name";
		public const string _A_Address2Value = "Address2Value";
		public const string _A_Address2NameDisplayed = "Address2NameDisplayed";
		public const string _A_AllowBackDating = "AllowBackDating";
		public const string _A_AllowComments = "AllowComments";
		public const string _A_AllRoles = "AllRoles";
		public const string _A_AllStatuses = "AllStatuses";
        public const string _A_AttachmentEditorType = "AttachmentEditorType";
        public const string _A_Attachments = "Attachments";
        public const string _A_BaseDateOffset = "BaseDateOffset";
		public const string _A_BaseDateTermID = "BaseDateTermID";
		public const string _A_BaseDateTermName = "BaseDateTermName";	//TODO - For backward compatibility
		public const string _A_BaseDateTermPart = "BaseDateTermPart";
		public const string _A_BigText = "BigText";
		public const string _A_BreakParagraphs = "BreakParagraphs";
		public const string _A_ButtonText = "ButtonText";
		public const string _A_CanGenerateDocument = "GenerateDocument";
        public const string _A_CanGenerateUserDocuments = "GenerateUserDocuments";
		public const string _A_Caption = "Caption";
		public const string _A_CenterText = "CenterText";
		public const string _A_ChildNumberingScheme = "ChildNumberingScheme";
		public const string _A_CityName = "CityName";
		public const string _A_CityNameDisplayed = "CityNameDisplayed";
		public const string _A_CityValue = "CityValue";
		public const string _A_Code = "Code";
		public const string _A_ColumnCount = "ColumnCount";
		public const string _A_Comments = "Comments";
		public const string _A_ComplexListID = "ComplexListID";
        public const string _A_ConfirmationText = "ConfirmationText";
		public const string _A_Created = "Created";
		public const string _A_DateOffset = "DateOffset";
		public const string _A_DateOffsetType = "DateOffsetType";
		public const string _A_DaysAfterToday = "DaysAfterToday";
		public const string _A_DaysAfterWorkflowEntry = "DaysAfterWorkflowEntry";
		public const string _A_DBFieldName = "DBFieldName";
		public const string _A_Default = "Default";
		public const string _A_DefaultValue = "DefaultValue";
		public const string _A_Deletable = "Deletable";
		public const string _A_DependentTerm = "DependentTerm";	// Still need for backward compatibility
		public const string _A_DependentTermID = "DependentTermID";
		public const string _A_DependsOnOperator = "DependsOnOperator";
		public const string _A_DependsOnTermID = "DependsOnTermID";
		public const string _A_DependsOnTermName = "DependsOnTermName";
		public const string _A_DependsOnValue = "DependsOnValue";
        public const string _A_Description = "Description";
        public const string _A_DetailedDescriptionType = "DetailedDescriptionType";
		public const string _A_DetailID = "DetailID";
		public const string _A_Display = "Display";
		public const string _A_DisplayedDate = "DisplayedDate";
		public const string _A_DisplayFormat = "DisplayFormat";
		public const string _A_DisplayOrder = "DisplayOrder";
		public const string _A_DisplayName = "DisplayName";
		public const string _A_DisplayWidth = "DisplayWidth";
        public const string _A_Document_DefaultDocument = "DefaultDocument";        
        public const string _A_Document_Name = "DocumentName";
        public const string _A_Document_WorkflowEnabled = "WorkflowEnabled";
		public const string _A_Editable = "Editable";
		public const string _A_EffectiveDateFormat = "EffectiveDateFormat";
		public const string _A_EventType = "EventType";
		public const string _A_Executed = "Executed";
		public const string _A_ExecutionDate = "ExecutionDate";
		public const string _A_ExcludeFromFirstPage = "ExcludeFromFirstPage";
		public const string _A_ExpirationDateFormat = "ExpirationDateFormat";
		public const string _A_FacilitySort = "FacilitySort";
		public const string _A_FacilityStatus = "FacilityStatus";
        public const string _A_FieldID = "FieldID";
        public const string _A_FieldName = "FieldName";
		public const string _A_FieldValue = "FieldValue";
		public const string _A_FileName = "FileName";
		public const string _A_Filter = "Filter";
        public const string _A_FilterFacilityTermID = "FilterFacilityTermID";
		public const string _A_Format = "Format";
		public const string _A_GeneratedDocument = "GeneratedDocument";
        public const string _A_GeneratedDocumentID = "GeneratedDocumentID";
		public const string _A_HangingIndent = "HangingIndent";
		public const string _A_ID = "ID";
		public const string _A_IncludeChildren = "IncludeChildren";
		public const string _A_IndentFirstParagraph = "IndentFirstParagraph";
		public const string _A_IndentSubsequentParagraphs = "IndentSubsequentParagraphs";
		public const string _A_InterfaceConfigName = "InterfaceConfigName";
        public const string _A_IsActive = "IsActive";
		public const string _A_IsBase = "IsBase";
		public const string _A_IsDraft = "IsDraft";
		public const string _A_IsExit = "IsExit";
        public const string _A_IsHeader = "IsHeader";
		public const string _A_IsKey = "IsKey";
		public const string _A_IsManagedItemReference = "IsManagedItemReference";
		public const string _A_IsPrimary = "IsPrimary";
		public const string _A_IsRef = "IsRef";
		public const string _A_ITATImageElement = "itatImage";
		public const string _A_ITATSystemID = "ITATSystemID";
		public const string _A_Key = "Key";
		public const string _A_KeywordSearchable = "KeywordSearchable";
		public const string _A_LeftText = "LeftText";
		public const string _A_LimitedByFacility = "LimitedByFacility";
		public const string _A_LinkSource = "LinkSource";
		public const string _A_Location = "Location";
		public const string _A_ManagedItemNumber = "ManagedItemNumber";
        public const string _A_Max = "Max";
        public const string _A_Min = "Min";
        public const string _A_MSOName = "MSOName";
        public const string _A_MSONameDisplayed = "MSONameDisplayed";
        public const string _A_MSOValue = "MSOValue";
        public const string _A_MultiSelect = "MultiSelect";
        public const string _A_MultiSelectSearch = "MultiSelectSearch";
        public const string _A_Name = "Name";
        public const string _A_NameValuePair = "NameValuePair";
        public const string _A_NameValuePairs = "NameValuePairs";
        public const string _A_ObjectID = "ObjectID";
        public const string _A_OffsetDays = "OffsetDays";
        public const string _A_OffsetDefaultValue = "OffsetDefaultValue";
		public const string _A_OffsetTermID = "OffsetTermID";
		public const string _A_OffsetTermName = "OffsetTermName";	//TODO - For backward compatibility
        public const string _A_Oper = "Oper";
        public const string _A_PageBreakBefore = "PageBreakBefore";
		public const string _A_PhoneName = "PhoneName";
		public const string _A_PhoneNameDisplayed = "PhoneNameDisplayed";
		public const string _A_PhoneValue = "PhoneValue";
        public const string _A_PlaceHolderType = "PlaceHolderType";
        public const string _A_PopUpIfNot = "PopUpIfNot";
		public const string _A_PopUpText = "PopUpText";
        public const string _A_PreserveWhiteSpaceDocument = "PreserveWhiteSpaceDocument";
        public const string _A_PreserveWhiteSpaceSummary = "PreserveWhiteSpaceSummary";
        public const string _A_Quantity = "Quantity";
        public const string _A_RemoveBlank = "RemoveBlank";
		public const string _A_RenewalCount = "RenewalCount";
		public const string _A_RenewalDurationEditable = "RenewalDurationEditable";
		public const string _A_RenewalTermType = "RenewalTermType";
		public const string _A_RenewalType = "RenewalType";
        public const string _A_ReportID = "ReportID";
        public const string _A_Required = "Required";
		public const string _A_RequiredSelectedValue = "RequiredSelectedValue";
        public const string _A_RequiresConfirmation = "RequiresConfirmation";
        public const string _A_RequiresValidation = "RequiresValidation";
        public const string _A_RetroModel = "RetroModel";
		public const string _A_RightText = "RightText";
		public const string _A_Role = "Role";
		public const string _A_ScheduleBaseDate = "ScheduleBaseDate";
		public const string _A_ScheduledEventId = "ScheduledEventID";
        public const string _A_SecurityModel = "SecurityModel";
		public const string _A_Selectable = "Selectable";
		public const string _A_Selected = "Selected";
		public const string _A_SelectionMode = "SelectionMode";
		public const string _A_SendNotification = "SendNotification";
		public const string _A_SendNotificationStatus = "SendNotificationStatus";
		public const string _A_Sequence = "Sequence";
		public const string _A_SettingName = "Name";
		public const string _A_SetValue = "SetValue";
		public const string _A_ShowOnItemSummary = "ShowOnItemSummary";
		public const string _A_ShowCents = "ShowCents";
		public const string _A_SortOrder = "SortOrder";
        public const string _A_SourceTerm = "SourceTerm";	//Still need for backward compatibility
        public const string _A_SourceTermID = "SourceTermID";
        public const string _A_StateName = "StateName";
		public const string _A_StateNameDisplayed = "StateNameDisplayed";
		public const string _A_StateValue = "StateValue";
		public const string _A_Status = "Status";
		public const string _A_Subject = "Subject";
		public const string _A_SuppressSpacingBefore = "SuppressSpacingBefore";
		public const string _A_Summary = "Summary";
		public const string _A_SystemTerm = "SystemTerm";
		public const string _A_TargetState = "TargetState";
        public const string _A_TargetStateID = "TargetStateID";
        public const string _A_Target = "Target";
        public const string _A_TermGroupID = "TermGroupID";
        public const string _A_TermGroupSecurityType = "TermGroupSecurityType";
		public const string _A_Text = "Text";
        public const string _A_TransformTermType = "TransformTermType";
        public const string _A_Type = "Type";
		public const string _A_TypeID = "TypeID";
		public const string _A_UserID = "UserID";
		public const string _A_UserName = "UserName";
		public const string _A_URL = "URL";
		public const string _A_UseDBField = "UseDBField";
		public const string _A_UseDetailedDescription = "UseDetailedDescription";
		public const string _A_UseFunction = "UseFunction";
		public const string _A_UseTextNumberFormat = "UseTextNumberFormat";
		public const string _A_UseUserSecurity = "UseUserSecurity";
		public const string _A_ValidateOnSave = "ValidateOnSave";
		public const string _A_Value = "Value";
        public const string _A_Value1 = "Value1";
        public const string _A_Value2 = "Value2";
        public const string _A_Version = "Version";
		public const string _A_Visible = "Visible";
		public const string _A_ZipName = "ZipName";
		public const string _A_ZipNameDisplayed = "ZipNameDisplayed";
		public const string _A_ZipValue = "ZipValue";



		//System Elements
		public const string _E_ApplicationFunction = "ApplicationFunction";
		public const string _E_ApplicationFunctions = "ApplicationFunctions";
		public const string _E_DocumentStoreConfig = "DocumentStoreConfig";
		public const string _E_DocumentType = "DocumentType";
		public const string _E_DocumentTypes = "DocumentTypes";
		public const string _E_ExternalInterface = "ExternalInterface";
		public const string _E_ExternalInterfaces = "ExternalInterfaces";
		public const string _E_IntroPage = "IntroPage";

		//Template Elements
		public const string _E_Action = "Action";
		public const string _E_Actions = "Actions";
		public const string _E_AlternateFooter = "AlternateFooter";
		public const string _E_AlternateHeader = "AlternateHeader";
        public const string _E_AttachmentRemover = "AttachmentRemover";
        public const string _E_AttachmentRemovers = "AttachmentRemovers";
		public const string _E_AvailableField = "AvailableField";
		public const string _E_AvailableFields = "AvailableFields";
		public const string _E_Body = "Body";
		public const string _E_Clause = "Clause";
		public const string _E_Comment = "Comment";
		public const string _E_Comments = "Comments";
		public const string _E_ComplexList = "ComplexList";
		public const string _E_ComplexLists = "ComplexLists";
        public const string _E_Config = "Config";
        public const string _E_Date = "Date";
		public const string _E_DateExitedBaseState = "DateExitedBaseState";
        public const string _E_DependentTerm = "DependentTerm";
        public const string _E_DependentTerms = "DependentTerms";
        public const string _E_Description = "Description";
		public const string _E_DetailedDescription = "DetailedDescription";
		public const string _E_DetailedDescriptions = "DetailedDescriptions";
		public const string _E_Document = "Document";
        public const string _E_Documents = "Documents";
        public const string _E_DocumentPrinter = "DocumentPrinter";
        public const string _E_DocumentPrinters = "DocumentPrinters";
        public const string _E_UserDocumentPrinter = "UserDocumentPrinter";
        public const string _E_UserDocumentPrinters = "UserDocumentPrinters";
        public const string _E_DurationUnit = "DurationUnit";
		public const string _E_Editor = "Editor";
		public const string _E_Editors = "Editors";
		public const string _E_EffectiveDate = "EffectiveDate";
		public const string _E_Event = "Event";
		public const string _E_Events = "Events";
		public const string _E_ExpirationDate = "ExpirationDate";
		public const string _E_External = "External";
		public const string _E_Extension = "Extension";
		public const string _E_Extensions = "Extensions";
		public const string _E_Facility = "Facility";
		public const string _E_FacilityType = "FacilityType";
		public const string _E_FacilityID = "FacilityID";
		public const string _E_FacilityTypes = "FacilityTypes";
		public const string _E_Field = "Field";
		public const string _E_Fields = "Fields";
		public const string _E_Footer = "Footer";
		public const string _E_Header = "Header";
		public const string _E_Item = "Item";
		public const string _E_Items = "Items";
		public const string _E_ItemValue = "ItemValue";
		public const string _E_Link = "Link";
        public const string _E_ListItem = "ListItem";
        public const string _E_Message = "Message";
		public const string _E_Messages = "Messages";
		public const string _E_MSO = "MSO";
		public const string _E_InitialDurationUnit = "InitialDurationUnit";
		public const string _E_InitialDurationUnitCount = "InitialDurationUnitCount";
		public const string _E_NameValuePair = "NameValuePair";
		public const string _E_NameValuePairs = "NameValuePairs";
		public const string _E_NameValueList = "NameValueList";
		public const string _E_OwningFacilityIDs = "OwningFacilityIDs";
		public const string _E_Performer = "Performer";
		public const string _E_Performers = "Performers";
		public const string _E_PickList = "PickList";
        public const string _E_PlaceHolder = "PlaceHolder";
        public const string _E_PlaceHolderAttachments = "PlaceHolderAttachments";
        public const string _E_PlaceHolderComments = "PlaceHolderComments";
        public const string _E_Recipient = "Recipient";
		public const string _E_Recipients = "Recipients";
		public const string _E_Rendering = "Rendering";
		public const string _E_Renewal = "Renewal";
		public const string _E_RenewalDurationUnit = "RenewalDurationUnit";
		public const string _E_RenewalDurationUnitCount = "RenewalDurationUnitCount";
		public const string _E_Renewers = "Renewers";
		public const string _E_Renewer = "Renewer";
		public const string _E_Report = "Report";
		public const string _E_Role = "Role";
		public const string _E_Roles = "Roles";
		public const string _E_Rule = "Rule";
		public const string _E_RuleItem = "RuleItem";
		public const string _E_RuleItemDetail = "RuleItemDetail";
		public const string _E_RuleItemDetails = "RuleItemDetails";
		public const string _E_Rules = "Rules";
        public const string _E_ScannedAttachmentRemover = "ScannedAttachmentRemover";
        public const string _E_ScannedAttachmentRemovers = "ScannedAttachmentRemovers";
        public const string _E_ScheduledEvent = "ScheduledEvent";
		public const string _E_ScheduledEvents = "ScheduledEvents";
		public const string _E_SearchableField = "SearchableField";
		public const string _E_SearchableFields = "SearchableFields";
		public const string _E_SelectedFacilityIDs = "SelectedFacilityIDs";
		public const string _E_Setting = "Setting";
		public const string _E_Settings = "Settings";
		public const string _E_StandardFooter = "StandardFooter";
		public const string _E_StandardHeader = "StandardHeader";
		public const string _E_State = "State";
		public const string _E_States = "States";
        public const string _E_StateTermGroup = "StateTermGroup";
        public const string _E_StateTermGroups = "StateTermGroups";
        public const string _E_status = "status";
        public const string _E_Status = "Status";
		public const string _E_Statuses = "Statuses";
		public const string _E_SystemDef = "SystemDef";
		public const string _E_TemplateDef = "TemplateDef";
		public const string _E_Term = "Term";
        public const string _E_TermGroup = "TermGroup";
        public const string _E_TermGroups = "TermGroups";
        public const string _E_Terms = "Terms";
		public const string _E_TermDependencies = "TermDependencies";
		public const string _E_TermDependency = "TermDependency";
		public const string _E_TermDependencyAction = "Action";
		public const string _E_TermDependencyCondition = "Condition";
		public const string _E_TermDependencyConditions = "Conditions";
		public const string _E_Text = "Text";
        public const string _E_Transform = "Transform";
        public const string _E_Transforms = "Transforms";
        public const string _E_validateOn = "validateOn";
        public const string _E_Value = "Value";
        public const string _E_Viewer = "Viewer";
        public const string _E_Viewers = "Viewers";
        public const string _E_Workflow = "Workflow";
		public const string _E_Workflows = "Workflows";


		//Miscellaneous

		public const string _M_TemplateXMLVersion_T2006NOV01 = "T2006NOV01";
		/*
		 * The "TemplateXMLVersion" reflects any changes in the structure of the template xml that affects the code.
		 * In other words, a new TemplateXMLVersion is generated whenever a change is made to the structure of the
		 * basic template that is not backward compatible with the previous template structure, in terms of the
		 * code.  So if such a change occurs, the code will know in advance how to interpret the template xml.
		 * This mechanism allows the same code base to work with any future (unanticipated) changes in the template
		 * xml structure.  This version string should not change very often, if at all, throughout the lifetime
		 * of this application.
		 * 
		 */
		public const string _M_TemplateXMLVersion_T2007OCT17 = "T2007OCT17";
		/*
		 * TODO - Enter description of version here
		*/

		public static string[] _M_TemplateXMLVersions = 
			{ 
				_M_TemplateXMLVersion_T2006NOV01, 
				_M_TemplateXMLVersion_T2007OCT17
			};

		public const char _M_Delimiter = ' ';
		public const string _M_NameSpaceURI = "";
		public const string _M_ZeroBaseOffset = "0";
		public const string _M_EnvironmentHolder = "_INSERT_ENVIRONMENT_NAME_HERE_";
		public const string _M_SystemNameHolder = "_INSERT_SYSTEM_NAME_HERE_";
		public const string _M_ManagedItemIdHolder = "_INSERT_MANAGED_ITEM_ID_HERE_";
		public const string _M_TallComponents_URL = "http://www.tallcomponents.com/schemas/tallpdf/v3";
		public const string _M_TermImageTemplate = @"<img.class=""TermImg"".src="".*?TextImage.ashx\?text=(.+?)""\s?/>";

        public const string _M_FieldImageTemplate = @"&lt;img class=&quot;TermImg&quot; src=&quot;TextImage.ashx\?text=(.+?)&quot;";

        public const string _M_ImageTemplate = @"<img.*?(style="".*?WIDTH:\s?(?<width>\d+?)px;.*?HEIGHT:\s?(?<height>\d+?)px;?.*?"")?\s?(hspace=""(?<hspace>\d+?)"")?\s?src="".*?ShowImage.ashx\?id=(?<id>.+?)""\s?(vspace=""(?<vspace>\d+?)"")?\s?/>";
		public const string _M_TermImageFindPattern = @"<img class=""TermImg"" src=""TextImage.ashx?text={0}"" />";
		public const int _M_SizeBigText = 2000;
		public const string _M_EmailFrom = "ApplicationReply@kindredhealthcare.com";		//Based on HCMS project, Settings table, SenderAddress
		public const string _M_AllStatuses = "All";
		public const string _M_TermID = "__EMBEDDED_TERMID__:";
		public const string _M_ExternalTerm = "External Term";
		public const string _M_NotificationBaseDateTermSeparator = "~~~";
        public const string _M_FieldID = "__EMBEDDED_FIELDID__:";

		//Application Functions - list also found in System XML (Security Roles)
		public const string _AF_EditLanguage = "EditLanguage";
		public const string _AF_EditTemplate = "EditTemplate";
		public const string _AF_ImportData = "ImportData";
		public const string _AF_DeleteReport = "DeleteReport";
        public const string _AF_EditAttachment = "EditAttachment";      //TODO: Get rid of this once it is no longer needed (after the introduction of release 1.6)
        public const string _AF_AdminViewer = "AdminViewer";
        public const string _AF_RetroAdmin = "RetroAdmin";
        public const string _AF_AdminRecipient = "AdminRecipient";
        public const string _AF_ITATAdmin = "ITATAdmin";

		//Attribute Values
		public const string _AV_ExternalTerm = "ExternalTerm";
		public const string _AV_ReferenceManagedItem = "ReferenceManagedItem";
		public const string _AV_URL = "URL";
		public const string _AV_ComplexList = "ComplexList";

        //Term Transform Info
        public const string _XFRM_IncludeChildren = "IncludeChildren";
        public const string _XFRM_SelectedFacilityIDs = "SelectedFacilityIDs";

		//Term Part Specifiers
		public const string _TPS_None = "";

		//Term Part Specifiers - RenewalTerm
		public const string _TPS_InitialTermDuration = "Initial Term Duration";		//Initial Duration?
		public const string _TPS_RenewalTermDuration = "Renewal Term Duration";		//Renewal Duration?
		public const string _TPS_TerminationDate = "Termination Date";   //OBSOLETE; retained for backward compatibility
		public const string _TPS_RenewalDate = "Renewal Date";
		public const string _TPS_ExpirationDate = "Expiration Date";
		public const string _TPS_EffectiveDate = "Effective Date";

		//Term Part Specifiers - FacilityTerm
		public const string _TPS_SAPID = "SAP ID";
		public const string _TPS_FacilityType = "FacilityType";
		public const string _TPS_Address = "Address";
		public const string _TPS_City = "City";
		public const string _TPS_County = "County";
		public const string _TPS_State = "State";
		public const string _TPS_StateCode = "StateCode";
		public const string _TPS_Zip = "Zip";
		public const string _TPS_AreaCode = "Area Code";
		public const string _TPS_Phone = "Phone";
		public const string _TPS_Fax = "Fax";
		public const string _TPS_FacilityName = "Facility Name";
		public const string _TPS_LegalEntityName = "Legal Entity Name";

		//ChildNumberingSchemeType
		public const string _CNST_A = "A";
		public const string _CNST_a = "a";
		public const string _CNST_I = "I";
		public const string _CNST_i = "i";
		public const string _CNST_1 = "1";
		public const string _CNST_None = "";

		//NotificationType
		public const string _NOTIFICATION_TYPE_ASTER = "*";
		public const string _NOTIFICATION_TYPE_W = "W";
		public const string _NOTIFICATION_TYPE_E = "E";
		public const string _NOTIFICATION_TYPE_R = "R";
		public const string _NOTIFICATION_TYPE_RETROREVERT = "RR";

		//System Defined Values

		//ManagedItemNumberSystemType
		public const string _SYSTEM_MINST_Fac_SAP4 = "Fac:SAP4";		//TODO - Should tie in with FacilityTerm.GetFacilityDisplayValue (_TPS_SAPID)

		//Term Dependency Operators
		public const string _TermDependencyOperator_Equals = "Equals";
		public const string _TermDependencyOperator_GreaterThan = "Is Greater Than";
		public const string _TermDependencyOperator_NoLessThan = "Is No Less Than";
		public const string _TermDependencyOperator_LessThan = "Is Less Than";
		public const string _TermDependencyOperator_NoMoreThan = "Is No More Than";
		public const string _TermDependencyOperator_Between = "Is Between";
		public const string _TermDependencyOperator_NotEqual = "Is Not Equal To";
		public const string _TermDependencyOperator_Contains = "Contains";
		public const string _TermDependencyOperator_StartsWith = "Starts With";
		public const string _TermDependencyOperator_EndsWith = "Ends With";
		public const string _TermDependencyOperator_InRole = "InRole";
		public const string _TermDependencyOperator_NotInRole = "NotInRole";

		//Term Dependency Actions
		public const string _TermDependencyAction_Enabled = "Enabled";
		public const string _TermDependencyAction_Visible = "Visible";
		public const string _TermDependencyAction_Required = "Required";

        //Special Term Names for the Rad Editor
        public const string _SpecialTermName_WorkflowState = "Workflow State";
        public const string _SpecialTermName_Status = "Status";
        public const string _SpecialTermName_Number = " Number";

	}
}
