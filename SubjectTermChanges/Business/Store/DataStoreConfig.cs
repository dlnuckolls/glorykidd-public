using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data;
using Kindred.Knect.ITAT.Business;

namespace Kindred.Knect.ITAT.Business
{
    public class DataStoreConfig
    {
        #region Private Members
        private SearchCriteria _searchCriteria;
        private Dictionary<Guid, string> _templates;
        private List<DataStoreField> _terms;
        private int _noOfDays;
        private LoadType _loadType;
        private string _path;
        private string _primaryfacility;
        private string _errorlogrecepientemail;
        private int _deltadays;
        private string _defaultDateFormat;
        private readonly ITATSystem _system;

        #endregion


        #region Public Properties
        public SearchCriteria SearchCriteria
        {
            get
            {
                if (_searchCriteria == null)
                    _searchCriteria = new SearchCriteria();
                return _searchCriteria; 
            }
            set { _searchCriteria = value; }
        }

        public string DefaultDateFormat
        {
            get { return _defaultDateFormat; }
            set { _defaultDateFormat = value; }
        }
        public string ErrorLogRecepientEmail
        {
            get { return _errorlogrecepientemail; }
            set { _errorlogrecepientemail = value; }
        }

        public List<string> ErrorLogRecepientEmailList
        {
            get { return _errorlogrecepientemail.Split(';').ToList(); }
        }

        public Dictionary<Guid,string> Templates
        {
            get
            {
                if (_templates == null)
                    _templates = new Dictionary<Guid, string>();
                return _templates;
            }
            set { _templates = value; }
        }

        public List<DataStoreField> Terms
        {
            get
            {
                if (_terms == null)
                    _terms = new List<DataStoreField>();
                return _terms;
            }
            set { _terms = value; }
        }

        public int NoOfDays
        {
            get { return _noOfDays; }
            set { _noOfDays = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public string PrimaryFacility
        {
            get { return _primaryfacility; }
            set { _primaryfacility = value; }
        }
        public LoadType LoadType
        {
            get { return _loadType; }
            set { _loadType = value; }
        }

        public int DeltaDays
        {
            get { return _deltadays; }
            set { _deltadays = value; }
        }
        #endregion

        public DataStoreConfig(ITATSystem system)
        {
            _system = system;
        }

        public DataStoreConfig(
            string configXML, 
            Guid systemID, 
            Dictionary<Guid, Dictionary<Guid, string>> termNameLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<Guid /*FieldID*/, string /*FieldName*/>>> fieldNameLookup,
            bool testTermName)
        {
            this.Templates = GetSelectedTemplateListByID(configXML,systemID);
            //Note - System Terms are referenced by Term Name across templates.  Therefore, no need to use TermID's here...
            this.Terms = GetSelectedTermsFromDef(configXML, this.Templates, termNameLookup, fieldNameLookup, testTermName);
            XElement xmlTree = XElement.Parse(configXML);
            var Criteria = from item in xmlTree.Descendants(StoreNames._XE_CRITERIA)
                           select new
                           {
                               textTerm1 = (string)item.Element(StoreNames._XE_TERM_1),
                               textTerm2 = (string)item.Element(StoreNames._XE_TERM_2),
                               textTerm4 = (string)item.Element(StoreNames._XE_TERM_4),
                               textTerm5 = (string)item.Element(StoreNames._XE_TERM_5),
                               days = (string)item.Element(StoreNames._XE_TERM_3),
                               primaryFacility = (string)item.Element(StoreNames._XE_PRIMARY_FACILITY),
                               loadType = (LoadType)Enum.Parse(LoadType.Full.GetType(), (string)item.Element(StoreNames._XE_LOAD_TYPE), true),
                               deltadays = (string)item.Element(StoreNames._XE_DELTA_DAYS),
                               statuses = (from state in item.Element(StoreNames._XE_STATUSES).Descendants(StoreNames._XE_STATUS)
                                          orderby state.Value
                                           select state.Value).ToList()
                           };

            this.SearchCriteria.TextTerm1 = Criteria.First().textTerm1;
            this.SearchCriteria.TextTerm2 = Criteria.First().textTerm2;
            this.SearchCriteria.TextTerm4 = Criteria.First().textTerm4;
            this.SearchCriteria.TextTerm5 = Criteria.First().textTerm5;
            //TODO - Need to reintroduce the date search info whenever it is included.
            this.SearchCriteria.DateTerm3Range = new SearchCriteria.DateRange();
            this.SearchCriteria.DateTerm6Range = new SearchCriteria.DateRange();
            this.SearchCriteria.DateTerm7Range = new SearchCriteria.DateRange();

            this.PrimaryFacility = Criteria.First().primaryFacility;
            if (this.PrimaryFacility.Length > 0)
                this.SearchCriteria.FacilityIds = this.PrimaryFacility.Split(',').ToList<string>().ConvertAll<int>(delegate(string i) { return int.Parse(i); }); ;
            this.LoadType = Criteria.First().loadType;
            this.DefaultDateFormat = GetDefaultDateFormatDefinition(configXML); ;
            int.TryParse(Criteria.First().deltadays.ToString(), out _deltadays);
            this.SearchCriteria.Statuses = Criteria.First().statuses;
            this.Path = GetPathfromDefinition(configXML);
            this.ErrorLogRecepientEmail = GetEmailAddressesfromDefinition(configXML);
        }

        public static Dictionary<Guid, string> GetSelectedTemplateListByID(string ConfigXML, Guid systemID)
        {
            XDocument xdoc = XDocument.Parse(ConfigXML);

            IEnumerable<XElement> Templates = xdoc.Root.Descendants(StoreNames._XE_TEMPLATE);

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

            return finalTemplateList.ToDictionary(x => x.Key, x => x.name);
        }

        public static Dictionary<string, Guid> GetActiveSearchOnlyTemplateListByName(Guid systemID)
        {
            DataSet ds = SystemStore.GetActiveSearchOnlySystemTemplateList(systemID);
            var finalTemplateList = from t in ds.Tables[0].AsEnumerable()
                                    where Business.Template.FinalTemplateDefValid(t.Field<Guid>("TemplateID"))
                                     select new { Key = t.Field<Guid>("TemplateID"), name = t.Field<string>("TemplateName") };

            return finalTemplateList.ToDictionary(x => x.name, x => x.Key);
        }

        //Converts TemplateID|TermID|FieldName to TemplateName|TermName|FieldName
        private string RetrievedTermName(
            string concatenatedName, 
            Dictionary<Guid /*TemplateID*/, string /*TemplateName*/> templateNameLookup, 
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*TermID*/, string /*TermName*/>> termNameLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<Guid /*FieldID*/, string /*FieldName*/>>> fieldNameLookup,
            List<string> standardColumns,
            bool isExternal,
            bool testTermName)
        {
            try
            {
                if (standardColumns.Contains(DataStoreField.GetTermName(concatenatedName)))
                    return concatenatedName;
                Guid templateID = new Guid(DataStoreField.GetTermTemplate(concatenatedName));
                Guid termID = new Guid(DataStoreField.GetTermName(concatenatedName));
                string templateName = templateNameLookup[templateID];
                string termName = termNameLookup[templateID][termID];

                string fieldID = DataStoreField.GetTermField(concatenatedName);
                if (!string.IsNullOrEmpty(fieldID))
                {
                    if (isExternal)
                    {
                        return TermStore.GetTermConcatenatedName(templateName, termName, fieldID);
                    }
                    else
                    {
                        string fieldName = fieldNameLookup[templateID][termID][new Guid(fieldID)];
                        return TermStore.GetTermConcatenatedName(templateName, termName, fieldName);
                    }
                }
                else
                {
                    return TermStore.GetTermConcatenatedName(templateName, termName, string.Empty);
                }
            }
            catch
            {
                if (testTermName)
                {
                    //We want this to fail when called by the Tidal job...
                    throw new Exception(string.Format("Unable to retrieve Term info: {0}", concatenatedName));
                }
                else
                {
                    return concatenatedName;
                }
            }
        }

        //Converts TemplateName|TermName|FieldName to TemplateID|TermID|FieldName
        public static string StoredTermName(
            string concatenatedName,
            Dictionary<string /*TemplateName*/, Guid /*TemplateID*/> templateIDLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<string /*TermName*/, Guid /*TermID*/>> termIDLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<string /*FieldName*/, Guid /*FieldID*/>>> fieldIDLookup,
            bool isStandard,
            TermType termType)
        {
            try
            {
                if (isStandard)
                    return concatenatedName;
                Guid templateID = templateIDLookup[DataStoreField.GetTermTemplate(concatenatedName)];
                Guid termID = termIDLookup[templateID][DataStoreField.GetTermName(concatenatedName)];
                string fieldName = DataStoreField.GetTermField(concatenatedName);
                if (!string.IsNullOrEmpty(fieldName))
                {
                    if (termType == TermType.External)
                    {
                        return TermStore.GetTermConcatenatedName(templateID.ToString(), termID.ToString(), fieldName);
                    }
                    else
                    {
                        Guid fieldID = fieldIDLookup[templateID][termID][fieldName];
                        return TermStore.GetTermConcatenatedName(templateID.ToString(), termID.ToString(), fieldID.ToString());
                    }
                }
                else
                    return TermStore.GetTermConcatenatedName(templateID.ToString(), termID.ToString(), string.Empty);
            }
            catch
            {
                throw new Exception(string.Format("Failed to create stored term name for {0}", concatenatedName));
            }
        }

        private List<DataStoreField> GetSelectedTermsFromDef(
            string configXML, 
            Dictionary<Guid /*TemplateID*/, string /*TemplateName*/> templateNameLookup, 
            Dictionary<Guid, Dictionary<Guid, string>> termNameLookup,
            Dictionary<Guid /*TemplateID*/, Dictionary<Guid /*ComplexListID*/, Dictionary<Guid /*FieldID*/, string /*FieldName*/>>> fieldNameLookup,
            bool testTermName)
        {

            List<string> standardColumns = Enum.GetNames(typeof(SystemStore.StandardColumn)).ToList();

            XDocument xdoc = XDocument.Parse(configXML);
            List<DataStoreField> dataStoreFields = new List<DataStoreField>();

            IEnumerable<XElement> Terms = xdoc.Root.Descendants(StoreNames._XE_TERMS).Descendants(StoreNames._XE_TERM);

            try
            {
                var termList = from t in Terms
                               where (1 == 1)
                               select new
                               {
                                   TermName = RetrievedTermName(t.Attribute(StoreNames._XA_TERM_NAME).Value, templateNameLookup, termNameLookup, fieldNameLookup, standardColumns, t.Attribute(StoreNames._XA_TERM_TYPE).Value.Equals("External"), testTermName),      //Attribute is 'TemplateID|TermID'
                                   Alias = t.Attribute(StoreNames._XA_TERM_ALIAS).Value,
                                   Index = (int?)t.Attribute(StoreNames._XA_TERM_INDEX),
                                   Size = (int?)t.Attribute(StoreNames._XA_TERM_SIZE),
                                   Type = t.Attribute(StoreNames._XA_TERM_TYPE).Value,
                                   IsStandard = standardColumns.Contains(DataStoreField.GetTermName(t.Attribute(StoreNames._XA_TERM_NAME).Value)),
                               };


                foreach (var t in termList)
                {
                    dataStoreFields.Add(new DataStoreField(t.TermName, t.Alias, t.Size, (TermType)Enum.Parse(typeof(TermType), t.Type), t.IsStandard));
                }
            }
            catch
            {
            }

            return dataStoreFields;
        }

        public static bool GetTermInUseFromDef(string configXML, Guid termID)
        {
            XDocument xdoc = XDocument.Parse(configXML);
            IEnumerable<XElement> Terms = xdoc.Root.Descendants(StoreNames._XE_TERMS).Descendants(StoreNames._XE_TERM);

            try
            {
                var termList = from t in Terms
                               where (!t.Attribute(StoreNames._XA_TERM_ISSTANDARD).Value.Equals("true"))
                               select new
                               {
                                   TermID = new Guid(DataStoreField.GetTermName((t.Attribute(StoreNames._XA_TERM_NAME).Value))),
                               };

                foreach (var t in termList)
                {
                    if (t.TermID == termID)
                        return true;
                }
            }
            catch
            {
            }
            return false;
        }

        private string GetPathfromDefinition(string configxml)
        {
            XElement xmlTree = XElement.Parse(configxml);
            return xmlTree.Element(StoreNames._XE_PATH).Value;
        }
         
        private string GetDefaultDateFormatDefinition(string configxml)
        {
            XElement xmlTree = XElement.Parse(configxml);
            return xmlTree.Element(StoreNames._XE_DEFAULT_DATE_FORMAT).Value;
        }

        private string GetEmailAddressesfromDefinition(string configxml)
        {
            XElement xmlTree = XElement.Parse(configxml);
            return xmlTree.Element(StoreNames._XE_EMAIL_ADDRESSES).Value;
        }
    }
}
