using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	public enum RenewalTermType
	{
		Manual,
		None
	}

	public enum DisplayedDate
	{
		ExpirationDate,
		EffectiveDate
	}

	[Serializable]
	public class RenewalTerm : Term
	{
		#region private members
		private bool? _allowBackDating;
		private string _renewalType;
		private string _popUpText;

		private DateTime? _effectiveDate;
		private string _effectiveDateFormat;
		private string _expirationDateFormat;
		private DisplayedDate _displayedDate;

		private int? _initialDurationUnitCountDefault;
		private int? _initialDurationUnitCount;
		private int? _renewalDurationUnitCountDefault;
		private int? _renewalDurationUnitCount;
		private string _expirationDate_DBField;
		private bool? _initialDurationUnit_PopUpIfNot;
		private string _initialDurationUnit_PopUpText;
		private List<DurationUnit> _initialDurationUnits;
		private bool? _renewalDurationUnit_PopUpIfNot;
		private string _renewalDurationUnit_PopUpText;
		private List<DurationUnit> _renewalDurationUnits;
		private RenewalTermType _renewalTermType;
		private int? _renewalCount;
		private bool? _renewalDurationEditable;
		private bool? _sendNotification;
		private List<Role> _renewerRoles;
		private bool _isManagedItem = false;

        public const string _LABEL_EFFECTIVE_DATE = "EffectiveDate";
        public const string _LABEL_INTIAL_DURATION_COUNT = "InitialDurationCount";
        public const string _LABEL_RENEWAL_DURATION_COUNT = "RenewalDurationCount";
        public const string _LABEL_RENEWAL_COUNT = "RenewalCount";
        public const char _RENEWAL_TERM_DATE_DELIMITER = '^';

		#endregion

		#region Properties

		public bool? AllowBackDating
		{
			get { return _allowBackDating; }
			set { _allowBackDating = value; }
		}

		public string RenewalType
		{
			get { return Utility.XMLHelper.GetXMLText(_renewalType); }
			set { _renewalType = Utility.XMLHelper.SetXMLText(value); }
		}

		public string PopUpText
		{
			get { return Utility.XMLHelper.GetXMLText(_popUpText); }
			set { _popUpText = Utility.XMLHelper.SetXMLText(value); }
		}

		//EffectiveDate
		public DateTime? EffectiveDate
		{
			get { return _effectiveDate; }
			set { _effectiveDate = value; }
		}

		public string EffectiveDateFormat
		{
			get { return _effectiveDateFormat; }
			set { _effectiveDateFormat = value; }
		}

		public string ExpirationDateFormat
		{
			get { return _expirationDateFormat; }
			set { _expirationDateFormat = value; }
		}

		public DisplayedDate DisplayedDate
		{
			get { return _displayedDate; }
			set { _displayedDate = value; }
		}

		//ExpirationDate
		public DateTime? ExpirationDate
		{
			get { return CalculateExpirationDate(); }
		}
		
		public string ExpirationDateDBField
		{
			get { return Utility.XMLHelper.GetXMLText(_expirationDate_DBField); }
			set { _expirationDate_DBField = Utility.XMLHelper.SetXMLText(value); }
		}

		public int? RenewalCount
		{
			get { return _renewalCount; }
			set { _renewalCount = value; }
		}

		//InitialDurationUnit
		public bool? InitialDurationUnitPopUpIfNot
		{
			get { return _initialDurationUnit_PopUpIfNot; }
			set { _initialDurationUnit_PopUpIfNot = value; }
		}

		public string InitialDurationUnitPopUpText
		{
			get { return Utility.XMLHelper.GetXMLText(_initialDurationUnit_PopUpText); }
			set { _initialDurationUnit_PopUpText = Utility.XMLHelper.SetXMLText(value); }
		}

		public string InitialDurationUnitDefault
		{
			get 
			{
				foreach (DurationUnit du in _initialDurationUnits)
				{
					if (du.Default ?? false)
						return du.Value;
				}
				return null; 
			}
		}

		public string InitialDurationUnitSelected
		{
			get
			{
				foreach (DurationUnit du in _initialDurationUnits)
				{
					if (du.Selected ?? false)
						return du.Value;
				}
				return null;
			}
			set
			{
				foreach (DurationUnit du in _initialDurationUnits)
					du.Selected = (du.Value == value);
			}
		}

		public List<DurationUnit> InitialDurationUnits
		{
			get { return _initialDurationUnits; }
			set { _initialDurationUnits = value; }
		}

		//InitialDurationUnitCount
		public int? InitialDurationUnitCountDefault
		{
			get { return _initialDurationUnitCountDefault; }
			set { _initialDurationUnitCountDefault = value; }
		}

		public int? InitialDurationUnitCount
		{
			get { return _initialDurationUnitCount; }
			set { _initialDurationUnitCount = value; }
		}

		//RenewalDurationUnit
		public bool? RenewalDurationUnitPopUpIfNot
		{
			get { return _renewalDurationUnit_PopUpIfNot; }
			set { _renewalDurationUnit_PopUpIfNot = value; }
		}

		public string RenewalDurationUnitPopUpText
		{
			get { return Utility.XMLHelper.GetXMLText(_renewalDurationUnit_PopUpText); }
			set { _renewalDurationUnit_PopUpText = Utility.XMLHelper.SetXMLText(value); }
		}

		public List<DurationUnit> RenewalDurationUnits
		{
			get { return _renewalDurationUnits; }
			set { _renewalDurationUnits = value; }
		}

		public string RenewalDurationUnitDefault
		{
			get
			{
				foreach (DurationUnit du in _renewalDurationUnits)
				{
					if (du.Default ?? false)
						return du.Value;
				}
				return null;
			}
		}

		public string RenewalDurationUnitSelected
		{
			get
			{
				foreach (DurationUnit du in _renewalDurationUnits)
				{
					if (du.Selected ?? false)
						return du.Value;
				}
				return null;
			}
			set
			{
				foreach (DurationUnit du in _renewalDurationUnits)
					du.Selected = (du.Value == value);
			}
		}

		//RenewalDurationUnitCount
		public int? RenewalDurationUnitCount
		{
			get { return _renewalDurationUnitCount; }
			set { _renewalDurationUnitCount = value; }
		}

		public int? RenewalDurationUnitCountDefault
		{
			get { return _renewalDurationUnitCountDefault; }
			set { _renewalDurationUnitCountDefault = value; }
		}

		public RenewalTermType RenewalTermType
		{
			get { return _renewalTermType; }
			set { _renewalTermType = value; }
		}

		public bool IsTypeNone
		{
			get { return _renewalTermType == RenewalTermType.None; }
		}

		public override string Keyword
		{
			get
			{
				if (!HasKeyWord)
					return null;
				string sEffectiveDate = "";
				string sExpirationDate = "";
				if (_effectiveDate.HasValue)
				{
					sEffectiveDate = Utility.DateHelper.FormatDate(_effectiveDate.Value, EffectiveDateFormat);
				}
				DateTime? expirationDate = CalculateExpirationDate();
				if (expirationDate.HasValue)
				{
					sExpirationDate = Utility.DateHelper.FormatDate(expirationDate.Value, ExpirationDateFormat);
				}
				string keyWord = string.Concat(sEffectiveDate, XMLNames._M_Delimiter, sExpirationDate).Trim(XMLNames._M_Delimiter);
				return keyWord.Length > 0 ? keyWord : null;
			}
		}

		public bool? RenewalDurationEditable
		{
			get { return _renewalDurationEditable; }
			set { _renewalDurationEditable = value; }
		}

		public bool? SendNotification
		{
			get { return _sendNotification; }
			set { _sendNotification = value; }
		}

		public List<Role> RenewerRoles
		{
			get { return _renewerRoles; }
			set { _renewerRoles = value; }
		}


		public Event RenewalEvent
		{
			get { return _template.RenewalEvent; }
			//set { _renewalEvent = value; }
		}

        public override int? RequiredSize
        {
            get
            {
                return null;
            }
        }

		#endregion

		#region Constructors

        public RenewalTerm(bool systemTerm, bool IsManagedItem, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
			TermType = TermType.Renewal;
			NameRequired = true;
			_initialDurationUnits = new List<DurationUnit>();
			_renewalDurationUnits = new List<DurationUnit>();
			_renewerRoles = new List<Role>();
			_isManagedItem = IsManagedItem;
			_displayedDate = DisplayedDate.ExpirationDate;

			if (template != null)
				if (template.RenewalEvent == null)
					template.Events.Add(new Event(EventType.Renewal, IsManagedItem));
		}

        public RenewalTerm(XmlNode termNode, bool IsManagedItem, Template template, bool isFilter)
            : base(termNode, template, isFilter)
		{
			_isManagedItem = IsManagedItem;
			TermType = TermType.Renewal;
			NameRequired = true;
	
			if (template != null)
				if (template.RenewalEvent == null)
					template.Events.Add(new Event(EventType.Renewal, IsManagedItem));

			XmlNode nodeEffectiveDate = termNode.SelectSingleNode(XMLNames._E_EffectiveDate);
			XmlNode nodeExpirationDate = termNode.SelectSingleNode(XMLNames._E_ExpirationDate);
			XmlNode nodeInitialDurationUnit = termNode.SelectSingleNode(XMLNames._E_InitialDurationUnit);
			XmlNode nodeInitialDurationUnitCount = termNode.SelectSingleNode(XMLNames._E_InitialDurationUnitCount);
			XmlNode nodeRenewalDurationUnit = termNode.SelectSingleNode(XMLNames._E_RenewalDurationUnit);
			XmlNode nodeRenewalDurationUnitCount = termNode.SelectSingleNode(XMLNames._E_RenewalDurationUnitCount);
			XmlNodeList listInitialDurationUnits = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_InitialDurationUnit,XMLNames._E_DurationUnit));
			XmlNodeList listRenewalDurationUnits = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_RenewalDurationUnit,XMLNames._E_DurationUnit));

			_renewerRoles = new List<Role>();
			XmlNodeList listRenewers = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Renewers, XMLNames._E_Renewer));
			if (listRenewers != null)
				foreach (XmlNode nodeRenewer in listRenewers)
				{
					_renewerRoles.Add(new Role(Utility.XMLHelper.GetAttributeString(nodeRenewer, XMLNames._A_Role)));
				}
			_renewalType = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_RenewalType);
			_popUpText = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_PopUpText);
			_allowBackDating = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_AllowBackDating);
			_renewalCount = Utility.XMLHelper.GetAttributeInt(termNode, XMLNames._A_RenewalCount);
			_renewalDurationEditable = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_RenewalDurationEditable);
			_effectiveDateFormat = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_EffectiveDateFormat);
			_expirationDateFormat = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_ExpirationDateFormat);

			try
			{
				_displayedDate = (DisplayedDate)Enum.Parse(typeof(DisplayedDate), Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_DisplayedDate));
			}
			catch
			{
				_displayedDate = DisplayedDate.ExpirationDate;
			}

			_sendNotification = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_SendNotification);
			string offsetType = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_DateOffsetType);

			try
			{
				_renewalTermType = (RenewalTermType)Enum.Parse(typeof(RenewalTermType), Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_RenewalTermType));
			}
			catch (ArgumentException)
			{
				_renewalTermType = RenewalTermType.None;
			}
			_popUpText = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_PopUpText);

			//EffectiveDate
			if (nodeEffectiveDate != null)
			{
				_effectiveDate = Utility.XMLHelper.GetValueDate(nodeEffectiveDate);
			}

			//ExpirationDate
			if (nodeExpirationDate != null)
			{
				//_expirationDate = Utility.XMLHelper.GetValueDate(nodeExpirationDate);
				_expirationDate_DBField = Utility.XMLHelper.GetAttributeString(nodeExpirationDate, XMLNames._A_DBFieldName);
			}

			//InitialDurationUnit
			if (nodeInitialDurationUnit != null)
			{
				_initialDurationUnit_PopUpIfNot = Utility.XMLHelper.GetAttributeBool(nodeInitialDurationUnit, XMLNames._A_PopUpIfNot);
				_initialDurationUnit_PopUpText = Utility.XMLHelper.GetAttributeString(nodeInitialDurationUnit, XMLNames._A_PopUpText);

				//Sub element DurationUnit_Value, Display
				if (listInitialDurationUnits != null)
				{
					_initialDurationUnits = new List<DurationUnit>(listInitialDurationUnits.Count);
					foreach (XmlNode nodeInitialDurationUnitDU in listInitialDurationUnits)
					{
						DurationUnit InitialDurationUnit = new DurationUnit(nodeInitialDurationUnitDU);
						_initialDurationUnits.Add(InitialDurationUnit);
					}
				}
			}

			//InitialDurationUnitCount
			if (nodeInitialDurationUnitCount != null)
			{
				_initialDurationUnitCountDefault = Utility.XMLHelper.GetAttributeInt(nodeInitialDurationUnitCount, XMLNames._A_Default);
				_initialDurationUnitCount = Utility.XMLHelper.GetValueInt(nodeInitialDurationUnitCount);
			}

			//RenewalDurationUnit
			if (nodeRenewalDurationUnit != null)
			{
				_renewalDurationUnit_PopUpIfNot = Utility.XMLHelper.GetAttributeBool(nodeRenewalDurationUnit, XMLNames._A_PopUpIfNot);
				_renewalDurationUnit_PopUpText = Utility.XMLHelper.GetAttributeString(nodeRenewalDurationUnit, XMLNames._A_PopUpText);

				//Sub element DurationUnit_Value, Display
				if (listRenewalDurationUnits != null)
				{
					_renewalDurationUnits = new List<DurationUnit>(listRenewalDurationUnits.Count);
					foreach (XmlNode nodeRenewalDurationUnitDU in listRenewalDurationUnits)
					{
						DurationUnit renewalDurationUnit = new DurationUnit(nodeRenewalDurationUnitDU);
						_renewalDurationUnits.Add(renewalDurationUnit);
					}
				}

			}

			//RenewalDurationUnitCount
			if (nodeRenewalDurationUnitCount != null)
			{
				_renewalDurationUnitCountDefault = Utility.XMLHelper.GetAttributeInt(nodeRenewalDurationUnitCount, XMLNames._A_Default);
				_renewalDurationUnitCount = Utility.XMLHelper.GetValueInt(nodeRenewalDurationUnitCount);
			}
		}

		#endregion

		#region Private Methods

		private DateTime? CalculateExpirationDate()
		{
			//ExpirationDate = EffectiveDate + InitialDuration + RenewalCount*RenewalDuration
			if (!_effectiveDate.HasValue)
				return null;
			if (!_initialDurationUnitCount.HasValue)
				return null;
			if (!IsTypeNone)
				if (!_renewalDurationUnitCount.HasValue)
					return null;
			if (_initialDurationUnits == null)
				return null;
			if (!IsTypeNone)
				if (_renewalDurationUnits == null)
					return null;
			int renewalCount = _renewalCount ?? 0;

			string sInitialDurationUnit = "";
			foreach (DurationUnit InitialDurationUnit in _initialDurationUnits)
			{
				if (InitialDurationUnit.Selected ?? false)
				{
					sInitialDurationUnit = InitialDurationUnit.Value;
					break;
				}
			}

			string sRenewalDurationUnit = "";
			foreach (DurationUnit renewalDurationUnit in _renewalDurationUnits)
			{
				if (renewalDurationUnit.Selected ?? false)
				{
					sRenewalDurationUnit = renewalDurationUnit.Value;
					break;
				}
			}

			DateTime? expirationDate = _effectiveDate;
			expirationDate = ModifyDate(expirationDate, sInitialDurationUnit, _initialDurationUnitCount.Value);
			if (!IsTypeNone)
				expirationDate = ModifyDate(expirationDate, sRenewalDurationUnit, renewalCount * _renewalDurationUnitCount.Value);

			//ADDED 11/2/2007: the expiration date should be the day BEFORE the anniverary date.
			//	Example: a managed item with a duration of 1 year and an effective date of February 1, 2008 
			//	should have an expiration date of January 31, 2009.
			if (expirationDate.HasValue)
				expirationDate = expirationDate.Value.AddDays(-1);

			return expirationDate;
		}

		private DateTime? ModifyDate(DateTime? dtOld, string sDUType, int nCount)
		{
			if (!dtOld.HasValue)
				return null;

			if (string.IsNullOrEmpty(sDUType))
				return null;

			DurationUnitType duType = (DurationUnitType)Enum.Parse(typeof(DurationUnitType), sDUType);

			switch (duType)
			{
				case DurationUnitType.Day:
					return dtOld.Value.AddDays(nCount);

				case DurationUnitType.Week:
					return dtOld.Value.AddDays(nCount * 7);

				case DurationUnitType.Month:
					return dtOld.Value.AddMonths(nCount);

				case DurationUnitType._90Days:
					return dtOld.Value.AddDays(nCount * 90);

				case DurationUnitType._3Months:
					return dtOld.Value.AddMonths(nCount * 3);

				case DurationUnitType.Year:
					return dtOld.Value.AddYears(nCount);
			}

			return null;
		}



		#endregion

		#region Build XML

		//The input to this call should be an "empty" parent node
		public override void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
			}

			base.Build(xmlDoc, termNode, bValidate);

			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_RenewalType, _renewalType);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_AllowBackDating, _allowBackDating);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_RenewalTermType, _renewalTermType.ToString());
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_PopUpText, _popUpText);
			Utility.XMLHelper.AddAttributeInt(xmlDoc, termNode, XMLNames._A_RenewalCount, _renewalCount);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_RenewalDurationEditable, _renewalDurationEditable);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_EffectiveDateFormat, _effectiveDateFormat);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_ExpirationDateFormat, _expirationDateFormat);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_DisplayedDate, _displayedDate.ToString());

			XmlNode elementRenewers = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Renewers, XMLNames._M_NameSpaceURI);
			foreach (Role renewerRole in _renewerRoles)
			{
				XmlNode elementRenewerRole = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Renewer, XMLNames._M_NameSpaceURI);
				Utility.XMLHelper.AddAttributeString(xmlDoc, elementRenewerRole, XMLNames._E_Role, renewerRole.Name);
				elementRenewers.AppendChild(elementRenewerRole);
			}
			termNode.AppendChild(elementRenewers);

			if (_template != null)
			{
				if (RenewalEvent != null)
				{
					Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_SendNotification, _sendNotification);
				}
			}

			//EffectiveDate
			XmlNode elementEffectiveDate = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_EffectiveDate, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.SetValueDate(xmlDoc, elementEffectiveDate, _effectiveDate);
			termNode.AppendChild(elementEffectiveDate);

			//ExpirationDate
			XmlNode elementExpirationDate = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_ExpirationDate, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.SetValueDate(xmlDoc, elementExpirationDate, CalculateExpirationDate());
			Utility.XMLHelper.AddAttributeString(xmlDoc, elementExpirationDate, XMLNames._A_DBFieldName, _expirationDate_DBField);
			termNode.AppendChild(elementExpirationDate);

			//InitialDurationUnit
			XmlNode elementInitialDurationUnit = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_InitialDurationUnit, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, elementInitialDurationUnit, XMLNames._A_PopUpIfNot, _initialDurationUnit_PopUpIfNot);
			Utility.XMLHelper.AddAttributeString(xmlDoc, elementInitialDurationUnit, XMLNames._A_PopUpText, _initialDurationUnit_PopUpText);

			//InitialDurationUnitDurationUnit - subelement of InitialDurationUnit
			if (_initialDurationUnits != null)
			{
				foreach (DurationUnit InitialDurationUnit in _initialDurationUnits)
				{
					XmlNode elementInitialDurationUnitDurationUnit = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_DurationUnit, XMLNames._M_NameSpaceURI);
					InitialDurationUnit.Build(xmlDoc, elementInitialDurationUnitDurationUnit, bValidate);
					elementInitialDurationUnit.AppendChild(elementInitialDurationUnitDurationUnit);
				}
			}
			termNode.AppendChild(elementInitialDurationUnit);

			//InitialDurationUnitCount
			XmlNode elementInitialDurationUnitCount = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_InitialDurationUnitCount, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.AddAttributeInt(xmlDoc, elementInitialDurationUnitCount, XMLNames._A_Default, _initialDurationUnitCountDefault);
			Utility.XMLHelper.SetValueInt(xmlDoc, elementInitialDurationUnitCount, _initialDurationUnitCount);
			termNode.AppendChild(elementInitialDurationUnitCount);

			//RenewalDurationUnit
			XmlNode elementRenewalDurationUnit = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_RenewalDurationUnit, XMLNames._M_NameSpaceURI);

			Utility.XMLHelper.AddAttributeBool(xmlDoc, elementRenewalDurationUnit, XMLNames._A_PopUpIfNot, _renewalDurationUnit_PopUpIfNot);
			Utility.XMLHelper.AddAttributeString(xmlDoc, elementRenewalDurationUnit, XMLNames._A_PopUpText, _renewalDurationUnit_PopUpText);

			//RenewalDurationUnitDurationUnit - subelement of RenewalDurationUnit
			if (_renewalDurationUnits != null)
			{
				foreach (DurationUnit renewalDurationUnit in _renewalDurationUnits)
				{
					XmlNode elementRenewalDurationUnitDurationUnit = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_DurationUnit, XMLNames._M_NameSpaceURI);
					renewalDurationUnit.Build(xmlDoc, elementRenewalDurationUnitDurationUnit, bValidate);
					elementRenewalDurationUnit.AppendChild(elementRenewalDurationUnitDurationUnit);
				}
			}
			termNode.AppendChild(elementRenewalDurationUnit);

			//RenewalDurationUnitCount
			XmlNode elementRenewalDurationUnitCount = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_RenewalDurationUnitCount, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.AddAttributeInt(xmlDoc, elementRenewalDurationUnitCount, XMLNames._A_Default, _renewalDurationUnitCountDefault);
			Utility.XMLHelper.SetValueInt(xmlDoc, elementRenewalDurationUnitCount, _renewalDurationUnitCount);
			termNode.AppendChild(elementRenewalDurationUnitCount);
		}

		#endregion
		

		public override string DisplayValue(string termPartSpecifier)
		{
			switch (termPartSpecifier)
			{
				case XMLNames._TPS_InitialTermDuration:
					if (InitialDurationUnitCount.HasValue)
						return Utility.TextHelper.TextPlusNumber(InitialDurationUnitCount.Value) + " " + InitialDurationUnitSelected.ToLower() + (InitialDurationUnitCount.Value != 1 ? "s" : "");
					else
						return "";

				case XMLNames._TPS_RenewalTermDuration:
					if (!IsTypeNone)
						if (RenewalDurationUnitCount.HasValue)
							return Utility.TextHelper.TextPlusNumber(RenewalDurationUnitCount.Value) + " " + RenewalDurationUnitSelected.ToLower() + (RenewalDurationUnitCount.Value != 1 ? "s" : "");
						else
							return "";
					else
						return "";

				case XMLNames._TPS_TerminationDate:
				case XMLNames._TPS_RenewalDate:
				case XMLNames._TPS_ExpirationDate:
					if (ExpirationDate.HasValue)
						return Utility.DateHelper.FormatDate(ExpirationDate.Value, ExpirationDateFormat);
					else
						return "";

				case XMLNames._TPS_EffectiveDate:
				default:
					if (EffectiveDate.HasValue)
						return Utility.DateHelper.FormatDate(EffectiveDate.Value, EffectiveDateFormat);
					else
						return "";
			}
		}


        public override List<string> Validate(bool includeTab, string filterTermTabName)
		{
			//Since the default value of the Required property of a  Renewal Term is true and there is no place in the application to set it to false, 
			// it is assumed that the value of the "Required" property is true.
			Runtime.Required = true;

			List<string> rtn = new List<string>();

			//Check for missing portion of the Renewal Term
			List<string> missingItems = new List<string>();
			if (!EffectiveDate.HasValue)
				missingItems.Add("Effective Date");
			if (!InitialDurationUnitCount.HasValue || string.IsNullOrEmpty(InitialDurationUnitSelected))
				missingItems.Add("Initial Duration");
			if (!IsTypeNone)
				if (!RenewalDurationUnitCount.HasValue || string.IsNullOrEmpty(RenewalDurationUnitSelected))
				missingItems.Add("Renewal Duration");
			switch (missingItems.Count)
			{
				case 0:
					break;
				case 1:
                    rtn.Add(string.Format("\"{0}\"{1} is a required term, but the {2} is missing or invalid.", Name, includeTab ? string.Format(" (tab {0})", TermGroupName) : string.Empty, missingItems[0]));
					break;
				case 2:
                    rtn.Add(string.Format("\"{0}\"{3} is a required term, but the {1} and {2} are missing or invalid.", Name, missingItems[0], missingItems[1], includeTab ? string.Format(" (tab {0})", TermGroupName) : string.Empty));
					break;
				default:
					System.Text.StringBuilder sb = new StringBuilder();
					for (int i = 0; i < missingItems.Count - 1; i++)
						sb.AppendFormat("{0}, ", missingItems[i]);
					sb.AppendFormat("and {0}", missingItems[missingItems.Count - 1]);
                    rtn.Add(string.Format("\"{0}\"{2} is a required term, but the {1} are missing or invalid.", Name, sb.ToString(), includeTab ? string.Format(" (tab {0})", TermGroupName) : string.Empty));
					break;
			}

			//Check for backdating the managed item
			if (!(this.AllowBackDating ?? false))
				if (EffectiveDate.HasValue)
					if (EffectiveDate.Value.Date < DateTime.Today.Date)
                        rtn.Add(string.Format("Backdating is not permitted on the \"{0}\" term{1}.  The Effective Date cannot be prior to today.", Name, includeTab ? string.Format(" (tab {0})", TermGroupName) : string.Empty));
	
		return rtn;
		}

        public override List<string> CheckType(bool includeTab, string filterTermTabName)
        {
            return null;
        }
	
		public override bool EmitPDFXML(XmlWriter writer)
		{
			PdfHelper.AddPDFXMLHeader(writer);
			PdfHelper.AddPDFXMLRow(writer, Name, string.Concat(XMLNames._TPS_EffectiveDate, ":"), DisplayValue(XMLNames._TPS_EffectiveDate));
			PdfHelper.AddPDFXMLRow(writer, "", string.Concat(XMLNames._TPS_InitialTermDuration, ":"), DisplayValue(XMLNames._TPS_InitialTermDuration));
			if (IsTypeNone)
			{
				PdfHelper.AddPDFXMLRow(writer, "", string.Concat(XMLNames._TPS_ExpirationDate, ":"), DisplayValue(XMLNames._TPS_RenewalDate));
			}
			else
			{
				PdfHelper.AddPDFXMLRow(writer, "", string.Concat(XMLNames._TPS_RenewalTermDuration, ":"), DisplayValue(XMLNames._TPS_RenewalTermDuration));
				PdfHelper.AddPDFXMLRow(writer, "", string.Concat(XMLNames._TPS_RenewalDate, ":"), DisplayValue(XMLNames._TPS_RenewalDate));
			}
			PdfHelper.AddPDFXMLFooter(writer);
			return true;
		}


		public List<DateTime> GetEventDates()
		{
			if (RenewalEvent == null)
				return null;
			if (RenewalEvent.ScheduledEvents == null)
				return null;
			if (!ExpirationDate.HasValue)
				return null;
			List<DateTime> dtEvents = new List<DateTime>();
			foreach (ScheduledEvent scheduledEvent in RenewalEvent.ScheduledEvents)
			{
				dtEvents.Add(ExpirationDate.Value.AddDays(-1 * scheduledEvent.DateOffset));
			}
			return dtEvents;
		}


		public override void SetDefaultValue()
		{
			if ((_initialDurationUnitCountDefault ?? 0) > 0)
				_initialDurationUnitCount = _initialDurationUnitCountDefault.Value;
			if ((_renewalDurationUnitCountDefault ?? 0) > 0)
				_renewalDurationUnitCount = _renewalDurationUnitCountDefault;
		}


		public void Renew()
		{
			if (_renewalCount.HasValue)
				_renewalCount++;
			else
				_renewalCount = 1;
			CalculateExpirationDate();
		}

        public override void Clear()
		{
			_initialDurationUnitCount = null;
			_renewalDurationUnitCount = null;
			_effectiveDate = null;
			InitialDurationUnitSelected = string.Empty;
			RenewalDurationUnitSelected = string.Empty;
		}

        public override void Migrate(Term term)
        {
            if (term == null)
                base.Migrate(term);
            if (!(term is RenewalTerm))
                return;
            else
            {
                RenewalTerm sourceTerm = term as RenewalTerm;
                _effectiveDate = sourceTerm._effectiveDate;
                _initialDurationUnitCount = sourceTerm._initialDurationUnitCount;
                _renewalDurationUnitCount = sourceTerm._renewalDurationUnitCount;
                _initialDurationUnits = new List<DurationUnit>(sourceTerm._initialDurationUnits);
                _renewalDurationUnits = new List<DurationUnit>(sourceTerm._renewalDurationUnits);
                _renewalCount = sourceTerm._renewalCount;
                term.Runtime.Migrated = true;
            }
            Runtime.Migrated = true;
        }

        public static DisplayedDate GetLoadBaseDateTermDisplayType(string columnName)
        {
            if (columnName.IndexOf(_RENEWAL_TERM_DATE_DELIMITER) > 0)
            {
                if (columnName.IndexOf(XMLNames._TPS_EffectiveDate) > 0)
                    return DisplayedDate.EffectiveDate;
                else
                    if (columnName.IndexOf(XMLNames._TPS_ExpirationDate) > 0)
                        return DisplayedDate.ExpirationDate;
            }
            throw new Exception(string.Format("DateTermDisplayType could not be determined from '{0}'", columnName));
        }

        public static string GetLoadBaseDateTermName(string columnName)
        {
            return columnName.Replace(string.Format("{0}", _RENEWAL_TERM_DATE_DELIMITER), "").Replace(XMLNames._TPS_EffectiveDate, "").Replace(XMLNames._TPS_ExpirationDate, "");
        }

        public override void Load(string value, string pattern, char? delimiter)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string[] values = value.Split(delimiter.Value);
                string[] sections = pattern.Split(delimiter.Value);
                if (values.Length != sections.Length)
                    throw new Exception(string.Format("Received {0:D} values, expected {1:D}", values.Length, sections.Length));
                for (int i = 0; i < sections.Length; i++)
                {
                    if (!string.IsNullOrEmpty(values[i]))
                    {
                        switch (sections[i])
                        {
                            case _LABEL_EFFECTIVE_DATE:
                                _effectiveDate = DateTime.Parse(values[i]);
                                break;

                            case _LABEL_INTIAL_DURATION_COUNT:
                                _initialDurationUnitCount = int.Parse(values[i]);
                                break;

                            case _LABEL_RENEWAL_DURATION_COUNT:
                                _renewalDurationUnitCount = int.Parse(values[i]);
                                break;

                            case _LABEL_RENEWAL_COUNT:
                                _renewalCount = int.Parse(values[i]);
                                break;

                            case Business.Event._LABEL_OFFSET_TERM_NAME:
                                Term term = _template.FindBasicTerm(values[i]);
                                if (term != null)
                                {
                                    _template.RenewalEvent.OffsetTermID = term.ID;
                                }
                                break;

                            case Business.Event._LABEL_OFFSET_DEFAULT_VALUE:
                                int offsetDefaultValue;
                                if (int.TryParse(values[i], out offsetDefaultValue))
                                {
                                    _template.RenewalEvent.OffsetDefaultValue = offsetDefaultValue;
                                }
                                break;

                            case Business.Event._LABEL_BASE_DATE_OFFSET:
                                _template.RenewalEvent.BaseDateOffset = values[i];
                                break;
                        }
                    }
                }
            }
        }

        public static TermStore CreateStore(string termName, XmlReader reader)
        {
            using (reader)
            {
                reader.Read();

                TermStore termStore = new TermStore(termName, TermType.Renewal);
                DisplayedDate displayedDateType = (DisplayedDate)Enum.Parse(typeof(DisplayedDate), reader.GetAttribute(XMLNames._A_DisplayedDate));

                bool effectiveDateProcessed = false;
                bool expirationDateProcessed = false;

                string value = string.Empty;

                reader.Read();

                ReadPastSubTree(reader, XMLNames._E_validateOn);

                int loopCount = 0;
                while (!(effectiveDateProcessed && expirationDateProcessed))
                {
                    if (loopCount++ > TermStore.maxLoopCount)
                    {
                        throw new Exception(string.Format("CreateStore stopped at loopCount {0:D} for Renewal term '{1}'", loopCount, termName));
                    }
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case XMLNames._E_EffectiveDate:
                                    value = Utility.XMLHelper.SafeReadElementString(reader);
                                    if (displayedDateType == DisplayedDate.EffectiveDate)
                                    {
                                        termStore.AddMultiValue(value);
                                        expirationDateProcessed = true;
                                    }
                                    effectiveDateProcessed = true;
                                    break;

                                case XMLNames._E_ExpirationDate:
                                    value = Utility.XMLHelper.SafeReadElementString(reader);
                                    if (displayedDateType == DisplayedDate.ExpirationDate)
                                    {
                                        termStore.AddMultiValue(value);
                                        effectiveDateProcessed = true;
                                    }
                                    expirationDateProcessed = true;
                                    break;

                                default:
                                    reader.Read();
                                    break;
                            }
                            break;

                        default:
                            reader.Read();
                            break;
                    }
                }
                return termStore;
            }
        }

        public override string[] StoreColumns
        {
            get
            {
                List<string> columns = new List<string>();
                columns.Add(DisplayedDate.ExpirationDate.ToString());
                columns.Add(DisplayedDate.EffectiveDate.ToString());
                return columns.ToArray();
            }
        }
	}
}
