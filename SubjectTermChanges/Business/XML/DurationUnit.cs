using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	public enum DurationUnitType
	{
        [Utility.EnumDescription("Day")]
        Day,
        [Utility.EnumDescription("Week")]
        Week,
        [Utility.EnumDescription("Month")]
        Month,
        [Utility.EnumDescription("(90 Days) Quarter")]
        _90Days,
        [Utility.EnumDescription("(3 Months) Quarter")]
        _3Months,
        [Utility.EnumDescription("Year")]
        Year
	};

	[Serializable]
	public class DurationUnit
	{

		#region private members
		private string _value;
		private string _display;
		private bool? _default;
		private bool? _selected;
		#endregion


		#region Properties

		public string Value
		{
			get { return Utility.XMLHelper.GetXMLText(_value); }
			set { _value = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Display
		{
			get { return Utility.XMLHelper.GetXMLText(_display); }
			set { _display = Utility.XMLHelper.SetXMLText(value); }
		}

		public bool? Default
		{
			get { return _default; }
			set { _default = value; }
		}

		public bool? Selected
		{
			get { return _selected; }
			set { _selected = value; }
		}

		#endregion


		#region Constructors

		public DurationUnit()
		{
		}

		public DurationUnit(XmlNode termNode)
		{
			_value = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Value);
			_display = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Display);
			_default = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_Default);
			_selected = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_Selected);
		}

		public DurationUnit(string value, string display)
		{
			_value = value;
			_display = display;
			_default = false;
			_selected = false;
		}

		#endregion


		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
				Utility.XMLHelper.ValidateString(XMLNames._A_Value, _value);
				Utility.XMLHelper.ValidateString(XMLNames._A_Display, _display);
			}

			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Value, _value);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Display, _display);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_Default, _default);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_Selected, _selected);
		}

		#endregion


		#region static methods

		public static List<DurationUnit> All()
		{
			List<DurationUnit> rtn = new List<DurationUnit>(6);
            //rtn.Add(new DurationUnit(DurationUnitType.Day.ToString(), "Day"));
            //rtn.Add(new DurationUnit(DurationUnitType.Week.ToString(), "Week"));
            //rtn.Add(new DurationUnit(DurationUnitType.Month.ToString(), "Month"));
            //rtn.Add(new DurationUnit(DurationUnitType._90Days.ToString(), "(90 Days) Quarter"));
            //rtn.Add(new DurationUnit(DurationUnitType._3Months.ToString(), "(3 Months) Quarter"));
            //rtn.Add(new DurationUnit(DurationUnitType.Year.ToString(), "Year"));

            rtn.Add(new DurationUnit(DurationUnitType.Day.ToString(), Utility.EnumHelper.GetDescription(DurationUnitType.Day)));
            rtn.Add(new DurationUnit(DurationUnitType.Week.ToString(), Utility.EnumHelper.GetDescription(DurationUnitType.Week)));
            rtn.Add(new DurationUnit(DurationUnitType.Month.ToString(), Utility.EnumHelper.GetDescription(DurationUnitType.Month)));
            rtn.Add(new DurationUnit(DurationUnitType._90Days.ToString(), Utility.EnumHelper.GetDescription(DurationUnitType._90Days)));
            rtn.Add(new DurationUnit(DurationUnitType._3Months.ToString(), Utility.EnumHelper.GetDescription(DurationUnitType._3Months)));
            rtn.Add(new DurationUnit(DurationUnitType.Year.ToString(), Utility.EnumHelper.GetDescription(DurationUnitType.Year)));

			return rtn;
		}

		#endregion

	}
}
