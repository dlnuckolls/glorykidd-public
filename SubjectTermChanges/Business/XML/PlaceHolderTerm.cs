using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class PlaceHolderTerm : Term
	{

        #region Constructors

        public PlaceHolderTerm(bool systemTerm, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
			NameRequired = true;
            _template = template;
        }

        public PlaceHolderTerm(XmlNode termNode, Template template, bool isFilter)
            : base(termNode, template, isFilter)
		{
            NameRequired = true;
        }

		#endregion

        public override string DisplayValue(string termPartSpecifier)
        {
            return null;
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
