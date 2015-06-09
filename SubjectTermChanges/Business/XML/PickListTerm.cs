using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class PickListTerm : Term
	{
		#region private members
		private bool? _multiSelect;
		private List<PickListItem> _pickListItems;
		private string _requiredSelectedValue;
		private bool? _useTextNumberFormat;
		#endregion

		#region Properties

		public bool? MultiSelect
		{
			get { return _multiSelect; }
			set { _multiSelect = value; }
		}

		public List<PickListItem> PickListItems
		{
			get { return _pickListItems; }
			set { _pickListItems = value; }
		}

        public override string Keyword
		{
			get
			{
				if (!HasKeyWord)
					return null;
				string sKeyWord = "";
				if (Editable ?? false)
				{
					foreach (PickListItem pickListItem in _pickListItems)
					{
						if (pickListItem.Selected ?? false)
							sKeyWord = string.Concat(sKeyWord, pickListItem.Value, XMLNames._M_Delimiter);
					}
				}
				else
				{
					sKeyWord = DefaultValue;
				}
				if (sKeyWord == null)
					return null;
				return sKeyWord.Length > 0 ? sKeyWord.Trim(XMLNames._M_Delimiter) : null;
			}
		}


		public string RequiredSelectedValue
		{
			get { return _requiredSelectedValue; }
			set { _requiredSelectedValue = value; }
		}

		public bool? UseTextNumberFormat
		{
			get { return _useTextNumberFormat; }
			set { _useTextNumberFormat = value; }
		}

        public override int? RequiredSize
        {
            get 
            {
                if (_pickListItems == null)
                    return null;
                int requiredSize = 0;
                foreach (PickListItem pickListItem in _pickListItems)
                {
                    if (pickListItem.Value.Length > requiredSize)
                        requiredSize = pickListItem.Value.Length;
                }
                return requiredSize; 
            }
        }

		#endregion

		#region Constructors

        public PickListTerm(bool systemTerm, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
			TermType = TermType.PickList;
			NameRequired = true;
			_pickListItems = new List<PickListItem>();
		}

        public PickListTerm(XmlNode termNode, Template template, bool isFilter)
            : base(termNode, template, isFilter)
		{
			TermType = TermType.PickList;
			NameRequired = true;
			_pickListItems = new List<PickListItem>();

			_multiSelect = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_MultiSelect);
			_requiredSelectedValue = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_RequiredSelectedValue);
			_useTextNumberFormat = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_UseTextNumberFormat);

            XmlNodeList pickListItemNodes = termNode.SelectNodes(XMLNames._E_ListItem);
			foreach (XmlNode pickListItemNode in pickListItemNodes)
			{
				PickListItem pickListItem = new PickListItem(pickListItemNode);
				//@@@ RR 05/06/2008:  Commented out to eliminate erroneous display of the DefaultValue on the Summary Page
				//if (!(this.Editable ?? false) && (pickListItem.Value == this.DefaultValue))
				//	pickListItem.Selected = true;
				_pickListItems.Add(pickListItem);
			}
		}

		#endregion

		#region Build XML

		public override void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			string selectedValue = DisplayValue(XMLNames._TPS_None);
			base.Build(xmlDoc, termNode, bValidate);

			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_RequiredSelectedValue, _requiredSelectedValue);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_MultiSelect, _multiSelect);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_UseTextNumberFormat, _useTextNumberFormat);

			//Pull from list of PickListItem's to build sub nodes
			foreach (PickListItem pickListItem in _pickListItems)
			{
                XmlNode elementPickListItem = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_ListItem, XMLNames._M_NameSpaceURI);
				pickListItem.Build(xmlDoc, elementPickListItem, bValidate);
				termNode.AppendChild(elementPickListItem);
			}
		}

		#endregion


		#region public methods

		//Predicate method (used in the List<> methods such as Find, FindAll, Exists, Index, etc.)
		public PickListItem FindItem(string value)
		{
			return FindItem(_pickListItems, value);
		}

		public static bool Contains(List<PickListItem> pickListItems, string value)
		{
			return FindItem(pickListItems, value) != null;
		}

		public static PickListItem FindItem(List<PickListItem> pickListItems, string value)
		{
			for (int i = 0, j = pickListItems.Count; i < j; i++)
				if (pickListItems[i].Value == value)
					return pickListItems[i];
			//if no match found, return null
			return null;
		}

		public void DeselectAll()
		{
			for (int i = 0, j = _pickListItems.Count; i < j; i++)
				_pickListItems[i].Selected = false;
		}

		#endregion


		private List<string> GetSelectedValues()
		{
			List<string> values = new List<string>();
			foreach (PickListItem pickListItem in PickListItems)
				if (pickListItem.Selected ?? false)
					values.Add(pickListItem.Value);
			if (Runtime.SetValue != null)
			{
				Clear();
				values.Clear();
				foreach (PickListItem pickListItem in PickListItems)
					if (Runtime.SetValue == pickListItem.Value)
					{
						pickListItem.Selected = true;
						values.Add(pickListItem.Value);
						break;
					}
			}
			return values;
		}


		public override string DisplayValue(string termPartSpecifier)
		{
			List<string> selectedValues = GetSelectedValues();
			switch (selectedValues.Count)
			{
				case 0:
					return string.Empty;
				case 1:
					return FormattedValue(selectedValues[0]);
				case 2:
					return FormattedValue(selectedValues[0]) + " and " + FormattedValue(selectedValues[1]);
				default:
					System.Text.StringBuilder sb = new StringBuilder();
					for (int i = 0; i < selectedValues.Count - 1; i++)
					{
						sb.Append(FormattedValue(selectedValues[i]));
						sb.Append(", ");
					}
					sb.Append("and ");
					sb.Append(FormattedValue(selectedValues[selectedValues.Count - 1]));
					return sb.ToString() ;
			}
		}


		//Apply Number (##) formatting to numeric selected values, if the UseTextNumberFormat property is true
		private string FormattedValue(string unformattedValue)
		{
			int n;
			if ((_useTextNumberFormat ?? false) && (int.TryParse(unformattedValue, out n)))
				return Utility.TextHelper.TextPlusNumber(n);
			else
				return unformattedValue;
		}


        public override List<string> Validate(bool includeTab, string filterTermTabName)
		{
			List<string> rtn = new List<string>();
            string tabString = IsFilter ? filterTermTabName : TermGroupName;

			string selectedValue = DisplayValue(XMLNames._TPS_None);
			if (string.IsNullOrEmpty(selectedValue))
				selectedValue = "nothing";

			if (Runtime.Required)
			{
				if (string.IsNullOrEmpty(this.DisplayValue(XMLNames._TPS_None)))
				{
                    rtn.Add(string.Format("\"{0}\"{1} is a required term, but no value was selected.", this.Name, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
				}
				else
				{
					if (!string.IsNullOrEmpty(RequiredSelectedValue))
					{
						bool requiredValueSelected = false;
						foreach (PickListItem pickListItem in PickListItems)
						{
							if ((pickListItem.Selected ?? false) && (pickListItem.Value == RequiredSelectedValue))
							{
								requiredValueSelected = true;
								break;
							}
						}
						if (!requiredValueSelected)
						{
							string verb = (selectedValue.Contains("and") ? "were" : "was");
                            rtn.Add(string.Format("\"{0}\"{4} requires that {1} be selected, but {2} {3} selected.", Name, RequiredSelectedValue, selectedValue, verb, includeTab ? string.Format(" (tab {0})", tabString) : string.Empty));
						}
					}
				}
			}

			return rtn;
		}


        public override List<string> CheckType(bool includeTab, string filterTermTabName)
        {
            return null;
        }

		public override bool EmitPDFXML(XmlWriter writer)
		{
			PdfHelper.AddPDFXMLHeader(writer);

			List<string> selectedValues = GetSelectedValues();
			if (selectedValues.Count > 0)
			{
				bool bFirstOne = true;
				foreach (string value in selectedValues)
				{
					if (bFirstOne)
						PdfHelper.AddPDFXMLRow(writer, Name, FormattedValue(value));
					else
						PdfHelper.AddPDFXMLRow(writer, "", FormattedValue(value));
					bFirstOne = false;
				}
			}
			else
			{
				PdfHelper.AddPDFXMLRow(writer, Name, "");
			}
			PdfHelper.AddPDFXMLFooter(writer);
			return true;
		}

        public override Term RetroCopy(bool systemTerm, Template template)
        {
            PickListTerm pickListTerm = new PickListTerm(systemTerm, template, false);
            CopyBase(pickListTerm, template);

            pickListTerm._multiSelect = _multiSelect;
            pickListTerm._requiredSelectedValue = _requiredSelectedValue;
            pickListTerm._useTextNumberFormat = _useTextNumberFormat;
            pickListTerm._pickListItems = new List<PickListItem>(_pickListItems.Count);
            foreach (PickListItem pickListItem in _pickListItems)
                pickListTerm._pickListItems.Add(pickListItem.Copy());

            return pickListTerm;
        }

        public override Term Copy()
        {
            PickListTerm pickListTerm = new PickListTerm(SystemTerm, _template, IsFilter);
            CopyBase(pickListTerm, _template);

            pickListTerm._multiSelect = _multiSelect;
            pickListTerm._requiredSelectedValue = _requiredSelectedValue;
            pickListTerm._useTextNumberFormat = _useTextNumberFormat;
            pickListTerm._pickListItems = new List<PickListItem>(_pickListItems.Count);
            foreach (PickListItem pickListItem in _pickListItems)
                pickListTerm._pickListItems.Add(pickListItem.Copy());

            return pickListTerm;
        }

		public override void SetDefaultValue()
		{
			if (this.Default ?? false)
				foreach (PickListItem pickListItem in _pickListItems)
					pickListItem.Selected = (pickListItem.Value == DefaultValue);
			else
				foreach (PickListItem pickListItem in _pickListItems)
					pickListItem.Selected = false;
		}

        public override void Clear()
		{
			foreach (PickListItem pickListItem in _pickListItems)
				pickListItem.Selected = false;
		}

        public override string TestValue(string termName, string tabMessage, string value)
        {
            return string.Empty;
        }

        public override void SetValue(string value)
        {
            Clear();
            foreach (PickListItem pickListItem in _pickListItems)
            {
                if (pickListItem.Value.Equals(value))
                {
                    pickListItem.Selected = true;
                    break;
                }
            }
        }

        public override string GetValue(string defaultValue)
        {
            foreach (PickListItem pickListItem in _pickListItems)
            {
                if (pickListItem.Selected ?? false)
                {
                    return pickListItem.Value;
                }
            }
            return defaultValue;
        }

        public override void Migrate(Term term)
        {
            if (term == null)
                base.Migrate(term);
            if (!(term is PickListTerm))
                return;
            else
            {
                //Initialize all pickListItem's to not be selected
                Clear();
                //Determine which source PickListTerm is currently selected.  Keep that selection if possible.
                List<PickListItem> sourcePickListItems = (term as PickListTerm).PickListItems;
                foreach (PickListItem sourcePickListItem in sourcePickListItems)
                {
                    PickListItem destPickListItem = null;
                    if (sourcePickListItem.Selected ?? false)
                    {
                        destPickListItem = FindItem(sourcePickListItem.Value);
                        if (destPickListItem != null)
                            destPickListItem.Selected = true;
                    }
                }
                term.Runtime.Migrated = true;
            }
            Runtime.Migrated = true;
        }

        public override void Load(string value, string pattern, char? delimiter)
        {
            if (!string.IsNullOrEmpty(value))
            {
                DeselectAll();
                string[] selections = null;
                if (_multiSelect ?? false)
                {
                    selections = value.Split(delimiter.Value);
                }
                else
                {
                    selections = new string[] { value };
                }
                foreach (string selection in selections)
                {
                    foreach (PickListItem item in _pickListItems)
                    {
                        if (item.Value.Equals(selection))
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        public static TermStore CreateStore(string termName, XmlReader reader)
        {
            using (reader)
            {
                TermStore termStore = new TermStore(termName, TermType.PickList);
                bool abort = false;
                reader.Read();
                reader.Read();

                ReadPastSubTree(reader, XMLNames._E_validateOn);

                int loopCount = 0;
                while (!abort)
                {
                    if (loopCount++ > TermStore.maxLoopCount)
                    {
                        throw new Exception(string.Format("CreateStore stopped at loopCount {0:D} for PickList term '{1}'", loopCount, termName));
                    }
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            abort = true;
                            break;

                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case XMLNames._E_ListItem:
                                    string selected = reader.GetAttribute(XMLNames._A_Selected);
                                    string value = Utility.XMLHelper.SafeReadElementString(reader);
                                    if (selected.Equals("True", StringComparison.OrdinalIgnoreCase))
                                    {
                                        termStore.AddMultiValue(value);
                                    }
                                    break;

                                default:
                                    reader.Read();
                                    break;
                            }
                            break;

                        default:
                            reader.Read();
                            break;
                    }
                }
                return termStore;
            }
        }

	}
}
