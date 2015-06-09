using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class DocumentType		
	{
		private string _name;

		public string Name
		{
			get { return Utility.XMLHelper.GetXMLText(_name); }
			set { _name = Utility.XMLHelper.SetXMLText(value); }
		}


		public DocumentType(string documentTypeName)
		{
			_name = documentTypeName;
		}

	}
}
