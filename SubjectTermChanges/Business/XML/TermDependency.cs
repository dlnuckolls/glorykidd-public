using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{

	[Serializable]
	public enum DependencyQuantifier
	{
		All = 0,   //ALL of the conditions must be true for the dependency action to be applied
		Any       //if ANY of the conditions are true, the dependency action should be applied
	}

	[Serializable]
	public enum DependencyTarget
	{
		Term,		//The Term dependency will affect a named BasicTerm or ComplexList.
		Workflow	//The Term dependency will affect a named Workflow.
	}

    [Serializable]
    public enum TermDependencyType
    {
        None,
        All,
        TextDatePicklist,
        ComplexList
    }
    
    [Serializable]
	public class TermDependency
	{

		#region private fields
		private Guid _id;
        private List<Guid> _dependentTermIDs;
		private DependencyQuantifier _quantifier;
		private DependencyTarget _target;
		private List<TermDependencyCondition> _conditions;
		private TermDependencyAction _action;
		//This reference is needed when converting from TermID to TermName
		private Template _template;
        private bool? _isActive = true;
		#endregion


		#region instance properties

		public Guid ID
		{
			get { return _id; }
			set { _id = value; }
		}

        public List<Guid> DependentTermIDs
        {
            get { return _dependentTermIDs; }
        }

        //Note - This is needed by the GUI for binding....
        public string DependentTerms
        {
            get {
                    string listTerms = string.Empty;
                    foreach (Guid dependentTermID in DependentTermIDs)
                    {
                        string termName = null;
                        try { termName = _template.FindTerm(dependentTermID).Name; }
                        catch { }
                        if (!string.IsNullOrEmpty(termName))
                        {
                            if (listTerms.Length > 0)
                                listTerms += "<br/>" + termName;
                            else
                                listTerms = termName;
                        }
                    }
                    return listTerms; 
                }
        }
        
        public DependencyQuantifier Quantifier
		{
			get { return _quantifier; }
			set { _quantifier = value; }
		}

		public DependencyTarget Target
		{
			get { return _target; }
			set { _target = value; }
		}

		public List<TermDependencyCondition> Conditions
		{
			get { return _conditions; }
		}

		public TermDependencyAction Action
		{
			get { return _action; }
		}

        public bool? IsActive
        {
            get { return _isActive;  }
            set { _isActive = value; }
        }

		public string SourceTermText
		{
			get
			{
				Dictionary<string, int> sourceTerms = new Dictionary<string, int>();
				foreach (TermDependencyCondition condition in _conditions)
				{
					string sourceTermName = condition.SourceTerm;
					if (string.IsNullOrEmpty(sourceTermName))
						sourceTermName = " ";
					if (sourceTerms.ContainsKey(sourceTermName))
						sourceTerms[sourceTermName]++;
					else
						sourceTerms.Add(sourceTermName, 1);
				}
				System.Text.StringBuilder sb = new StringBuilder();
				foreach (string sourceTermName in sourceTerms.Keys)
					if (sourceTerms[sourceTermName] > 1)
						sb.AppendFormat("{0} (x{1}), ", sourceTermName, sourceTerms[sourceTermName]);
					else
						sb.AppendFormat("{0}, ", sourceTermName);
				return sb.ToString().TrimEnd(new char[] {' ', ','});   //trim off the trailing comma and space from the string
			}
		}

		#endregion

		#region constructors

		public TermDependency(Template template, DependencyTarget target)
		{
			_id = Guid.NewGuid();
			//_dependentTermID = Guid.Empty;
            _dependentTermIDs = new List<Guid>();
            _quantifier = DependencyQuantifier.All;
			_target = target;
			_conditions = new List<TermDependencyCondition>();
			_action = new TermDependencyAction(TermDependencyActionValue.Default, TermDependencyActionValue.Default);
			_template = template;
		}

		public TermDependency(Template template, XmlNode termDependencyNode)
		{
			_template = template;

			string dependentTerm = Utility.XMLHelper.GetAttributeString(termDependencyNode, XMLNames._A_DependentTerm);
            Guid dependentTermID;
			if (!string.IsNullOrEmpty(dependentTerm))
			{
                dependentTermID = _template.FindTerm(dependentTerm).ID;
			}
			else
			{
                dependentTermID = Term.CreateID(termDependencyNode, XMLNames._A_DependentTermID);
			}

			string idString = Utility.XMLHelper.GetAttributeString(termDependencyNode, XMLNames._A_ID);
			_id = new Guid(idString);
			_quantifier = (DependencyQuantifier)Enum.Parse(typeof(DependencyQuantifier), Utility.XMLHelper.GetAttributeString(termDependencyNode, XMLNames._A_Quantity));
			string sTarget = Utility.XMLHelper.GetAttributeString(termDependencyNode, XMLNames._A_Target);
			if (string.IsNullOrEmpty(sTarget))
				_target = DependencyTarget.Term;
			else
				_target = (DependencyTarget)Enum.Parse(typeof(DependencyTarget), sTarget);

            if (_target == DependencyTarget.Term)
            {
                //Add dependent terms.  If this is the 'old' structure, then the dependentTermID is defined.  If not, then the DependentTerms collection is defined.
                _dependentTermIDs = new List<Guid>();
                XmlNode nodeDependentTerms = termDependencyNode.SelectSingleNode(Utility.XMLHelper.GetXPath(false, XMLNames._E_DependentTerms));
                if (nodeDependentTerms != null)
                {
                    XmlNodeList nodelistDependentTerms = termDependencyNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_DependentTerms, XMLNames._E_DependentTerm));
                    foreach (XmlNode nodeDependentTerm in nodelistDependentTerms)
                    {
                        _dependentTermIDs.Add(Term.CreateID(nodeDependentTerm, XMLNames._A_ID));
                    }
                }
                else
                {
                    if (!Term.ValidID(dependentTermID))
                    {
                        throw new Exception(string.Format("Dependent term not defined for Term Dependency {0}", _id.ToString()));
                    }
                    else
                    {
                        _dependentTermIDs.Add(dependentTermID);
                    }
                }
            }

			//Add Conditions
			XmlNodeList nodelistConditions = termDependencyNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_TermDependencyConditions, XMLNames._E_TermDependencyCondition));
			if (nodelistConditions != null && nodelistConditions.Count > 0)
			{
				_conditions = new List<TermDependencyCondition>(nodelistConditions.Count);
				foreach (XmlNode nodeCondition in nodelistConditions)
					_conditions.Add(new TermDependencyCondition(_template, nodeCondition));
			}
			else
			{
				_conditions = new List<TermDependencyCondition>();
			}

			//Add Action
			XmlNode nodeAction = termDependencyNode.SelectSingleNode(Utility.XMLHelper.GetXPath(false, XMLNames._E_TermDependencyAction));
			if (nodeAction != null)
				_action = new TermDependencyAction(nodeAction);
			else
				_action = new TermDependencyAction(TermDependencyActionValue.Default, TermDependencyActionValue.Default);


            //Is Active
            try
            {
                _isActive = Utility.XMLHelper.GetAttributeBool(termDependencyNode, XMLNames._A_IsActive);
                if (!_isActive.HasValue)
                    _isActive = true;
            }
            catch
            {
            }
		}



		#endregion


		#region instance methods

		public bool ShouldApplyAction(ManagedItem managedItem)
		{
			if (_quantifier == DependencyQuantifier.Any)
				return AnyConditionMet(managedItem);
			else
				return AllConditionsMet(managedItem);
		}


		private bool AllConditionsMet(ManagedItem managedItem)
		{
			foreach (TermDependencyCondition condition in _conditions)
			{
				if (!condition.ConditionMet(managedItem.FindBasicTerm(condition.SourceTerm)))
					return false;
			}
			return true;
		}

		private bool AnyConditionMet(ManagedItem managedItem)
		{
			foreach (TermDependencyCondition condition in _conditions)
			{
				if (condition.ConditionMet(managedItem.FindBasicTerm(condition.SourceTerm)))
					return true;
			}
			return false;
		}


		#endregion


		#region static methods

		public static List<TermDependency> Create(Template template, string templateDef)
		{
			if (templateDef == null)
				return null;
            //Convert the xml into an xmldocument
            XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);

			return Create(template, xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_TermDependencies)));
		}

		//Generate the Terms collection (of Term) based on an xml document
		public static List<TermDependency> Create(Template template, XmlNode termDependenciesNode)
		{
			List<TermDependency> rtn = new List<TermDependency>();
			if (termDependenciesNode != null)
				foreach (XmlNode termDependencyNode in termDependenciesNode)
					rtn.Add(new TermDependency(template, termDependencyNode));
			return rtn;
		}

		public static bool Save(XmlDocument xmlTemplateDoc, Template template, bool bValidate)
		{
            //Convert the objects stored in memory to an xmldocument
            XmlDocument xmlDoc = new XmlDocument();
			XmlElement rootTermDependency = xmlDoc.CreateElement(XMLNames._E_TermDependencies);

			if (template.TermDependencies != null)
				foreach (TermDependency td in template.TermDependencies)
				{
					XmlElement elementTermDependency = xmlDoc.CreateElement(XMLNames._E_TermDependency);
					td.Build(xmlDoc, elementTermDependency, bValidate);
					rootTermDependency.AppendChild(elementTermDependency);
				}


			//Replace the impacted portion of the complete xml with this version from memory
			XmlNode importedRootTerm = xmlTemplateDoc.ImportNode(rootTermDependency, true);
			//Find the "TermDependencies" child node
			XmlNode termsChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_TermDependencies));
			if (termsChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedRootTerm, termsChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedRootTerm);

			return true;
		}

        //Generate the Terms collection (of Term) based on an xml document
        //public static string Save(Template template, EventType eventType, ref string sXml, bool bValidate)
        public static bool Save(Template template, ref string sXml, bool bValidate)
        {
            //Convert the xml into an xmldocument
            XmlDocument xmlTemplateDoc = new XmlDocument();
            xmlTemplateDoc.PreserveWhitespace = false;
            xmlTemplateDoc.LoadXml(sXml);

            if (template.TermDependencies == null)
                throw new NullReferenceException("template.Events is null.   Contact customer support.");
            //Convert the objects stored in memory to an xmldocument
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement nodeTermDependencies = xmlDoc.CreateElement(XMLNames._E_TermDependencies);
            foreach (TermDependency termDependency in template.TermDependencies)
            {
                XmlElement nodeTermDependency = xmlDoc.CreateElement(XMLNames._E_TermDependency);
                termDependency.Build(xmlDoc, nodeTermDependency, bValidate);
                nodeTermDependencies.AppendChild(nodeTermDependency);
            }
            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeTermDependencies = xmlTemplateDoc.ImportNode(nodeTermDependencies, true);
            //Find the "Events" child node
            XmlNode termDependenciesChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_TermDependencies));
            if (termDependenciesChildNode != null)
                xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeTermDependencies, termDependenciesChildNode);
            else
                xmlTemplateDoc.DocumentElement.AppendChild(importedNodeTermDependencies);
            //Return the revised template XML
            sXml = xmlTemplateDoc.OuterXml;
            return true;
        }


		private void Build(XmlDocument xmlDoc, XmlNode termDependencyNode, bool bValidate)
		{
			if (bValidate)
			{
				//TODO:  ???
			}

			Utility.XMLHelper.AddAttributeString(xmlDoc, termDependencyNode, XMLNames._A_ID, _id.ToString());
			Utility.XMLHelper.AddAttributeString(xmlDoc, termDependencyNode, XMLNames._A_Quantity, _quantifier.ToString());
			Utility.XMLHelper.AddAttributeString(xmlDoc, termDependencyNode, XMLNames._A_Target, _target.ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, termDependencyNode, XMLNames._A_IsActive, _isActive.ToString());

            //Add dependent terms
            if (_target == DependencyTarget.Term)
            {
                XmlNode elementDependentTerms = termDependencyNode.AppendChild(xmlDoc.CreateElement(XMLNames._E_DependentTerms));
                foreach (Guid dependentTermID in _dependentTermIDs)
                {
                    XmlElement elementDependentTerm = xmlDoc.CreateElement(XMLNames._E_DependentTerm);
                    Term.StoreID(xmlDoc, elementDependentTerm, null, null, XMLNames._A_ID, dependentTermID);
                    elementDependentTerms.AppendChild(elementDependentTerm);
                }
            }

			//build Conditions
			XmlNode elementConditions =  termDependencyNode.AppendChild(xmlDoc.CreateElement(XMLNames._E_TermDependencyConditions));
			foreach (TermDependencyCondition condition in _conditions)
			{
				XmlElement elementCondition = xmlDoc.CreateElement(XMLNames._E_TermDependencyCondition);
				condition.Build(xmlDoc, elementCondition, bValidate);
				elementConditions.AppendChild(elementCondition);
			}

			//build Action
			XmlElement elementAction = xmlDoc.CreateElement(XMLNames._E_TermDependencyAction);
			_action.Build(xmlDoc, elementAction, bValidate);
			termDependencyNode.AppendChild(elementAction);
		}

		public static List<string> ReplaceEmbeddedTermNames(Template template, List<TermDependency> termDependencies)
		{
			List<string> sErrors = new List<string>();
			foreach (TermDependency termDependency in termDependencies)
			{
				foreach (TermDependencyCondition termDependencyCondition in termDependency.Conditions)
				{
					if (!Term.ValidID(termDependencyCondition.SourceTermID))
						if (!string.IsNullOrEmpty(termDependencyCondition.SourceTerm))
						{
							try
							{
								termDependencyCondition.SourceTermID = template.FindTerm(termDependencyCondition.SourceTerm).ID;
							}
							catch (Exception)
							{
								sErrors.Add(string.Format("Unable to find TermDependency SourceTerm '{0}'", termDependencyCondition.SourceTerm));
							}
						}
				}
			}
			return sErrors;
		}

		public static List<string> TermReferences(Template template, string termName, Guid termID)
		{
			List<string> sErrors = new List<string>();

			foreach (TermDependency termDependency in template.TermDependencies)
			{
                foreach (Guid dependentTermID in termDependency.DependentTermIDs)
                {
                    if (dependentTermID.Equals(termID))
                        sErrors.Add(string.Format("Term \"{0}\" is a dependent term", termName));
                }

				foreach (TermDependencyCondition termDependencyCondition in termDependency.Conditions)
				{
					if (termDependencyCondition.SourceTerm != null)
					{
						if (termDependencyCondition.SourceTerm == termName)
							sErrors.Add(string.Format("Term \"{0}\" is a SourceTerm for TermDependency \"{1}\".", termName, termDependency.SourceTermText));
					}
					else
					{
						if (termDependencyCondition.SourceTermID == termID)
							sErrors.Add(string.Format("Term \"{0}\" is a SourceTerm for TermDependency \"{1}\".", termName, termDependency.SourceTermText));
					}
				}
			}
			return sErrors;
		}

		#endregion
	}
}
