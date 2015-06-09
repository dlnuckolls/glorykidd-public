using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class ComplexList : Term
	{
		#region private members
		private int? _columnCount;
		private string _rendering;
		private bool? _showOnItemSummary;
		private string _standardHeader;
		private string _alternateHeader;
		private List<ComplexListField> _fields;
		private List<ComplexListItem> _items;


		#endregion

		#region Properties


		public int? ColumnCount
		{
			get { return _columnCount; }
			set { _columnCount = value; }
		}

		public string Rendering
		{
			get { return Utility.XMLHelper.GetXMLText(_rendering); }
			set { _rendering = Utility.XMLHelper.SetXMLText(value); }
		}

		public bool ShowOnItemSummary
		{
		    get { return _showOnItemSummary ?? false; }  // default value is false
		    set { _showOnItemSummary = value; }
		}

		public string StandardHeader
		{
			get { return Utility.XMLHelper.GetXMLText(_standardHeader); }
			set { _standardHeader = Utility.XMLHelper.SetXMLText(value); }
		}

		public string AlternateHeader
		{
			get { return Utility.XMLHelper.GetXMLText(_alternateHeader); }
			set { _alternateHeader = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Header
		{
			get
			{
				if (SelectedItems > 0)
					return _standardHeader;
				else
					return _alternateHeader;
			}
		}

		public List<ComplexListField> Fields
		{
			get { return _fields; }
		}

		public List<ComplexListItem> Items
		{
			get { return _items; }
			set { _items = value; }
		}

		public bool ItemsDefined
		{
			get { return _items.Count > 0; }
		}

		public int SelectedItems
		{
			get
			{
				int rtn = 0;
				foreach (ComplexListItem item in _items)
					if (item.Selected ?? false)
						rtn++;
				return rtn;
			}
		}

		public bool HasBigText
		{
			get {
					if (_fields != null)
						foreach (ComplexListField field in _fields)
							if (field.BigText ?? false)
								return true;
					return false; 
				}
		}

		public override string Keyword
		{
			get
			{
				if (!HasKeyWord)
					return null;
				//Create a 'Keyword' which consists of all the default values concatenated together
				string keyWord = "";
				if (_items != null)
				{
					foreach (ComplexListItem item in _items)
					{
						foreach (ComplexListItemValue itemValue in item.ItemValues)
						{
                            keyWord = string.Concat(keyWord, XMLNames._M_Delimiter, itemValue.DisplayValue);
						}
					}
				}
				return keyWord.Length > 0 ? keyWord.Trim(XMLNames._M_Delimiter) : null;
			}
		}

        public override string[] StoreColumns
        {
            get
            {
                List<string> columns = new List<string>();
                foreach (ComplexListField complexListField in Fields)
                {
                    columns.Add(complexListField.Name);
                }
                return columns.ToArray();
            }
        }

		#endregion

		#region Constructors

        public ComplexList(bool systemTerm, Template template, bool retroCopy, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
			TermType = TermType.ComplexList;
			NameRequired = true;
			_fields = new List<ComplexListField>();
			_items = new List<ComplexListItem>();

            if (template != null && !retroCopy)
            {
                if (template.SecurityModel == SecurityModel.Advanced)
                    TermGroupID = template.AddTermGroup("Complex List", string.Empty, template.SecurityModel, TermGroup.TermGroupType.AdvancedComplexList);
                else
                    TermGroupID = template.BasicSecurityTermGroupID;
            }
        }

        public ComplexList(XmlNode node, Template template, bool isFilter)
            : base(node, template, isFilter)
		{
			TermType = TermType.ComplexList;
			NameRequired = true;

			_columnCount = Utility.XMLHelper.GetAttributeInt(node, XMLNames._A_ColumnCount);
			_showOnItemSummary = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_ShowOnItemSummary);

			_fields = new List<ComplexListField>();
			XmlNodeList nodeFields = node.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Fields, XMLNames._E_Field));
			if (nodeFields != null)
			{
				foreach (XmlNode nodeField in nodeFields)
				{
					ComplexListField field = new ComplexListField(nodeField, this);
					_fields.Add(field);
				}
			}

			_items = new List<ComplexListItem>();
			XmlNodeList nodeItems = node.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Items, XMLNames._E_Item));
			if (nodeItems != null)
			{
				foreach (XmlNode nodeItem in nodeItems)
				{
					ComplexListItem item = new ComplexListItem(nodeItem, this);
					//Set the BigText value stored within each itemValue - for use by the interface
					foreach (ComplexListItemValue itemValue in item.ItemValues)
					{
                        if (!itemValue.FieldIDDefined)
                        {
                            itemValue.BigText = FindField(itemValue.FieldName).BigText ?? false;
                            itemValue.RemoveBlank = FindField(itemValue.FieldName).RemoveBlank ?? false;
                        }
                        else
                        {
                            itemValue.BigText = FindField(itemValue.FieldID).BigText ?? false;
                            itemValue.RemoveBlank = FindField(itemValue.FieldID).RemoveBlank ?? false;
                        }
					}
					_items.Add(item);
				}
			}

            foreach (ComplexListItem item in _items)
            {
                foreach (ComplexListItemValue itemValue in item.ItemValues)
                {
                    if (!itemValue.FieldIDDefined)
                    {
                        itemValue.FieldID = FindField(itemValue.FieldName).ID;
                    }
                }
            }

            XmlNode nodeRendering = node.SelectSingleNode(XMLNames._E_Rendering);
            if (nodeRendering != null)
            {
                _rendering = Utility.XMLHelper.GetText(nodeRendering);
                _rendering = ComplexListField.EmbedFieldNames(this, _rendering);
            }

            XmlNode nodeStandardHeader = node.SelectSingleNode(XMLNames._E_StandardHeader);
            if (nodeStandardHeader != null)
            {
                _standardHeader = Utility.XMLHelper.GetText(nodeStandardHeader);
                _standardHeader = ComplexListField.EmbedFieldNames(this, _standardHeader);
            }

            XmlNode nodeAlternateHeader = node.SelectSingleNode(XMLNames._E_AlternateHeader);
            if (nodeAlternateHeader != null)
            {
                _alternateHeader = Utility.XMLHelper.GetText(nodeAlternateHeader);
                _alternateHeader = ComplexListField.EmbedFieldNames(this, _alternateHeader);
            }
		}

		#endregion

		#region Build XML

		public override void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			if (bValidate)
			{
				Utility.XMLHelper.ValidateString(XMLNames._A_Name, Name);
				Utility.XMLHelper.ValidateString(XMLNames._E_Rendering, _rendering);
			}

			base.Build(xmlDoc, node, bValidate);

			Utility.XMLHelper.AddAttributeInt(xmlDoc, node, XMLNames._A_ColumnCount, _columnCount);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_ShowOnItemSummary, _showOnItemSummary);

			XmlNode elementRendering = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Rendering, XMLNames._M_NameSpaceURI);
            string rendering = ComplexListField.EmbedFieldIDs(this, _rendering);
            Utility.XMLHelper.SetText(xmlDoc, elementRendering, rendering);
			node.AppendChild(elementRendering);

            XmlNode elementStandardHeader = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_StandardHeader, XMLNames._M_NameSpaceURI);
            Utility.XMLHelper.SetText(xmlDoc, elementStandardHeader, _standardHeader);
            node.AppendChild(elementStandardHeader);

            XmlNode elementAlternateHeader = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_AlternateHeader, XMLNames._M_NameSpaceURI);
            Utility.XMLHelper.SetText(xmlDoc, elementAlternateHeader, _alternateHeader);
            node.AppendChild(elementAlternateHeader);

			XmlNode elementFields = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Fields, XMLNames._M_NameSpaceURI);

			if (_fields != null)
			{
				foreach (ComplexListField field in _fields)
				{
					XmlNode elementField = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Field, XMLNames._M_NameSpaceURI);
					field.Build(xmlDoc, elementField, bValidate);
					elementFields.AppendChild(elementField);
				}
			}

			node.AppendChild(elementFields);

			XmlNode elementItems = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Items, XMLNames._M_NameSpaceURI);
			if (_items != null)
			{
				foreach (ComplexListItem item in _items)
				{
					XmlNode elementItem = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Item, XMLNames._M_NameSpaceURI);
					item.Build(xmlDoc, elementItem, bValidate);
					elementItems.AppendChild(elementItem);
				}
			}
			node.AppendChild(elementItems);

		}

        public bool AddFieldToCurrentItems(Guid fieldID, bool bigText, Term fieldFilterTerm)
		{
			foreach (ComplexListItem item in _items)
			{
				if (item.ItemValues != null)
				{
                    ComplexListItemValue itemValue = new ComplexListItemValue(this, fieldFilterTerm);
                    itemValue.FieldID = fieldID;
					itemValue.BigText = bigText;
					item.ItemValues.Add(itemValue);
				}
			}
			return true;
		}

        public bool DeleteItemValue(Guid fieldID)
        {
            foreach (ComplexListItem item in _items)
            {
                if (item.ItemValues != null)
                {
                    item.ItemValues.RemoveAll(iv => iv.FieldID == fieldID);
                }
            }
            return true;
        }

		#endregion

        public override Term RetroCopy(bool systemTerm, Template template)
        {
            ComplexList complexList = new ComplexList(systemTerm, template, true, false);
            CopyBase(complexList, template);

            complexList._columnCount = _columnCount;
            complexList._rendering = _rendering;
            complexList._showOnItemSummary = _showOnItemSummary;
            complexList._standardHeader = _standardHeader;
            complexList._alternateHeader = _alternateHeader;
            complexList._fields = new List<ComplexListField>(_fields);
            complexList._items = new List<ComplexListItem>();

            return complexList;
        }

        public override string DisplayValue(string termPartSpecifier)
		{
			string rtn = string.Empty;
			if (Items.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				foreach (ComplexListItem item in this.Items)
				{
					sb.Append(SubstituteTerms(item));
				}
				rtn = sb.ToString();
			}
			return rtn;
		}

		public string ItemDisplayValue(ComplexListItem item)
		{
			string rtn = SubstituteTerms(item);
			return rtn;
		}


		private string TransformParagraphs(string html)
		{
			//Remove any <p></p> tag pairs, leaving the contents between the tags.   (This may be temporary code.)
			const string pattern = @"<p (.+?)>(.+?)</p>";
			string text = HttpUtility.HtmlDecode(html);
			if (!string.IsNullOrEmpty(html))
			{
				MatchCollection matches = Regex.Matches(html, pattern);
				while (matches.Count > 0)
				{
					Match match = matches[0];
					string matchedText = match.Result("$0");
					string replacementText = match.Groups[2].Value;
					html = HttpUtility.HtmlDecode(text).Replace(matchedText, replacementText);
					matches = Regex.Matches(html, pattern);
				}
			}
			return html;
		}


		private string SubstituteTerms(ComplexListItem item)
		{
			//match anything of the form <img...src="TextImage.ashx?text="...".../> in _text
			string text = HttpUtility.HtmlDecode(this.Rendering);
			if (!string.IsNullOrEmpty(text))
			{
				MatchCollection matches = Regex.Matches(text, XMLNames._M_TermImageTemplate);
				foreach (Match match in matches)
				{
					string matchedText = match.Result("$0");
					string fieldName = match.Groups[1].Value;
					string replacementText = SubstituteItemValue(item, fieldName);
					text = HttpUtility.HtmlDecode(text).Replace(matchedText, replacementText);
				}
			}
			return text;
		}

		private string SubstituteItemValue(ComplexListItem item, string fieldName)
		{
			try
			{

				for (int i = 0; i < item.ItemValues.Count; i++)
					if (this.Fields[i].Name == fieldName)
                        if (item.ItemValues[i].RemoveBlank)
                            if (string.IsNullOrEmpty(item.ItemValues[i].DisplayValue))
                                return string.Empty;
                            else
                                return item.ItemValues[i].DisplayValue;
                        else
                            return item.ItemValues[i].DisplayValue;
				return string.Empty;
			}
			catch
			{
				return string.Empty;
			}
		}

        public override List<string> Validate(bool includeTab, string filterTermTabName)
		{
            List<string> errors = new List<string>();

            foreach (ComplexListItem item in _items)
            {
                foreach (ComplexListItemValue itemValue in item.ItemValues)
                {
                    errors.AddRange(itemValue.Validate(includeTab, Name));
                }
            }
            return errors;
		}

        public override List<string> CheckType(bool includeTab, string filterTermTabName)
        {
            List<string> errors = new List<string>();

            foreach (ComplexListItem item in _items)
            {
                foreach (ComplexListItemValue itemValue in item.ItemValues)
                {
                    errors.AddRange(itemValue.CheckType(includeTab, filterTermTabName));
                }
            }
            return errors;
        }

		public override bool EmitPDFXML(XmlWriter writer)
		{
			PdfHelper.AddPDFXMLHeader(writer);
			string complexListName = Name;
			bool firstRowWritten = false;
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].Selected ?? false)
				{
					if (firstRowWritten)
						PdfHelper.AddListRowSeparator(writer);
					foreach (ComplexListItemValue itemValue in Items[i].ItemValues)
					{
                        PdfHelper.AddPDFXMLRow(writer, complexListName, string.Concat(itemValue.FieldName, ":"), itemValue.DisplayValue);
						complexListName = string.Empty;
						firstRowWritten = true;
					}
				}
			}
			if (!firstRowWritten)
				PdfHelper.AddPDFXMLRow(writer, complexListName, string.Empty, string.Empty);
			PdfHelper.AddPDFXMLFooter(writer);
			return true;
		}

        public ComplexListField FindField(string fieldName)
		{
			Predicate<ComplexListField> p = delegate(ComplexListField clf) { return (clf.Name == fieldName); };
			return _fields.Find(p);
		}

        public ComplexListField FindField(Guid fieldID)
        {
            Predicate<ComplexListField> p = delegate(ComplexListField clf) { return (clf.ID == fieldID); };
            return _fields.Find(p);
        }

        //Returns true if the suggested field name is already in use by another field
        public bool FieldNameInUse(string fieldName, Guid fieldID)
        {
            if (string.IsNullOrEmpty(fieldName))
                return false;
            //The FindIndex search stops on the first match.  If there are more than one
            //match, and the search happened to stop on the current index, then the other match
            //would not be detected.  Therefore check for >= 2 matches first.
            List<ComplexListField> fieldMatches = Fields.FindAll(f => f.Name.Equals(fieldName));
            if (fieldMatches == null)
                return false;
            if (fieldMatches.Count == 0)
                return false;
            if (fieldMatches.Count >= 2)
                return true;

            return !fieldMatches[0].ID.Equals(fieldID);
        }


		public override void SetDefaultValue()
		{
			//nothing
		}

		public static string NewTermName(Template template)
		{
			//TODO:  new name should be of the form "New Complex List" or "New Complex List (x)" where x >= 2
			string baseNewName = "New Complex List";
			string newName = baseNewName;
			int termNameSuffixIndex = 2;
			while (template.FindComplexList(newName) != null)
				newName = string.Format("{0} ({1})", baseNewName, termNameSuffixIndex++);
			return newName;
		}

        public override void Clear()
		{
            _items = new List<ComplexListItem>();
		}

        public void ClearField(Guid fieldID)
        {
            foreach (ComplexListItem item in _items)
            {
                foreach (ComplexListItemValue itemValue in item.ItemValues)
                {
                    if (itemValue.FieldID.Equals(fieldID))
                    {
                        itemValue.Clear();
                        break;
                    }
                }
            }
        }

        public static bool ModifyItems(ComplexList sourceTerm, ComplexList destTerm)
        {
            return destTerm.Items.Count == 0 && sourceTerm.Items.Count > 0;
        }

        public override void Migrate(Term term)
        {
            if (term == null)
                base.Migrate(term);
            if (!(term is ComplexList))
                return;
            else
            {
                ComplexList sourceTerm = term as ComplexList;
                //Any Items found within the destination term ComplexList are considered Default Items.  If there are no Default Items, then
                //migrate the sourceTerm ComplexList Items.  If there are Default Items, then do not migrate the source term ComplexList Items.
                if (ModifyItems(sourceTerm, this))
                {
                    //Migrate the sourceTerm Items.  First, create a mapping of FieldID's.  Match source to destination based on ID, then by name.
                    Dictionary<Guid /*destCLFieldID*/, ComplexListField /*sourceCLField*/> fieldMapping = new Dictionary<Guid, ComplexListField>();
                    foreach (ComplexListField destCLField in Fields)
                    {
                        ComplexListField sourceCLField = sourceTerm.FindField(destCLField.ID);
                        if (sourceCLField == null)
                            sourceCLField = sourceTerm.FindField(destCLField.Name);
                        if (sourceCLField != null)
                            fieldMapping.Add(destCLField.ID, sourceCLField);
                        else
                            fieldMapping.Add(destCLField.ID, destCLField);
                    }

                    foreach (ComplexListItem sourceCLItem in sourceTerm.Items)
                    {
                        ComplexListItem destCLItem = new ComplexListItem(sourceCLItem);
                        foreach (ComplexListField destCLField in Fields)
                        {
                            ComplexListItemValue destCLItemValue = new ComplexListItemValue(null, destCLField.FilterTerm);
                            destCLItemValue.BigText = fieldMapping[destCLField.ID].BigText ?? false;
                            destCLItemValue.RemoveBlank = fieldMapping[destCLField.ID].RemoveBlank ?? false;
                            destCLItemValue.FieldID = destCLField.ID;
                            ComplexListItemValue sourceCLItemValue = sourceCLItem.FindItemValue(fieldMapping[destCLField.ID].ID);
                            if (sourceCLItemValue != null)
                                destCLItemValue.Migrate(sourceCLItemValue);
                            else
                                destCLItemValue.Clear();
                            destCLItem.ItemValues.Add(destCLItemValue);
                        }
                        Items.Add(destCLItem);
                    }
                }
                sourceTerm.Runtime.Migrated = true;
            }
            Runtime.Migrated = true;
        }

        public void Load(Dictionary<string /*FieldName*/, string /*FieldValue*/> fieldValues)
        {
            if (fieldValues != null)
            {
                ComplexListItem item = new Business.ComplexListItem();

                foreach (Business.ComplexListField field in _fields)
                {
                    if (!fieldValues.ContainsKey(field.Name))
                        throw new Exception(string.Format("Field {0} not included in data", field.Name));
                    Business.ComplexListItemValue itemValue = new Business.ComplexListItemValue(this, field.FilterTerm);
                    itemValue.BigText = field.BigText ?? false;
                    itemValue.FieldID = field.ID;
                    itemValue.Load(fieldValues[field.Name]);
                    item.ItemValues.Add(itemValue);
                }
                _items.Add(item);
            }
        }

        public ComplexListItem FindItem(Guid itemID)
        {
            Predicate<ComplexListItem> p = delegate(ComplexListItem item) { return (item.ID == itemID); };
            return _items.Find(p);
        }

        public static TermStore CreateStore(string termName, XmlReader reader, List<string> templateFieldNames)
        {
            TermStore termStore = new TermStore(termName, TermType.ComplexList);
            bool abort = false;

            int outerLoopCount1 = 0;

            //First need to parse out the Fields collection for this ManagedItem Complex List.
            Dictionary<Guid, string> allFieldNames = new Dictionary<Guid, string>();

            using (reader)
            {
                while (!abort)
                {
                    if (outerLoopCount1++ > TermStore.maxLoopCount)
                    {
                        throw new Exception(string.Format("Complex List CreateStore stopped at outerLoopCount1 {0:D} for termName '{1}'", outerLoopCount1, termName));
                    }
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == XMLNames._E_Fields)
                    {
                        int innerLoopCount1 = 0;
                        while (reader.Read() && !abort)
                        {
                            if (innerLoopCount1++ > TermStore.maxLoopCount)
                            {
                                throw new Exception(string.Format("Complex List CreateStore stopped at innerLoopCount1 {0:D} for termName '{1}'", innerLoopCount1, termName));
                            }
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == XMLNames._E_Field)
                            {
                                //NOTE - Allow for some ComplexLists that do not have FieldID's assigned...
                                try
                                {
                                    allFieldNames.Add(new Guid(reader.GetAttribute(XMLNames._A_ID)), reader.GetAttribute(XMLNames._A_Name));
                                }
                                catch
                                {
                                    abort = true;
                                }
                            }
                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == XMLNames._E_Fields)
                            {
                                abort = true;
                            }
                        }
                    }
                    else
                    {
                        abort = !reader.Read();
                    }
                }

                if (allFieldNames.Count > 0)
                {
                    //Create a new collection of the Complex List fields that are in common with the ones found in the Template.
                    Dictionary<Guid, string> matchedFieldNames = new Dictionary<Guid, string>();
                    foreach (string templateFieldName in templateFieldNames)
                    {
                        if (allFieldNames.ContainsValue(templateFieldName))
                        {
                            foreach (KeyValuePair<Guid, string> kvp in allFieldNames)
                            {
                                if (kvp.Value == templateFieldName)
                                {
                                    matchedFieldNames.Add(kvp.Key, kvp.Value);
                                    break;
                                }
                            }
                        }
                    }

                    abort = false;
                    //Now we need to parse out the Items collection.
                    int outerLoopCount2 = 0;
                    while (!abort)
                    {
                        if (outerLoopCount2++ > TermStore.maxLoopCount)
                        {
                            throw new Exception(string.Format("Complex List CreateStore stopped at outerLoopCount2 {0:D} for termName '{1}'", outerLoopCount2, termName));
                        }
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == XMLNames._E_Items)
                        {
                            int innerLoopCount2 = 0;
                            while (reader.Read() && !abort)
                            {
                                if (innerLoopCount2++ > TermStore.maxLoopCount)
                                {
                                    throw new Exception(string.Format("Complex List CreateStore stopped at innerLoopCount2 {0:D} for termName '{1}'", innerLoopCount2, termName));
                                }
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == XMLNames._E_Item)
                                {
                                    Dictionary<string, string> item = ComplexListItem.CreateComplexListItemStore(reader.ReadSubtree(), matchedFieldNames);
                                    if (item != null)
                                        termStore.AddFieldValue(item);
                                }
                                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == XMLNames._E_Items)
                                {
                                    abort = true;
                                }
                            }
                        }
                        else
                        {
                            abort = !reader.Read();
                        }
                    }
                }
            }
            return termStore;
        }
	}
}
