using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{
	public class MSOKeyValueItem
	{
		private string _key;
		private string _fieldName;
		private string _fieldValue;
		private string _termName;


		public MSOKeyValueItem(string key, string fieldName, string fieldValue, string termName)
		{
			_key = key;
			_fieldName = fieldName;
			_fieldValue = fieldValue;
			_termName = termName;
		}

		public string Key {get { return _key; }}
		public string FieldName { get { return _fieldName; } }
		public string FieldValue { get { return _fieldValue; } }
		public string TermName { get { return _termName; } }
	}
}
