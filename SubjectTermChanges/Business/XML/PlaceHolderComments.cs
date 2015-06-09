using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
    public class PlaceHolderComments : PlaceHolderTerm
	{
        #region Constructors

        public PlaceHolderComments(bool systemTerm, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
            Name = "Comments";
			TermType = TermType.PlaceHolderComments;
        }

        public PlaceHolderComments(XmlNode termNode, Template template, bool isFilter)
            : base(termNode, template, isFilter)
		{
            TermType = TermType.PlaceHolderComments;
		}

		#endregion

        public override int? RequiredSize
        {
            get
            {
                throw new Exception("PlaceHolderComments should not be stored as data"); ;
            }
        }
	}
}
