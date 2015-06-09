using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class ComplexListField
	{
		#region private members
		private string _name;
        private readonly Guid _id;      //This is the ID which will be referenced within the ComplexListItemValue
		private bool? _bigText;
		private bool? _summary;
		private bool? _removeBlank;
        private Term _filterTerm;
        private ComplexList _complexList;

		#endregion

		#region Properties

		public string Name
		{
			get { return Utility.XMLHelper.GetXMLText(_name); }
			set { _name = Utility.XMLHelper.SetXMLText(value); }
		}

        public Guid ID
        {
            get { return _id; }
        }

        public string EmbeddedID
        {
            get { return string.Format("{0}{1}", XMLNames._M_FieldID, _id.ToString().ToUpper()); }
        }

		public bool? BigText
		{
			get { return _bigText; }
			set { _bigText = value; }
		}

		public bool? Summary
		{
			get { return _summary; }
			set { _summary = value; }
		}

		public bool? RemoveBlank
		{
			get { return _removeBlank; }
			set { _removeBlank = value; }
		}

        public Term FilterTerm
        {
            get { return _filterTerm; }
            set 
            {
                if (value != null)
                {
                    CheckTermType(value.TermType);
                    if (!value.IsFilter)
                    {
                        throw new Exception("Term must be set to IsFilter");
                    }
                    if (value.TermType == TermType.PickList && (value as PickListTerm).MultiSelect.HasValue && (value as PickListTerm).MultiSelect.Value)
                        throw new Exception("PickList Term must be set to single select");
                    //If the termtype changes, null out the current values.
                    if (_filterTerm != null && _filterTerm.TermType != value.TermType)
                        _complexList.ClearField(ID);
                }
                _filterTerm = value;
            }
        }

        public string TermTypeName
        {
            get { return _filterTerm.TermType.ToString(); }
        }

        public ComplexList ComplexList
        {
            get { return _complexList; }
        }

        public bool DefaultValueDefined
        {
            get { return _filterTerm.Default ?? false; }
        }

        public string DefaultValue
        {
            get { return _filterTerm.DefaultValue; }
        }

		#endregion

		#region Constructors

        public ComplexListField(ComplexList complexList)
		{
            _id = Guid.NewGuid();
            _complexList = complexList;
		}

        public ComplexListField(XmlNode node, ComplexList complexList)
		{
			_name = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Name);
            _complexList = complexList;

            _id = Guid.Empty;
            string id = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_ID);
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    _id = new Guid(id);
                }
                catch
                {
                }
            }
            //If an ID was not assigned before, assign it now....
            if (_id == Guid.Empty)
                _id = Guid.NewGuid();

			_bigText = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_BigText);
			_summary = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_Summary);
			_removeBlank = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_RemoveBlank);

            //Try to retrieve the Term info, if there.  If not, assign a Text Term by default.
            _filterTerm = null;
            if (node.HasChildNodes)
            {
                XmlNode termNode = node.FirstChild;
                TermType termType = (TermType)Enum.Parse(typeof(TermType), termNode.Name.ToString());
                CheckTermType(termType);
                switch (termType)
                {
                    case TermType.Text:
                        _filterTerm = new TextTerm(termNode, null, true);
                        break;

                    case TermType.Date:
                        _filterTerm = new DateTerm(termNode, null, true);
                        break;

                    case TermType.PickList:
                        _filterTerm = new PickListTerm(termNode, null, true);
                        break;
                }
            }
            else
            {
                _filterTerm = new TextTerm(false, null, true);
                TextTerm textTerm = _filterTerm as TextTerm;
                textTerm.Format = TextTermFormat.Plain;
            }
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			if (bValidate)
			{
				Utility.XMLHelper.ValidateString(XMLNames._A_Name, _name);
			}

			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Name, _name);
            Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_ID, _id.ToString());
            Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_BigText, _bigText);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_Summary, _summary);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_RemoveBlank, _removeBlank);

            CheckTermType(_filterTerm.TermType);

            XmlElement xmlTerm = null;
            switch (_filterTerm.TermType)
            {
                case TermType.Text:
                    xmlTerm = xmlDoc.CreateElement(XMLNames._E_Text);
                    (_filterTerm as TextTerm).Build(xmlDoc, xmlTerm, bValidate);
                    break;

                case TermType.Date:
                    xmlTerm = xmlDoc.CreateElement(XMLNames._E_Date);
                    (_filterTerm as DateTerm).Build(xmlDoc, xmlTerm, bValidate);
                    break;

                case TermType.PickList:
                    xmlTerm = xmlDoc.CreateElement(XMLNames._E_PickList);
                    (_filterTerm as PickListTerm).Build(xmlDoc, xmlTerm, bValidate);
                    break;
            }
            node.AppendChild(xmlTerm);
		}

		#endregion

        #region Private Methods

        private void CheckTermType(TermType termType)
        {
            if (!ValidTermTypes().Contains(termType))
                throw new Exception(string.Format("TermType {0} is not valid for a ComplexListField", _filterTerm.TermType.ToString()));
        }

        #endregion

        #region Public Static Methods

        public static List<TermType> ValidTermTypes()
        {
            List<TermType> termTypes = new List<TermType>();
            termTypes.Add(TermType.Text);
            termTypes.Add(TermType.Date);
            termTypes.Add(TermType.PickList);

            return termTypes;
        }

        public static bool IsEmbeddedID(string sField)
        {
            return sField.IndexOf(XMLNames._M_FieldID) >= 0;
        }

        public static string FilterEmbeddedID(string sField)
        {
            return sField.Replace(XMLNames._M_FieldID, "");
        }

        //For each term in the text, replace the FieldName (if found) with the FieldID
        public static string EmbedFieldIDs(ComplexList complexList, string sText)
        {
            //match anything of the form <img...src="TextImage.ashx?text="...".../> in _text
            if (!string.IsNullOrEmpty(sText))
            {
                MatchCollection matches = Regex.Matches(sText, XMLNames._M_FieldImageTemplate);
                foreach (Match match in matches)
                {
                    string fieldName = match.Groups[1].Value;
                    ComplexListField field = null;
                    string sFieldName = fieldName.Replace("&quot;", "\"");
                    if (!IsEmbeddedID(sFieldName))
                    {
                        field = complexList.FindField(sFieldName);
                        if (field != null)
                        {
                            //Replace <img ... > with term value
                            string matchedText = match.Value;
                            string replacementText = "";
                            replacementText = matchedText.Replace(fieldName, field.EmbeddedID);
                            sText = sText.Replace(matchedText, replacementText);
                        }
                        else
                            //Need to check for 'reserved term names'
                            if (!sFieldName.StartsWith("*"))
                                throw new Exception(string.Format("Unable to locate term named '{0}' when substituting terms", sFieldName));
                    }
                }
            }
            return sText;
        }

        //For each term in the text, replace the FieldID (if found) with the FieldName
        public static string EmbedFieldNames(ComplexList complexList, string sText)
        {
            //match anything of the form <img...src="TextImage.ashx?text="...".../> in _text
            if (!string.IsNullOrEmpty(sText))
            {
                MatchCollection matches = Regex.Matches(sText, XMLNames._M_FieldImageTemplate);
                foreach (Match match in matches)
                {
                    string sFieldName = match.Groups[1].Value;
                    ComplexListField field = null;
                    if (IsEmbeddedID(sFieldName))
                    {
                        field = complexList.FindField(new Guid(FilterEmbeddedID(sFieldName)));
                        if (field != null)
                        {
                            //Replace <img ... > with field value
                            string matchedText = match.Value;
                            string replacementText = "";
                            replacementText = matchedText.Replace(sFieldName, field.Name);
                            sText = sText.Replace(matchedText, replacementText);
                        }
                    }
                }
            }
            return sText;
        }

        #endregion
    }
}
