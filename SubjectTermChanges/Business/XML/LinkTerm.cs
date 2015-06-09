using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class LinkTerm : Term
	{
		#region private members
		private string _url;
		private string _caption;
		private Guid _complexListID;
		private string _linkSource;
        private bool? _isManagedItemReference;
        
		#endregion

		#region Properties

		public string URL
		{
			get { return Utility.XMLHelper.GetXMLText(_url); }
			set { _url = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Caption
		{
			get { return Utility.XMLHelper.GetXMLText(_caption); }
			set { _caption = Utility.XMLHelper.SetXMLText(value); }
		}

		public bool? IsManagedItemReference
		{
			get { return _isManagedItemReference; }
			set { _isManagedItemReference = value; }
		}

		public override string Keyword
		{
			get
			{
				if (!HasKeyWord)
					return null;
				return string.Concat(Caption, XMLNames._M_Delimiter, URL).Trim(XMLNames._M_Delimiter);
			}
		}

		public Guid ComplexListID
		{
			get { return _complexListID; }
			set	{_complexListID = value;}
		}

        public string LinkSource
        {
            get { return Utility.XMLHelper.GetXMLText(_linkSource); }
            set { _linkSource = Utility.XMLHelper.SetXMLText(value); }
        }

        public override int? RequiredSize
        {
            get
            {
                throw new Exception("LinkTerm should not be stored as data"); ;
            }
        }

		#endregion

		#region Constructors

        public LinkTerm(bool systemTerm, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
			TermType = TermType.Link;
			NameRequired = true;
		}

        public LinkTerm(XmlNode termNode, Template template, bool isFilter)
            : base(termNode, template, isFilter)
		{
			TermType = TermType.Link;
			NameRequired = true;

			_url = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_URL);
			_caption = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Caption);
			_isManagedItemReference = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_IsManagedItemReference);
			_complexListID = Term.CreateID(termNode, XMLNames._A_ComplexListID);
			_linkSource = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_LinkSource);
		}

		#endregion

		#region Build XML

		public override void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
                //if (!(_isManagedItemReference ?? false))
                //    Utility.XMLHelper.ValidateString(XMLNames._A_URL, _url);
			    Utility.XMLHelper.ValidateString(XMLNames._A_Caption, _caption);
			}

			base.Build(xmlDoc, termNode, bValidate);

			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_URL, _url);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Caption, _caption);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_IsManagedItemReference, _isManagedItemReference);
			Term.StoreID(xmlDoc, termNode, null, null, XMLNames._A_ComplexListID, _complexListID);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_LinkSource, _linkSource);
		}

		#endregion


		public override string DisplayValue(string termPartSpecifier)
		{
			if (_isManagedItemReference ?? false)
				return string.Format(@"<a href=""{0}itat/?system={1}&item={2}"" >{3}</a>", XMLNames._M_EnvironmentHolder, XMLNames._M_SystemNameHolder, XMLNames._M_ManagedItemIdHolder, Caption);
			else
				return string.Format(@"<a href=""{0}"" >{1}</a>", URL, Caption);
		}

        public override List<string> Validate(bool includeTab, string filterTermTabName)
		{
			return null;
		}

        public override List<string> CheckType(bool includeTab, string filterTermTabName)
        {
            return null;
        }
        
        public override bool EmitPDFXML(XmlWriter writer)
		{
            PdfHelper.AddPDFXMLHeader(writer);
            PdfHelper.AddPDFXMLRow(writer, Name, Caption);
            PdfHelper.AddPDFXMLFooter(writer);
            return true;
		}



		public override void SetDefaultValue()
		{
			//nothing
		}

        public override void Clear()
		{
			// nothing
		}

	}
}
