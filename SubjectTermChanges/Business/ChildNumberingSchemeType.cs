using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	public enum ChildNumberingSchemeType
	{
		None = 0,
		UpperCaseAlpha = 1,
		LowerCaseAlpha = 2,
		UpperCaseRoman = 3,
		LowerCaseRoman = 4,
		Number = 5
	}

	public static class ChildNumberingSchemeHelper
	{
		public static string ParagraphNumber(ChildNumberingSchemeType numberingSchemeType, int number)
		{
			switch (numberingSchemeType)
			{
				case ChildNumberingSchemeType.UpperCaseAlpha:
					if (number > 26)
						throw new ArgumentOutOfRangeException(number.ToString(), "The number of paragraphs is limited to 26 when using the Alpha numbering scheme.");
					return ((char)(64 + number)).ToString() + ".";
				case ChildNumberingSchemeType.LowerCaseAlpha:
					if (number > 26)
						throw new ArgumentOutOfRangeException(number.ToString(), "The number of paragraphs is limited to 26 when using the Alpha numbering scheme.");
					return ((char)(96 + number)).ToString() + ".";
				case ChildNumberingSchemeType.UpperCaseRoman:
					return Utility.RomanNumeralHelper.RomanNumeral(number) + ".";
				case ChildNumberingSchemeType.LowerCaseRoman:
					return Utility.RomanNumeralHelper.RomanNumeral(number).ToLower() + ".";
				case ChildNumberingSchemeType.Number:
					return number.ToString() + ".";
				default:
					return string.Empty;
			}
		}

		public static string GetSchemeDisplayText(ChildNumberingSchemeType cnst)
		{
			switch (cnst)
			{
				case ChildNumberingSchemeType.UpperCaseAlpha:
					return "A, B, C, D";
				case ChildNumberingSchemeType.LowerCaseAlpha:
					return "a, b, c, d";
				case ChildNumberingSchemeType.UpperCaseRoman:
					return "I, II, III, IV";
				case ChildNumberingSchemeType.LowerCaseRoman:
					return "i, ii, iii, iv";
				case ChildNumberingSchemeType.Number:
					return "1, 2, 3, 4";
				case ChildNumberingSchemeType.None:
					return "None";
				default:
					throw new XmlException(string.Format("Not able to get text version for ChildNumberingSchemeType value '{0}'", ((long)cnst).ToString()));
			}
		}


		public static string GetSchemeText(ChildNumberingSchemeType cnst)
		{
			switch (cnst)
			{
				case ChildNumberingSchemeType.UpperCaseAlpha:
					return XMLNames._CNST_A;
				case ChildNumberingSchemeType.LowerCaseAlpha:
					return XMLNames._CNST_a;
				case ChildNumberingSchemeType.UpperCaseRoman:
					return XMLNames._CNST_I;
				case ChildNumberingSchemeType.LowerCaseRoman:
					return XMLNames._CNST_i;
				case ChildNumberingSchemeType.Number:
					return XMLNames._CNST_1;
				case ChildNumberingSchemeType.None:
					return XMLNames._CNST_None;
				default:
					throw new XmlException(string.Format("Not able to get text version for ChildNumberingSchemeType value '{0}'", ((long)cnst).ToString()));
			}
		}

		public static ChildNumberingSchemeType GetSchemeType(string name)
		{
			if (string.IsNullOrEmpty(name))
				return ChildNumberingSchemeType.None;
			else
				switch (name)
				{
					case XMLNames._CNST_A:
						return ChildNumberingSchemeType.UpperCaseAlpha;
					case XMLNames._CNST_a:
						return ChildNumberingSchemeType.LowerCaseAlpha;
					case XMLNames._CNST_I:
						return ChildNumberingSchemeType.UpperCaseRoman;
					case XMLNames._CNST_i:
						return ChildNumberingSchemeType.LowerCaseRoman;
					case XMLNames._CNST_1:
						return ChildNumberingSchemeType.Number;
					case XMLNames._CNST_None:
						return ChildNumberingSchemeType.None;
					default:
						throw new XmlException(string.Format("Not able to assign ChildNumberingScheme value '{0}'", name));
				}
		}
	}


}
