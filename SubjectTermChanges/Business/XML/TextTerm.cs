using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{

	public enum TextTermFormat
	{
		Plain = 0,
		Number,
		Currency,
		SSN,
		Phone,
		PhonePlusExtension
	}


	[Serializable]
	public class TextTerm : Term
	{
		#region private members
		private string _value;
		private TextTermFormat _format;
		private int? _min;
		private int? _max;
		private bool? _showCents;
		private bool? _useTextNumberFormat;
		#endregion

		#region Properties

		public string Value
		{
			get { return Utility.XMLHelper.GetXMLText(_value); }
			set { _value = Utility.XMLHelper.SetXMLText(value); }
		}

		public TextTermFormat Format
		{
			get { return _format; }
			set { _format = value; }
		}

		public int? Min
		{
			get { return _min; }
			set { _min = value; }
		}

		public int? Max
		{
			get { return _max; }
			set { _max = value; }
		}

		public bool? ShowCents
		{
			get { return _showCents; }
			set { _showCents = value; }
		}

		public bool? UseTextNumberFormat
		{
			get { return _useTextNumberFormat; }
			set { _useTextNumberFormat = value; }
		}

		public override string Keyword
		{
			get { return HasKeyWord ? Value : null; }
		}

		public string UnformattedValue
		{
			get { return _value; }
		}

        public override int? RequiredSize
        {
            get
            {
                switch (_format)
                {
                    case TextTermFormat.Currency:
                    case TextTermFormat.Number:
                    case TextTermFormat.Phone:
                    case TextTermFormat.PhonePlusExtension:
                    case TextTermFormat.SSN:
                        break;
                    case TextTermFormat.Plain:
                        if (_max.HasValue)
                            return _max.Value;
                        break;
                }
                return null;
            }
        }

 
		#endregion

		#region Constructors

        public TextTerm(bool systemTerm, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
			TermType = TermType.Text;
			NameRequired = true;
		}

        public TextTerm(XmlNode termNode, Template template, bool isFilter)
            : base(termNode, template, isFilter)
		{
			TermType = TermType.Text;
			NameRequired = true;

			_value = Utility.XMLHelper.GetText(termNode);
			_format = (TextTermFormat)Enum.Parse(typeof(TextTermFormat), Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Format));
			_min = Utility.XMLHelper.GetAttributeInt(termNode, XMLNames._A_Min);
			_max = Utility.XMLHelper.GetAttributeInt(termNode, XMLNames._A_Max);
			_showCents = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_ShowCents);
			_useTextNumberFormat = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_UseTextNumberFormat);
		}

		#endregion


		#region Public Methods


		//20070810_DEG	For Bug #132, changed the way 'DisplayValue' works.  Operate on 'Value' vice '_value'
         public override string DisplayValue(string termPartSpecifier)
		{
			if (string.IsNullOrEmpty(Value))
				return string.Empty;
			switch (_format)
			{
				case TextTermFormat.Number:
					int n;
					if ((_useTextNumberFormat ?? false) && (int.TryParse(Value, out n)))
						return Utility.TextHelper.TextPlusNumber(n);
					else
						return Utility.TextHelper.FormatAsNumber(Value);
				
				case TextTermFormat.Currency:
					return Utility.TextHelper.FormatAsCurrency(Value, _showCents ?? false, false);
				
				case TextTermFormat.SSN:
					return Utility.TextHelper.FormatAsSSN(Value);
				
				case TextTermFormat.Phone:
					return Utility.TextHelper.FormatAsPhone(Value);
				
				case TextTermFormat.PhonePlusExtension:
					return Utility.TextHelper.FormatAsPhonePlusExtension(Value);

                case TextTermFormat.Plain:
				default:
					return Value;
			}
		}

         public override string TestValue(string termName, string tabMessage, string value)
        {
            return string.Empty;
        }

        public override void SetValue(string value)
        {
            _value = value;
        }

        public override string GetValue(string defaultValue)
        {
            if (!string.IsNullOrEmpty(_value))
                return _value;
            else
                return defaultValue;
        }

		#endregion



		#region private Methods

		//20070810_DEG	Related to Bug #132, changed the way 'Validate...' works.  Operate on 'Value' vice '_value'
        private List<string> ValidateCurrency(bool bTypeCheck, bool includeTab, string filterTermTabName)
		{
			List<string> rtn = new List<string>();
            string tabString = IsFilter ? filterTermTabName : TermGroupName;
            if (bTypeCheck && string.IsNullOrEmpty(this.Value))
                return rtn;
            decimal d;
			if (!string.IsNullOrEmpty(Value) || this.Runtime.Required) // 112607 AM R-129 Error displayed when field is blank and not required
                if (!Utility.TextHelper.ValidateAsCurrency(Value, out d))
                    rtn.Add(string.Format("\"{0}\"{2} is not a valid value for the Currency Text Term \"{1}\".", this.Value, this.Name, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
				else
                    if (!bTypeCheck)
                    {
                        if (this.Max.HasValue && d > this.Max.Value)
                            rtn.Add(string.Format("{0}{3} is larger than the maximum allowed value ({2}) for the Currency Text Term \"{1}\".", this.Value, this.Name, this.Max, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
                        else
                            if (this.Min.HasValue && d < this.Min.Value)
                                rtn.Add(string.Format("{0}{3} is smaller than the minimum allowed value ({2}) for the Currency Text Term \"{1}\".", this.Value, this.Name, this.Min, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
                    }
			return rtn;
		}


        private List<string> ValidateNumber(bool bTypeCheck, bool includeTab, string filterTermTabName)
		{
			List<string> rtn = new List<string>();
            string tabString = IsFilter ? filterTermTabName : TermGroupName;
            if (bTypeCheck && string.IsNullOrEmpty(this.Value))
                return rtn;
			if (!string.IsNullOrEmpty(this.Value) || this.Runtime.Required == true) // 112607 AM R-129 Error displayed when field is blank and not required
			{
				if ((_format == TextTermFormat.Number) &&  (_useTextNumberFormat ?? false))
				{
					//Look for a number surrounded by parentheses.  If found, use that number for validation.  Otherwise, use the entire "rawInput" value.
					Match match = Regex.Match(this.Value, @"\x28(.+?)\x29");
					if (match != null)
						if (match.Groups.Count >= 2)
							this.Value = match.Groups[1].Value;
				}

				decimal d;
                if (Utility.TextHelper.ValidateAsNumber(this.Value, out d))
				{
                    if (!bTypeCheck)
                    {
                        if (this.Max.HasValue && d > this.Max.Value)
                            rtn.Add(string.Format("{0}{3} is larger than the maximum allowed value ({2}) for the Number Text Term \"{1}\".", this.Value, this.Name, this.Max, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
                        else
                            if (this.Min.HasValue && d < this.Min.Value)
                                rtn.Add(string.Format("{0}{3} is smaller than the minimum allowed value ({2}) for the Number Text Term \"{1}\".", this.Value, this.Name, this.Min, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
                    }
				}
				else
				{
                    rtn.Add(string.Format("\"{0}\"{2} is not a valid value for the Number Text Term \"{1}\".", this.Value, this.Name, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
				}
			}
			return rtn;
		}


        private List<string> ValidateSSN(bool includeTab, string filterTermTabName)
		{
            string tabString = IsFilter ? filterTermTabName : TermGroupName;
            List<string> rtn = new List<string>();
            if (!Utility.TextHelper.ValidateAsSSN(Value))
                rtn.Add(string.Format("The SSN Text Term \"{0}\"{1} must have 9 digits.", this.Name, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
			return rtn;
		}


        private List<string> ValidatePlain(bool includeTab, string filterTermTabName)
		{
			List<string> rtn = new List<string>();
            string tabString = IsFilter ? filterTermTabName : TermGroupName;

			//TODO - Should 'length' checks be made on all formats?
			int length = 0;
            if (!string.IsNullOrEmpty(Value) || this.Required == true) // 112607 AM R-129 Error displayed when field is blank and not required
            {
                AddMissingNonSpaceCharacterError(Value, ref rtn);

                if (!string.IsNullOrEmpty(Value))
                    length = Value.Length;

                if (_max.HasValue)
                    if (length > _max)
                        rtn.Add(string.Format("The length of the Text Term \"{0}\" ({1}){3} is longer than the maximum allowed length ({2}).", Name, length, _max, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));

                if (this.Min.HasValue) // 112607 AM R-129 Error displayed when field is blank and not required
                    if (length < _min)
                        rtn.Add(string.Format("The length of the Text Term \"{0}\" ({1}){3} is shorter than the minimum allowed length ({2}).", Name, length, _min, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
            }
			return rtn;
		}



		#endregion


		#region Build XML

		public override void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
				Utility.XMLHelper.ValidateString(XMLNames._A_Format, _format.ToString());
			}

			base.Build(xmlDoc, termNode, bValidate);

			Utility.XMLHelper.SetText(xmlDoc, termNode, _value);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Format, _format.ToString());
			Utility.XMLHelper.AddAttributeLong(xmlDoc, termNode, XMLNames._A_Min, _min);
			Utility.XMLHelper.AddAttributeLong(xmlDoc, termNode, XMLNames._A_Max, _max);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_ShowCents, _showCents);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_UseTextNumberFormat, _useTextNumberFormat);
		}

		#endregion

        private bool AddMissingNonSpaceCharacterError(string sValue, ref List<string> sValidationErrors)
        {
            if (!string.IsNullOrEmpty(sValue))
                if (sValue.TrimStart().TrimEnd().Length == 0)
                {
                    sValidationErrors.Add(string.Format("Text Term \"{0}\" does not include any non-space characters as required.", Name));
                    return true;
                }
            return false;
        }

        public override List<string> CheckType(bool includeTab, string filterTermTabName)
        {
            List<string> rtn = new List<string>();
            if (!string.IsNullOrEmpty(Value))
            {
                switch (_format)
                {
                    case TextTermFormat.Number:
                        rtn.AddRange(ValidateNumber(true, includeTab, filterTermTabName));
                        break;

                    case TextTermFormat.Currency:
                        rtn.AddRange(ValidateCurrency(true, includeTab, filterTermTabName));
                        break;

                    case TextTermFormat.SSN:
                        rtn.AddRange(ValidateSSN(includeTab, filterTermTabName));
                        break;

                    case TextTermFormat.Phone:
                    case TextTermFormat.PhonePlusExtension:
                        AddMissingNonSpaceCharacterError(Value, ref rtn);
                        break;

                    case TextTermFormat.Plain:
                    default:
                        break;
                }
            }
            return rtn;
        }

        public override List<string> Validate(bool includeTab, string filterTermTabName)
		{
			List<string> rtn = new List<string>();

            if (string.IsNullOrEmpty(Value))
            {
                if (Runtime.Required)
                {
                    string tabString = IsFilter ? filterTermTabName : TermGroupName;
                    rtn.Add(string.Format("\"{0}\"{1} is a required term, but no value was entered.", this.Name, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
                }
            }
            else
            {
                switch (_format)
                {
                    case TextTermFormat.Plain:
                        rtn.AddRange(ValidatePlain(includeTab, filterTermTabName));
                        break;

                    case TextTermFormat.Number:
                        rtn.AddRange(ValidateNumber(false, includeTab, filterTermTabName));
                        break;

                    case TextTermFormat.Currency:
                        rtn.AddRange(ValidateCurrency(false, includeTab, filterTermTabName));
                        break;

                    case TextTermFormat.SSN:
                        rtn.AddRange(ValidateSSN(includeTab, filterTermTabName));
                        break;

                    case TextTermFormat.Phone:
                    case TextTermFormat.PhonePlusExtension:
                        break;

                    default:
                        break;
                }
            }
			return rtn;
		}


		public override bool EmitPDFXML(XmlWriter writer)
		{
			PdfHelper.AddPDFXMLHeader(writer);
			PdfHelper.AddPDFXMLRowText(writer, Name, DisplayValue(XMLNames._TPS_None),PreserveWhiteSpaceSummary);
			PdfHelper.AddPDFXMLFooter(writer);
			return true;
		}


		//20070810_DEG	Related to Bug #132, changed the way 'SetDefaultValue...' works.  Operate on 'Value' vice '_value'
		public override void SetDefaultValue()
		{
			if (this.Default ?? false)
				Value = DefaultValue;
			else
				Value = string.Empty;
		}


        public override void Clear()
		{
			_value = string.Empty;
		}

        public override void Migrate(Term term)
        {
            if (term == null)
                base.Migrate(term);
            if (!(term is TextTerm))
                return;
            else
            {
                Value = (term as TextTerm).Value;
                term.Runtime.Migrated = true;
            }
            Runtime.Migrated = true;
        }

        public override void Load(string value, string pattern, char? delimiter)
        {
            if (!string.IsNullOrEmpty(value))
                Value = value;
        }

        public static TextTermStore CreateStore(string termName, XmlReader reader)
        {
            using (reader)
            {
                reader.Read();

                TextTermFormat textTermFormat = (TextTermFormat)Enum.Parse(typeof(TextTermFormat), reader.GetAttribute(XMLNames._A_Format)); ;
                string maxLength = reader.GetAttribute(XMLNames._A_Max);
                int length;
                if (!int.TryParse(maxLength, out length))
                    length = -1;

                TextTermStore textTermStore = new TextTermStore(termName, textTermFormat, length);

                //Look for the Text node
                reader.Read();
                ReadPastSubTree(reader, XMLNames._E_validateOn);

                string value = string.Empty;
                if (reader.NodeType == XmlNodeType.Text)
                {
                    value = reader.Value;
                }

                textTermStore.AddMultiValue(value);
                return textTermStore;
            }
        }

        public override string IsValid(TermStore termStore)
        {
            string error = base.IsValid(termStore);

            if (!string.IsNullOrEmpty(error))
                return error;

            if (Max.HasValue)
                (termStore as TextTermStore).Length = Max.Value;

            return string.Empty;
        }

        public override Term Copy()
        {
            TextTerm textTerm = new TextTerm(SystemTerm, _template, IsFilter);
            CopyBase(textTerm, _template);

            textTerm._value = _value;
            textTerm._format = _format;
            textTerm._min = _min;
            textTerm._max = _max;
            textTerm._showCents = _showCents;
            textTerm._useTextNumberFormat = _useTextNumberFormat;

            return textTerm;
        }

	}
}
