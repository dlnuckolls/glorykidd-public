using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
    public class PlaceHolderAttachments : PlaceHolderTerm
	{
        #region Constructors

        public PlaceHolderAttachments(bool systemTerm, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
            Name = "Attachments";
            TermType = TermType.PlaceHolderAttachments;
        }

        public PlaceHolderAttachments(XmlNode termNode, Template template, bool isFilter)
            : base(termNode, template, isFilter)
		{
            TermType = TermType.PlaceHolderAttachments;
		}

        public override int? RequiredSize
        {
            get
            {
                return null;
            }
        }

		#endregion
	}
}
