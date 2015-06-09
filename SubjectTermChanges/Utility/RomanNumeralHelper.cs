using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Kindred.Knect.ITAT.Utility
{

	public static class RomanNumeralHelper
	{
		static KeyValuePair<int, string>[] _numbers;

		static RomanNumeralHelper()
		{ 
			_numbers = new KeyValuePair<int, string>[]
			{
				new KeyValuePair<int, string>(1000, "M"),
				new KeyValuePair<int, string>(900, "CM"), 
				new KeyValuePair<int, string>(500, "D"), 
				new KeyValuePair<int, string>(400, "CD"), 
				new KeyValuePair<int, string>(100, "C"), 
				new KeyValuePair<int, string>(90, "XC"), 
				new KeyValuePair<int, string>(50, "L"), 
				new KeyValuePair<int, string>(40, "XL"), 
				new KeyValuePair<int, string>(10, "X"), 
				new KeyValuePair<int, string>(9, "IX"), 
				new KeyValuePair<int, string>(5, "V"), 
				new KeyValuePair<int, string>(4, "IV"), 
				new KeyValuePair<int, string>(1, "I")
			};
		}


		public static string RomanNumeral(int number)
		{
			// Should check first for number in range... omitted for clarity. 
			if (number <= 0)
				throw new ArgumentOutOfRangeException(string.Format("Cannot convert {0} to a Roman Numeral.", number));
			if (number >= 4000)
				throw new ArgumentOutOfRangeException(string.Format("Cannot convert {0} to a Roman Numeral.", number));
			StringBuilder result = new StringBuilder();
			int numIndex = 0;
			while (number > 0)
			{
				int val = _numbers[numIndex].Key;
				if (val <= number)
				{
					number -= val;
					result.Append(_numbers[numIndex].Value);
				}
				else
				{
					numIndex++;
				}
			}
			return result.ToString();
		}



	}
}
