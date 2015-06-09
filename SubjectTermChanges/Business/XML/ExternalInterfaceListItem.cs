using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{

	[Serializable]
	public class ExternalInterfaceListItem
	{

		private ExternalInterfaceConfig _interfaceConfig;
		private Dictionary<string, string> _fieldValues;
		private string _key;

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public Dictionary<string, string> FieldValues
		{
			get { return _fieldValues; }
			set { _fieldValues = value; }
		}

		public ExternalInterfaceListItem(ExternalInterfaceConfig interfaceConfig)
		{
			//TODO: probably will change this
			_interfaceConfig = interfaceConfig;
			_fieldValues = new Dictionary<string, string>();
		}

		public string DisplayValue()
		{
			List<string> displayValues = new List<string>();
			foreach (ExternalInterfaceAvailableField availableField in _interfaceConfig.DisplayedFields())
				displayValues.Add(_fieldValues[availableField.Name]);
			return string.Format(_interfaceConfig.DisplayFormat, displayValues.ToArray());
		}


		internal XmlNode NameValueXMLNode()
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<?xml version='1.0' encoding='UTF-8' ?><Fields />");
			XmlNode elementRoot = xmlDoc.DocumentElement;

			foreach (ExternalInterfaceAvailableField availableField in _interfaceConfig.AvailableFields)
			{
				XmlNode nodeItem = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Field, XMLNames._M_NameSpaceURI);
				Utility.XMLHelper.AddAttributeString(xmlDoc, nodeItem, XMLNames._A_DetailID, Guid.NewGuid().ToString());
				Utility.XMLHelper.AddAttributeString(xmlDoc, nodeItem, XMLNames._A_Name, availableField.Name);

				string fieldValue = string.Empty;
				try { fieldValue = _fieldValues[availableField.Name]; }
				catch { }
				Utility.XMLHelper.AddAttributeString(xmlDoc, nodeItem, XMLNames._A_Value, fieldValue);
				
				elementRoot.AppendChild(nodeItem);
			}
			return elementRoot;
		}


	}

}
