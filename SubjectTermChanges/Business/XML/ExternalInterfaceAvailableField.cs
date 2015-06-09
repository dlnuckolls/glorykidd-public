using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{

	[Serializable]
	public class ExternalInterfaceAvailableField 
	{
		private string _name;
		private string _displayName;
		private int _displayOrder;  // less than 0 == Not Displayed
		private string _displayWidth;
		private bool _isKeyField;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public int DisplayOrder
		{
			get { return _displayOrder; }
			set { _displayOrder = value; }
		}

		public string DisplayWidth
		{
			get { return _displayWidth; }
			set { _displayWidth = value; }
		}

		public string DisplayName
		{
			get { return _displayName; }
			set { _displayName = value; }
		}

		public bool IsKeyField
		{
			get { return _isKeyField; }
			set { _isKeyField = value; }
		}

        public bool IsDisplayed
        {
            get { return _displayOrder >= 0; }
        }

		public class DisplayOrderComparer : IComparer<ExternalInterfaceAvailableField>
		{
			#region IComparer<ExternalInterfaceAvailableField> Members

			public int Compare(ExternalInterfaceAvailableField x, ExternalInterfaceAvailableField y)
			{
				return x.DisplayOrder.CompareTo(y.DisplayOrder);
			}

			#endregion
		}


		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Name, _name);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_DisplayName, _displayName);
			Utility.XMLHelper.AddAttributeInt(xmlDoc, node, XMLNames._A_DisplayOrder, _displayOrder);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_DisplayWidth, _displayWidth);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_IsKey, _isKeyField);
		}


	}
}
