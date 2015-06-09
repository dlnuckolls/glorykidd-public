using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
    public class TermStore
    {
        #region Constants

        const char _TERM_NAME_DELIMITER = '|';
        public const int maxLoopCount = 1000;

        #endregion Constants

        #region Private Members

        private TermType _termType;
        private string _name;
        //Term will either use '_multiValue' or '_fieldValues', not both!
        private List<string> _multiValue;
        private List<Dictionary<string /*Field Name*/, string /*Field Value*/>> _fieldValues;
        private int? _length;        //Only used during validity checks!

        #endregion Private Members

        #region Public Properties

        public string Name
        {
            get { return _name; }
        }

        public TermType TermType
        {
            get { return _termType; }
        }

        public int? Length
        {
            get { return _length; }
            set { _length = value; }
        }

        #endregion Public Properties

        #region Constructor

        public TermStore(string name, TermType termType)
        {
            _name = name;
            _termType = termType;
            _multiValue = null;
            _fieldValues = null;
        }

        #endregion Constructor

        #region Public Methods

        public int AddMultiValue(string value)
        {
            if (_multiValue == null)
                _multiValue = new List<string>();
            _multiValue.Add(value);
            return _multiValue.Count;
        }
         
        public string GetMultiValue(int index)
        {
            return _multiValue[index];
        }

        public int AddFieldValue(Dictionary<string, string> fieldValue)
        {
            if (_fieldValues == null)
            {
                _fieldValues = new List<Dictionary<string, string>>();
            }
            if (fieldValue != null)
                _fieldValues.Add(fieldValue);

            return _fieldValues.Count;
        }

        //Used to determine the max number of rows needed for the DataTable that will be created.  Then used to iteratively add rows as needed.
        public virtual int GetRowCount()
        {
            switch (_termType)
            {
                case TermType.Text:
                case TermType.Date:
                case TermType.MSO:
                case TermType.Renewal:
                    return 1;

                case TermType.Facility:
                case TermType.PickList:
                case TermType.PlaceHolderAttachments:
                    if (_multiValue == null)
                        return 0;
                    else
                        return _multiValue.Count;

                case TermType.ComplexList:
                case TermType.External:
                     if (_fieldValues == null)
                        return 0;
                    else
                        return _fieldValues.Count;

                case TermType.None:
                case TermType.Link:
                case TermType.PlaceHolderComments:
                    throw new Exception(String.Format("TermType '{0}' should not have GetRowCount called", _termType.ToString()));

                default:
                    throw new Exception(String.Format("TermType '{0}' not handled for call GetRowCount", _termType.ToString()));
            }
        }

        //This returns the value without taking validation into consideration.
        public virtual string GetIndexedValue(int index, string fieldName)
        {
            switch (_termType)
            {
                case TermType.Text:
                case TermType.Date:
                case TermType.Renewal:
                    return index == 0 ? _multiValue[index] : string.Empty;

                case TermType.Facility:
                case TermType.PickList:
                case TermType.PlaceHolderAttachments:
                    if (_multiValue == null)
                        return string.Empty;
                    else
                        return index > _multiValue.Count - 1 ? string.Empty : _multiValue[index];

                case TermType.MSO:
                case TermType.ComplexList:
                case TermType.External:
                    if (_fieldValues == null || string.IsNullOrEmpty(fieldName))
                        return string.Empty;
                    else
                    {
                        if (_fieldValues[0].ContainsKey(fieldName))
                        {
                            try
                            {
                                return index > _fieldValues.Count - 1 ? string.Empty : _fieldValues[index][fieldName];
                            }
                            catch
                            {
                                return string.Empty;
                            }
                        }
                        else
                            return string.Empty;
                    }

                case TermType.None:
                case TermType.Link:
                case TermType.PlaceHolderComments:
                default:
                    throw new Exception(String.Format("TermType '{0}' should not have GetIndexedValue called", _termType.ToString()));
            }
        }

        #endregion Public Methods

        #region Public Static Methods

        public static string GetTermConcatenatedName(string templateName, string termName, string fieldName)
        {
            if (string.IsNullOrEmpty(templateName) || string.IsNullOrEmpty(termName))
                throw new Exception("The template name and term name are required");
            if (templateName.IndexOf(_TERM_NAME_DELIMITER) > -1)
                throw new Exception(string.Format("The template name cannot contain the delimiter character '{0}'", _TERM_NAME_DELIMITER));
            if (termName.IndexOf(_TERM_NAME_DELIMITER) > -1)
                throw new Exception(string.Format("The term name cannot contain the delimiter character '{0}'", _TERM_NAME_DELIMITER));
            if (!string.IsNullOrEmpty(fieldName) && fieldName.IndexOf(_TERM_NAME_DELIMITER) > -1)
                throw new Exception(string.Format("The term fieldName name cannot contain the delimiter character '{0}'", _TERM_NAME_DELIMITER));
            if (!string.IsNullOrEmpty(fieldName))
                return string.Format("{0}{1}{2}{3}{4}", templateName, _TERM_NAME_DELIMITER, termName, _TERM_NAME_DELIMITER, fieldName);
            else
                return string.Format("{0}{1}{2}", templateName, _TERM_NAME_DELIMITER, termName);
        }

        public static bool RequiresValidation(TextTermFormat textTermFormat)
        {
            switch (textTermFormat)
            {
                case TextTermFormat.Currency:
                case TextTermFormat.Number:
                case TextTermFormat.Phone:
                case TextTermFormat.PhonePlusExtension:
                case TextTermFormat.SSN:
                    return true;

                case TextTermFormat.Plain:
                default:
                    return false;
            }
        }

        public static string ValidateTermType(string value, TermType termType, TextTermFormat textTermFormat)
        {
            decimal d;
            switch (termType)
            {
                case Business.TermType.Text:
                    switch (textTermFormat)
                    {
                        case TextTermFormat.Currency:
                            if (!string.IsNullOrEmpty(value) && !Utility.TextHelper.ValidateAsCurrency(value, out d))
                                return string.Format("Value '{0}' is not a properly formatted currency", value);
                            break;

                        case TextTermFormat.Number:
                            if (!string.IsNullOrEmpty(value) && !Utility.TextHelper.ValidateAsNumber(value, out d))
                                return string.Format("Value '{0}' is not a properly formatted number", value);
                            break;

                        case TextTermFormat.SSN:
                            if (!string.IsNullOrEmpty(value) && !Utility.TextHelper.ValidateAsSSN(value))
                                return string.Format("Value '{0}' is not a properly formatted ssn", value);
                            break;

                        //case TextTermFormat.Phone:
                        //    if (!string.IsNullOrEmpty(value) && !Utility.TextHelper.ValidateAsPhone(value))
                        //        return string.Format("Value '{0}' is not a properly formatted phone number", value);
                        //    break;

                        //case TextTermFormat.PhonePlusExtension:
                        //    if (!string.IsNullOrEmpty(value) && !Utility.TextHelper.ValidateAsPhonePlusExtension(value))
                        //        return string.Format("Value '{0}' is not a properly formatted phone number plus extension", value);
                        //    break;

                        case TextTermFormat.Phone:
                        case TextTermFormat.PhonePlusExtension:
                        case TextTermFormat.Plain:
                        default:
                            break;
                    }
                    break;

                case Business.TermType.ComplexList:
                case Business.TermType.PickList:
                case Business.TermType.External:
                case Business.TermType.MSO:
                case Business.TermType.Facility:
                case Business.TermType.PlaceHolderAttachments:
                    break;

                case Business.TermType.Date:
                case Business.TermType.Renewal:
                    if (!string.IsNullOrEmpty(value) && !Utility.DateHelper.IsXMLDate(value))
                        return string.Format("Value '{0}' is not a properly formatted date", value);
                    break;
            }
            return string.Empty;
        }

        public static string GetFormattedValue(string value, TermType termType, TextTermFormat textTermFormat, string dateFormat, string defaultDateFormat)
        {
            string usedDateFormat = dateFormat;
            if (string.IsNullOrEmpty(usedDateFormat))
                usedDateFormat = defaultDateFormat;
            switch (termType)
            {
                case Business.TermType.Text:
                    switch (textTermFormat)
                    {
                        case TextTermFormat.Currency:
                            if (string.IsNullOrEmpty(value))
                                return string.Empty;
                            else
                                return Utility.TextHelper.FormatAsCurrency(value, true, false);

                        case TextTermFormat.Number:
                            return Utility.TextHelper.FormatAsNumber(value);

                        case TextTermFormat.Phone:
                            return Utility.TextHelper.FormatAsPhone(value);

                        case TextTermFormat.PhonePlusExtension:
                            return Utility.TextHelper.FormatAsPhonePlusExtension(value);

                        case TextTermFormat.SSN:
                            return Utility.TextHelper.FormatAsSSN(value);

                        case TextTermFormat.Plain:
                        default:
                            break;
                    }
                    break;

                case Business.TermType.ComplexList:
                case Business.TermType.PickList:
                case Business.TermType.External:
                case Business.TermType.MSO:
                case Business.TermType.Facility:
                case Business.TermType.PlaceHolderAttachments:
                    break;

                case Business.TermType.Date:
                case Business.TermType.Renewal:
                    if (string.IsNullOrEmpty(value))
                        return string.Empty;
                    else
                    {
                        DateTime dateTime = Utility.DateHelper.GetXMLDateStore(value);
                        return Utility.DateHelper.FormatDate(dateTime, usedDateFormat);
                    }
            }
            return value;
        }

        #endregion Public Static Methods
    
    }
}
