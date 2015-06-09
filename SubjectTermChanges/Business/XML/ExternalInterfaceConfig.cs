using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;

namespace Kindred.Knect.ITAT.Business
{

	public enum ExternalInterfaceSelectionMode
	{
		//The only SelectionMode that is currently implemented is "MultiValued".
		//So all other values are currently ignored (i.e., treated as MultiValued).
		Unknown,   
		MultiValued,
		SingleValuedReadOnly,
		SingleValuedEditable
	}

	[Serializable]
	public class ExternalInterfaceConfig
	{

		#region private fields

		private string _name;
		private string _webServiceURL;
		private List<ExternalInterfaceAvailableField> _availableFields;
		private List<ExternalInterfaceSearchableField> _searchableFields;
		private ExternalInterfaceSelectionMode _selectionMode;
		private string _displayFormat;

        //Term Transform Info:  These are normally inherited from Term.
        private List<Term.TermTransform> _termTransforms;
        readonly private TermType? _termTransformType;

		#endregion

		#region properties

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string WebServiceURL
		{
			get { return _webServiceURL; }
			set { _webServiceURL = value; }
		}

		public List<ExternalInterfaceAvailableField> AvailableFields
		{
			get
			{
				_availableFields.Sort(new ExternalInterfaceAvailableField.DisplayOrderComparer());
				return _availableFields;
			}
			set { _availableFields = value; }
		}

		public List<ExternalInterfaceSearchableField> SearchableFields
		{
			get { return _searchableFields; }
			set { _searchableFields = value; }
		}

		public ExternalInterfaceSelectionMode SelectionMode
		{
			get { return _selectionMode; }
			set { _selectionMode = value; }
		}

		public string DisplayFormat
		{
			get { return _displayFormat; }
			set { _displayFormat = value; }
		}

        public List<Term.TermTransform> TermTransforms
        {
            get { return _termTransforms; }
            set { _termTransforms = value; }
        }

        public TermType? TransformTermType
        {
            get { return _termTransformType; }
        }

		#endregion



		#region constructors

        public ExternalInterfaceConfig()
        {

        }

		public ExternalInterfaceConfig(XmlNode externalInterfaceNode)
		{
			_name = Utility.XMLHelper.GetAttributeString(externalInterfaceNode, Business.XMLNames._A_Name);
			_webServiceURL = Utility.XMLHelper.GetAttributeString(externalInterfaceNode, Business.XMLNames._A_URL);
			_selectionMode = (ExternalInterfaceSelectionMode)Enum.Parse(typeof(ExternalInterfaceSelectionMode), Utility.XMLHelper.GetAttributeString(externalInterfaceNode, Business.XMLNames._A_SelectionMode));
			_displayFormat = Utility.XMLHelper.GetAttributeString(externalInterfaceNode, Business.XMLNames._A_DisplayFormat);
            string termTransformType = Utility.XMLHelper.GetAttributeString(externalInterfaceNode, XMLNames._A_TransformTermType);
            if (!string.IsNullOrEmpty(termTransformType))
                _termTransformType = (TermType)Enum.Parse(typeof(TermType), termTransformType);

			XmlNodeList listAvailableFields = externalInterfaceNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_AvailableFields, XMLNames._E_AvailableField));
			if (listAvailableFields != null)
			{
				_availableFields = new List<ExternalInterfaceAvailableField>(listAvailableFields.Count);
				foreach (XmlNode nodeAvailableField in listAvailableFields)
				{
					ExternalInterfaceAvailableField field = new ExternalInterfaceAvailableField();
					field.Name = Utility.XMLHelper.GetAttributeString(nodeAvailableField, XMLNames._A_Name);
					field.DisplayOrder = Utility.XMLHelper.GetAttributeInt(nodeAvailableField, XMLNames._A_DisplayOrder) ?? -1;
					field.DisplayWidth = Utility.XMLHelper.GetAttributeString(nodeAvailableField, XMLNames._A_DisplayWidth);
					field.DisplayName = Utility.XMLHelper.GetAttributeString(nodeAvailableField, XMLNames._A_DisplayName);
					field.IsKeyField = Utility.XMLHelper.GetAttributeBool(nodeAvailableField, XMLNames._A_IsKey) ?? false;
					_availableFields.Add(field);
				}
			}
			else
			{
				_availableFields = new List<ExternalInterfaceAvailableField>();
			}

			XmlNodeList listSearchableFields = externalInterfaceNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_SearchableFields, XMLNames._E_SearchableField));
			if (listSearchableFields != null)
			{
				_searchableFields = new List<ExternalInterfaceSearchableField>(listSearchableFields.Count);
				foreach (XmlNode nodeSearchableField in listSearchableFields)
				{
					ExternalInterfaceSearchableField field = new ExternalInterfaceSearchableField();
					field.Name = Utility.XMLHelper.GetAttributeString(nodeSearchableField, XMLNames._A_Name);
					//TODO:  error on next line (???) if Type="Fixed"
					field.FieldType = (SearchableFieldType)Enum.Parse(typeof(SearchableFieldType),  Utility.XMLHelper.GetAttributeString(nodeSearchableField, XMLNames._A_Type));
					field.Visible  = Utility.XMLHelper.GetAttributeBool(nodeSearchableField, XMLNames._A_Visible) ?? false;
					field.Filter = Utility.XMLHelper.GetAttributeString(nodeSearchableField, XMLNames._A_Filter);
					_searchableFields.Add(field);
				}
			}
			else
			{
				_searchableFields = new List<ExternalInterfaceSearchableField>();
			}
            _termTransforms = Term.GetTransforms(externalInterfaceNode);
		}

		#endregion


		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Name, _name);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_DisplayFormat, _displayFormat);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_SelectionMode, _selectionMode.ToString());
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_URL, _webServiceURL);
            if (_termTransformType.HasValue)
                Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_TransformTermType, _termTransformType.Value.ToString());

			XmlNode elementAvailableFields = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_AvailableFields, XMLNames._M_NameSpaceURI);
			foreach (ExternalInterfaceAvailableField availableField in _availableFields)
			{
				XmlNode elementAvailableField = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_AvailableField, XMLNames._M_NameSpaceURI);
				availableField.Build(xmlDoc, elementAvailableField, bValidate);
				elementAvailableFields.AppendChild(elementAvailableField);
			}
			node.AppendChild(elementAvailableFields);

			XmlNode elementSearchableFields = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_SearchableFields, XMLNames._M_NameSpaceURI);
			foreach (ExternalInterfaceSearchableField searchableField in _searchableFields)
			{
				XmlNode elementSearchableField = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_SearchableField, XMLNames._M_NameSpaceURI);
				searchableField.Build(xmlDoc, elementSearchableField, bValidate);
				elementSearchableFields.AppendChild(elementSearchableField);
			}
			node.AppendChild(elementSearchableFields);
		}


		public List<ExternalInterfaceAvailableField> DisplayedFields()
		{
			List<ExternalInterfaceAvailableField> displayedFields = new List<ExternalInterfaceAvailableField>();
			foreach (ExternalInterfaceAvailableField availableField in _availableFields)
				if (availableField.IsDisplayed)
					displayedFields.Add(availableField);
			displayedFields.Sort(new ExternalInterfaceAvailableField.DisplayOrderComparer());
			return displayedFields;
		}

		public string DisplayedFields(string delimeter)
		{
			List<ExternalInterfaceAvailableField> fieldList = DisplayedFields();
			fieldList.Sort(new ExternalInterfaceAvailableField.DisplayOrderComparer());
			string[] fieldNames = new string[fieldList.Count];
			for (int index = 0; index < fieldList.Count; index++)
				fieldNames[index] = fieldList[index].Name;
			return string.Join(delimeter, fieldNames);
		}


		public ExternalInterfaceAvailableField FindAvailableField(string fieldName)
		{
			Predicate<ExternalInterfaceAvailableField> p = delegate(ExternalInterfaceAvailableField f) { return (f.Name == fieldName); };
			return _availableFields.Find(p);

		}


		public ExternalInterfaceSearchableField FindSearchableField(string fieldName)
		{
			Predicate<ExternalInterfaceSearchableField> p = delegate(ExternalInterfaceSearchableField f) { return (f.Name == fieldName); };
			return _searchableFields.Find(p);
		}

        public ExternalInterfaceConfig Copy()
        {
            ExternalInterfaceConfig iConfig = new ExternalInterfaceConfig();
            iConfig._name = _name;
            iConfig._webServiceURL = _webServiceURL;
            iConfig._availableFields = new List<ExternalInterfaceAvailableField>(_availableFields);
            iConfig._searchableFields = new List<ExternalInterfaceSearchableField>(_searchableFields);
            iConfig._selectionMode = _selectionMode;
            iConfig._displayFormat = _displayFormat;
            return iConfig;
        }

			
	}
}
