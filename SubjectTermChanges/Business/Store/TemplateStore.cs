using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{
    public class TemplateStore
    {

        #region Private Members

        private List<string /*DSF Name*/> orderedDSFNames;             //A listing of the DSF names in the order in which they are encountered.
        private List<string /*DSF Alias Name*/> orderedAliasNames;     //A listing of the unique alias names in the order in which they are encountered.
        private List<string /*Term Name*/> reportedTermNames;           //A listing of the unique term names that are reported.

        private Dictionary<string /*Term Name*/, Term> templateTerms;   //All terms within the template that are 'storable'.
        private Dictionary<string /*DSF Name*/, DataStoreField> dataStoreFields;

        private Dictionary<string /*DSF Alias Name*/, List<string /*DSF Name*/>> termAliasMapping;

        private string defaultDateFormat;

        private Template template;
        private bool isMultiplexed;      //True if the same Alias is reused

        #endregion Private Members

        #region Public Properties

        public List<string /*DSF Name*/> OrderedDSFNames
        {
            get { return orderedDSFNames; }
        }

        public List<string> OrderedAliasNames
        {
            get { return orderedAliasNames; } 
        }

        public List<string> ReportedTermNames
        {
            get { return reportedTermNames; }
        }

        public Dictionary<string /*Term Name*/, Term> TemplateTerms
        {
            get { return templateTerms; }
        }

        public Dictionary<string /*DSF Name*/, DataStoreField> DataStoreFields
        {
            get { return dataStoreFields; }
        }

        public Dictionary<string /*DSF Alias Name*/, List<string /*DSF Name*/>> TermAliasMapping
        {
            get { return termAliasMapping; }
        }

        public Template Template
        {
            get { return template; }
        }

        public bool IsMultiplexed
        {
            get { return isMultiplexed; }
        }

        public string DefaultDateFormat
        {
            get { return defaultDateFormat; }
        }

        #endregion Public Properties

        #region Constructor

        public TemplateStore(Guid templateID, List<DataStoreField> dsfTerms, string defaultDateFormat)
        {
            isMultiplexed = false;
            this.defaultDateFormat = defaultDateFormat;
            orderedAliasNames = new List<string>();
            reportedTermNames = new List<string>();
            orderedDSFNames = new List<string>();

            dataStoreFields = new Dictionary<string, DataStoreField>();
            foreach (DataStoreField dsf in dsfTerms)
            {
                dataStoreFields.Add(dsf.Name, dsf);
                orderedDSFNames.Add(dsf.Name);
                if (!reportedTermNames.Contains(dsf.TermName))
                {
                    reportedTermNames.Add(dsf.TermName);
                }
            }

            templateTerms = new Dictionary<string, Term>();
            //Note - we use the current template definition as a filter for the reported terms.
            template = new Business.Template(templateID, DefType.Final);
            foreach (Term term in template.BasicTerms)
            {
                if (term.IsStored)
                {
                    templateTerms.Add(term.Name, term);
                    DataStoreField dsfText = dsfTerms.Find(dsf => dsf.TermName.Equals(term.Name));
                    //Not all of the templateTerms that are storable will be found in the dsfTerms collection.
                    if (dsfText != null)
                    {
                        //Validation will be based on the latest template definition.
                        dsfText.TermType = term.TermType;
                        if (term.TermType == TermType.Text)
                        {
                            dsfText.TextTermFormat = (term as TextTerm).Format;
                        }
                    }
                }
            }
            foreach (Term term in template.ComplexLists)
            {
                if (term.IsStored)
                {
                    templateTerms.Add(term.Name, term);
                }
            }
        }

        #endregion Constructor

        #region Private Methods

        private void GetTermAliasMapping()
        {
            isMultiplexed = false;
            termAliasMapping = new Dictionary<string /*DSF Alias Name*/, List<string /*DSF Name*/>>();
            foreach (KeyValuePair<string /*DSF Name*/, DataStoreField> kvp in dataStoreFields)
            {
                if (!termAliasMapping.ContainsKey(kvp.Value.Alias))
                {
                    termAliasMapping.Add(kvp.Value.Alias, new List<string /*DSF Name*/>());
                }

                termAliasMapping[kvp.Value.Alias].Add(kvp.Value.Name);
                if (!isMultiplexed && termAliasMapping[kvp.Value.Alias].Count > 1)
                    isMultiplexed = true;
            }
        }

        #endregion Private Methods

        #region Public Static Methods

        //This call builds up a collection of TemplateStores based on the supplied DataStoreField collection for a given DataStoreDefinition.
        //Note - the supplied 'templates' dictionary may contain more templates than are actually referenced within the dsfTerms.
        public static Dictionary<Guid /*TemplateID*/, TemplateStore> GetTemplateStoreCollection(Dictionary<string /*Template Name*/, Guid> templates, List<DataStoreField> dsfTerms, string defaultDateFormat)
        {
            Dictionary<Guid /*TemplateID*/, TemplateStore> templateStores = new Dictionary<Guid /*TemplateID*/, TemplateStore>();
            foreach (DataStoreField dsfTerm in dsfTerms)
            {
                //The 'Special Terms' (e.g. ManagedItemNumber, WorkflowState, etc.) will not have an associated Template
                if (!string.IsNullOrEmpty(dsfTerm.TemplateName))
                {
                    if (!templates.ContainsKey(dsfTerm.TemplateName))
                    {
                        throw new Exception(string.Format("Template named '{0}' not found in templates collection", dsfTerm.TemplateName));
                    }

                    TemplateStore templateStore = null;
                    if (!templateStores.ContainsKey(templates[dsfTerm.TemplateName]))
                    {
                        List<DataStoreField> templateDSFTerms = dsfTerms.FindAll(dsf => dsf.TemplateName == null || dsf.TemplateName.Equals(dsfTerm.TemplateName));
                        templateStore = new TemplateStore(templates[dsfTerm.TemplateName], templateDSFTerms, defaultDateFormat);
                    }
                    else
                    {
                        templateStore = templateStores[templates[dsfTerm.TemplateName]];
                    }

                    //Add the unique alias names in the order they are encountered.
                    if (!templateStore.orderedAliasNames.Contains(dsfTerm.Alias))
                        templateStore.orderedAliasNames.Add(dsfTerm.Alias);

                    if (!templateStores.ContainsKey(templates[dsfTerm.TemplateName]))
                        templateStores.Add(templates[dsfTerm.TemplateName], templateStore);
                }
            }

            //Here handle the special case of the definition only containing 'standard columns'.
            if (templateStores.Count == 0 && templates.Count > 0)
            {
                foreach (KeyValuePair<string /*Template Name*/, Guid> template in templates)
                {
                    TemplateStore templateStore = new TemplateStore(template.Value, dsfTerms, defaultDateFormat);
                    foreach (DataStoreField dsfTerm in dsfTerms)
                    {
                        //Add the unique alias names in the order they are encountered.
                        if (!templateStore.orderedAliasNames.Contains(dsfTerm.Alias))
                            templateStore.orderedAliasNames.Add(dsfTerm.Alias);
                    }
                    templateStores.Add(template.Value, templateStore);
                }
            }

            //Once the collections for all of the TemplateStore's have been completed, perform the 'Alias mapping'.
            foreach (KeyValuePair<Guid /*TemplateID*/, TemplateStore> kvp in templateStores)
            {
                kvp.Value.GetTermAliasMapping();
            }

            return templateStores;
        }

        #endregion Public Static Methods

    }
}
