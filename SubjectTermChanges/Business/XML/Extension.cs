using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class Extension
	{
		#region private members
		private string _fileName;
		private string _objectID;

		#endregion

		#region Properties

		public string FileName
		{
			get { return Utility.XMLHelper.GetXMLText(_fileName); }
			set { _fileName = Utility.XMLHelper.SetXMLText(value); }
		}

		public string ObjectID
		{
			get { return Utility.XMLHelper.GetXMLText(_objectID); }
			set { _objectID = Utility.XMLHelper.SetXMLText(value); }
		}


		#endregion

		#region Constructors

		public Extension()
		{
		}

		public Extension(XmlNode node)
		{
			_fileName = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_FileName);
			_objectID = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_ObjectID);
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			if (bValidate)
			{
			}

			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_FileName, _fileName);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_ObjectID, _objectID);
		}

		#endregion

	}
}
