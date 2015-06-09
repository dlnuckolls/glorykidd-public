using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class ComplexListItem
	{
		#region private members
		private bool? _selectable;
		private bool? _selected;
		private bool? _editable;
		private bool? _deletable;
		private bool? _default;             //TODO - Get rid of this since we no longer store this attribute.
		private List<ComplexListItemValue> _itemValues;
        private readonly Guid _id;   //Added to support sorting in the ManagedItem ComplexList screen

		#endregion

		#region Properties

        public Guid ID
        {
            get { return _id; }
        }

		public bool? Selectable
		{
			get { return _selectable; }
			set { _selectable = value; }
		}

		public bool? Selected
		{
			get { return _selected; }
			set { _selected = value; }
		}

		public bool? Editable
		{
			get { return _editable; }
			set { _editable = value; }
		}

		public bool? Deletable
		{
			get { return _deletable; }
			set { _deletable = value; }
		}

		public bool? Default
		{
			get { return _default; }
			set { _default = value; }
		}

		public List<ComplexListItemValue> ItemValues
		{
			get { return _itemValues; }
			set { _itemValues = value; }
		}

		#endregion

		#region Constructors

        public ComplexListItem()
        {
            _itemValues = new List<ComplexListItemValue>();
            _id = Guid.NewGuid();
        }

        public ComplexListItem(ComplexListItem sourceCLItem)
        {
            _itemValues = new List<ComplexListItemValue>();
            _id = Guid.NewGuid();
            _selectable = sourceCLItem._selectable;
            _selected = sourceCLItem._selected;
            _editable = sourceCLItem._editable;
            _deletable = sourceCLItem._deletable;
            _default = sourceCLItem._default;
        }

        public ComplexListItem(XmlNode node, ComplexList complexList)
		{
			_selectable = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_Selectable);
			_selected = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_Selected);
			_editable = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_Editable);
			_deletable = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_Deletable);
			_default = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_Default);

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

			XmlNodeList nodeItemValues = node.SelectNodes(XMLNames._E_ItemValue);

			if (nodeItemValues != null)
			{
				_itemValues = new List<ComplexListItemValue>(nodeItemValues.Count);
				foreach (XmlNode nodeItemValue in nodeItemValues)
				{
					ComplexListItemValue itemValue = new ComplexListItemValue(nodeItemValue, complexList);
					_itemValues.Add(itemValue);
				}
			}
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			if (bValidate)
			{
			}

			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_Selectable, _selectable);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_Selected, _selected);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_Editable, _editable);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_Deletable, _deletable);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_Default, _default);
            Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_ID, _id.ToString());

			foreach (ComplexListItemValue itemValue in _itemValues)
			{
				XmlNode elementItemValue = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_ItemValue, XMLNames._M_NameSpaceURI);
				itemValue.Build(xmlDoc, elementItemValue, bValidate);
				node.AppendChild(elementItemValue);
			}
		}

		#endregion

        public ComplexListItemValue FindItemValue(Guid fieldID)
        {
            Predicate<ComplexListItemValue> p = delegate(ComplexListItemValue cliv) { return (cliv.FieldID == fieldID); };
            return _itemValues.Find(p);
        }

        public void ClearRunTime()
        {
            foreach (ComplexListItemValue cliv in _itemValues)
            {
                cliv.FieldFilterTerm.Runtime.Clear(true, cliv.FieldFilterTerm.Required ?? false);
            }
        }

        public ComplexListItem Migrate(Dictionary<Guid /*destTermField*/, Guid /*sourceTermField*/> fieldMapping, ComplexListItem sourceItem)
        {
            return null;
        }

        public static Dictionary<string, string> CreateComplexListItemStore(XmlReader reader, Dictionary<Guid, string> fieldNames)
        {
            Dictionary<string, string> item = null;
            bool abort = false;
            using (reader)
            {
                reader.Read();
                while (!abort)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            abort = reader.Name == XMLNames._E_Item;
                            reader.Read();
                            break;

                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case XMLNames._E_Item:
                                    string selected = reader.GetAttribute(XMLNames._A_Selected);
                                    abort = string.IsNullOrEmpty(selected) || !selected.Equals("True", StringComparison.OrdinalIgnoreCase);
                                    reader.Read();
                                    break;

                                case XMLNames._E_ItemValue:
                                    Guid fieldID = new Guid(reader.GetAttribute(XMLNames._A_FieldID));
                                    string value = Utility.XMLHelper.SafeReadElementString(reader);
                                    if (fieldNames.ContainsKey(fieldID))
                                    {
                                        if (item == null)
                                            item = new Dictionary<string, string>();
                                        item.Add(fieldNames[fieldID], value);
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
            }
            return item;
        }




	}
}
