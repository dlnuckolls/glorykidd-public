using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{


	public enum SearchableFieldType
	{
		Fixed = 0,
		MultiValue,
		Entry
	}


	[Serializable]
	public class ExternalInterfaceSearchableField
	{
		private string _name;
		private SearchableFieldType _fieldType;
		private bool _visible;
		private string _filter;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		public SearchableFieldType FieldType
		{
			get { return _fieldType; }
			set { _fieldType = value; }
		}

		public string Filter
		{
			get { return _filter; }
			set { _filter = value; }
		}


		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Name, _name);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Type, _fieldType.ToString());
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_Visible, _visible);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Filter, _filter);
		}

	}
}
