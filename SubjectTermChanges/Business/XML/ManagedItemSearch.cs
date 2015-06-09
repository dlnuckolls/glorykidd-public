using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{

	[Serializable]
	public class SearchCriteria
	{
		[Serializable]
		public struct DateRange
		{
			public DateTime? Start;
			public DateTime? End;
		}

		#region public static
		public const string _SC_DateTerm3 = "DateTerm3";
		public const string _SC_DateTerm3EndDate = "DateTerm3EndDate";
		public const string _SC_DateTerm3StartDate = "DateTerm3StartDate";
		public const string _SC_DateTerm6 = "DateTerm6";
		public const string _SC_DateTerm6EndDate = "DateTerm6EndDate";
		public const string _SC_DateTerm6StartDate = "DateTerm6StartDate";
		public const string _SC_DateTerm7 = "DateTerm7";
		public const string _SC_DateTerm7EndDate = "DateTerm7EndDate";
		public const string _SC_DateTerm7StartDate = "DateTerm7StartDate";

		public const string _SC_Facility = "Facility";
		public const string _SC_FacilityIDs = "FacilityIDs";

		public const string _SC_KeyWords = "KeyWords";

		public const string _SC_ManagedItemID = "ManagedItemID";
		public const string _SC_ManagedItemNumber = "ManagedItemNumber";

		public const string _SC_Statuses = "Statuses";

		public const string _SC_TemplateID = "TemplateID";

		public const string _SC_TextTerm1 = "TextTerm1";
		public const string _SC_TextTerm2 = "TextTerm2";
		public const string _SC_TextTerm4 = "TextTerm4";
		public const string _SC_TextTerm5 = "TextTerm5";
		#endregion

		#region private members
		private Guid? _managedItemId;
		private Guid? _templateId;
		private string _managedItemNumber;
		private List<string> _statuses;
		private string _textTerm1;
		private string _textTerm2;
		private DateRange _dateTerm3Range;
		private string _textTerm4;
		private string _textTerm5;
		private DateRange _dateTerm6Range;
		private DateRange _dateTerm7Range;
		private List<int> _facilityIds;
		private Dictionary<string, ExternalInterfaceSearchCriteria> _externalTermsCriteria;
		private string _keyWords;
		#endregion

		#region properties

		public Guid? ManagedItemId
		{
			get { return _managedItemId; }
			set { _managedItemId = value; }
		}

		public Guid? TemplateId
		{
			get { return _templateId; }
			set { _templateId = value; }
		}

		public string ManagedItemNumber
		{
			get { return _managedItemNumber; }
			set { _managedItemNumber = value; }
		}

		public List<string> Statuses
		{
			get { return _statuses; }
			set { _statuses = value; }
		}

		public string TextTerm1
		{
			get { return _textTerm1; }
			set { _textTerm1 = value; }
		}

		public string TextTerm2
		{
			get { return _textTerm2; }
			set { _textTerm2 = value; }
		}

		public DateRange DateTerm3Range
		{
			get { return _dateTerm3Range; }
			set { _dateTerm3Range = value; }
		}

		public string TextTerm4
		{
			get { return _textTerm4; }
			set { _textTerm4 = value; }
		}

		public string TextTerm5
		{
			get { return _textTerm5; }
			set { _textTerm5 = value; }
		}

		public DateRange DateTerm6Range
		{
			get { return _dateTerm6Range; }
			set { _dateTerm6Range = value; }
		}

		public DateRange DateTerm7Range
		{
			get { return _dateTerm7Range; }
			set { _dateTerm7Range = value; }
		}

		public List<int> FacilityIds
		{
			get { return _facilityIds; }
			set { _facilityIds = value; }
		}

		public Dictionary<string, ExternalInterfaceSearchCriteria> ExternalTermsCriteria
		{
			get { return _externalTermsCriteria; }
			set { _externalTermsCriteria = value; }
		}

		public string KeyWords
		{
			get { return _keyWords; }
			set { _keyWords = value; }
		}

		#endregion

		#region constructor

		public SearchCriteria()
		{
			_statuses = new List<string>();
			_facilityIds = new List<int>();
			_externalTermsCriteria = new Dictionary<string, ExternalInterfaceSearchCriteria>();
		}

		#endregion

		#region xml constructor

		/*
		 *		<Rules>
		 *				<Rule Name="Statuses" >
		 *						<RuleItem>OPEN</RuleItem>
		 *						<RuleItem>CLOSED</RuleItem>
		 *						<RuleItem>ETC</RuleItem>
		 *				</Rule>
		 *				<Rule Name="FacilityIDs" >
		 *						<RuleItem>492</RuleItem>
		 *						<RuleItem>123</RuleItem>
		 *						<RuleItem>456</RuleItem>
		 *				</Rule>
		 *				<Rule Name="ManagedItemID" >123-456-0000-3333</Rule>
		 *				<Rule Name="TemplateID" >123-456-0000-3333</Rule>
		 *		</Rules>
		 */

		//for Report
		public SearchCriteria(XmlNode nodeReport)
		{
			_statuses = new List<string>();
			_facilityIds = new List<int>();
			_externalTermsCriteria = new Dictionary<string, ExternalInterfaceSearchCriteria>();

			if (nodeReport == null)
				return;

			XmlNodeList listRules = nodeReport.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Rules, XMLNames._E_Rule));
			if (listRules != null)
			{
				foreach (XmlNode nodeRule in listRules)
				{
					string sError = AddRule(nodeRule);
					if (!string.IsNullOrEmpty(sError))
						throw new Exception(string.Format("Unable to add rule - error '{0}'", sError));
				}
			}
		}

		public string UpdateTermValue(string systemTermDBStorage, string value, ITATSystem system)
		{
			if (system != null && system.TermIsMultiSelectSearch(systemTermDBStorage))
				value = "%" + value;
			return value;
		}

		private string AddRule(XmlNode nodeRule)
		{
			string sName = Utility.XMLHelper.GetAttributeString(nodeRule, XMLNames._A_Name);
			string sValue = Utility.XMLHelper.GetText(nodeRule);
			switch (sName)
			{
				case _SC_ManagedItemID:
					if (_managedItemId.HasValue)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					_managedItemId = new Guid(sValue);
					break;

				case _SC_TemplateID:
					if (_templateId.HasValue)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					_templateId = new Guid(sValue);
					break;

				case _SC_ManagedItemNumber:
					if (ManagedItemNumber != null)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					ManagedItemNumber = sValue;
					break;

				case _SC_Statuses:
					if (_statuses != null)
						if (_statuses.Count > 0)
							throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					_statuses = GetRuleValues(nodeRule);
					break;

				case _SC_TextTerm1:
					if (TextTerm1 != null)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					TextTerm1 = UpdateTermValue(Data.DataNames._C_Term1, sValue, null);
					break;

				case _SC_TextTerm2:
					if (TextTerm2 != null)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					TextTerm2 = UpdateTermValue(Data.DataNames._C_Term2, sValue, null);
					break;

				case _SC_DateTerm3StartDate:
					if (_dateTerm3Range.Start.HasValue)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					_dateTerm3Range.Start = Utility.DateHelper.GetXMLDate(sValue);
					break;

				case _SC_DateTerm3EndDate:
					if (_dateTerm3Range.End.HasValue)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					_dateTerm3Range.End = Utility.DateHelper.GetXMLDate(sValue);
					break;

				case _SC_TextTerm4:
					if (TextTerm4 != null)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					TextTerm4 = UpdateTermValue(Data.DataNames._C_Term4, sValue, null);
					break;

				case _SC_TextTerm5:
					if (TextTerm5 != null)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					TextTerm5 = UpdateTermValue(Data.DataNames._C_Term5, sValue, null);
					break;

				case _SC_DateTerm6StartDate:
					if (_dateTerm6Range.Start.HasValue)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					_dateTerm6Range.Start = Utility.DateHelper.GetXMLDate(sValue);
					break;

				case _SC_DateTerm6EndDate:
					if (_dateTerm6Range.End.HasValue)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					_dateTerm6Range.End = Utility.DateHelper.GetXMLDate(sValue);
					break;
				case _SC_DateTerm7StartDate:
					if (_dateTerm7Range.Start.HasValue)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					_dateTerm7Range.Start = Utility.DateHelper.GetXMLDate(sValue);
					break;

				case _SC_DateTerm7EndDate:
					if (_dateTerm7Range.End.HasValue)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					_dateTerm7Range.End = Utility.DateHelper.GetXMLDate(sValue);
					break;

				case _SC_FacilityIDs:
					if (_facilityIds != null)
						if (_facilityIds.Count > 0)
							throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					List<string> ruleValues = GetRuleValues(nodeRule);
					if (ruleValues != null)
					{
						int nFacilityID;
						foreach (string ruleValue in ruleValues)
						{
							if (int.TryParse(ruleValue, out nFacilityID))
							{
								_facilityIds.Add(nFacilityID);
							}
							else
							{
								throw new Exception(string.Format("Not able to parse '{0}' into an integer FacilityID for '{1}'", ruleValue, _SC_FacilityIDs));
							}
						}
					}
					break;

				case _SC_KeyWords:
					if (KeyWords != null)
						throw new Exception(String.Format("Encountered more than one value for '{0}'", sName));
					KeyWords = sValue;
					break;

				case XMLNames._AV_ExternalTerm:
					string interfaceConfigName = Utility.XMLHelper.GetAttributeString(nodeRule, XMLNames._A_InterfaceConfigName);
					_externalTermsCriteria.Add(interfaceConfigName, new ExternalInterfaceSearchCriteria(nodeRule));
					break;

				default:
					break;
			}

			return "";
		}

		private List<string> GetRuleValues(XmlNode nodeRule)
		{
			XmlNodeList listRuleItems = nodeRule.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_RuleItem));
			if (listRuleItems != null)
			{
				List<string> ruleValues = new List<string>(listRuleItems.Count);
				foreach (XmlNode nodeRuleItem in listRuleItems)
				{
					string ruleValue = Utility.XMLHelper.GetText(nodeRuleItem);
					ruleValues.Add(ruleValue);
				}
				return ruleValues;
			}
			return null;
		}

		#endregion

		#region Build XML (for Report)

		public void Build(XmlDocument xmlDoc, XmlNode elementRules)
		{
			if (_managedItemId.HasValue)
				BuildRule(xmlDoc, elementRules, _SC_ManagedItemID, _managedItemId.ToString());
			if (_templateId.HasValue)
				BuildRule(xmlDoc, elementRules, _SC_TemplateID, _templateId.ToString());
			if (!string.IsNullOrEmpty(_managedItemNumber))
				BuildRule(xmlDoc, elementRules, _SC_ManagedItemNumber, _managedItemNumber);

			if (_statuses.Count > 0)
			{
				XmlNode nodeRule = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Rule, XMLNames._M_NameSpaceURI);
				Utility.XMLHelper.AddAttributeString(xmlDoc, nodeRule, XMLNames._A_Name, _SC_Statuses);
				foreach (string status in _statuses)
					BuildRuleItem(xmlDoc, nodeRule, status);
				elementRules.AppendChild(nodeRule);
			}

			if (!string.IsNullOrEmpty(_textTerm1))
				BuildRule(xmlDoc, elementRules, _SC_TextTerm1, _textTerm1);
			if (!string.IsNullOrEmpty(_textTerm2))
				BuildRule(xmlDoc, elementRules, _SC_TextTerm2, _textTerm2);
			if (_dateTerm3Range.Start.HasValue)
				BuildRule(xmlDoc, elementRules, _SC_DateTerm3StartDate, Utility.DateHelper.SetXMLDate(_dateTerm3Range.Start.Value));
			if (_dateTerm3Range.End.HasValue)
				BuildRule(xmlDoc, elementRules, _SC_DateTerm3EndDate, Utility.DateHelper.SetXMLDate(_dateTerm3Range.End.Value));
			if (!string.IsNullOrEmpty(_textTerm4))
				BuildRule(xmlDoc, elementRules, _SC_TextTerm4, _textTerm4);
			if (!string.IsNullOrEmpty(_textTerm5))
				BuildRule(xmlDoc, elementRules, _SC_TextTerm5, _textTerm5);
			if (_dateTerm6Range.Start.HasValue)
				BuildRule(xmlDoc, elementRules, _SC_DateTerm6StartDate, Utility.DateHelper.SetXMLDate(_dateTerm6Range.Start.Value));
			if (_dateTerm6Range.End.HasValue)
				BuildRule(xmlDoc, elementRules, _SC_DateTerm6EndDate, Utility.DateHelper.SetXMLDate(_dateTerm6Range.End.Value));
			if (_dateTerm7Range.Start.HasValue)
				BuildRule(xmlDoc, elementRules, _SC_DateTerm7StartDate, Utility.DateHelper.SetXMLDate(_dateTerm7Range.Start.Value));
			if (_dateTerm7Range.End.HasValue)
				BuildRule(xmlDoc, elementRules, _SC_DateTerm7EndDate, Utility.DateHelper.SetXMLDate(_dateTerm7Range.End.Value));

			if (_facilityIds.Count > 0)
			{
				XmlNode nodeRule = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Rule, XMLNames._M_NameSpaceURI);
				Utility.XMLHelper.AddAttributeString(xmlDoc, nodeRule, XMLNames._A_Name, _SC_FacilityIDs);
				foreach (int facilityId in _facilityIds)
					BuildRuleItem(xmlDoc, nodeRule, facilityId.ToString());
				elementRules.AppendChild(nodeRule);
			}

			if (!string.IsNullOrEmpty(_keyWords))
				BuildRule(xmlDoc, elementRules, _SC_KeyWords, _keyWords);

			if (_externalTermsCriteria != null)
			{
				foreach (string interfaceName in _externalTermsCriteria.Keys)
				{
					ExternalInterfaceSearchCriteria interfaceSearchCriteria = _externalTermsCriteria[interfaceName];
					elementRules.AppendChild(interfaceSearchCriteria.BuildRule(xmlDoc));
				}
			}
		}


		private bool BuildRule(XmlDocument xmlDoc, XmlNode nodeRules, string sName, string sValue)
		{
			XmlNode nodeRule = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Rule, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.AddAttributeString(xmlDoc, nodeRule, XMLNames._A_Name, sName);
			Utility.XMLHelper.AddText(xmlDoc, nodeRule, sValue);
			nodeRules.AppendChild(nodeRule);
			return true;
		}

		private bool BuildRuleItem(XmlDocument xmlDoc, XmlNode nodeRule, string sValue)
		{
			XmlNode nodeRuleItem = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_RuleItem, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.AddText(xmlDoc, nodeRuleItem, sValue);
			nodeRule.AppendChild(nodeRuleItem);
			return true;
		}

		#endregion

		#region Serialization Routines


		public string Serialize()
		{

			string xmlSerializedString = String.Empty;
			MemoryStream memoryStream = new MemoryStream();
			XmlSerializer xs = new XmlSerializer(typeof(SearchCriteria));
			StringWriter sw = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(sw);
			xs.Serialize(xmlTextWriter, this);

			// memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
			xmlSerializedString = sw.ToString();
			// xmlSerializedString = SearchCriteria.UTF8ByteArrayToString(memoryStream.ToArray());
			return xmlSerializedString;

		}

		public static object DeserializeObject(String pXmlizedString)
		{
			XmlSerializer xs = new XmlSerializer(typeof(SearchCriteria));
			//MemoryStream memoryStream = new MemoryStream(SearchCriteria.StringToAsciiByteArray(pXmlizedString));
			//XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);

			StringReader reader = new StringReader(pXmlizedString);

			object results = xs.Deserialize(reader);

			return results;
		}

		private static string UTF8ByteArrayToString(byte[] characters)
		{

			UTF8Encoding encoding = new UTF8Encoding();
			string constructedString = encoding.GetString(characters);
			return (constructedString);
		}

		private static byte[] StringToUTF8ByteArray(string pXmlString)
		{
			UTF8Encoding encoding = new UTF8Encoding();

			byte[] byteArray = encoding.GetBytes(pXmlString);
			return byteArray;
		}

		#endregion

	}

	public enum SearchResultSortField
	{
		ManagedItemNumber,
		TemplateName,
		TemplateStatus,
		Status,
		State,
		Facility,
		TextTerm1,
		TextTerm2,
		DateTerm3,
		TextTerm4,
		TextTerm5,
		DateTerm6,
		DateTerm7
	}

	[Serializable]
	public class SearchResultItem
	{
		#region  private members
		private Guid _managedItemId;
		private Guid _templateId;
		private string _templateName;
		private TemplateStatusType _templateStatus;
		private string _managedItemNumber;
		private string _status;
		private string _state;
		private string _facility;   //facility_name
		private string _textTerm1;
		private string _textTerm2;
		private DateTime? _dateTerm3;
		private string _textTerm4;
		private string _textTerm5;
		private DateTime? _dateTerm6;
		private DateTime? _dateTerm7;
		private Dictionary<string, List<string>> _externalTermKeyValues;
		#endregion

		#region properties

		public Guid ManagedItemId
		{
			get { return _managedItemId; }
			set { _managedItemId = value; }
		}

		public Guid TemplateId
		{
			get { return _templateId; }
			set { _templateId = value; }
		}

		public string TemplateName
		{
			get { return _templateName; }
			set { _templateName = value; }
		}

		public TemplateStatusType TemplateStatus
		{
			get { return _templateStatus; }
			set { _templateStatus = value; }
		}

		public string ManagedItemNumber
		{
			get { return _managedItemNumber; }
			set { _managedItemNumber = value; }
		}

		public string Status
		{
			get { return _status; }
			set { _status = value; }
		}
		public string State
		{
			get { return _state; }
			set { _state = value; }
		}


		public string Facility
		{
			get { return _facility; }
			set { _facility = value; }
		}

		public string TextTerm1
		{
			get { return _textTerm1; }
			set { _textTerm1 = value; }
		}

		public string TextTerm2
		{
			get { return _textTerm2; }
			set { _textTerm2 = value; }
		}

		public DateTime? DateTerm3
		{
			get { return _dateTerm3; }
			set { _dateTerm3 = value; }
		}

		public string TextTerm4
		{
			get { return _textTerm4; }
			set { _textTerm4 = value; }
		}

		public string TextTerm5
		{
			get { return _textTerm5; }
			set { _textTerm5 = value; }
		}

		public DateTime? DateTerm6
		{
			get { return _dateTerm6; }
			set { _dateTerm6 = value; }
		}

		public DateTime? DateTerm7
		{
			get { return _dateTerm7; }
			set { _dateTerm7 = value; }
		}

		//NOTE: as of now, ExternalTermKeys is returned so that records not meeting the criteria can be filtered out.   
		//This does not return the user-displayable data to be shown or sorted on the Search Results screen.
		public Dictionary<string, List<string>> ExternalTermKeyValues
		{
			get { return _externalTermKeyValues; }
			set { _externalTermKeyValues = value; }
		}

		#endregion

		public SearchResultItem()
		{
			_externalTermKeyValues = new Dictionary<string, List<string>>();
		}
	}


	[Serializable]
	public class SearchResults : List<SearchResultItem>
	{

		private static int CompareByTemplateName(SearchResultItem a, SearchResultItem b)
		{
			if (a.TemplateName == b.TemplateName)
				return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
			else
				return a.TemplateName.CompareTo(b.TemplateName);
		}

		private static int CompareByTemplateStatus(SearchResultItem a, SearchResultItem b)
		{
			if (a.TemplateStatus == b.TemplateStatus)
				return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
			else
				return a.TemplateStatus.ToString("D").CompareTo(b.TemplateStatus.ToString("D"));
		}


		private static int CompareByManagedItemNumber(SearchResultItem a, SearchResultItem b)
		{
			return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
		}

		private static int CompareByStatus(SearchResultItem a, SearchResultItem b)
		{
			if (a.Status == b.Status)
				return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
			else
				return a.Status.CompareTo(b.Status);
		}

		private static int CompareByState(SearchResultItem a, SearchResultItem b)
		{
			if (a.State == b.State)
				return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
			else
				return a.State.CompareTo(b.State);
		}

		private static int CompareByFacility(SearchResultItem a, SearchResultItem b)
		{
			if (a.Facility == b.Facility)
				return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
			else
				return a.Facility.CompareTo(b.Facility);
		}

		private static int CompareByTextTerm1(SearchResultItem a, SearchResultItem b)
		{
			if (a.TextTerm1 == b.TextTerm1)
				return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
			else
				return a.TextTerm1.CompareTo(b.TextTerm1);
		}

		private static int CompareByTextTerm2(SearchResultItem a, SearchResultItem b)
		{
			if (a.TextTerm2 == b.TextTerm2)
				return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
			else
				return a.TextTerm2.CompareTo(b.TextTerm2);
		}

		private static int CompareByDateTerm3(SearchResultItem a, SearchResultItem b)
		{
			if (a.DateTerm3.HasValue)
			{
				if (b.DateTerm3.HasValue)
				{
					if (a.DateTerm3.Value == b.DateTerm3.Value)
					{
						return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
					}
					else
					{
						return a.DateTerm3.Value.CompareTo(b.DateTerm3.Value);
					}
				}
				else
				{
					return -1;
				}
			}
			else
			{
				if (b.DateTerm3.HasValue)
				{
					return 1;
				}
				else
				{
					return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
				}
			}
		}

		private static int CompareByTextTerm4(SearchResultItem a, SearchResultItem b)
		{
			if (a.TextTerm4 == b.TextTerm4)
				return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
			else
				return a.TextTerm4.CompareTo(b.TextTerm4);
		}

		private static int CompareByTextTerm5(SearchResultItem a, SearchResultItem b)
		{
			if (a.TextTerm5 == b.TextTerm5)
				return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
			else
				return a.TextTerm5.CompareTo(b.TextTerm5);
		}

		private static int CompareByDateTerm6(SearchResultItem a, SearchResultItem b)
		{
			if (a.DateTerm6.HasValue)
			{
				if (b.DateTerm6.HasValue)
				{
					if (a.DateTerm6.Value == b.DateTerm6.Value)
					{
						return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
					}
					else
					{
						return a.DateTerm6.Value.CompareTo(b.DateTerm6.Value);
					}
				}
				else
				{
					return -1;
				}
			}
			else
			{
				if (b.DateTerm6.HasValue)
				{
					return 1;
				}
				else
				{
					return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
				}
			}
		}

		private static int CompareByDateTerm7(SearchResultItem a, SearchResultItem b)
		{
			if (a.DateTerm7.HasValue)
			{
				if (b.DateTerm7.HasValue)
				{
					if (a.DateTerm7.Value == b.DateTerm7.Value)
					{
						return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
					}
					else
					{
						return a.DateTerm7.Value.CompareTo(b.DateTerm7.Value);
					}
				}
				else
				{
					return -1;
				}
			}
			else
			{
				if (b.DateTerm7.HasValue)
				{
					return 1;
				}
				else
				{
					return a.ManagedItemNumber.CompareTo(b.ManagedItemNumber);
				}
			}
		}

		private static int ReverseCompareByTemplateName(SearchResultItem a, SearchResultItem b)
		{
			return CompareByTemplateName(b, a);
		}

		private static int ReverseCompareByManagedItemNumber(SearchResultItem a, SearchResultItem b)
		{
			return CompareByManagedItemNumber(b, a);
		}

		private static int ReverseCompareByStatus(SearchResultItem a, SearchResultItem b)
		{
			return CompareByStatus(b, a);
		}

		private static int ReverseCompareByState(SearchResultItem a, SearchResultItem b)
		{
			return CompareByState(b, a);
		}

		private static int ReverseCompareByFacility(SearchResultItem a, SearchResultItem b)
		{
			return CompareByFacility(b, a);
		}

		private static int ReverseCompareByTextTerm1(SearchResultItem a, SearchResultItem b)
		{
			return CompareByTextTerm1(b, a);
		}

		private static int ReverseCompareByTextTerm2(SearchResultItem a, SearchResultItem b)
		{
			return CompareByTextTerm2(b, a);
		}

		private static int ReverseCompareByDateTerm3(SearchResultItem a, SearchResultItem b)
		{
			return CompareByDateTerm3(b, a);
		}

		private static int ReverseCompareByTextTerm4(SearchResultItem a, SearchResultItem b)
		{
			return CompareByTextTerm4(b, a);
		}

		private static int ReverseCompareByTextTerm5(SearchResultItem a, SearchResultItem b)
		{
			return CompareByTextTerm5(b, a);
		}

		private static int ReverseCompareByDateTerm6(SearchResultItem a, SearchResultItem b)
		{
			return CompareByDateTerm6(b, a);
		}

		private static int ReverseCompareByDateTerm7(SearchResultItem a, SearchResultItem b)
		{
			return CompareByDateTerm7(b, a);
		}

		public void SortBy(SearchResultSortField sortField, bool sortAscending)
		{
			switch (sortField)
			{
				case SearchResultSortField.TemplateName:
					if (sortAscending)
						Sort(SearchResults.CompareByTemplateName);
					else
						Sort(SearchResults.ReverseCompareByTemplateName);
					break;
				case SearchResultSortField.ManagedItemNumber:
					if (sortAscending)
						Sort(SearchResults.CompareByManagedItemNumber);
					else
						Sort(SearchResults.ReverseCompareByManagedItemNumber);
					break;
				case SearchResultSortField.Status:
					if (sortAscending)
						Sort(SearchResults.CompareByStatus);
					else
						Sort(SearchResults.ReverseCompareByStatus);
					break;
				case SearchResultSortField.State:
					if (sortAscending)
						Sort(SearchResults.CompareByState);
					else
						Sort(SearchResults.ReverseCompareByState);
					break;
				case SearchResultSortField.Facility:
					if (sortAscending)
						Sort(SearchResults.CompareByFacility);
					else
						Sort(SearchResults.ReverseCompareByFacility);
					break;
				case SearchResultSortField.TextTerm1:
					if (sortAscending)
						Sort(SearchResults.CompareByTextTerm1);
					else
						Sort(SearchResults.ReverseCompareByTextTerm1);
					break;
				case SearchResultSortField.TextTerm2:
					if (sortAscending)
						Sort(SearchResults.CompareByTextTerm2);
					else
						Sort(SearchResults.ReverseCompareByTextTerm2);
					break;
				case SearchResultSortField.DateTerm3:
					if (sortAscending)
						Sort(SearchResults.CompareByDateTerm3);
					else
						Sort(SearchResults.ReverseCompareByDateTerm3);
					break;
				default:
					if (sortAscending)
						Sort(SearchResults.CompareByManagedItemNumber);
					else
						Sort(SearchResults.ReverseCompareByManagedItemNumber);
					break;
			}
		}
	}

	public class ManagedItemSearch
	{
		#region  public methods

		public SearchResults Search(Guid systemId, SearchCriteria criteria, List<string> userRoles)
		{
			if (systemId == Guid.Empty || systemId == null)
				throw new ArgumentNullException("ITAT System ID is not a valid Guid");
			if (criteria == null)
				throw new ArgumentNullException("SearchCriteria is null");

			SearchResults rtn = new SearchResults();
			System.Data.DataSet ds;


			//If ManagedItemID specified, retrieve that Managed Item
			//If ManagedItemNumber specified, retrieve any ManagedItems that begin with specified value
			//If no criteria is specified, return all managed items for a given system
			//Otherwise, populate the SearchCriteria class with the specified values, and call the general Data.ManagedItem.Find method.
			if (criteria.ManagedItemId.HasValue)
			{
				ds = Data.ManagedItem.FindById(criteria.ManagedItemId.Value, userRoles);
			}
			else
			{
				if (!string.IsNullOrEmpty(criteria.ManagedItemNumber))
				{
					ds = Data.ManagedItem.FindByNumber(systemId, criteria.ManagedItemNumber, userRoles);
				}
				else
				{
					if (
						!criteria.TemplateId.HasValue &&
						criteria.Statuses.Count == 0 &&
						criteria.FacilityIds.Count == 0 &&
						string.IsNullOrEmpty(criteria.TextTerm1) &&
						string.IsNullOrEmpty(criteria.TextTerm2) &&
						!criteria.DateTerm3Range.Start.HasValue &&
						string.IsNullOrEmpty(criteria.KeyWords) &&
						(criteria.ExternalTermsCriteria == null || criteria.ExternalTermsCriteria.Count == 0)
						)
					{
						ds = Data.ManagedItem.FindByNumber(systemId, string.Empty, userRoles);
					}
					else
					{
						ds = Data.ManagedItem.Find(
								systemId,
								criteria.TemplateId,
								criteria.Statuses,
								criteria.FacilityIds,
								criteria.TextTerm1,
								criteria.TextTerm2,
								criteria.DateTerm3Range.Start,
								criteria.DateTerm3Range.End,
								criteria.TextTerm4,
								criteria.TextTerm5,
								criteria.DateTerm6Range.Start,
								criteria.DateTerm6Range.End,
								criteria.DateTerm7Range.Start,
								criteria.DateTerm7Range.End,
								criteria.KeyWords,
								userRoles);
					}
				}
			}

			//Note - the dataset will always contain information for Term1, Term2, Term3.
			//Whether or not the data is 'valid' depends on the system xml information.
			if (ds.Tables.Count > 0)
				foreach (System.Data.DataRow row in ds.Tables[0].Rows)
				{
					SearchResultItem searchResultItem = new SearchResultItem();
					searchResultItem.ManagedItemId = new Guid(row[Data.DataNames._C_ManagedItemID].ToString());
					searchResultItem.TemplateName = row[Data.DataNames._C_TemplateName].ToString();
					searchResultItem.TemplateId = new Guid(row[Data.DataNames._C_TemplateID].ToString());
					searchResultItem.TemplateStatus = (TemplateStatusType)Enum.Parse(typeof(TemplateStatusType), row[Data.DataNames._C_TemplateStatus].ToString());
					searchResultItem.ManagedItemNumber = row[Data.DataNames._C_ManagedItemNumber].ToString();
					searchResultItem.Status = row[Data.DataNames._C_Status].ToString();
					searchResultItem.State = row[Data.DataNames._C_State].ToString();
					searchResultItem.Facility = row[Data.DataNames._C_facility_name].ToString();
					searchResultItem.TextTerm1 = row[Data.DataNames._C_Term1].ToString();
					searchResultItem.TextTerm2 = row[Data.DataNames._C_Term2].ToString();
					string dateValue = row[Data.DataNames._C_Term3].ToString();
					DateTime dt;
					if (DateTime.TryParse(dateValue, out dt))
						searchResultItem.DateTerm3 = dt;
					else
						searchResultItem.DateTerm3 = null;
					searchResultItem.TextTerm4 = row[Data.DataNames._C_Term4].ToString();
					searchResultItem.TextTerm5 = row[Data.DataNames._C_Term5].ToString();

					dateValue = row[Data.DataNames._C_Term6].ToString();
					if (DateTime.TryParse(dateValue, out dt))
						searchResultItem.DateTerm6 = dt;
					else
						searchResultItem.DateTerm6 = null;

					dateValue = row[Data.DataNames._C_Term7].ToString();
					if (DateTime.TryParse(dateValue, out dt))
						searchResultItem.DateTerm7 = dt;
					else
						searchResultItem.DateTerm7 = null;

					bool includeSearchResultItem = true;
					
					//exclude ManagedItems associated with "Inactive" Templates
					if (searchResultItem.TemplateStatus == TemplateStatusType.Inactive)
					{
						includeSearchResultItem = false;
					}

					if (includeSearchResultItem)
					{
						//if there are external terms, and if there is any selection criteria specified for them, check to see if the data returned matches that criteria
						string externalTermValues = (string) row[Data.DataNames._C_ExternalTermKeyValues];
						if (string.IsNullOrEmpty(externalTermValues))
						{
							// no external term values were returned for this managed item, 
							// so include the search result item if no external terms were specified in the search criteria
							foreach (string interfaceConfigName in criteria.ExternalTermsCriteria.Keys)
							{
								if (criteria.ExternalTermsCriteria[interfaceConfigName].Values.Count > 0)
								{
									includeSearchResultItem = false;
									break;
								}
							}
						}
						else
						{
							//match the external term values for this manageditem with the ones in the search criteria, to determine whether to leave the item in the search results
							searchResultItem.ExternalTermKeyValues = ParseExternalTermKeyValues(externalTermValues);
							foreach (string interfaceConfigName in criteria.ExternalTermsCriteria.Keys)
							{
								List<string> searchCriteriaKeyValues = criteria.ExternalTermsCriteria[interfaceConfigName].KeyValues();
								if (searchCriteriaKeyValues.Count > 0)
								{
									List<string> resultItemKeyValues = searchResultItem.ExternalTermKeyValues[interfaceConfigName];
									if (!Utility.ListHelper.HaveAMatch<string>(searchCriteriaKeyValues, resultItemKeyValues))
									{
										includeSearchResultItem = false;
										break;
									}
								}
							}
						}
					}

					if (includeSearchResultItem)
						rtn.Add(searchResultItem);
				}
			return rtn;
		}


		private Dictionary<string, List<string>> ParseExternalTermKeyValues(string externalTermValues)
		{
			// sample string:  |Interface=FM~2523~2571|Interface=2ndInterface~ABC~XYZ~DEF
			Dictionary<string, List<string>> rtn = new Dictionary<string, List<string>>();
			string[] termIdPlusKeysArray = externalTermValues.Split(new string[] { "|Interface=" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string interfacePlusKeys in termIdPlusKeysArray)
			{
				int firstDelimeterPos = interfacePlusKeys.IndexOf('~');
				string interfaceName = interfacePlusKeys.Substring(0, firstDelimeterPos);
				string[] keys = interfacePlusKeys.Substring(firstDelimeterPos + 1).Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
				rtn.Add(interfaceName, new List<string>(keys));
			}
			return rtn;
		}

		public DataTable FindStore(Guid systemID, IEnumerable<Guid> templateId, IEnumerable<string> statuses, IEnumerable<int> facilityIds, string textTerm1, string textTerm2, SearchCriteria.DateRange dateTerm3, string textTerm4, string textTerm5, SearchCriteria.DateRange dateTerm6, SearchCriteria.DateRange dateTerm7)
		{
			return Data.ManagedItem.FindStore(systemID, templateId, statuses, facilityIds, textTerm1, textTerm2, dateTerm3.Start, dateTerm3.End, textTerm4, textTerm5, dateTerm6.Start, dateTerm6.End, dateTerm7.Start, dateTerm7.End);
		}

		#endregion
	}
}
