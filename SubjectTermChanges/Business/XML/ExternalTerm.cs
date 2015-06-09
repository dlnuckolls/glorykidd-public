using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
    [Serializable]
    public class ExternalTerm : Term
    {

        #region private fields

        private List<ExternalInterfaceListItem> _selectedItems;
        private ExternalInterfaceConfig _interfaceConfig;
        private ManagedItem _managedItem;
        private string _value = null; //Added with the Retro change - for storing the MakeNameValueXml output
        private bool _valuesLoaded;

        #endregion


        #region properties

        public List<ExternalInterfaceListItem> SelectedItems
        {
            get { return _selectedItems; }
        }

        public override string Keyword
        {
            get { return DisplayValue(""); }
        }

        public ExternalInterfaceConfig InterfaceConfig
        {
            get { return _interfaceConfig; }
        }

        public ManagedItem ManagedItem
        {
            get { return _managedItem; }
        }

        public bool ValuesLoaded
        {
            get { return _valuesLoaded; }
            set { _valuesLoaded = value; }
        }

        public override string[] StoreColumns
        {
            get 
            {
                List<string> columns = new List<string>();
                foreach (ExternalInterfaceAvailableField availableField in _interfaceConfig.AvailableFields)
                    if (availableField.IsDisplayed)
                        columns.Add(availableField.Name);
                return columns.ToArray(); 
            }
        }

        #endregion


        #region constructors

        public ExternalTerm(bool systemTerm, Business.ExternalInterfaceConfig eic, ManagedItem managedItem, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
        {
            TermType = TermType.External;
            NameRequired = true;
            Name = eic.Name;
            _interfaceConfig = eic;
            //NOTE - This operation is valid only the FIRST time this is set...
            if (TermTransforms == null && _interfaceConfig.TransformTermType.HasValue)
            {
                TransformTermType = _interfaceConfig.TransformTermType;
                TermTransforms = _interfaceConfig.TermTransforms;
            }
            _managedItem = managedItem;
            _valuesLoaded = false;
            _value = null;
        }

        public ExternalTerm(XmlNode termNode, ManagedItem managedItem, Template template, bool isFilter)
            : base(termNode, template, isFilter)
        {
            TermType = TermType.External;
            NameRequired = true;

            XmlNode nodeInterfaceConfig = termNode.SelectSingleNode(XMLNames._E_Config);
            if (nodeInterfaceConfig == null)
                throw new Exception("Config node not found for External Term");
            _interfaceConfig = new ExternalInterfaceConfig(nodeInterfaceConfig);
            _managedItem = managedItem;
            _valuesLoaded = false;
            XmlNode nodeValue = termNode.SelectSingleNode(XMLNames._E_Value);
            if (nodeValue != null)
                _value = Utility.XMLHelper.GetXMLText(Utility.XMLHelper.GetText(nodeValue));
            else
                _value = MakeNameValueXml();
        }


        #endregion

        public void InitializeSelectedItems()
        {
            _selectedItems = new List<ExternalInterfaceListItem>();
        }

        public void RefreshInterfaceConfig(ITATSystem system)
        {
            _interfaceConfig = system.FindExternalInterfaceConfig(_interfaceConfig.Name);
        }

        public void SaveValues(bool refreshXML)
        {
            if (_managedItem == null)
                return;
            if (refreshXML)
                _value = MakeNameValueXml();
            Data.ExternalTerm.SaveValues(_managedItem.ManagedItemID, _interfaceConfig.Name, _value);
        }


        public void LoadValues()
        {
            if (_managedItem == null)
                return;
            _selectedItems = new List<ExternalInterfaceListItem>();
            DataTable dt = Data.ExternalTerm.GetValues(_managedItem.ManagedItemID, _interfaceConfig.Name);
            string previousKeyValue = string.Empty;
            Business.ExternalInterfaceListItem listItem = null;
            foreach (DataRow row in dt.Rows)
            {
                string keyValue = (string)row[Data.DataNames._C_KeyValue];
                if (keyValue != previousKeyValue)
                {
                    previousKeyValue = keyValue;
                    if (listItem != null)
                        _selectedItems.Add(listItem);
                    listItem = new Business.ExternalInterfaceListItem(_interfaceConfig);
                    listItem.Key = keyValue;
                }
                listItem.FieldValues.Add((string)row[1], (string)row[2]);
            }
            if (listItem != null)
                _selectedItems.Add(listItem);
            _valuesLoaded = true;
        }


        public DataTable SelectedItemValues()
        {
            DataTable rtn = new DataTable();
            List<ExternalInterfaceAvailableField> fields = _interfaceConfig.DisplayedFields();
            rtn.Columns.Add("Key");
            rtn.Columns.Add("DisplayValue");
            foreach (ExternalInterfaceAvailableField field in fields)
                rtn.Columns.Add(field.Name, typeof(string));
            if (_selectedItems != null)
            {
                foreach (ExternalInterfaceListItem listItem in _selectedItems)
                {
                    string[] rowValues = new string[rtn.Columns.Count];
                    rowValues[0] = listItem.Key;
                    rowValues[1] = listItem.DisplayValue();
                    for (int columnNumber = 2; columnNumber < rowValues.Length; columnNumber++)
                    {
                        string fieldValue = string.Empty;
                        try { fieldValue = listItem.FieldValues[rtn.Columns[columnNumber].ColumnName]; }
                        catch { }
                        rowValues[columnNumber] = fieldValue;
                    }
                    rtn.Rows.Add(rowValues);
                }
            }
            return rtn;
        }

        public override List<string> GetTransformList(string name)
        {
            List<string> transformList = new List<string>();

            if (_selectedItems != null && _selectedItems.Count > 0)
            {
                foreach (ExternalInterfaceListItem li in _selectedItems)
                {
                    transformList.Add(li.FieldValues[name]);
                }
            }
            return transformList;
        }

        public override void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
        {
            base.Build(xmlDoc, termNode, bValidate);
            XmlNode elementConfig = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Config, XMLNames._M_NameSpaceURI);
            _interfaceConfig.Build(xmlDoc, elementConfig, bValidate);
            termNode.AppendChild(elementConfig);
            XmlNode elementValue = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Value, XMLNames._M_NameSpaceURI);
            if (string.IsNullOrEmpty(_value))
                _value = MakeNameValueXml();
            Utility.XMLHelper.SetText(xmlDoc, elementValue, Utility.XMLHelper.SetXMLText(_value));
            termNode.AppendChild(elementValue);
        }


        public override void SetDefaultValue()
        {
            InitializeSelectedItems();
        }

        public override List<string> CheckType(bool includeTab, string filterTermTabName)
        {
            return null;
        }

        public override string DisplayValue(string termPartSpecifier)
        {
            if (_interfaceConfig == null)
                throw new NullReferenceException("InterfaceConfig has not been set.");
            if (!_valuesLoaded)
                LoadValues();
            if (_selectedItems == null)
                return string.Empty;
            switch (_selectedItems.Count)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return _selectedItems[0].DisplayValue();
                case 2:
                    return _selectedItems[0].DisplayValue() + " and " + _selectedItems[1].DisplayValue();
                default:
                    System.Text.StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < _selectedItems.Count - 1; i++)
                    {
                        sb.Append(_selectedItems[i].DisplayValue());
                        sb.Append(", ");
                    }
                    sb.Append("and ");
                    sb.Append(_selectedItems[_selectedItems.Count - 1].DisplayValue());
                    return sb.ToString();
            }
        }

        public override List<string> Validate(bool includeTab, string filterTermTabName)
        {
            List<string> rtn = new List<string>();
            if (Runtime.Required)
            {
                if (!_valuesLoaded)
                    LoadValues();
                if (_selectedItems.Count == 0)
                    rtn.Add(string.Format("At least one item must be selected in the Term \"{0}\"{1}", Name, includeTab ? string.Format(" (tab {0})", TermGroupName) : string.Empty));
            }
            return rtn;
        }

        public override void Delete()
        {
            Clear();
        }

        public override void MigrateReset()
        {
            //Do not perform any actions here.
        }

        public override void Clear()
        {
            Data.ExternalTerm.SaveValues(_managedItem.ManagedItemID, null, null);
            InitializeSelectedItems();
        }

        public override bool EmitPDFXML(System.Xml.XmlWriter writer)
        {
            PdfHelper.AddPDFXMLHeader(writer);
            bool bFirstOne = true;
            if (_managedItem != null)
                if (!_valuesLoaded)
                    LoadValues();
            if (_selectedItems.Count == 0)
            {
                PdfHelper.AddPDFXMLRow(writer, Name, "");
            }
            else
            {
                foreach (ExternalInterfaceListItem listItem in _selectedItems)
                {
                    if (bFirstOne)
                        PdfHelper.AddPDFXMLRow(writer, Name, listItem.DisplayValue());
                    else
                        PdfHelper.AddPDFXMLRow(writer, "", listItem.DisplayValue());
                    bFirstOne = false;
                }
            }
            PdfHelper.AddPDFXMLFooter(writer);
            return true;
        }


        private string MakeNameValueXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<?xml version='1.0' encoding='UTF-8' ?><Items />");
            if (_selectedItems != null)
            {
                XmlElement elementRoot = xmlDoc.DocumentElement;
                for (int index = 0; index < _selectedItems.Count; index++)
                {
                    XmlNode nodeItem = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Item, XMLNames._M_NameSpaceURI);
                    ExternalInterfaceListItem item = _selectedItems[index];
                    Utility.XMLHelper.AddAttributeString(xmlDoc, nodeItem, XMLNames._A_Key, item.Key);
                    Utility.XMLHelper.AddAttributeInt(xmlDoc, nodeItem, XMLNames._A_Sequence, index + 1);
                    Utility.XMLHelper.AddAttributeString(xmlDoc, nodeItem, XMLNames._A_ID, Guid.NewGuid().ToString());
                    XmlNode nameValueNode = _selectedItems[index].NameValueXMLNode();
                    nodeItem.AppendChild(xmlDoc.ImportNode(nameValueNode, true));
                    elementRoot.AppendChild(nodeItem);
                }
            }
            return xmlDoc.OuterXml;
        }

        public override void Migrate(Term term)
        {
            if (term == null)
                base.Migrate(term);
            if (!(term is ExternalTerm))
                return;
            else
            {
                ExternalTerm sourceTerm = term as ExternalTerm;
                _managedItem = sourceTerm._managedItem; //This is okay since we only need the ManagedItemID
                _value = sourceTerm._value;
                _interfaceConfig = sourceTerm._interfaceConfig.Copy();
                SaveValues(false);
                LoadValues();
                term.Runtime.Migrated = true;
            }
            Runtime.Migrated = true;
        }

        public static TermStore CreateStore(string termName, ExternalTerm externalTerm, Guid managedItemID)
        {
            if (externalTerm == null)
                throw new Exception(string.Format("External Term named '{0}' not found within the template", termName));

            TermStore termStore = new TermStore(termName, TermType.External);
            DataTable dt = Data.ExternalTerm.GetValues(managedItemID, externalTerm._interfaceConfig.Name);
            string previousKeyValue = string.Empty;
            Dictionary<string, string> item = new Dictionary<string, string>();

            for (int index = 0; index < dt.Rows.Count; index++)
            {
                DataRow dr = dt.Rows[index];
                string keyValue = (string)dr[Data.DataNames._C_KeyValue];
                if (index == 0)
                    previousKeyValue = keyValue;
                if (!item.ContainsKey((string)dr[1]))
                    item.Add((string)dr[1], (string)dr[2]);
                if (index == dt.Rows.Count - 1)
                    termStore.AddFieldValue(item);
                else
                {
                    if (keyValue != previousKeyValue)
                    {
                        termStore.AddFieldValue(item);
                        previousKeyValue = keyValue;
                        item = new Dictionary<string, string>();
                        item.Add((string)dr[1], (string)dr[2]);
                    }
                }
            }

            return termStore;
        }

        public override bool RequiresSystemSync(ITATSystem system)
        {
            if (system != null && system.ExternalInterfaces != null && system.ExternalInterfaces.Count == 1)
            {
                Business.ExternalInterfaceConfig eic = system.ExternalInterfaces[0];

                if (TransformTermType.HasValue != eic.TransformTermType.HasValue)
                    return true;

                if (eic.TransformTermType.HasValue)
                {
                    if (TermTransforms == null)
                        return true;

                    if (eic.TermTransforms != null)
                    {
                        if (eic.TermTransforms.Count != TermTransforms.Count)
                            return true;

                        for (int index = 0; index < eic.TermTransforms.Count; index++)
                            if (!Term.TermTransformMatch(eic.TermTransforms[index], TermTransforms[index]))
                                return true;
                    }
                }
            }
            return false;
        }

        //Note - this approach assumes that the system only has one ExternalTerm
        public override void SyncWithSystem(ITATSystem system)
        {
            if (system.ExternalInterfaces != null && system.ExternalInterfaces.Count == 1)
            {
                Business.ExternalInterfaceConfig eic = system.ExternalInterfaces[0];
                TransformTermType = eic.TransformTermType;
                TermTransforms = eic.TermTransforms;
            }
        }
    }
}
