using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{

	/// <summary>
	/// ExternalInterfaceSearchCriteriaDetail class -- 
	///		used within the ExternalInterfaceSearchCriteriaValue class, 
	///		which in turn is used in the ExternalInterfaceSearchCriteria class
	/// </summary>
	[Serializable]
	public class ExternalInterfaceSearchCriteriaDetail
	{
		private string _fieldName;
		private string _fieldValue;

		public string FieldName
		{
			get { return _fieldName; }
			set { _fieldName = value; }
		}

		public string FieldValue
		{
			get { return _fieldValue; }
			set { _fieldValue = value; }
		}

		public ExternalInterfaceSearchCriteriaDetail()
		{
		}

		public ExternalInterfaceSearchCriteriaDetail(string fieldName, string fieldValue)
		{
			_fieldName = fieldName;
			_fieldValue = fieldValue;
		}


		public XmlNode BuildRuleItemDetail(XmlDocument xmlDoc)
		{
			XmlNode nodeRuleItemDetail = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_RuleItemDetail, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.AddAttributeString(xmlDoc, nodeRuleItemDetail, XMLNames._A_FieldName, _fieldName);
			Utility.XMLHelper.AddAttributeString(xmlDoc, nodeRuleItemDetail, XMLNames._A_FieldValue, _fieldValue);
			return nodeRuleItemDetail;
		}

	}



	/// <summary>
	/// ExternalInterfaceSearchCriteriaValue class -- 
	///		used within the ExternalInterfaceSearchCriteria class
	/// </summary>
	[Serializable]
	public class ExternalInterfaceSearchCriteriaValue
	{
		private string _keyValue;
		private List<ExternalInterfaceSearchCriteriaDetail> _details;

		public string KeyValue
		{
			get { return _keyValue; }
			set { _keyValue = value; }
		}

		public List<ExternalInterfaceSearchCriteriaDetail> Details
		{
			get { return _details; }
			set { _details = value; }
		}

		public ExternalInterfaceSearchCriteriaValue()
		{
			_details = new List<ExternalInterfaceSearchCriteriaDetail>();
		}

		public XmlNode BuildRuleItem(XmlDocument xmlDoc)
		{
			XmlNode nodeRuleItem = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_RuleItem, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.AddText(xmlDoc, nodeRuleItem, _keyValue);
			XmlNode nodeRuleItemDetails = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_RuleItemDetails, XMLNames._M_NameSpaceURI);
			nodeRuleItem.AppendChild(nodeRuleItemDetails);
			foreach (ExternalInterfaceSearchCriteriaDetail detail in _details)
				nodeRuleItemDetails.AppendChild(detail.BuildRuleItemDetail(xmlDoc));
			return nodeRuleItem;			
		}
	}



	/// <summary>
	/// ExternalInterfaceSearchCriteria class -- 
	///		used in the SearchCriteria classes
	/// </summary>
	[Serializable]
	public class ExternalInterfaceSearchCriteria
	{
		private string _interfaceConfigName;
		private List<ExternalInterfaceSearchCriteriaValue> _values;

		public List<string> KeyValues()
		{
			List<string> rtn = new List<string>(_values.Count);
			foreach (ExternalInterfaceSearchCriteriaValue value in _values)
				rtn.Add(value.KeyValue);
			return rtn;
		}


		public string InterfaceConfigName
		{
			get { return _interfaceConfigName; }
			set { _interfaceConfigName = value; }
		}

		public List<ExternalInterfaceSearchCriteriaValue> Values
		{
			get { return _values; }
			set { _values = value; }
		}

		public ExternalInterfaceSearchCriteria()
		{
			_values = new List<ExternalInterfaceSearchCriteriaValue>();
		}

		public ExternalInterfaceSearchCriteria(XmlNode nodeRule)
		{
			string nodeName = Utility.XMLHelper.GetAttributeString(nodeRule, XMLNames._A_Name);
			if (nodeName != XMLNames._AV_ExternalTerm)
				throw new Exception("The node '{0}' does not describe an external term.");
			_interfaceConfigName = Utility.XMLHelper.GetAttributeString(nodeRule, XMLNames._A_InterfaceConfigName);
			XmlNodeList listRuleItems = nodeRule.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_RuleItem));
			if (listRuleItems != null)
			{
				_values = new List<ExternalInterfaceSearchCriteriaValue>(listRuleItems.Count);
				foreach (XmlNode nodeRuleItem in listRuleItems)
				{
					ExternalInterfaceSearchCriteriaValue eiscv = new ExternalInterfaceSearchCriteriaValue();
					eiscv.KeyValue = Utility.XMLHelper.GetText(nodeRuleItem);
					eiscv.Details = GetSearchCriteriaValueDetails(nodeRuleItem);
					_values.Add(eiscv);
				}
			}
		}

		private List<ExternalInterfaceSearchCriteriaDetail> GetSearchCriteriaValueDetails(XmlNode nodeRuleItem)
		{
			XmlNodeList listRuleItemDetails = nodeRuleItem.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_RuleItemDetails, XMLNames._E_RuleItemDetail));
			if (listRuleItemDetails != null)
			{
				List<ExternalInterfaceSearchCriteriaDetail> rtn = new List<ExternalInterfaceSearchCriteriaDetail>(listRuleItemDetails.Count);
				foreach (XmlNode nodeRuleItemDetail in listRuleItemDetails)
					rtn.Add(new ExternalInterfaceSearchCriteriaDetail(Utility.XMLHelper.GetAttributeString(nodeRuleItemDetail, XMLNames._A_FieldName), Utility.XMLHelper.GetAttributeString(nodeRuleItemDetail, XMLNames._A_FieldValue)));
				return rtn;
			}
			return new List<ExternalInterfaceSearchCriteriaDetail>();
		}


		public XmlNode BuildRule(XmlDocument xmlDoc)
		{
			XmlNode nodeRule = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Rule, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.AddAttributeString(xmlDoc, nodeRule, XMLNames._A_Name, XMLNames._AV_ExternalTerm);
			Utility.XMLHelper.AddAttributeString(xmlDoc, nodeRule, XMLNames._A_InterfaceConfigName, _interfaceConfigName);
			foreach (ExternalInterfaceSearchCriteriaValue eiscv in _values)
				nodeRule.AppendChild(eiscv.BuildRuleItem(xmlDoc));
			return nodeRule;
		}
	}


}
