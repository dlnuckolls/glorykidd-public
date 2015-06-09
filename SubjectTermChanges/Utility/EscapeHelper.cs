using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Kindred.Knect.ITAT.Utility
{

	/// <summary>
	/// This enum is used when identifying escape modes.
	/// </summary>
	public enum EscapeMode
	{

		Undefined = 0,
		None,
		Html,
		JavaScript,
		Url
	}; /* enum */

	public class EscapeHelper
	{
		/// <summary>
		/// This function returns the string escaped according to the supplied mode.
		/// </summary>
		/// <param name="value">The string to escape.</param>
		/// <param name="mode">The escape mode.</param>
		/// <returns>An escaped string.</returns>
		public static string Escape(string value, EscapeMode mode)
		{
			if (value == null)
				throw new ArgumentNullException("value", "The string must be supplied.");
			if (mode < EscapeMode.None || mode > EscapeMode.Url)
				throw new ArgumentOutOfRangeException("mode", "The supplied mode is not a recognized escape mode.");

			switch (mode)
			{
				case EscapeMode.Html:
						return HttpUtility.HtmlEncode(value);
				//Note - use of the JavaScript mode would prevent the use of embedded
				//carraige returns in the text to be alerted.  Alerts require the
				//term "\\n" for carriage return, which would be converted to "\\\\n"
				//by this function.
				case EscapeMode.JavaScript:
						return value.Replace("\\", "\\\\").Replace("'", "\\'");
				case EscapeMode.Url:
						return HttpUtility.UrlEncode(value);
				default:
					return value;
			} /* switch */
		}

	}

}
