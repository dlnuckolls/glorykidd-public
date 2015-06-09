using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Kindred.Knect.ITAT.Utility
{

	public enum SpecialDateFormats
	{
		OrdinalDate1 = 1,       
		OrdinalDate2 = 2			
	}

	public static class DateHelper
	{

		/// <summary>
		///  Date Formats supported within this application
		/// </summary>
		public static List<string> DateFormats = null;

		static DateHelper()
		{
            DateFormats = new List<string>(11);
            DateFormats.Add("MM/dd/yyyy");
            DateFormats.Add("MM/dd/yy");
            DateFormats.Add("M/d/yyyy");
            DateFormats.Add("M/d/yy");
            DateFormats.Add("MMMM dd, yyyy");
            DateFormats.Add("MMMM d, yyyy");
            DateFormats.Add(Names._DH_OrdinalDate2);					//ex: March 1st, 2006
            DateFormats.Add("dd\" day of \"MMMM,  yyyy");
            DateFormats.Add("d\" day of \"MMMM,  yyyy");
            DateFormats.Add(Names._DH_OrdinalDate1);					//ex: 1st day of March, 2006
            DateFormats.Add("yyyyMMdd");
        }

		/// <summary>
		/// Similar to the formatted DateTime.ToString(), but adds a special format called "OrdinalLongDate": for example, "1st day of March, 2006"
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string FormatDate(DateTime dt, string format)
		{
			switch (format)
			{
				case Names._DH_OrdinalDate1:
					return OrdinalDate1(dt);
				case Names._DH_OrdinalDate2:
					return OrdinalDate2(dt);
				case Names._DH_Sortable:
					return dt.ToString(DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern);
				case Names._DH_SystemDefault:
					return dt.ToString(DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern);
				default:
					return dt.ToString(format);
			}
		}

		private static string OrdinalDate1(DateTime dt)
		{
			return string.Concat("the ", TextHelper.OrdinalSuffix(dt.Day), " day of ", dt.ToString("MMMM, yyyy"));
		}

		private static string OrdinalDate2(DateTime dt)
		{
			return string.Concat(dt.ToString("MMMM "), TextHelper.OrdinalSuffix(dt.Day), dt.ToString(", yyyy"));
		}

		//Retrieve the date from an xml stream
		public static DateTime GetXMLDate(string value)
		{
			return DateTime.ParseExact(value, DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern, CultureInfo.InvariantCulture);
		}

        public static DateTime GetXMLDateStore(string value)
        {
            return DateTime.Parse(value);
        }

		//Write the date to an xml stream
		public static string SetXMLDate(DateTime date)
		{
			return date.ToString(DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern);
		}

        public static bool IsXMLDate(string value)
        {
            DateTime dt;
            return DateTime.TryParse(value, out dt);
        }

        public static bool SameDay(DateTime left, DateTime right)
        {
            return left.Day == right.Day && left.Month == right.Month && left.Year == right.Year;
        }
    }
}
