using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class DateTerm : Term
	{
		#region private members
		private DateTime? _value;
		private string _format;     //For date format, such as "mm/dd/yyyy"
		#endregion

		#region Properties

		public DateTime? Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public string Format
		{
			get { return Utility.XMLHelper.GetXMLText(_format); }
			set { _format = Utility.XMLHelper.SetXMLText(value); }
		}

		public override string Keyword
		{
			get 
			{
				if (!HasKeyWord)
					return null;

				if (_value.HasValue)
				{
					return Utility.DateHelper.FormatDate(_value.Value, Format);
				}
				return null;
			}
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

        public DateTerm(bool systemTerm, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
			TermType = TermType.Date;
			NameRequired = true;
		}

        public DateTerm(XmlNode termNode, Template template, bool isFilter)
            : base(termNode, template, isFilter)
		{
			TermType = TermType.Date;
			NameRequired = true;

			_value = Utility.XMLHelper.GetValueDate(termNode);
			_format = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Format);
		}

		#endregion

		#region Build XML

		public override void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
				Utility.XMLHelper.ValidateString(XMLNames._A_Format, _format);
			}

			base.Build(xmlDoc, termNode, bValidate);

			Utility.XMLHelper.SetValueDate(xmlDoc, termNode, _value);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Format, _format);
		}

		#endregion


		public override string DisplayValue(string termPartSpecifier)
		{
			if (Value.HasValue)
				return Utility.DateHelper.FormatDate(Value.Value, Format);
			else
				return string.Empty;
		}

        public override string TestValue(string termName, string tabMessage, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                DateTime dt;
                if (!DateTime.TryParse(value, out dt))
                {
                    if (string.IsNullOrEmpty(tabMessage))
                        return string.Format("For Date Term '{0}', '{1}' is not a properly formatted date", termName, value);
                    else
                        return string.Format("For Date Term '{0}' (tab {1}), '{2}' is not a properly formatted date", termName, tabMessage, value);
                }
            }
            return string.Empty;
        }

        public override void SetValue(string value)
        {
            DateTime dt;
            if (DateTime.TryParse(value, out dt))
                _value = dt;
            else
                _value = null;
        }

        public override string GetValue(string defaultValue)
        {
            if (_value.HasValue)
                return Utility.DateHelper.SetXMLDate(_value.Value);
            else
                return defaultValue;
        }

        public override List<string> Validate(bool includeTab, string filterTermTabName)
		{
			List<string> rtn = new List<string>();

			if (Runtime.Required)
                if (!Value.HasValue)
                {
                    string tabString = IsFilter ? filterTermTabName : TermGroupName;
                    rtn.Add(string.Format("\"{0}\"{1} is a required term, but no value was entered.", this.Name, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
                }

			return rtn;
		}

        public override List<string> CheckType(bool includeTab, string filterTermTabName)
        {
            return null;
        }

		public override bool EmitPDFXML(XmlWriter writer)
		{
			PdfHelper.AddPDFXMLHeader(writer);
			PdfHelper.AddPDFXMLRow(writer, Name, DisplayValue(XMLNames._TPS_None));
			PdfHelper.AddPDFXMLFooter(writer);
			return true;
		}


		public override void SetDefaultValue()
		{
			_value = null;
		}

        public override void Clear()
		{
			_value = null;
		}

        public override void Migrate(Term term)
        {
            if (term == null)
                base.Migrate(term);
            if (!(term is DateTerm))
                return;
            else
            {
                Value = (term as DateTerm).Value;
                term.Runtime.Migrated = true;
            }
            Runtime.Migrated = true;
        }

        public override void Load(string value, string pattern, char? delimiter)
        {
            if (!string.IsNullOrEmpty(value))
                 Value = DateTime.Parse(value);
        }

        public override Term Copy()
        {
            DateTerm dateTerm = new DateTerm(SystemTerm, _template, IsFilter);
            CopyBase(dateTerm, _template);

            dateTerm._value = _value;
            dateTerm._format = _format;

            return dateTerm;
        }

	}
}
