using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
    [Serializable]
    public class DataStoreField
    {
        public enum AssignedTermType
        {
            [Utility.EnumDescription("None")]
            None,
            [Utility.EnumDescription("Text")]
            Text,
            [Utility.EnumDescription("DateTime")]
            DateTime,
            [Utility.EnumDescription("Currency")]
            Currency
        }

        private const char _TERM_NAME_DELIMITER = '|';

        private readonly string _name;          //Template Name + Term Name + Field Name
        private readonly string _termName;
        private readonly string _templateName;
        private readonly string _fieldName;

        private TermType _termType;
        private TextTermFormat _textTermFormat;

        private string _alias;
        private int? _maxLength;
        private int? _length;
        private int? _sortSequence;
        private bool _sortAscending;
        private string _dateFormat;
        private bool _isChecked;
        private bool _isStandard;

        public string Name
        {
            get { return _name; }
        }

        public string TemplateName
        {
            get { return _templateName; }
        }

        public string FieldName
        {
            get { return _fieldName; }
        }

        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        public TextTermFormat TextTermFormat
        {
            get { return _textTermFormat; }
            set { _textTermFormat = value; }
        }

        public string TermName
        {
            get { return _termName; }
        }

        public int? MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }

        public int? Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public int? SortSequence
        {
            get { return _sortSequence; }
            set { _sortSequence = value; }
        }

        public bool SortAscending
        {
            get { return _sortAscending; }
            set { _sortAscending = value; }
        }
        //Not Required as 2011 Enhancement
        public string DateFormat
        {
            get { return _dateFormat; }
            set { _dateFormat = value; }
        }

        public TermType TermType
        {
            get { return _termType; }
            set { _termType = value; }
        }

        public bool IsComplexList
        {
            get { return _termType == Business.TermType.ComplexList; }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        public bool IsStandard
        {
            get { return _isStandard; }
            set { _isStandard = value; }
        }

        public bool RequiresTextLength
        {
            get
            {
                switch (_termType)
                {
                    case Business.TermType.Text:
                    case Business.TermType.MSO:
                    case Business.TermType.ComplexList:
                    case Business.TermType.PickList:
                    case Business.TermType.External:
                    case Business.TermType.PlaceHolderAttachments:
                        return true;

                    case Business.TermType.Date:
                    case Business.TermType.Renewal:
                    case Business.TermType.Facility:
                    case Business.TermType.None:
                    case Business.TermType.Link:
                    case Business.TermType.PlaceHolderComments:
                        return false;

                    default:
                        throw new Exception(string.Format("Term Type '{0}' not evaluated", _termType.ToString()));
                }
            }
        }

        public DataStoreField(string name, string alias, int? length, TermType termType, bool isStandard)
        {
            _name = name;
            _alias = alias;
            _length = length;
            _sortSequence = -1;
            _termType = termType;
            _templateName = GetTermTemplate(_name);
            _termName = GetTermName(_name);
            _fieldName = GetTermField(_name);
            _textTermFormat = Business.TextTermFormat.Plain;
            _isStandard = isStandard;
        }

        public DataStoreField(string name, string alias, int? length, TermType termType, TextTermFormat format, bool isStandard)
        {
            _name = name;
            _alias = alias;
            _length = length;
            _sortSequence = -1;
            _termType = termType;
            _templateName = GetTermTemplate(_name);
            _termName = GetTermName(_name);
            _fieldName = GetTermField(_name);
            _textTermFormat = format;
            _isStandard = isStandard;
        }

        public static string GetTermTemplate(string termConcatenatedName)
        {
            if (HasTermNameParts(termConcatenatedName))
                return termConcatenatedName.Split(_TERM_NAME_DELIMITER)[0];
            else
                return null;
        }

        public static string GetTermName(string termConcatenatedName)
        {
            string[] termParts = termConcatenatedName.Split(_TERM_NAME_DELIMITER);
            if (termParts.Length > 1)
                return termParts[1];
            else
                return termParts[0];
        }

        public static string GetHeaderName(string termConcatenatedName)
        {
            string termName = GetTermName(termConcatenatedName);
            string fieldName = GetTermField(termConcatenatedName);
            if (string.IsNullOrEmpty(fieldName))
                return termName;
            else
                return string.Format("{0}{1}{2}", termName, _TERM_NAME_DELIMITER, fieldName);
        }

        public static bool HasTermNameParts(string termConcatenatedName)
        {
            return termConcatenatedName.Split(_TERM_NAME_DELIMITER).Length > 1;
        }

        public static string GetTermField(string termConcatenatedName)
        {
            string[] termParts = termConcatenatedName.Split(_TERM_NAME_DELIMITER);
            if (termParts.Length == 3)
                return termParts[2];
            else
                return string.Empty;
        }

    }

    public class DataStoreFieldComparer : IEqualityComparer<DataStoreField>
    {

        public bool Equals(DataStoreField x, DataStoreField y)
        {
            if(Object.ReferenceEquals(x,y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            return false;

            return x.Name == y.Name && x.FieldName == x.FieldName;

        }

        public int GetHashCode(DataStoreField datastoreField)
        {

            if (Object.ReferenceEquals(datastoreField, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashDataStoreName = datastoreField.Name == null ? 0 : datastoreField.Name.GetHashCode();

            //Get hash code for the Code field.
            int hashDataStoreFieldName = datastoreField.FieldName.GetHashCode();

            //Calculate the hash code for the product.
            return hashDataStoreName ^ hashDataStoreFieldName;

        }
    }
}
