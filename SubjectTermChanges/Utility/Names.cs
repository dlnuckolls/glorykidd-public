using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Utility
{
	public class Names
	{
		//DateHelper:
		public const string _DH_OrdinalDate1 = "OrdinalDate1";
		public const string _DH_OrdinalDate2 = "OrdinalDate2";
		public const string _DH_SystemDefault = "SystemDefault";
		public const string _DH_Sortable = "Sortable";

		//Documentum (KDMS)
		public const string KDMS_DocType = "k_doc_itat";
		public const string KDMS_Property_DocumentName = "object_name";

		//FileShare
		public const string FS_FilePathPattern = @"\\{0}\apps\ITAT\{1}\attach\{2}";

		//Exception messages
		public const string _EM_UnrecognizedExtension = "Unrecognized file extension";
	}
}
