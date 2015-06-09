using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Web;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class ComplexListItemValue
	{
		#region private members
		private string _fieldName;
        private Guid _fieldID;
		private string _fieldValue;
		private bool _bigText;	//Used within the user interface
        private bool _removeBlank;	//Used within the user interface
        private bool _fieldIDDefined;
        private ComplexList _complexList;
        private Term _fieldFilterTerm;
        //Only for use during run time - do not store to xml:
        private string _guiDisplay = "Error - Value not set";
        private readonly bool _guiOverride = false;


		#endregion

		#region Properties

        public string FieldName
		{
            get {
                    if (_complexList == null)
                        return _fieldName;
                    if (string.IsNullOrEmpty(_fieldName))
                        return _complexList.FindField(_fieldID).Name;
                    else
                        return _fieldName;
                }
		}

        public Guid FieldID
        {
            get { return _fieldID; }
            set { 
                    _fieldID = value;
                    _fieldIDDefined = true;
                    _fieldName = null;
                }
        }
        
        public string FieldValue
		{
			get 
            {
                return Utility.XMLHelper.GetXMLText(_fieldValue); 
            }
			set 
            { 
                _fieldValue = Utility.XMLHelper.SetXMLText(value); 
            }
		}

        public bool DefaultValueDefined
        {
            get
            {
                Term fieldFilterTerm = _fieldFilterTerm.Copy();
                return fieldFilterTerm.Default ?? false;
            }
        }

        public string DefaultValue
        {
            get
            {
                Term fieldFilterTerm = _fieldFilterTerm.Copy();
                return fieldFilterTerm.DefaultValue;
            }
        }

        public string DisplayValue
        {
            get
            {
                if (_guiOverride)
                    return _guiDisplay;
                else
                {
                    Term fieldFilterTerm = _fieldFilterTerm.Copy();
                    fieldFilterTerm.SetValue(_fieldValue);
                    return fieldFilterTerm.DisplayValue(string.Empty);
                }
            }
        }

		public bool BigText
		{
			get { return _bigText; }
			set { _bigText = value; }
		}

        public bool RemoveBlank
        {
            get { return _removeBlank; }
            set { _removeBlank = value; }
        }

        public bool FieldIDDefined
        {
            get { return _fieldIDDefined; }
        }

        //This property is intended for handling the _fieldValue.
        public Term Term
        {
            get 
            {
                Term fieldFilterTerm = _fieldFilterTerm.Copy();
                fieldFilterTerm.SetValue(_fieldValue);
                return fieldFilterTerm; 
            }
            set 
            {
                if (value.TermType != _fieldFilterTerm.TermType)
                    throw new Exception(string.Format("Was expecting a term type of {0} but a term type of {1} was supplied.", _fieldFilterTerm.TermType.ToString(),value.TermType.ToString()));
                _fieldValue = value.GetValue(string.Empty);
            }
        }

        public TermType TermType
        {
            get
            {
                return _fieldFilterTerm.TermType;
            }
        }

        public Term FieldFilterTerm
        {
            get
            {
                return _fieldFilterTerm;
            }
        }


		#endregion

		#region Constructors

        public ComplexListItemValue(ComplexList complexList, Term fieldFilterTerm)
		{
            _complexList = complexList;
            _fieldFilterTerm = fieldFilterTerm;
        }

        //This call is used to display the ComplexListItemValue in the Summary page.
        public ComplexListItemValue(Guid fieldID, string fieldValue, ComplexList complexList, Term fieldFilterTerm, bool guiOverride)
		{
            _fieldID = fieldID;
            _fieldIDDefined = true;
            _complexList = complexList;
            _fieldFilterTerm = fieldFilterTerm;
            _guiOverride = guiOverride;
            if (_guiOverride)
                _guiDisplay = fieldValue;
            else
                _fieldValue = fieldValue;
        }

        public ComplexListItemValue(XmlNode node, ComplexList complexList)
		{
            _complexList = complexList;

            try
            {
                string fieldID = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_FieldID);
                _fieldID = new Guid(fieldID);
                _fieldIDDefined = true;
            }
            catch
            {
                _fieldIDDefined = false;
                try
                {
                    _fieldName = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_FieldName);
                }
                catch
                {
                }
            }

            _fieldFilterTerm = complexList.FindField(_fieldID).FilterTerm;
            _bigText = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_BigText) ?? false;
            _fieldValue = Utility.XMLHelper.GetText(node);
            _removeBlank = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_RemoveBlank) ?? false;
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			if (bValidate)
			{
			}

            Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_FieldID, _fieldID.ToString());
            Utility.XMLHelper.AddText(xmlDoc, node, _fieldValue);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, node,XMLNames._A_BigText, _bigText);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_RemoveBlank, _removeBlank);
		}

		#endregion

        public List<string> Validate(bool includeTab, string filterTermTabName)
        {
            Term fieldFilterTerm = _fieldFilterTerm.Copy();
            fieldFilterTerm.SetValue(_fieldValue);
            return fieldFilterTerm.Validate(includeTab, filterTermTabName);
        }

        public List<string> CheckType(bool includeTab, string filterTermTabName)
        {
            Term fieldFilterTerm = _fieldFilterTerm.Copy();
            fieldFilterTerm.SetValue(_fieldValue);
            return fieldFilterTerm.CheckType(includeTab, filterTermTabName);
        }

        public void Clear()
        {
            _fieldValue = null;
        }

        public void Migrate(ComplexListItemValue itemValue)
        {
            _fieldValue = itemValue._fieldValue;
        }

        //In the loader, we assume that the value provided in the excel file is already in the correct 'internal format'.  
        public void Load(string fieldValue)
        {
            _fieldValue = fieldValue;
        }
    }
}
