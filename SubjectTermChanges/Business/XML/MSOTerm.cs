using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class MSOTerm : Term
	{
		#region private members

		private static int _fieldCount = 7;  //number of fields in each of the 3 following groups; used to initialize arrays and lists elsewhere in the app

		private string _MSOName;
		private string _address1Name;
		private string _address2Name;
		private string _cityName;
		private string _stateName;
		private string _zipName;
		private string _phoneName;

		private bool? _MSONameSpecified;
		private bool? _address1NameSpecified;
		private bool? _address2NameSpecified;
		private bool? _cityNameSpecified;
		private bool? _stateNameSpecified;
		private bool? _zipNameSpecified;
		private bool? _phoneNameSpecified;

		private string _MSOValue;
		private string _address1Value;
		private string _address2Value;
		private string _cityValue;
		private string _stateValue;
		private string _zipValue;
		private string _phoneValue;

        public const string _MSO_FieldName = "MSO";
        public const string _MSO_Address1FieldName = "Address1";
        public const string _MSO_Address2FieldName = "Address2";
        public const string _MSO_CityFieldName = "City";
        public const string _MSO_StateFieldName = "State";
        public const string _MSO_ZipFieldName = "Zip";
        public const string _MSO_PhoneFieldName = "Phone";


		#endregion

		#region Properties

		public static int FieldCount
		{
			get { return _fieldCount; }
		}

		public string MSOName
		{
			get { return ((_MSONameSpecified ?? false) ? Utility.XMLHelper.GetXMLText(_MSOName) : "Name"); }
			set { _MSOName = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Address1Name
		{
			get { return ((_address1NameSpecified ?? false) ? Utility.XMLHelper.GetXMLText(_address1Name) : "Address 1"); }
			set { _address1Name = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Address2Name
		{
			get { return ((_address2NameSpecified ?? false) ? Utility.XMLHelper.GetXMLText(_address2Name) : "Address 2"); }
			set { _address2Name = Utility.XMLHelper.SetXMLText(value); }
		}

		public string CityName
		{
			get { return ((_cityNameSpecified ?? false) ? Utility.XMLHelper.GetXMLText(_cityName) : "City"); }
			set { _cityName = Utility.XMLHelper.SetXMLText(value); }
		}

		public string StateName
		{
			get { return ((_stateNameSpecified ?? false) ? Utility.XMLHelper.GetXMLText(_stateName) : "State"); }
			set { _stateName = Utility.XMLHelper.SetXMLText(value); }
		}

		public string ZipName
		{
			get { return ((_zipNameSpecified ?? false) ? Utility.XMLHelper.GetXMLText(_zipName) : "Zip"); }
			set { _zipName = Utility.XMLHelper.SetXMLText(value); }
		}

		public string PhoneName
		{
			get { return ((_phoneNameSpecified ?? false) ? Utility.XMLHelper.GetXMLText(_phoneName) : "Phone"); }
			set { _phoneName = Utility.XMLHelper.SetXMLText(value); }
		}

		public bool? MSONameSpecified
		{
			get { return _MSONameSpecified; }
			set { _MSONameSpecified = value; }
		}

		public bool? Address1NameSpecified
		{
			get { return _address1NameSpecified; }
			set { _address1NameSpecified = value; }
		}

		public bool? Address2NameSpecified
		{
			get { return _address2NameSpecified; }
			set { _address2NameSpecified = value; }
		}

		public bool? CityNameSpecified
		{
			get { return _cityNameSpecified; }
			set { _cityNameSpecified = value; }
		}

		public bool? StateNameSpecified
		{
			get { return _stateNameSpecified; }
			set { _stateNameSpecified = value; }
		}

		public bool? ZipNameSpecified
		{
			get { return _zipNameSpecified; }
			set { _zipNameSpecified = value; }
		}

		public bool? PhoneNameSpecified
		{
			get { return _phoneNameSpecified; }
			set { _phoneNameSpecified = value; }
		}

		public string MSOValue
		{
			get { return Utility.XMLHelper.GetXMLText(_MSOValue); }
			set { _MSOValue = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Address1Value
		{
			get { return Utility.XMLHelper.GetXMLText(_address1Value); }
			set { _address1Value = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Address2Value
		{
			get { return Utility.XMLHelper.GetXMLText(_address2Value); }
			set { _address2Value = Utility.XMLHelper.SetXMLText(value); }
		}

		public string CityValue
		{
			get { return Utility.XMLHelper.GetXMLText(_cityValue); }
			set { _cityValue = Utility.XMLHelper.SetXMLText(value); }
		}

		public string StateValue
		{
			get { return Utility.XMLHelper.GetXMLText(_stateValue); }
			set { _stateValue = Utility.XMLHelper.SetXMLText(value); }
		}

		public string ZipValue
		{
			get { return Utility.XMLHelper.GetXMLText(_zipValue); }
			set { _zipValue = Utility.XMLHelper.SetXMLText(value); }
		}

		public string PhoneValue
		{
			get { return Utility.XMLHelper.GetXMLText(_phoneValue); }
			set { _phoneValue = Utility.XMLHelper.SetXMLText(value); }
		}



		public override string Keyword
		{
			get
			{
				if (!HasKeyWord)
					return null;
				return MSOValue;
			}
		}

        public override string[] StoreColumns
        {
            get
            {
                List<string> columns = new List<string>();
                columns.Add(_MSO_FieldName);
                columns.Add(_MSO_Address1FieldName);
                columns.Add(_MSO_Address1FieldName);
                columns.Add(_MSO_CityFieldName);
                columns.Add(_MSO_StateFieldName);
                columns.Add(_MSO_ZipFieldName);
                columns.Add(_MSO_PhoneFieldName);
                return columns.ToArray();
            }
        }

		#endregion

		#region Constructors

        public MSOTerm(bool systemTerm, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
			TermType = TermType.MSO;
			NameRequired = true;

			_MSOName		= "Name";
			_address1Name	= "Address1";
			_address2Name	= "Address2";
			_cityName		= "City";
			_stateName		= "State";
			_zipName		= "Zip";
			_phoneName		= "Phone";

			_MSONameSpecified = true;
			_address1NameSpecified = true;
			_address2NameSpecified = true;
			_cityNameSpecified = true;
			_stateNameSpecified = true;
			_zipNameSpecified = true;
			_phoneNameSpecified = true;

		}

        public MSOTerm(XmlNode termNode, Template template, bool isFilter)
            : base(termNode, template, isFilter)
		{
			TermType = TermType.MSO;
			NameRequired = true;

			_MSOName = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_MSOName);
			_address1Name = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Address1Name);
			_address2Name = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Address2Name);
			_cityName = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_CityName);
			_stateName = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_StateName);
			_zipName = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_ZipName);
			_phoneName = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_PhoneName);

			_MSONameSpecified = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_MSONameDisplayed);
			_address1NameSpecified = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_Address1NameDisplayed);
			_address2NameSpecified = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_Address2NameDisplayed);
			_cityNameSpecified = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_CityNameDisplayed);
			_stateNameSpecified = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_StateNameDisplayed);
			_zipNameSpecified = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_ZipNameDisplayed);
			_phoneNameSpecified = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_PhoneNameDisplayed);

			_MSOValue = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_MSOValue);
			_address1Value = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Address1Value);
			_address2Value = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Address2Value);
			_cityValue = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_CityValue);
			_stateValue = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_StateValue);
			_zipValue = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_ZipValue);
			_phoneValue = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_PhoneValue);
		}

		#endregion

		#region Build XML

		public override void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
			}

			base.Build(xmlDoc, termNode, bValidate);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_MSOName, _MSOName);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Address1Name, _address1Name);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Address2Name, _address2Name);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_CityName, _cityName);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_StateName, _stateName);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_ZipName, _zipName);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_PhoneName, _phoneName);

			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_MSONameDisplayed, _MSONameSpecified);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_Address1NameDisplayed, _address1NameSpecified);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_Address2NameDisplayed, _address2NameSpecified);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_CityNameDisplayed, _cityNameSpecified);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_StateNameDisplayed, _stateNameSpecified);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_ZipNameDisplayed, _zipNameSpecified);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_PhoneNameDisplayed, _phoneNameSpecified);

			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_MSOValue, _MSOValue);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Address1Value, _address1Value);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Address2Value, _address2Value);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_CityValue, _cityValue);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_StateValue, _stateValue);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_ZipValue, _zipValue);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_PhoneValue, _phoneValue);
		}

		#endregion



		public override string DisplayValue(string termPartSpecifier)
		{
			switch (termPartSpecifier)
			{
				case "Address1":
					return Address1Value;
				case "Address2":
					return Address2Value;
				case "City":
					return CityValue;
				case "State":
					return StateValue;
				case "Zip":
					return ZipValue;
				case "Phone":
					return PhoneValue;
				case "Name":
					return MSOValue;
				default:
					if (termPartSpecifier == this.Address1Name)
						return Address1Value;
					else if (termPartSpecifier == this.Address2Name)
						return Address2Value;
					else if (termPartSpecifier == this.CityName)
						return CityValue;
					else if (termPartSpecifier == this.StateName)
						return StateValue;
					else if (termPartSpecifier == this.ZipName)
						return ZipValue;
					else if (termPartSpecifier == this.PhoneName)
						return PhoneValue;
					else
						return MSOValue;
			}
		}

        public override List<string> Validate(bool includeTab, string filterTermTabName)
		{
			List<string> rtn = new List<string>();

			if (Runtime.Required)
				if (string.IsNullOrEmpty(_MSOValue.Trim()))
                    rtn.Add(string.Format("\"{0}\"{1} is a required term, but no value was entered.", this.Name, includeTab ? string.Format(" (tab {0})", TermGroupName) : string.Empty));

			return rtn;
		}

        public override List<string> CheckType(bool includeTab, string filterTermTabName)
        {
            return null;
        }

		#region PDF Writer Method(s)

		public override bool EmitPDFXML(XmlWriter writer)
		{
			PdfHelper.AddPDFXMLHeader(writer);
			PdfHelper.AddPDFXMLRow(writer, Name, MSOName + ":", MSOValue);
			PdfHelper.AddPDFXMLRow(writer, string.Empty, Address1Name + ":", Address1Value);
			PdfHelper.AddPDFXMLRow(writer, string.Empty, Address2Name + ":", Address2Value);
			PdfHelper.AddPDFXMLRow(writer, string.Empty, CityName + ":", CityValue);
			PdfHelper.AddPDFXMLRow(writer, string.Empty, StateName + ":", StateValue);
			PdfHelper.AddPDFXMLRow(writer, string.Empty, ZipName + ":", ZipValue);
			PdfHelper.AddPDFXMLRow(writer, string.Empty, PhoneName + ":", PhoneValue);
			PdfHelper.AddPDFXMLFooter(writer);
			return true;
		}


		#endregion


		public override void SetDefaultValue()
		{
			_MSOValue = string.Empty;
			_address1Value = string.Empty;
			_address2Value = string.Empty;
			_cityValue = string.Empty;
			_stateValue = string.Empty;
			_zipValue = string.Empty;
			_phoneValue = string.Empty;
		}


        public override void Clear()
		{
			_MSOValue = string.Empty;
			_address1Value = string.Empty;
			_address2Value = string.Empty;
			_cityValue = string.Empty;
			_stateValue = string.Empty;
			_zipValue = string.Empty;
			_phoneValue = string.Empty;
		}

        public override void Migrate(Term term)
        {
            if (term == null)
                base.Migrate(term);
            if (!(term is MSOTerm))
                return;
            else
            {
                MSOTerm msoTerm = term as MSOTerm;
                MSOValue = msoTerm.MSOValue;
                Address1Value = msoTerm.Address1Value;
                Address2Value = msoTerm.Address2Value;
                CityValue = msoTerm.CityValue;
                StateValue = msoTerm.StateValue;
                ZipValue = msoTerm.ZipValue;
                PhoneValue = msoTerm.PhoneValue;
                term.Runtime.Migrated = true;
            }
            Runtime.Migrated = true;
        }

        public static TermStore CreateStore(string termName, XmlReader reader)
        {
            using (reader)
            {
                reader.Read();
                TermStore termStore = new TermStore(reader.GetAttribute(XMLNames._A_Name), TermType.MSO);
                Dictionary<string, string> item = new Dictionary<string, string>();

                item.Add(_MSO_FieldName, reader.GetAttribute(XMLNames._A_MSOValue));
                item.Add(_MSO_Address1FieldName, reader.GetAttribute(XMLNames._A_Address1Value));
                item.Add(_MSO_Address2FieldName, reader.GetAttribute(XMLNames._A_Address2Value));
                item.Add(_MSO_CityFieldName, reader.GetAttribute(XMLNames._A_CityValue));
                item.Add(_MSO_StateFieldName, reader.GetAttribute(XMLNames._A_StateValue));
                item.Add(_MSO_ZipFieldName, reader.GetAttribute(XMLNames._A_ZipValue));
                item.Add(_MSO_PhoneFieldName, reader.GetAttribute(XMLNames._A_PhoneValue));
                termStore.AddFieldValue(item);

                return termStore;
            }
        }

	}
}
