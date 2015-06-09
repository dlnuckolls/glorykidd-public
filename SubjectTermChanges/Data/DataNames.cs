using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Data
{
	public class DataNames
	{
		//Column field names

		public const string _C_AllowComments			= "AllowComments";
		public const string _C_Attachments				= "Attachments";

        public const string _C_AuditTypeID = "AuditTypeID";
        public const string _C_AuditTypeName = "AuditTypeName";

        public const string _C_Comments					= "Comments";
        public const string _C_DateOfChange             = "DateOfChange";
		public const string _C_Description				= "Description";

        public const string _C_DataStoreDefinitionID = "DataStoreDefinitionID";

        public const string _C_Deleted = "Deleted";
        public const string _C_DocumentDescription = "DocumentDescription";
        public const string _C_DocumentID = "DocumentID";
        public const string _C_DocumentName = "DocumentName";
        public const string _C_DocumentStoreID = "DocumentStoreID";
        public const string _C_DocumentType = "DocumentType";

		public const string _C_DraftTemplateDef			= "DraftTemplateDef";
		public const string _C_EventID = "EventID";
		public const string _C_Executed = "Executed";
		public const string _C_ExternalTermKeyValues = "ExternalTermKeyValues";
		public const string _C_FacilityID = "FacilityID";
		public const string _C_facility_name = "facility_name";

		public const string _C_FinalTemplateDef			= "FinalTemplateDef";
		public const string _C_GenerateDocument			= "GenerateDocument";
        public const string _C_GenerateUserDocuments = "GenerateUserDocuments";
        public const string _C_IsScanned = "IsScanned";
        public const string _C_ITATSystemDef = "ITATSystemDef";
		public const string _C_ITATSystemID				= "ITATSystemID";
		public const string _C_ITATSystemName			= "ITATSystemName";

        public const string _C_KeyValue = "KeyValue";
        public const string _C_LastRunDate = "LastRunDate";
        
        public const string _C_ManagedItemID = "ManagedItemID";
		public const string _C_ManagedItemNumber = "ManagedItemNumber";
        public const string _C_IsOrphaned = "IsOrphaned";
        public const string _C_ReportConfigXML = "ReportConfigXML";
        public const string _C_RetroDate = "RetroDate";
        public const string _C_RetroModel = "RetroModel";
        public const string _C_ScheduledEventDate = "ScheduledEventDate";
		public const string _C_ScheduledEventID = "ScheduledEventID";
        public const string _C_SecurityModel = "SecurityModel";
		public const string _C_State = "State";
        public const string _C_StateID = "StateID";
        public const string _C_Status = "Status";
		public const string _C_SystemID = "SystemID";
		public const string _C_TemplateDef				= "TemplateDef";
        public const string _C_TemplateDefZipped = "TemplateDefZipped";
        public const string _C_TemplateID = "TemplateID";
		public const string _C_TemplateName				= "TemplateName";
		public const string _C_TemplateStatus = "TemplateStatus";
		public const string _C_Term1 = "Term1";
		public const string _C_Term2					= "Term2";
		public const string _C_Term3					= "Term3";      //DateTerm1
        public const string _C_Term4                    = "Term4";
        public const string _C_Term5                    = "Term5";
        public const string _C_Term6                    = "Term6";      //DateTerm2
        public const string _C_Term7                    = "Term7";      //DateTerm3

		public const string _C_TermFacility		= "TermFacility";
		public const string _C_TermName = "TermName";
		public const string _C_TermType = "TermType";
		public const string _C_UseDetailedDescription = "UseDetailedDescription";

		public const string _C_Value = "Value";

		//Stored Procedure return values
		public const string _SP_FacilityName = "FacilityName";
		public const string _SP_FacilitySAPID = "FacilitySAPID";

		//Table Field Lengths
		public const int _FL_Term1 = 880;
		public const int _FL_Term2 = 880;
        public const int _FL_Term4 = 880;
        public const int _FL_Term5 = 880;

		//Misc
		public const int _FACID_CORPORATE = 452;

		//App Config keys 
		public const string _AC_ApplicationWebServer = ".ApplicationWebServer";  //don't delete the period at the beginning of this string
	}
}
