using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Kindred.Knect.ITAT.Utility
{
	public static class TextHelper
	{
		private static Regex _format;
		private const string guidFormatString = @"^[A-Fa-f0-9]{32}$|^({|\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\))?$|^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$";
		private static string[] ZeroToNineteen = new string[] {"", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"};
		private static string[] TwentyToNinety = new string[] { "", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

		static TextHelper()
		{
			_format = new Regex(guidFormatString);
		}

		/// <summary>
		/// Returns true if s is a valid Guid
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool IsGuid(string s)
		{
			if (string.IsNullOrEmpty(s))
				return false;
			return _format.IsMatch(s);
		}


			
		/// <summary>
		/// If the input string is "too long" (length > maxLength), replace te middle of the string with ellipses
		/// </summary>
		/// <param name="s"></param>
		/// <param name="maxLength"></param>
		/// <returns>a string that contains the beginning and ending of the input string, with ellipses in the middle</returns>
		public static string FormatLongString(string s, int maxLength)
		{
			int len = s.Length;
			if (len <= maxLength)
				return s;
			if (maxLength < 20)
				throw new Exception("maxLength parameter must be at least 20.");
			return string.Concat(s.Substring(0, maxLength - 15), "...", s.Substring(len - 10));
		}



		public static string TextPlusNumber(int n)
		{
			return string.Format("{0} ({1:N0})", NumberToText(n), n);
		}


		/// <summary>
		/// Converts an integer to its textual representation
		/// </summary>
		/// <param name="n">The input integer</param>
		/// <returns>The textual representation of n.</returns>
		public static string NumberToText(int n)
		{
			if (n < 0)
				return "minus " + RecursiveNumberToText(-n).Trim();
			else if (n == 0)
				return "zero";
			else
				return RecursiveNumberToText(n).Trim();
		}


		private static string RecursiveNumberToText(int n)
		{
			if (n <= 19)
				return TextHelper.ZeroToNineteen[n];
			else if (n <= 99)
			{
				if (n % 10 == 0)
					return TextHelper.TwentyToNinety[n / 10];
				else
					return string.Concat(TextHelper.TwentyToNinety[n / 10], "-", TextHelper.ZeroToNineteen[n % 10]);
			}
			else if (n <= 999)
				return string.Concat(RecursiveNumberToText(n / 100), " hundred ", RecursiveNumberToText(n % 100));
			else if (n <= 999999)
				return string.Concat(RecursiveNumberToText(n / 1000), " thousand ", RecursiveNumberToText(n % 1000));
			else if (n <= 999999999)
				return string.Concat(RecursiveNumberToText(n / 1000000), " million ", RecursiveNumberToText(n % 1000000));
			else
				return string.Concat(RecursiveNumberToText(n / 1000000000), " billion ", RecursiveNumberToText(n % 1000000000));
		}



		/// <summary>
		/// Returns the ordinal version of the number; i.e., the number plus the suffix.   For example, 1st, 2nd, 3rd, etc.
		/// </summary>
		/// <param name="n">A positive integer (an exception is thrown if n is not positive)</param>
		/// <returns>the ordinal version of the number; i.e., the number plus the suffix.   For example, 1st, 2nd, 3rd, etc.</returns>
		public static string OrdinalSuffix(int n)
		{
			string suffix = string.Empty;
			switch (n % 10)
			{
				case 1:
					if (n % 100 == 11)
						suffix = "th";
					else
						suffix = "st";
					break;

				case 2:
					if (n % 100 == 12)
						suffix = "th";
					else
						suffix = "nd";
					break;

				case 3:
					if (n % 100 == 13)
						suffix = "th";
					else
						suffix = "rd";
					break;

				default:
					suffix = "th";
					break;
			}
			return n.ToString() + suffix;
		}


		public static bool IsPositiveInteger(string text)
		{
			int n;
			if (int.TryParse(text, out n))
				return n > 0;
			else
				return false;
		}

		public static bool IsZeroOrPositiveInteger(string text)
		{
			int n;
			if (int.TryParse(text, out n))
				return n >= 0;
			else
				return false;
		}

		public static string QueryString(bool bQuestionMark, params string[] sArguments)
		{
			string sQueryString = bQuestionMark ? "?" : "";
			for (int i = 0; i < sArguments.Length;i++)
			{
				sQueryString = string.Concat(sQueryString, sArguments[i++], "=", sArguments[i],"&");
			}
			return sQueryString.Trim('&');
		}

		#region Text-Formatting methods


		public static string FormatAsNumber(string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;

			int n;
			if (int.TryParse(s, out n))
				return string.Format("{0:N0}", n);

			decimal d;
			if (decimal.TryParse(s, out d))
				return string.Format("{0:N2}", d);

			return s;
		}

        public static bool ValidateAsCurrency(string Value, out decimal d)
        {
            return decimal.TryParse(Value, System.Globalization.NumberStyles.Currency, null, out d);
        }

        public static bool ValidateAsNumber(string Value, out decimal d)
        {
            return decimal.TryParse(Value, out d);
        }

        public static bool ValidateAsSSN(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            //strip out everything except numbers
            string str = Regex.Replace(value, "[^0-9]", "", RegexOptions.IgnoreCase);
            return str.Length == 9;
        }

        public static bool ValidateAsPhone(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                //strip out everything except numbers
                string str = Regex.Replace(s, "[^0-9]", "", RegexOptions.IgnoreCase);
                switch (str.Length)
                {
                    case 7:
                    case 10:
                        return true;
                }
            }
            return false;
        }

        public static bool ValidateAsPhonePlusExtension(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                //strip out everything except numbers and the letter x
                string str = Regex.Replace(s, "[^0-9x]", "", RegexOptions.IgnoreCase);
                int index = str.ToLower().IndexOf("x");
                switch (index)
                {
                    case -1:
                        return ValidateAsPhone(s);
                    case 7:
                    case 10:
                        return true;
                }
            }
            return false;
        }
      
        public static string FormatAsCurrency(string s, bool showCents, bool noText)
		{
			decimal d;
            if (decimal.TryParse(s, out d))
            {
                if (noText)
                    return showCents ? string.Format("{0:F2}", d) : string.Format("{0:F0}", d);
                else
                    return showCents ? string.Format("{0:C2}", d) : string.Format("{0:C0}", d);
            }
            else
                return s;
		}

		public static string FormatAsSSN(string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;
			//strip out everything except numbers
			string str = Regex.Replace(s, "[^0-9]", "", RegexOptions.IgnoreCase);
			switch (str.Length)
			{
				case 9:
					return string.Format("{0}-{1}-{2}", str.Substring(0, 3), str.Substring(3, 2), str.Substring(5, 4));
				default:
					return s;
			}
		}

		public static string FormatAsPhone(string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;
			//strip out everything except numbers
			string str = Regex.Replace(s, "[^0-9]", "", RegexOptions.IgnoreCase);
			switch (str.Length)
			{
				case 7:
					return string.Format("{0}-{1}", str.Substring(0, 3), str.Substring(3, 4));
				case 10:
					return string.Format("({0}) {1}-{2}", str.Substring(0, 3), str.Substring(3, 3), str.Substring(6, 4));
				default:
					return s;
			}
		}

		public static string FormatAsPhonePlusExtension(string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;
			//strip out everything except numbers and the letter x
			string str = Regex.Replace(s, "[^0-9x]", "", RegexOptions.IgnoreCase);
			int index = str.ToLower().IndexOf("x");
			if (index > -1)
			{
				string phone = str.Substring(0, index).Trim();
				string ext = str.Substring(index + 1).Trim();
				return string.Format("{0} x{1}", FormatAsPhone(phone), ext);
			}
			else
			{
				return FormatAsPhone(s);
			}
		}

        public static string ReplaceWhiteSpace(string value)
        {
            return value.Replace("\n", " ").Replace("\t", " ").Replace("\r", " ").Replace("\f", " ").Replace("\v", " ");
        }

        public static string CSVFormat(int? maxLength, string value, bool lastItem)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string formattedValue = value;
                if (maxLength.HasValue)
                {
                    int length = value.Length > maxLength.Value ? maxLength.Value : value.Length;
                    formattedValue = value.Substring(0, length);
                }

                StringBuilder sb = new StringBuilder();
                if (formattedValue.StartsWith(" ") || formattedValue.EndsWith(" ") || formattedValue.IndexOf(",") > 0 || formattedValue.IndexOf("\"") > 0 || formattedValue.IndexOf(Environment.NewLine) > 0)
                    sb = sb.Append("\"").Append(formattedValue.Replace("\"", "\"\"")).Append("\"");
                else
                    sb = sb.Append(formattedValue);
                return sb.Append((lastItem ? string.Empty : ",")).ToString();
            }
            else
                return (lastItem ? string.Empty : ",");
        }

		#endregion


	}
}
