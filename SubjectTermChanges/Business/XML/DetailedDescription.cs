using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class DetailedDescription
	{
        const int _TEXT_LIMIT = 1500;

        public enum DetailedDescriptionType
        {
            None,
            WhenToUse,
            WhenNotToUse,
            AdditionalInfo
        }

		#region private members

		private readonly string _text;
        private DetailedDescriptionType _detailedDescriptionType;

		#endregion

		#region Properties

		public string Text
		{
			get { return Utility.XMLHelper.GetXMLText(_text); }
		}

        public DetailedDescriptionType DescriptionType
        {
            get { return _detailedDescriptionType; }
            set { _detailedDescriptionType = value; }
        }
        
        #endregion

		#region Constructors

		public DetailedDescription()
		{
		}

        public DetailedDescription(DetailedDescriptionType detailedDescriptionType, string text)
		{
            if (text.Length > _TEXT_LIMIT)
                _text = text.Substring(0, _TEXT_LIMIT);
            else
                _text = text;
            _detailedDescriptionType = detailedDescriptionType;
		}

		public DetailedDescription(XmlNode node)
		{
			_text = Utility.XMLHelper.GetText(node);

            try
            {
                _detailedDescriptionType = (DetailedDescriptionType)Enum.Parse(typeof(DetailedDescriptionType), Utility.XMLHelper.GetAttributeString(node, XMLNames._A_DetailedDescriptionType));
            }
            catch (ArgumentException)
            {
                _detailedDescriptionType = DetailedDescriptionType.None;
            }
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			if (bValidate)
			{
			}

			Utility.XMLHelper.AddText(xmlDoc, node, _text);
            Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_DetailedDescriptionType, _detailedDescriptionType.ToString());
        }

		#endregion

	}
}
