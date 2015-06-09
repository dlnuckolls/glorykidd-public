using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Utility
{
	public class StringWriterWithEncoding : StringWriter
	{
		private Encoding _encoding;

		public StringWriterWithEncoding(Encoding encoding)
			: base()
		{
			_encoding = encoding;
		}

		public override Encoding Encoding
		{
			get
			{
				return _encoding;
			}
		}

	}
}
