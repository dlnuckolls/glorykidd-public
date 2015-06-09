using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.UI.WebControls;
using System.Web;
using System.Text.RegularExpressions;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
    [Flags]
    public enum TermType : ulong
    {
        None = 0,
        Text = 1,
        Date = 2,
        MSO = 4,
        Renewal = 8,
        Facility = 16,
        ComplexList = 32,
        PickList = 64,
        Link = 128,
        External = 256,
        PlaceHolderAttachments = 512,
        PlaceHolderComments = 1024
    }

	[Serializable, System.Diagnostics.DebuggerDisplay("Name={Name}, Value={DisplayValue(\"\")}")]
	public abstract class Term : IComparable<Term>
	{
		private TermType _termType;
        protected Term _originalTerm;     //Information at runtime only - not stored in xml.

		#region public members
		public const string _SET_VALUE_DEFAULT = "(Default)";
        public const string _ELEMENT_PRESERVE_WHITE_SPACE = "pws";  //DG Added for Release 1.5
        #endregion

		#region private/protected members
		private string _name;
		private Guid _id;	//Added with XML Version T2007OCT17
		private bool? _default;
		private bool? _required;
		private string _defaultValue;
		private bool? _editable;
		private bool? _useDBField;
		private bool? _keywordSearchable;
		private bool _systemTerm;  //Used to indicate that this term is a system term, and therefore cannot be deleted
		private string _dbFieldName;

		private bool _nameRequired = false;
		private bool _defaultValueRequired = false;
		private bool _validateOnSave;
        private bool _preserveWhiteSpaceDocument = false;
        private bool _preserveWhiteSpaceSummary = false;

        private List<string> _validationStatuses = new List<string>();
        private Guid _termGroupID;	//Added with the Security change
        private bool _isHeader;     //Denotes that this term will be displayed on all 'Tabs'
        protected Template _template;
        private int _order = 0; //Used to assist in sorting - not saved back to the xml

		private TermRuntimeSettings _runtime;
        private readonly bool _isFilter;    //Used to support the ComplexListField usage - not saved to xml

        private bool _bigText;              //Used to support passing values to ComplexListField - not stored locally (Term XML)
        private bool _summary;          //Used to support passing values to ComplexListField - not stored locally (Term XML)

        private string _reportName;     //Used to display the term name when this is a filter term - do not store in the Term XML.
        private bool? _multiSelectSearch;   //This is intended for System Term use only.

        //Term Transform Info:  There are only two types defined that serve as guidance as to how to retrieve data
        //from the original term to be used in the tarnsformed term.
        [Serializable]
        public enum TermTransformType
        {
            Attribute,      //These are normally stored as single-value attributes (string) for the term.
            Reference       //These are stored as multi-valued data types (such as lists).
        }

        //Term Transform Info
        [Serializable]
        public struct TermTransform
        {
            public TermTransformType TransformType;
            public string Name;
            public string Value;
        }

        //Term Transform Info
        protected List<TermTransform> _termTransforms;
        private TermType? _transformTermType;      //This represents the type that this term may be transformed to.
                                                   //Currently, there is no GUI to set this type.
		#endregion

		#region Properties

		public TermType TermType
		{
			get { return _termType; }
			set { _termType = value; }
		}

		public string Name
		{
			get 
            {
                if (_isFilter)
                {
                    if (string.IsNullOrEmpty(ReportName))
                    {
                        CheckIsFilter("Term Name not defined for a Filter Term - should use ReportName");
                        return null;                    
                    }
                    else
                        return ReportName;
                }
                else
                    return Utility.XMLHelper.GetXMLText(_name); 
            }

			//20070817_DEG Trim trailing spaces from the name - don't want them as they will not appear in the
			//embedded image within the clause.
			set 
            {
                CheckIsFilter("Term Name not defined for a Filter Term");
                _name = Utility.XMLHelper.SetXMLText(value.TrimEnd()); 
            }
		}

        public string ReportName
        {
            get
            {
                return _reportName == null ? string.Empty : _reportName;
            }

            set
            {
                _reportName = value;
            }
        }
        
        public Guid ID
		{
			get 
            {
                CheckIsFilter("Term ID not defined for a Filter Term");
                return _id; 
            }
		}

		public string EmbeddedID
		{
			get 
            {
                CheckIsFilter("Term ID not defined for a Filter Term");
                return string.Format("{0}{1}", XMLNames._M_TermID, _id.ToString().ToUpper()); 
            }
		}

		public bool? Required
		{
			get { return _required; }
			set { _required = value; }
		}

		public bool? Default
		{
			get { return _default; }
			set { _default = value; }
		}

		public string DefaultValue
		{
			get { return Utility.XMLHelper.GetXMLText(_defaultValue); }
			set { _defaultValue = Utility.XMLHelper.SetXMLText(value); }
		}

		public bool? Editable
		{
			get { return _editable; }
			set { _editable = value; }
		}

		public bool? UseDBField
		{
			get { return _useDBField; }
			set { _useDBField = value; }
		}

		public bool? KeywordSearchable
		{
			get { return _keywordSearchable; }
			set { _keywordSearchable = value; }
		}

		public bool NameRequired
		{
			get { return _nameRequired; }
			set { _nameRequired = value; }
		}

		public bool DefaultValueRequired
		{
			get { return _defaultValueRequired; }
			set { _defaultValueRequired = value; }
		}

		public bool SystemTerm
		{
			get { return _systemTerm; }
            set { _systemTerm = value; }
        }

		public string DBFieldName
		{
			get { return Utility.XMLHelper.GetXMLText(_dbFieldName); }
			set { _dbFieldName = Utility.XMLHelper.SetXMLText(value); }
		}

		public virtual string Keyword
		{
			get { return null; }
		}

		protected bool HasKeyWord
		{
			get { return (_useDBField ?? false) || (_keywordSearchable ?? false); }
		}

		public bool ValidateOnSave
		{
			get { return _validateOnSave; }
			set { _validateOnSave = value; }
		}

        public bool PreserveWhiteSpaceDocument
        {
            get { return _preserveWhiteSpaceDocument; }
            set { _preserveWhiteSpaceDocument = value; }
        }

        public bool PreserveWhiteSpaceSummary
        {
            get { return _preserveWhiteSpaceSummary; }
            set { _preserveWhiteSpaceSummary = value; }
        }
        
        public TermRuntimeSettings Runtime
		{
			get { return _runtime; }
		}

        public Guid TermGroupID
        {
            get 
            {
                CheckIsFilter("Term Group ID not defined for a Filter Term");
                return _termGroupID; 
            }
            set 
            {
                CheckIsFilter("Term Group ID not defined for a Filter Term");
                _termGroupID = value; 
            }
        }

        public string TermGroupName 
        {
            get
            {
                CheckIsFilter("Term Group Name not defined for a Filter Term");
                TermGroup tg = _template.FindTermGroup(_termGroupID);
                return tg.Name;
            }
        }

        public TermGroup TermGroup
        {
            get
            {
                CheckIsFilter("Term Group not defined for a Filter Term");
                return _template.FindTermGroup(_termGroupID);
            }
        }

        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public bool IsHeader
        {
            get { return _isHeader; }
            set { _isHeader = value; }
        }

        /// <summary>
        /// Gets or sets the validation statuses.
        /// This is a list of status that this term requires validation on.
        /// </summary>
        /// <value>The validation statuses.</value>
        /// Created by Larry Richardson LRR 4/2/2008
        public List<string> ValidationStatuses
        {
            get { return _validationStatuses; }
            set { _validationStatuses = value; }
        }

        /// <summary>
        /// Gets the size (in bytes) required for value storage.
        /// </summary>
        /// <value>The Required Size (bytes). int value == limit. null == undefined.</value>
        public virtual int? RequiredSize
        {
            get { return null; }
        }

        public virtual string[] StoreColumns
        {
            get { return new string[]{ }; }
        }

        public bool IsStored
        {
            get 
            {
                switch (_termType)
                {
                    case Business.TermType.Text:
                    case Business.TermType.Date:
                    case Business.TermType.MSO:
                    case Business.TermType.Renewal:
                    case Business.TermType.Facility:
                    case Business.TermType.ComplexList:
                    case Business.TermType.PickList:
                    case Business.TermType.External:
                    case Business.TermType.PlaceHolderAttachments:
                        return true;

                    case Business.TermType.None:
                    case Business.TermType.Link:
                    case Business.TermType.PlaceHolderComments:
                        return false;

                    default:
                        throw new Exception(string.Format("Term Type '{0}' not evaluated", _termType.ToString()));
                }
            }
        }

        public bool IsFilter
        {
            get { return _isFilter; }
        }

        public bool BigText
        {
            get { return _bigText; }
            set { _bigText = value; }
        }

        public bool Summary
        {
            get { return _summary; }
            set { _summary = value; }
        }

        //Term Transform Info
        public List<TermTransform> TermTransforms
        {
            get { return _termTransforms; }
            set { _termTransforms = value; }
        }

        //Term Transform Info
        public TermType? TransformTermType
        {
            get { return _transformTermType; }
            set { _transformTermType = value; }
        }

        //Added to help determine how to conduct system term searches
        public bool MultiSelectSearch
        {
            get { return _multiSelectSearch ?? false; }
        }

        #endregion

		#region Constructors

        public Term(bool systemTerm, Template template, bool isFilter)//:this()
		{
            _isFilter = isFilter;
            _systemTerm = systemTerm;
			_runtime = new TermRuntimeSettings();
            if (!_isFilter)
			    _id = Guid.NewGuid();
            _template = template;
		}

        //Term Transform Info:  This constructor was added to enable the conversion of the 'original term' to a 'transform term'.
        //Currently FacilityTerm is the only class that can serve as the term to be transformed to.
        public Term(bool systemTerm, Template template, Term originalTerm, TermType termType)
        {
            _termType = termType;
            //The originalTerm must be defined.
            if (originalTerm == null)
                throw new Exception(string.Format("Attempted to create term type '{0}' but no original term was supplied", _termType.ToString()));
            //This term must allow for transforms to take place.
            if (!originalTerm._transformTermType.HasValue)
                throw new Exception(string.Format("Attempted to convert term '{0}' to term type '{1}' but no transforms are defined", originalTerm._name, originalTerm._transformTermType.Value.ToString()));
            //The transform term type must match the type of the 'original term'.
            if (_termType != originalTerm._transformTermType.Value)
                throw new Exception(string.Format("Attempted to convert term '{0}' (transform term type {1}) to term type {2} but the types do not match", originalTerm._name, originalTerm._transformTermType.Value.ToString(), _termType.ToString()));
            _isFilter = false;
            _systemTerm = systemTerm;
            _runtime = new TermRuntimeSettings();
            _id = Guid.NewGuid();
            _template = template;
            _originalTerm = originalTerm;
            MapTransform();
        }

        protected Term(XmlNode termNode, Template template, bool isFilter)
		{
            _isFilter = isFilter;
            _template = template;
            if (!_isFilter)
            {
                _name = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Name);
                _id = Guid.Empty;
                string id = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_ID);
                if (!string.IsNullOrEmpty(id))
                {
                    try
                    {
                        _id = new Guid(id);
                    }
                    catch
                    {
                    }
                }
                //If an ID was not assigned before, assign it now....
                if (_id == Guid.Empty)
                    _id = Guid.NewGuid();

                string termGroupID = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_TermGroupID);
                if (!string.IsNullOrEmpty(termGroupID))
                {
                    _termGroupID = new Guid(termGroupID);
                }
                else
                {
                    try
                    {
                        _termGroupID = template.BasicSecurityTermGroupID;
                    }
                    catch
                    {
                        //If the System XML is loaded, template.TermGroups will not be defined
                        _termGroupID = Guid.Empty;
                    }
                }
            }

			_required = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_Required);
			_default = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_Default);
			_defaultValue = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_DefaultValue);
			_editable = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_Editable) ?? true;
			_useDBField = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_UseDBField);
			_systemTerm = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_SystemTerm) ?? false;
			_keywordSearchable = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_KeywordSearchable) ?? false;
            _preserveWhiteSpaceDocument = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_PreserveWhiteSpaceDocument) ?? false;
            _preserveWhiteSpaceSummary = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_PreserveWhiteSpaceSummary) ?? false;
            _dbFieldName = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_DBFieldName);
			_validateOnSave = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_ValidateOnSave) ?? false;
            _isHeader = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_IsHeader) ?? false;
            string termTransformType = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_TransformTermType);
            if (!string.IsNullOrEmpty(termTransformType))
                _transformTermType = (TermType)Enum.Parse(typeof(TermType), termTransformType);

            _multiSelectSearch = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_MultiSelectSearch);


            // Larry Richardson 4/2/2008 allows for additional VOS statuses
            if (_validateOnSave)
            {
                _validationStatuses.Clear();
                foreach (XmlNode node in termNode.SelectNodes("validateOn/status"))
                {
                    _validationStatuses.Add(node.InnerText);    
                }
            }

            //Look for Transform information...
            _termTransforms = GetTransforms(termNode);

			_runtime = new TermRuntimeSettings();
			_runtime.Enabled = _editable ?? true;
			_runtime.Required = _required ?? false;
			//_runtime.Visible = true;
		}

        public void AssignTemplate(Template template)
        {
            _template = template;
        }

        public static List<TermTransform> GetTransforms(XmlNode termNode)
        {
            List<TermTransform> termTransforms = null;
            XmlNodeList listTransforms = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Transforms, XMLNames._E_Transform));
            if (listTransforms != null && listTransforms.Count > 0)
            {
                termTransforms = new List<TermTransform>();
                foreach (XmlNode nodeTransform in listTransforms)
                {
                    TermTransform termTransform = new TermTransform();
                    termTransform.TransformType = (TermTransformType)Enum.Parse(typeof(TermTransformType), Utility.XMLHelper.GetAttributeString(nodeTransform, XMLNames._A_Type));
                    termTransform.Name = Utility.XMLHelper.GetAttributeString(nodeTransform, XMLNames._A_Name);
                    termTransform.Value = Utility.XMLHelper.GetAttributeString(nodeTransform, XMLNames._A_Value);
                    termTransforms.Add(termTransform);
                }
            }
            return termTransforms;
        }

		#endregion

		#region Build XML


		public virtual void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
				if (_nameRequired && !_isFilter)
					Utility.XMLHelper.ValidateString(XMLNames._A_Name, _name);
				if (_defaultValueRequired)
					Utility.XMLHelper.ValidateString(XMLNames._A_DefaultValue, _defaultValue);
			}

            if (!_isFilter)
            {
                Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Name, _name);
                Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_ID, _id.ToString());
                Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_TermGroupID, _termGroupID.ToString());
            }
            Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_Required, _required);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_Default, _default);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_DefaultValue, _defaultValue);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_Editable, _editable);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_UseDBField, _useDBField);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_SystemTerm, _systemTerm);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_KeywordSearchable, _keywordSearchable);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_PreserveWhiteSpaceDocument, _preserveWhiteSpaceDocument);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_PreserveWhiteSpaceSummary, _preserveWhiteSpaceSummary);
            Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_DBFieldName, _dbFieldName);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_ValidateOnSave, _validateOnSave);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_IsHeader, _isHeader);
            if (_transformTermType.HasValue)
                Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_TransformTermType, _transformTermType.Value.ToString());

            if (_multiSelectSearch.HasValue)
                Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_MultiSelectSearch, _multiSelectSearch);

            // Larry Richardson 4/2/2008 allows for additional VOS statuses
            if (_validateOnSave)
            {
                XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_validateOn, String.Empty);
                foreach(string item in this._validationStatuses)
                {
                    XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_status, String.Empty);
                    child.InnerText = item;
                    node.AppendChild(child);
                    
                }
                termNode.AppendChild(node);
            }

            //Store Transform information
            if (_termTransforms != null && _termTransforms.Count > 0)
            {
                XmlNode nodeTransforms = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Transforms, String.Empty);
                foreach (TermTransform termTransform in _termTransforms)
                {
                    XmlNode nodeTransform = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Transform, String.Empty);
                    Utility.XMLHelper.AddAttributeString(xmlDoc, nodeTransform, XMLNames._A_Type, termTransform.TransformType.ToString());
                    Utility.XMLHelper.AddAttributeString(xmlDoc, nodeTransform, XMLNames._A_Name, termTransform.Name);
                    Utility.XMLHelper.AddAttributeString(xmlDoc, nodeTransform, XMLNames._A_Value, termTransform.Value);
                    nodeTransforms.AppendChild(nodeTransform);
                }
                termNode.AppendChild(nodeTransforms);
            }
		}

		#endregion

		#region Method(s) to get/set the term's value

		public abstract void SetDefaultValue();
		public abstract string DisplayValue(string termPartSpecifier);
		public abstract List<string> Validate(bool includeTab, string filterTermTabName);
        public abstract List<string> CheckType(bool includeTab, string filterTermTabName);       //Note - Added for 1.5.1
        public abstract void Clear();

        //Term Transform Info: This call will perform the necessary actions to obtain Attribute and Reference information from the
        //original term and populate the corresponding fields of the transform term.
        protected virtual bool MapTransform()
        {
            return false;
        }

        //Term Transform Info: This call will read from a list of values within the original term known by the 'name' (_XFRM_ ...), and return
        //those values as a list of string.  The transform term will then convert those strings into the appropriate data type.
        public virtual List<string> GetTransformList(string name)
        {
            return null;
        }

        //This is intended to be used with 'FilterTerm Validation' logic, where it is neccessary to assign a value in order to perform validation.
        public virtual void SetValue(string value)
        {
        }

        //This is intended to be used with 'FilterTerm Validation' logic, where it is neccessary to assign a value in order to perform validation.
        public virtual string TestValue(string termName, string tabMessage, string value)
        {
            throw new NotImplementedException();
        }


        //This is intended to be used with 'FilterTerm Validation' logic, where it is neccessary to obtain a value in order to perform validation.
        public virtual string GetValue(string defaultValue)
        {
            return defaultValue;
        }

        public virtual void MigrateReset()
        {
            if (Default ?? false)
                SetDefaultValue();
            else
                Clear();
        }
        
        public virtual void Migrate(Term term)
        {
            if (term == null)
            {
                if (Default ?? false)
                    SetDefaultValue();
            }
            else
                term.Runtime.Migrated = true;
            Runtime.Migrated = true;
        }

        public virtual Term Copy()
        {
            return null;
        }

        public virtual Term RetroCopy(bool systemTerm, Template template)
        {
            return this;
        }

        protected void CopyBase(Term term, Template template)
        {
            if (!_isFilter)
            {
                term._name = _name;
                term._id = _id;
                term._termGroupID = _termGroupID;
            }
            term._default = _default;
            term._required = _required;
            term._defaultValue = _defaultValue;
            term._editable = _editable;
            term._useDBField = _useDBField;
            term._keywordSearchable = _keywordSearchable;
            term._systemTerm = _systemTerm;
            term._dbFieldName = _dbFieldName;
            term._nameRequired = _nameRequired;
            term._defaultValueRequired = _defaultValueRequired;
            term._validateOnSave = _validateOnSave;
            term._preserveWhiteSpaceDocument = _preserveWhiteSpaceDocument;
            term._preserveWhiteSpaceSummary = _preserveWhiteSpaceSummary;
            term._validationStatuses = new List<string>(_validationStatuses);
            if (term._termTransforms != null)
                term._termTransforms = new List<TermTransform>(_termTransforms);
            term._isHeader = _isHeader;
            term._template = template;
            term._order = _order;
            term._bigText = _bigText;
            term._summary = _summary;
            term._multiSelectSearch = _multiSelectSearch;
        }

        private void CheckIsFilter(string message)
        {
            if (_isFilter)
                throw new Exception(message);
        }

        //Intended for use with Retro Migration, for cleanup required apart from the xml
        public virtual void Delete()
        {
        }

        public virtual void Load(string value, string pattern, char? delimiter)
        {
        }

		#endregion

		#region PDF Writer Method(s)

		public abstract bool EmitPDFXML(XmlWriter writer);





		#endregion

        #region Public Static Clause Methods

        public static bool ValidName(string name)
		{
			bool rtn = true;
			//20070809_DEG	Removed this check for Bug #131
			//if (name.Contains("\""))
			//    rtn = false;
			return rtn;
		}

		public static bool IsEmbeddedID(string sTerm)
		{
			return sTerm.IndexOf(XMLNames._M_TermID) >= 0;
		}

		public static string FilterEmbeddedID(string sTerm)
		{
			return sTerm.Replace(XMLNames._M_TermID, "");
		}

		public static bool ValidID(Guid id)
		{
			return id != Guid.Empty;
		}

		//For each term in the clause, replace the TermName (if found) with the TermID
		public static string SubstituteTermNames(Template template, string sText)
		{
			string[] delimiter = new string[] { " | " };
			//match anything of the form <img...src="TextImage.ashx?text="...".../> in _text
			if (!string.IsNullOrEmpty(sText))
			{
				MatchCollection matches = Regex.Matches(sText, XMLNames._M_TermImageTemplate);
				foreach (Match match in matches)
				{
					string termName = match.Groups[1].Value;
					string[] termNameParts = termName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
					//first search for a matching Term
					//20070817_DEG  Bug 131 - Special case - replace the quotes
					Term term = null;
					string sTermName = termNameParts[0].Replace("&quot;", "\"");
					//Note - this approach will be backward compatible.
					if (!IsEmbeddedID(sTermName))
					{
						term = template.FindTerm(sTermName);
						if (term != null)
						{
							//Replace <img ... > with term value
							string matchedText = match.Value;
							string replacementText = "";
							if (termNameParts.Length > 1)
								replacementText = matchedText.Replace(termName, string.Format("{0} | {1}", term.EmbeddedID, termNameParts[1]));
							else
								replacementText = matchedText.Replace(termName, term.EmbeddedID);
							sText = sText.Replace(matchedText, replacementText);
						}
						else
							//Need to check for 'reserved term names'
							if (!sTermName.StartsWith("*"))
								throw new Exception(string.Format("Unable to locate term named '{0}' when substituting terms", sTermName));
					}
				}
			}
			return sText;
		}

		//For each term in the clause, replace the TermID (if found) with the TermName
		public static string SubstituteTermIDs(Template template, string sText)
		{
			string[] delimiter = new string[] { " | " };
			//match anything of the form <img...src="TextImage.ashx?text="...".../> in _text
			if (!string.IsNullOrEmpty(sText))
			{
				MatchCollection matches = Regex.Matches(sText, XMLNames._M_TermImageTemplate);
				foreach (Match match in matches)
				{
					string termName = match.Groups[1].Value;
					string[] termNameParts = termName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
					//first search for a matching Term
					//20070817_DEG  Bug 131 - Special case - replace the quotes
					Term term = null;
					string sTermName = termNameParts[0].Replace("&quot;", "\"");
					//Note - this approach will be backward compatible.
					if (IsEmbeddedID(sTermName))
					{
						term = template.FindTerm(new Guid(FilterEmbeddedID(sTermName)));
						if (term != null)
						{
							//Replace <img ... > with term value
							string matchedText = match.Value;
							string replacementText = "";
							if (termNameParts.Length > 1)
								replacementText = matchedText.Replace(termName, string.Format("{0} | {1}", term.Name, termNameParts[1]));
							else
								replacementText = matchedText.Replace(termName, term.Name);
							sText = sText.Replace(matchedText, replacementText);
						}
					}
				}
			}
			return sText;
		}

		//Replace "special terms" (those term placeholders that begin with "* " -- without the quotes)
		//Note - the 'template' is assumed to belong to a ManagedItem
		public static void SubstituteSpecialTerms(ref string sText, Template template)
		{
			string[] delimiter = new string[] { " | " };
			//match anything of the form <img...src="TextImage.aspx?text="*...".../> in _text
			string text = HttpUtility.HtmlDecode(sText);
			if (!string.IsNullOrEmpty(text))
			{
				string managedItemName = template.GetManagedItemName();
				MatchCollection matches = Regex.Matches(text, XMLNames._M_TermImageTemplate);
				foreach (Match match in matches)
				{
					if (!match.Groups[1].Value.StartsWith("*"))
						continue;
					string[] termNameParts = match.Groups[1].Value.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
					//if (!termNameParts[0].StartsWith("* "))
					//	throw new Exception("Attempting to parse Special Term that does not start with '* ': " + termNameParts[0]);
					string specialTermName = termNameParts[0].Substring(2);  //strip off the leading 2 characters
					string matchedText = match.Result("$0");
					string replacementText = string.Concat(@"<span style=""font-weight:bold; text-decoration:underline;"">", (char)0xab, specialTermName, (char)0xbb, "</span>");
					ManagedItem managedItem = template as ManagedItem;
					if (managedItem != null)
					{
						//TODO Fix - This code should also be used to create the 'special term names' displayed to the user
						//TODO - Need to examine cases where the user creates a term name such as '*Status'.  Should we have a list of 'reserved term names' for 'special terms'?
                        if (specialTermName == managedItemName + XMLNames._SpecialTermName_Number)
							replacementText = managedItem.ItemNumber;
                        else if (specialTermName == XMLNames._SpecialTermName_Status)
							replacementText = managedItem.State.Status;
                        else if (specialTermName == XMLNames._SpecialTermName_WorkflowState)
							replacementText = managedItem.State.Name;
					}
					text = text.Replace(matchedText, replacementText);
				}
				sText = HttpUtility.HtmlEncode(text);
			}
		}

		//For each term in the clause, replace the placeholder with the term's VALUE
		public static void SubstituteBasicTerms(Template template, ref string sText)
		{
			string[] delimiter = new string[] { " | " };
			//match anything of the form <img...src="TextImage.ashx?text="...".../> in _text
			string text = HttpUtility.HtmlDecode(sText);
			if (!string.IsNullOrEmpty(text))
			{
				MatchCollection matches = Regex.Matches(text, XMLNames._M_TermImageTemplate);
				foreach (Match match in matches)
				{
					string termName = match.Groups[1].Value;
					string[] termNameParts = termName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
					//first search for a matching Term
					//20070817_DEG  Bug 131 - Special case - replace the quotes
					Term term = null;
					string sTermName = termNameParts[0].Replace("&quot;", "\"");
					//Note - this approach will be backward compatible.
					if (IsEmbeddedID(sTermName))
					{
						sTermName = FilterEmbeddedID(sTermName);
						term = template.FindTerm(new Guid(sTermName));
						//This could have found a ComplexList.  If so, ignore it; Complex Terms are processed elsewhere.
						if (term != null)
							if (term.TermType == TermType.ComplexList)
								term = null;
					}
					else
					{
						term = template.FindBasicTerm(sTermName);
					}
					if (term != null)
					{
						//Replace <img ... > with term value
						string matchedText = match.Result("$0");
						string replacementText;
                        if (template.IsManagedItem)
                        {
                            if (termNameParts.Length > 1)
                            {
                                replacementText = term.DisplayValue(termNameParts[1]);
                            }
                            else
                                replacementText = term.DisplayValue(string.Empty);
                            if (term._preserveWhiteSpaceDocument)
                                replacementText = string.Format("<{1}>{0}</{1}>", replacementText, _ELEMENT_PRESERVE_WHITE_SPACE);
                        }
                        else
                        {
                            replacementText = string.Concat(@"<span style=""font-weight:bold; text-decoration:underline;"">", (char)0xab, term.Name, (char)0xbb, "</span>");
                        }
						text = text.Replace(matchedText, replacementText);
					}
				}
				sText = HttpUtility.HtmlEncode(text);
			}
		}

		//The termName passed to this call is based on data from RAM, so should be considered up to date.
		//All calls that examine editor text should resolve down to this call to do the check.
		public static List<string> TermReferences(Template template, string termName, string decodedText, string client, string clientType)
		{
			//match anything of the form <img...src="TextImage.ashx?text="...".../> in _text
			List<string> rtn = new List<string>();
			//Assume that the caller has already decoded the text.
			Term term = template.FindTerm(termName);
			if (term != null && !string.IsNullOrEmpty(decodedText))
			{
				MatchCollection matches = Regex.Matches(decodedText, XMLNames._M_TermImageTemplate);
				foreach (Match match in matches)
				{
					string sTermMatch = IsEmbeddedID(match.Groups[1].Value) ? term.EmbeddedID : termName;
					if (match.Groups[1].Value == sTermMatch || match.Groups[1].Value.StartsWith(sTermMatch + " | "))
						if (string.IsNullOrEmpty(clientType))
							rtn.Add(string.Format("Term \"{0}\" is used by \"{1}\".", termName, client));
						else
							rtn.Add(string.Format("Term \"{0}\" is used by {2} \"{1}\".", termName, client, clientType));
				}
			}
			return rtn;
		}

        #endregion

		public void ApplyDependencyAction(bool canEdit, TermDependencyAction action)
		{
			switch (action.Enabled)
			{
				case TermDependencyActionValue.True:
					if (canEdit)
						_runtime.Enabled = true;
					break;
				case TermDependencyActionValue.False:
					_runtime.Enabled = false;
					SetDefaultValue();
					break;
				case TermDependencyActionValue.Default:
				default:
					break;
			}

			switch (action.Required)
			{
				case TermDependencyActionValue.True:
					_runtime.Required = true;
					break;
				case TermDependencyActionValue.False:
					_runtime.Required = false;
					break;
				case TermDependencyActionValue.Default:
				default:
					break;
			}

			if (action.SetValue != _SET_VALUE_DEFAULT)
			{
				_runtime.SetValue = action.SetValue;
			}
		}

		public static string NameAtFirstChar(string sName, char cFirst)
		{
			string sNamePart = sName;
			int nPos = sName.IndexOf(cFirst);
			if (nPos >= 0)
				sNamePart = sName.Substring(0, nPos);
			return sNamePart;
		}

		public static Guid CreateID(XmlNode node, string nodeName)
		{
			Guid gTermID = Guid.Empty;
			string sTermID = Utility.XMLHelper.GetAttributeString(node, nodeName);
			if (!string.IsNullOrEmpty(sTermID))
			{
				try
				{
					gTermID = new Guid(sTermID);
				}
				catch
				{
				}
			}
			return gTermID;
		}

		public static void StoreID(XmlDocument xmlDoc, XmlNode termNode, string nodeName, string termName, string nodeID, Guid termID)
		{
			if (Term.ValidID(termID))
			{
				Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, nodeID, termID.ToString());
			}
			else
				if (!string.IsNullOrEmpty(termName))
				{
					Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, nodeName, termName);
				}
		}

		public static bool MatchReference(Template template, Guid termID, string termName, Guid termIDToMatch, string termNameToMatch)
		{
			if (ValidID(termID) && ValidID(termIDToMatch))
			{
				return (termID.Equals(termIDToMatch));
			}
			else
				if (ValidID(termID))
				{
					return (template.FindTerm(termID).Name == termNameToMatch);
				}
				else
					return (!string.IsNullOrEmpty(termName) && termName == termNameToMatch);
        }

        #region Custom Comparers
        public static Comparison<Term> TermGroupOrderTermOrderComparison = 
            delegate(Term term1, Term term2)
            {
                if (term1.TermGroupName.Equals(term2.TermGroupName))
                {
                    return term1.Order.CompareTo(term2.Order);
                }
                else
                {
                    return term1.TermGroup.Order.CompareTo(term2.TermGroup.Order);
                }

            };
        #endregion

        #region IComparable Members
        public int CompareTo(Term term)
        {
            return Name.CompareTo(term.Name);

        }
        #endregion

        #region Data Store Members

        public static string GetTermName(XmlReader reader)
        {
            return reader.GetAttribute(XMLNames._A_Name);
        }

        public static TermStore CreateStore(string termName, XmlReader reader, TermType termType)
        {
            using (reader)
            {
                TermStore termStore = new TermStore(termName, termType);
                reader.Read();  
                reader.Read();

                ReadPastSubTree(reader, XMLNames._E_validateOn);
                termStore.AddMultiValue(Utility.XMLHelper.SafeReadElementString(reader));
                return termStore;
            }
        }

        protected static void ReadPastSubTree(XmlReader reader, string elementName)
        {
            //This handles the case of the 'validateOn' subnode...
            if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(elementName, StringComparison.InvariantCultureIgnoreCase))
            {
                using (reader.ReadSubtree()) { }
                if (reader.NodeType == XmlNodeType.EndElement)
                    reader.Read();
            }
        }

        public virtual string IsValid(TermStore termStore)
        {
            string error = string.Empty;

            if (termStore.Name != Name)
                return string.Format("Template term '{0}' does not match name '{1}'", Name, termStore.Name);

            if (termStore.TermType != TermType)
                return string.Format("Template term '{0}' has type '{1}' which does not match type '{2}", Name, TermType.ToString(), termStore.TermType.ToString());

            return string.Empty;
        }

        public virtual bool RequiresSystemSync(ITATSystem system)
        {
            return false;
        }

        public virtual void SyncWithSystem(ITATSystem system)
        {
        }

        public static bool TermTransformMatch(TermTransform left, TermTransform right)
        {
            if (left.TransformType != right.TransformType)
                return false;

            if (left.Name != right.Name)
                return false;

            if (left.Value != right.Value)
                return false;

            return true;
        }
    
        #endregion

    }
}
