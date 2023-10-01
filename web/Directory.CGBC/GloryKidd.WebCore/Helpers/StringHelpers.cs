using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GloryKidd.WebCore.Helpers {
  public static class StringHelpers {
    #region Fix Sql Helper Extensions
    /// <summary>
    /// Converts the SQL date.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <returns></returns>
    public static string ConvertSqlDate(this DateTime d) {
      var maxdate = "06/06/2079".GetAsDate();
      var mindate = "01/01/1900".GetAsDate();
      if(d > maxdate) d = maxdate;
      if(d < mindate) d = mindate;
      if(d == DateTime.MinValue) d = mindate;
      return string.Format("'{0}'", d.ToString("yyyy-MM-dd"));
    }
    public static string ConvertSqlDateTime(this DateTime d) {
      var maxdate = "06/06/2079".GetAsDate();
      var mindate = "01/01/1900".GetAsDate();
      if(d > maxdate) d = maxdate;
      if(d < mindate) d = mindate;
      if(d == DateTime.MinValue) d = mindate;
      return string.Format("'{0}'", d.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
    /// <summary>
    /// Fix string values that are to be inserted into the 
    /// database by removing single quotes and checking for
    /// null and replacing it w/empty.
    /// </summary>
    /// <param name="p">String value to be fixed.</param>
    /// <returns>Sql ready string.</returns>
    public static string FixSqlString(this string p) {
      if(p.IsNullOrEmpty()) return string.Empty;
      return p.Replace("'", "''");
    }
    /// <summary>
    /// Fix string values for sql insertion and set to a
    /// maximum length based on the parameter passed.
    /// </summary>
    /// <param name="p">String value.</param>
    /// <param name="len">Max length.</param>
    /// <returns>Sql ready string of proper length.</returns>
    public static string FixSqlString(this string p, int len) {
      if(p.IsNullOrEmpty()) return string.Empty;
      p = p.Replace("'", "''");
      if(p.Length > len) p = p.Substring(0, len);
      return p;
    }
    public static string FixShortSqlString(this string p, int len) {
      if(p.IsNullOrEmpty()) return string.Empty;
      p = p.Replace("'", "''");
      if(p.Length > len) p = p.Substring(p.Length - len, len);
      return p;
    }
    /// <summary>
    /// Fix string zip code values to fit our rules.
    /// </summary>
    /// <param name="p">String zip code value.</param>
    /// <returns>Sql ready zip code string.</returns>
    public static string FixSqlZip(this string p) {
      string rtn = p.Replace("-", "").Replace(" ", "");
      return rtn.Substring(0, (rtn.Length > 5) ? 5 : rtn.Length);
    }
    /// <summary>
    /// Fix string values for ssn numbers to fit 
    /// our rules.
    /// </summary>
    /// <param name="p">Incoming string value.</param>
    /// <returns>Sql ready ssn string.</returns>
    public static string FixSqlSsn(this string p) {
      return p.Replace("-", "").Replace(" ", "").Trim();
    }
    /// <summary>
    /// Fix string phone number values to fit our
    /// rules, ten digits only, no dashes or parens.
    /// </summary>
    /// <param name="p">Incoming phone number string.</param>
    /// <returns>Sql ready phone number string.</returns>
    public static string FixSqlPhone(this string p) {
      p = p.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();
      if(p.Length > 10)
        return p.Substring(0, 10);
      else
        return p;
    }
    #endregion
    #region Change datatypes
    ///<summary>
    /// Converts an alphanumeric string into a numeric string using 
    /// numbers for any letters in the string.
    /// </summary>
    /// <returns>A numeric string w/last 9 chars for conversion
    /// to an int32 value.</returns>
    public static string GetAsNumeric(this string s) {
      string rtn = "";
      string str = "abcdefghijklmnopqrstuvwxyz";
      char[] chars = s.ToLower().Replace(" ", "").ToCharArray();
      foreach(char c in chars) {
        if(char.IsDigit(c))
          rtn += c.ToString();
        else
          rtn += (str.IndexOf(c)).ToString();
      }
      if(rtn.Length > 9)
        rtn = rtn.Substring(rtn.Length - 9, 9);
      return rtn;
    }
    /// <summary>
    /// Gets an int32
    /// </summary>
    /// <param name="s">String Input</param>
    /// <returns></returns>
    public static int GetInt32(this string s) {
      try {
        s = s.Trim();
        if(s.IsEmpty()) return 0;
        return Convert.ToInt32(s);
      } catch { return 0; }
    }
    /// <summary>
    /// Gets an int64.
    /// </summary>
    /// <param name="s">String Input</param>
    /// <returns></returns>
    public static long GetInt64(this string s) {
      try {
        s = s.Trim();
        if(s.IsEmpty()) return 0;
        return Convert.ToInt64(s);
      } catch { return 0; }
    }
    /// <summary>
    /// Gets as money.
    /// </summary>
    /// <param name="s">String Input</param>
    /// <returns></returns>
    public static double GetMoney(this string s) {
      try {
        s = s.Trim();
        if(s.IsEmpty()) return 0.00;
        if(s.Equals("0")) return 0.00;
        int dec = s.IndexOf(".");
        //jjh Fixed strings length check 04/14/2014
        if(dec > -1 || s.Length < 3) {
          s = s.PadLeft(3, '0');
          return Convert.ToDouble("{0}.{1}".FormatWith(
          s.Substring(0, s.Length - 2),
          s.Substring(s.Length - 2)));
        } else
          return Convert.ToDouble("{0}.{1}".FormatWith(
          s.Substring(0, s.Length - 2),
          s.Substring(s.Length - 2)));
      } catch { return 0.00; }
    }
    /// <summary>
    /// Gets the payment money.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns>System.Double.</returns>
    public static double GetPaymentMoney(this string s) {
      try {
        s = s.Trim();
        if(s.IsEmpty()) return 0.00;
        if(s.Equals("0")) return 0.00;
        int dec = s.IndexOf(".");
        if(dec > -1) {
          return Convert.ToDouble(s);
        } else {
          s += "00";
          return Convert.ToDouble("{0}.{1}".FormatWith(
          s.Substring(0, s.Length - 2),
          s.Substring(s.Length - 2)));
        }
      } catch { return 0.00; }
    }
    /// <summary>
    /// Gets as double.
    /// </summary>
    /// <param name="s">String Input</param>
    /// <returns></returns>
    public static double GetAsDouble(this string s) {
      try {
        double d;
        double.TryParse(s, out d);
        return d;
      } catch { return 0.00; }
    }
    /// <summary>
    /// Gets as date.
    /// </summary>
    /// <param name="s">String Input</param>
    /// <returns></returns>
    public static DateTime GetAsDate(this string s) {
      try {
        DateTime dt;
        DateTime.TryParse(s, out dt);
        return dt;
      } catch { return DateTime.MinValue; }
    }
    /// <summary>
    /// Gets as bool.
    /// </summary>
    /// <param name="s">String Input</param>
    /// <returns></returns>
    public static bool GetAsBool(this string s) {
      try {
        bool rtn;
        bool.TryParse(s, out rtn);
        return rtn;
      } catch { return false; }
    }
    public static string FormatCreditCardMask(this string s, int displayCount) {
      try {
        var rtn = string.Empty;
        var chars = s.ToArray();
        if(chars.Length <= displayCount) rtn = s;
        int setPoint = chars.Length - displayCount;
        for(int i = 0; i < chars.Length; i++) {
          if(i < setPoint && chars[i] != ' ')
            rtn += "X";
          else if(i < setPoint && chars[i] == ' ')
            rtn += " ";
          else
            rtn += chars[i];
        }
        return rtn;
      } catch { return string.Empty; }
    }
    public static string FormatPhone(this string s) {
      try {
        var rtn = string.Empty;
        var chars = s.ToArray();
        if(chars.Length <= 6) rtn = s;
        else if(chars.Length == 10) {
          rtn = "({0}) {1}-{2}".FormatWith(s.Substring(0, 3), s.Substring(3, 3), s.Substring(6));
        } else if(chars.Length == 7) {
          rtn = "{0}-{1}".FormatWith(s.Substring(0, 3), s.Substring(3));
        }
        return rtn;
      } catch { return string.Empty; }
    }
    #endregion
    #region Fluent
    /// <summary>
    /// Converts to pascal case for strings
    /// </summary>
    /// <param name="str">The STR.</param>
    /// <returns></returns>
    public static string ConvertToPascal(this string str) {
      if(str.Length == 0) return str;
      string[] strWords = str.Split(' ');
      for(int i = 0; i < strWords.Length; i++) {
        if(strWords[i].Length > 0) {
          string strWord = strWords[i];
          char strFirstLetter = char.ToUpper(strWord[0]);
          strWords[i] = strFirstLetter + strWord.Substring(1).ToLower();
        }
      }
      return string.Join(" ", strWords);
    }
    /// <summary>
    /// Retrieve the custom name for Enumerations
    /// </summary>
    /// <param name="myEnum">Enum to read</param>
    /// <returns>TextValue or Enum to string</returns>
    public static string TextValue(this Enum myEnum) {
      string value;
      try {
        var customEnumAttribute = (CustomEnumAttribute)myEnum.GetType().GetCustomAttributes(typeof(CustomEnumAttribute), false).FirstOrDefault();
        if(customEnumAttribute == null || customEnumAttribute.IsCustomEnum == false) { throw new Exception(); }
        var textValueAttribute = (TextValueAttribute)myEnum.GetType().GetMember(myEnum.ToString()).Single().GetCustomAttributes(typeof(TextValueAttribute), false).FirstOrDefault();
        value = textValueAttribute != null ? textValueAttribute.Value : myEnum.ToString();
      } catch { value = myEnum.ToString(); }
      return value;
    }
    /// <summary>
    /// Encrypt Strings like passwords
    /// </summary>
    /// <param name="s">string to encrypt</param>
    /// <returns>encrypted string value</returns>
    public static string EncryptString(this string s) { return EncryptionHelper.Encrypt(s); }
    /// <summary>
    /// Decrypt string values like passwords
    /// </summary>
    /// <param name="s">Encrypted string</param>
    /// <returns>plain text string</returns>
    public static string DecryptString(this string s) { return EncryptionHelper.Decrypt(s); }
    /// <summary>
    /// Formats the string called on with the objects passed in
    /// to the method.
    /// </summary>
    /// <param name="format">The string being extended.</param>
    /// <param name="args">The args to use in the format call.</param>
    /// <returns></returns>
    public static string FormatWith(this string format, params object[] args) { return string.Format(format, args); }
    /// <summary>
    /// Joins the string array called on with the separator passed
    /// in between each item.
    /// </summary>
    /// <param name="join">The string array being extended.</param>
    /// <param name="separator">Separator value for the join call.</param>
    /// <returns></returns>
    public static string JoinWith(this string[] join, string separator) { return string.Join(separator, join); }

    #endregion
    #region Non-Fluent
    /// <summary>
    /// Determines whether the specified string is empty.
    /// </summary>
    /// <param name="s">The instance of string to be extended.</param>
    /// <returns>
    /// 	<c>true</c> if the specified s is empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEmpty(this string s) { return s.Length == 0; }
    /// <summary>
    /// Determines whether [is null or empty] [the specified s].
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns>
    /// 	<c>true</c> if [is null or empty] [the specified s]; 
    /// 	otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty(this string s) { return string.IsNullOrEmpty(s); }
    /// <summary>
    /// Determines whether [is null or empty] [the specified o].
    /// </summary>
    /// <param name="o">The o.</param>
    /// <returns>
    ///   <c>true</c> if [is null or empty] [the specified o]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty(this object o) { return (o == null); }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsValidEmail(this string s) {
      var validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
      var validTest = new Regex(validEmailPattern, RegexOptions.IgnoreCase);
      return !s.IsNullOrEmpty() && validTest.IsMatch(s);
    }
    #endregion
  }
}