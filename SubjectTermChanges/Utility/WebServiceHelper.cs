using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web.Services.Protocols;

namespace Kindred.Knect.ITAT.Utility
{

	public static class WebServiceHelper
	{
		/* Start Collection Helper Methods */
		public static object[][] ToJaggedArray(Hashtable ht)
		{
			object[][] oo = new object[ht.Count][];
			int i = 0;

			foreach (object key in ht.Keys)
			{
				oo[i] = new object[] { key, ht[key] };
				i++;
			}
			return oo;
		}

		public static Hashtable ToHashtable(object[][] oo)
		{
			Hashtable ht = new Hashtable(oo.Length);

			foreach (object[] pair in oo)
			{
				object key = pair[0];
				object value = pair[1];
				ht[key] = value;
			}
			return ht;
		}

		public static string MimeTypeToExtension(string mimeType)
		{
			switch (mimeType)
			{
				case "application/pdf":
					return "pdf";
				case "application/msword":
					return "doc";
				case "application/vnd.ms-excel":
					return "xls";
				case "application/vnd.ms-powerpoint":
					return "ppt";
				case "text/html":
				case "text/plain":
					return "txt";
				case "text/richtext":
					return "rtf";
				case "image/jpeg":
					return "jpg";
				case "application/octet-stream":
					return "doc";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    return "docx";
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    return "xlsx";
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    return "pptx";
				default:
					return "pdf";
			}
		}


		public static string ExtensionToMimeType(string docuType)
		{
			if (docuType.StartsWith("."))
				docuType = docuType.TrimStart('.');
			switch (docuType)
			{
				case "pdf":
					return "application/pdf";
				case "doc":
					return "application/msword";
				case "xls":
					return "application/vnd.ms-excel";
				case "ppt":
					return "application/vnd.ms-powerpoint";
				case "txt":
					return "text/plain";
				case "rtf":
					return "text/richtext";
				case "jpg":
					return "image/jpeg";
                case "docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
				default:
					return "";
			}
		}



		/* End Collection Helper Methods */

	}
}
