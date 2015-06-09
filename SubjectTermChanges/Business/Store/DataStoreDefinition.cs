using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace Kindred.Knect.ITAT.Business
{
    public enum LoadType
    { 
        Full = 0,
        Delta = 1
    }

    public enum DefinitionStatus
    { 
        Inactive = 0,
        Active = 1
    }
    public class DataStoreDefinition
    {
        #region private members

        private Guid _dataStoreDefinitionID;
        private string _name;
        private string _description;
        private bool _active;                   
        private string _definition;
        private DateTime? _lastRunDate;
        private Guid _systemID;
        private DataStoreConfig _dataStoreConfig;
        private DateTime _lastModifiedDate;
        private string _definitionFilePath;
        private readonly ITATSystem _system;
        #endregion


        #region Public Properties

        public Guid DataStoreDefinitionID
        {
            get { return _dataStoreDefinitionID; }
            set { _dataStoreDefinitionID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Guid SystemID
        {
            get { return _systemID; }
            set { _systemID = value; }
        }

        public DateTime? LastRunDate
        {
            get { return _lastRunDate; }
            set { _lastRunDate = value; }
        }


        public string Definition
        {
            get { return _definition; }
            set { _definition = value; }
        }

        public DateTime LastModifiedDate
        {
            get { return _lastModifiedDate; }
            set { _lastModifiedDate = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

    
        public DataStoreConfig DataStoreConfig
        {
            get {
                if (_dataStoreConfig == null)
                    _dataStoreConfig = new DataStoreConfig(_system);
                return _dataStoreConfig;
            }
            set { _dataStoreConfig = value; }
        }


        public string DefinitionFilePath
        {
            get { return _definitionFilePath; }
            set { _definitionFilePath = value; }
        }
        #endregion

        public DataStoreDefinition(ITATSystem system)
        {
            _system = system;
        }

        public DataStoreDefinition(Guid dataStoreDefinitionID, ITATSystem system)
        {
            _dataStoreDefinitionID = dataStoreDefinitionID;
            _system = system;
        }

        public static DataTable GetDataStoreDefinitionsBySystemID(Guid systemID)
        {
            return Data.DataStoreDefinitions.GetDataStoreDefinitionBySystemID(systemID);
        }

        public static DataTable GetDataStoreDefinitionsBySystemID(Guid systemID,bool isActive)
        {
            return Data.DataStoreDefinitions.GetDataStoreDefinitionBySystemID(systemID, isActive);
        }

        public static bool CheckDataStoreDefinitionExists(string name)
        {
            return Data.DataStoreDefinitions.CheckDataStoreDefinitionExists(name);
        }

        public static DataStoreDefinition GetDataStoreDefinitionByID(
            Guid dataStoreDefinitionID, 
            ITATSystem system, 
            Dictionary<Guid, Dictionary<Guid, string>> termNameLookup, 
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<Guid /*FieldID*/, string /*FieldName*/>>> fieldNameLookup,
            bool testTermName)
        {
            DataStoreDefinition rtn = null;
            DataTable dataStoreDef = Data.DataStoreDefinitions.GetDataStoreDefinitionsByDefinitionID(dataStoreDefinitionID);
            if (dataStoreDef.Rows.Count > 0)
            {
                try 
                {
                    rtn = new DataStoreDefinition((Guid)dataStoreDef.Rows[0][StoreNames._C_DEFINITION_ID], system);
                    rtn.Name = dataStoreDef.Rows[0][StoreNames._C_DEFINITION_NAME].ToString();
                    if (dataStoreDef.Rows[0][StoreNames._C_DESCRIPTION] == DBNull.Value)
                        rtn.Description = string.Empty;
                    else
                        rtn.Description = dataStoreDef.Rows[0][StoreNames._C_DESCRIPTION].ToString();
                    rtn.SystemID = (Guid)dataStoreDef.Rows[0][StoreNames._C_SYSTEM_ID];
                    if (dataStoreDef.Rows[0][StoreNames._C_LASTRUNDATE] == DBNull.Value)
                        rtn.LastRunDate = null;
                    else
                        rtn.LastRunDate = DateTime.Parse(dataStoreDef.Rows[0][StoreNames._C_LASTRUNDATE].ToString());
                    rtn.Definition = dataStoreDef.Rows[0][StoreNames._C_DEFINITION].ToString();
                    rtn.Active = Convert.ToBoolean(dataStoreDef.Rows[0][StoreNames._C_ACTIVE].ToString());
                    rtn.DataStoreConfig = new DataStoreConfig(dataStoreDef.Rows[0][StoreNames._C_DEFINITION].ToString(), rtn.SystemID, termNameLookup, fieldNameLookup,testTermName);
                    
                    if (dataStoreDef.Rows[0][StoreNames._C_DEFINITIONFILEPATH] == DBNull.Value)
                        rtn.DefinitionFilePath = string.Empty;
                    else
                        rtn.DefinitionFilePath = dataStoreDef.Rows[0][StoreNames._C_DEFINITIONFILEPATH].ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }


            return rtn;

        }

        public static Guid AddDataStoreDefinition(DataStoreDefinition dataStoreDefinition, 
            Dictionary<string /*TemplateName*/, Guid /*TemplateID*/> templateIDLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<string /*TermName*/, Guid /*TermID*/>> termIDLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<string /*FieldName*/, Guid /*FieldID*/>>> fieldIDLookup)
        {
            try
            {
                return Data.DataStoreDefinitions.InsertDataStoreDefinition(dataStoreDefinition.Name, dataStoreDefinition.Description,
                                                                            dataStoreDefinition.SystemID,
                                                                            GetDataStoreDefinitionXML(dataStoreDefinition.DataStoreConfig, dataStoreDefinition.Name, dataStoreDefinition.Description, templateIDLookup, termIDLookup, fieldIDLookup), dataStoreDefinition.Active);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static string GetDataStoreDefinitionXML(
            Business.DataStoreConfig dataStoreConfig, 
            string definitionName, 
            string description,
            Dictionary<string /*TemplateName*/, Guid /*TemplateID*/> templateIDLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<string /*TermName*/, Guid /*TermID*/>> termIDLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<string /*FieldName*/, Guid /*FieldID*/>>> fieldIDLookup)
        {
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "UTF-16", null));
            
            XElement xEleDataStoreDefinition = new XElement(StoreNames._XE_DATASTOREDEFINITION);
            XElement xEleTemplates = new XElement(StoreNames._XE_TEMPLATES);
            foreach (KeyValuePair<Guid,string> template in dataStoreConfig.Templates)
            {
                XElement xEleTemplate = new XElement(StoreNames._XE_TEMPLATE);
                xEleTemplate.Value = template.Key.ToString();
                xEleTemplates.Add(xEleTemplate);
            }

            xEleDataStoreDefinition.Add(xEleTemplates);
            XElement xEleTerms = new XElement(StoreNames._XE_TERMS);
            foreach (DataStoreField dsf in dataStoreConfig.Terms)
            {
                XElement xEleTerm = new XElement(StoreNames._XE_TERM);
                //'Name' is a concatenation of TemplateName|TermName|Field
                xEleTerm.SetAttributeValue(StoreNames._XA_TERM_NAME, DataStoreConfig.StoredTermName(dsf.Name, templateIDLookup, termIDLookup, fieldIDLookup, dsf.IsStandard, dsf.TermType));
                xEleTerm.SetAttributeValue(StoreNames._XA_TERM_ALIAS, dsf.Alias);
                xEleTerm.SetAttributeValue(StoreNames._XA_TERM_SIZE, dsf.Length);
                xEleTerm.SetAttributeValue(StoreNames._XA_TERM_TYPE, dsf.TermType);
                xEleTerm.SetAttributeValue(StoreNames._XA_TERM_TEXTFORMAT, dsf.TextTermFormat);
                xEleTerm.SetAttributeValue(StoreNames._XA_TERM_ISSTANDARD, dsf.IsStandard ? "true" : "false");
                xEleTerms.Add(xEleTerm);
            }

            xEleDataStoreDefinition.Add(xEleTerms);
            XElement xEleCriteria = new XElement(StoreNames._XE_CRITERIA);
            XElement xEleTerm1 = new XElement(StoreNames._XE_TERM_1);
            if (dataStoreConfig.SearchCriteria.TextTerm1!=null)
            { 
                xEleTerm1.Value = dataStoreConfig.SearchCriteria.TextTerm1;
            }

            xEleCriteria.Add(xEleTerm1);
            XElement xEleTerm2 = new XElement(StoreNames._XE_TERM_2);
            if (dataStoreConfig.SearchCriteria.TextTerm2 != null)
            {
                xEleTerm2.Value = dataStoreConfig.SearchCriteria.TextTerm2;
            }

            xEleCriteria.Add(xEleTerm2);
            XElement xEleTerm3 = new XElement(StoreNames._XE_TERM_3);
            xEleTerm3.Value = dataStoreConfig.NoOfDays.ToString();
            xEleCriteria.Add(xEleTerm3);


            XElement xEleTerm4 = new XElement(StoreNames._XE_TERM_4);
            if (dataStoreConfig.SearchCriteria.TextTerm4 != null)
            {
                xEleTerm4.Value = dataStoreConfig.SearchCriteria.TextTerm4;
            }
            xEleCriteria.Add(xEleTerm4);


            XElement xEleTerm5 = new XElement(StoreNames._XE_TERM_5);
            if (dataStoreConfig.SearchCriteria.TextTerm5 != null)
            {
                xEleTerm5.Value = dataStoreConfig.SearchCriteria.TextTerm5;
            }
            xEleCriteria.Add(xEleTerm5);

            XElement xElePrimaryFacility = new XElement(StoreNames._XE_PRIMARY_FACILITY);

            if (!dataStoreConfig.PrimaryFacility.ToString().Equals(string.Empty))
            {
                xElePrimaryFacility.Value = dataStoreConfig.PrimaryFacility.ToString();
            }

            xEleCriteria.Add(xElePrimaryFacility);

            XElement xEleLoadType = new XElement(StoreNames._XE_LOAD_TYPE);

            xEleLoadType.Value = dataStoreConfig.LoadType.ToString();

            xEleCriteria.Add(xEleLoadType);

            XElement xEleDeltaDays = new XElement(StoreNames._XE_DELTA_DAYS);

            xEleDeltaDays.Value = dataStoreConfig.DeltaDays.ToString();

            xEleCriteria.Add(xEleDeltaDays);

            XElement xEleStatuses = new XElement(StoreNames._XE_STATUSES);


            foreach (string status in dataStoreConfig.SearchCriteria.Statuses)
            {
                XElement xEleStatus = new XElement(StoreNames._XE_STATUS);
                xEleStatus.Value = status;
                xEleStatuses.Add(xEleStatus);
            }

            xEleCriteria.Add(xEleStatuses);

            xEleDataStoreDefinition.Add(xEleCriteria);

            XElement xEleDefaultDateFromat = new XElement(StoreNames._XE_DEFAULT_DATE_FORMAT);
            xEleDefaultDateFromat.Value = dataStoreConfig.DefaultDateFormat;
            xEleDataStoreDefinition.Add(xEleDefaultDateFromat);

            XElement xElePath = new XElement(StoreNames._XE_PATH);
            xElePath.Value = dataStoreConfig.Path;
            xEleDataStoreDefinition.Add(xElePath);

            XElement xEleEmailAddresses = new XElement(StoreNames._XE_EMAIL_ADDRESSES);
            xEleEmailAddresses.Value = dataStoreConfig.ErrorLogRecepientEmail;
            xEleDataStoreDefinition.Add(xEleEmailAddresses);

            xDoc.Add(xEleDataStoreDefinition);

            return xDoc.ToString();
            
        }
     
        public static DataStoreDefinition GetDatatStoreDefinitionBySystemId(Guid SystemID)
        {
            ITATSystem system = ITATSystem.Get(SystemID);
            DataStoreDefinition rtn = null;
            DataTable dataStoreDef = Data.DataStoreDefinitions.GetDataStoreDefinitionBySystemID(SystemID);
            if (dataStoreDef.Rows.Count > 0)
            {
                try
                {
                    rtn = new DataStoreDefinition((Guid)dataStoreDef.Rows[0][StoreNames._C_DEFINITION_ID], system);
                    rtn.Name = dataStoreDef.Rows[0][StoreNames._C_DEFINITION_NAME].ToString();
                    rtn.Description = dataStoreDef.Rows[0][StoreNames._C_DESCRIPTION].ToString();
                    rtn.SystemID = (Guid)dataStoreDef.Rows[0][StoreNames._C_SYSTEM_ID];
                    rtn.LastRunDate = dataStoreDef.Rows[0][StoreNames._C_LASTRUNDATE].ToString().Length > 0 ? DateTime.Parse(dataStoreDef.Rows[0][Data.DataNames._C_LastRunDate].ToString()) : Convert.ToDateTime(null);
                    rtn.Definition = dataStoreDef.Rows[0][StoreNames._C_DEFINITION].ToString();
                    rtn.Active = Convert.ToBoolean(dataStoreDef.Rows[0][StoreNames._C_ACTIVE].ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return rtn;
        }


        public static Dictionary<Guid, string> GetSelectedTemplateListFromDef(string draftConfig, Guid systemID)
        { 
            //XElement xmlTree = XElement.Parse(draftConfig);
            XDocument xdoc = XDocument.Parse(draftConfig);

            IEnumerable<XElement> Templates = xdoc.Root.Descendants(StoreNames._XE_TEMPLATES);

            List<Guid> templateIds = new List<Guid>();

            foreach (var t in Templates)
            {
                templateIds.Add(new Guid(t.Value));
            }

            DataTable dtSystemTemplates = Business.Template.GetTemplateList(systemID, null).Tables[0];

            var finalTemplateList = from t in dtSystemTemplates.AsEnumerable()
                                    where templateIds.Contains(t.Field<Guid>(Data.DataNames._C_TemplateID))
                                    select new 
                                    {                                 
                                        Key = t.Field<Guid>(Data.DataNames._C_TemplateID),
                                        name = t.Field<string>(Data.DataNames._C_TemplateName).ToString()
                                      };

            return finalTemplateList.ToDictionary(x => x.Key,x=>x.name);
        }

        public static bool UpdatedDataStoreDefinition(
            DataStoreDefinition dataStoreDefinition, 
            Dictionary<string /*TemplateName*/, Guid /*TemplateID*/> templateIDLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<string /*TermName*/, Guid /*TermID*/>> termIDLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<string /*FieldName*/, Guid /*FieldID*/>>> fieldIDLookup)
        {
            return Data.DataStoreDefinitions.UpdateDataStoreDefinition(dataStoreDefinition.DataStoreDefinitionID, GetDataStoreDefinitionXML(dataStoreDefinition.DataStoreConfig, dataStoreDefinition.Name, dataStoreDefinition.Description, templateIDLookup, termIDLookup, fieldIDLookup), dataStoreDefinition.Description, dataStoreDefinition.Active);
        }

        public static bool UpdateDataStoreDefinitionOnRun(DataStoreDefinition dataStoreDefinition)
        {
            return Data.DataStoreDefinitions.UpdateDataStoreDefinitionOnRun(dataStoreDefinition.DataStoreDefinitionID, dataStoreDefinition.DefinitionFilePath, dataStoreDefinition.LastRunDate);
        }

        public static List<string> GetDateFormatList()
        {
            List<string> rtnDateFormatList = Utility.DateHelper.DateFormats;
            //rtnDateFormatList.RemoveRange(6, 4);
            return rtnDateFormatList;
        }
    }
}
