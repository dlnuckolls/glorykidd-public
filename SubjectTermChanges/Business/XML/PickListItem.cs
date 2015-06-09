using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class PickListItem
	{
		#region private members
		private string _value;
		private bool? _selected;
		#endregion

		#region Properties

		public string Value
		{
			get { return Utility.XMLHelper.GetXMLText(_value); }
			set { _value = Utility.XMLHelper.SetXMLText(value); }
		}

		public bool? Selected
		{
			get { return _selected; }
			set { _selected = value; }
		}

		#endregion

		#region Constructors

		public PickListItem()
		{
		}

		public PickListItem(XmlNode termNode)
		{
			_selected = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_Selected);
			_value = Utility.XMLHelper.GetText(termNode);
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_Selected, _selected);
			Utility.XMLHelper.SetText(xmlDoc, termNode,_value);
		}

		#endregion

        public PickListItem Copy()
        {
            PickListItem pickListItem = new PickListItem();

            pickListItem._value = _value;
            pickListItem._selected = _selected;

            return pickListItem;
        }

	}
}
