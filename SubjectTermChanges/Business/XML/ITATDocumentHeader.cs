using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class ITATDocumentHeader
	{
		#region private members
		private bool? _excludeFromFirstPage;
		private string _leftText;
		private string _centerText;
		private string _rightText;

		#endregion

		#region Properties

		public bool? ExcludeFromFirstPage
		{
			get { return _excludeFromFirstPage; }
			set { _excludeFromFirstPage = value; }
		}

		public string LeftText
		{
			get { return Utility.XMLHelper.GetXMLText(_leftText); }
			set { _leftText = Utility.XMLHelper.SetXMLText(value); }
		}

		public string CenterText
		{
			get { return Utility.XMLHelper.GetXMLText(_centerText); }
			set { _centerText = Utility.XMLHelper.SetXMLText(value); }
		}

		public string RightText
		{
			get { return Utility.XMLHelper.GetXMLText(_rightText); }
			set { _rightText = Utility.XMLHelper.SetXMLText(value); }
		}

		#endregion

		#region Constructors

		public ITATDocumentHeader()
		{
		}

		public ITATDocumentHeader(XmlNode termNode)
		{
			_excludeFromFirstPage = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_ExcludeFromFirstPage);
			_leftText = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_LeftText);
			_centerText = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_CenterText);
			_rightText = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_RightText);
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
			}

			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_ExcludeFromFirstPage, _excludeFromFirstPage);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_LeftText, _leftText);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_CenterText, _centerText);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_RightText, _rightText);
		}

		#endregion

		#region Public Methods

		public string PreviewXml()
		{
			string rtn = string.Empty;
			return rtn;
		}

		#endregion
	}
}
