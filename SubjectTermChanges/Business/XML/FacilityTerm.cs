using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class FacilityTerm : Term
	{
		#region private members
		private bool? _multiSelect;
		private bool? _useUserSecurity;
		private bool? _includeChildren;
		private bool? _isPrimary;
		private List<int> _facilityTypes;
		private List<int> _selectedFacilityIDs;
		private List<int> _owningFacilityIDs;
		private FacilityStatusType _facilityStatus;
		private FacilitySort _sort;
        private const char _INTEGER_DELIMITER = ',';
		#endregion

		#region Properties

		public static string DefaultDisplayValueFormat   
		{
			get { return ""; }
		}

		public bool? MultiSelect
		{
			get { return _multiSelect; }
			set { _multiSelect = value; }
		}

		public bool? UseUserSecurity
		{
			get { return _useUserSecurity; }
			set { _useUserSecurity = value; }
		}

		public List<int> FacilityTypes
		{
			get { return _facilityTypes; }
			set { _facilityTypes = value; }
		}

		public List<int> SelectedFacilityIDs
		{
			get { return _selectedFacilityIDs; }
			set
			{
				//Must ensure that all OwningFacilityIDs are included in this list
				foreach (int facilityID in _owningFacilityIDs)
					if (!_selectedFacilityIDs.Contains(facilityID))
						throw new Exception("The list of SelectedFacilityIDs did not include all of the OwningFacilityIDs");
				_selectedFacilityIDs = value;
			}
		}

		public List<int> OwningFacilityIDs
		{
			get { return _owningFacilityIDs; }
			set {
					if (_owningFacilityIDs.Count > 0)
						throw new Exception("Attempted to set the values for OwningFacilityIDs after they have already been established");
					_owningFacilityIDs = value; 
				}
		}

		public bool? IncludeChildren
		{
			get { return _includeChildren; }
			set { _includeChildren = value; }
		}

		public bool? IsPrimary
		{
			get { return _isPrimary; }
			set { _isPrimary = value; }
		}

		public FacilityStatusType FacilityStatus
		{
			get { return _facilityStatus; }
			set { _facilityStatus = value; }
		}

		public override string Keyword
		{
			get
			{
				if (!HasKeyWord)
					return null;
				if (_selectedFacilityIDs != null)
				{
					if (_selectedFacilityIDs.Count > 0)
					{
						//Create a 'Keyword' which consists of all the facility id's, SAP ID's concatenated together
						using (DataSet ds = Data.ManagedItem.GetFacilityInfo(GetXml(_selectedFacilityIDs)))
						{
							if (ds.Tables.Count > 0)
							{
								if (ds.Tables[0].Rows.Count > 0)
								{
									string keyWord = "";
									foreach (DataRow row in ds.Tables[0].Rows)
									{
										keyWord = string.Concat(keyWord, XMLNames._M_Delimiter, row[Data.DataNames._SP_FacilityName].ToString(), XMLNames._M_Delimiter, row[Data.DataNames._SP_FacilitySAPID].ToString());
									}
									return keyWord.Length > 0 ? keyWord.Trim(XMLNames._M_Delimiter) : null;
								}
							}
						}
					}
				}
				return null;
			}
		}


		public FacilitySortField SortField
		{
			get { return _sort.Field; }
			set {_sort.Field = value;}
		}

        public override int? RequiredSize
        {
            get
            {
                return null;
            }
        }

		#endregion

		#region Constructors

        public FacilityTerm(bool systemTerm, Template template, bool isFilter)
            : base(systemTerm, template, isFilter)
		{
			TermType = TermType.Facility;
			NameRequired = true;
			_facilityTypes = new List<int>();
			_selectedFacilityIDs = new List<int>();
			_owningFacilityIDs = new List<int>();
			_sort = new FacilitySort();
		}

        //Term Transform Info:  This constructor was added to enable FacilityTerm to support Term Transformation.
        //It is currently the only term type that can serve as the destination of a transformation.
        public FacilityTerm(bool systemTerm, Template template, Term originalTerm)
            : base(systemTerm, template, originalTerm, TermType.Facility)
        {
        }

        public FacilityTerm(XmlNode termNode, Template template, bool isFilter)
            : base(termNode, template, isFilter)
		{
			TermType = TermType.Facility;
			NameRequired = true;
			_multiSelect = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_MultiSelect);
			_useUserSecurity = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_UseUserSecurity);
			_includeChildren = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_IncludeChildren);
			_isPrimary = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_IsPrimary);
			int? facilityStatus = Utility.XMLHelper.GetAttributeInt(termNode, XMLNames._A_FacilityStatus) ?? 0;
			_facilityStatus = (FacilityStatusType)facilityStatus;

			_sort = new FacilitySort(FacilitySortField.FacilityName);   //default to sorting by FacilityName
			string sortField = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_FacilitySort);
			if (!string.IsNullOrEmpty(sortField))
				_sort.Field = (FacilitySortField)Enum.Parse(typeof(FacilitySortField), sortField);

			XmlNodeList facilityTypeNodes = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_FacilityTypes,XMLNames._E_FacilityType));
			_facilityTypes = new List<int>(facilityTypeNodes.Count);
			foreach (XmlNode facilityTypeNode in facilityTypeNodes)
			{
				_facilityTypes.Add(Utility.XMLHelper.GetAttributeIntNonNull(facilityTypeNode, XMLNames._A_TypeID));
			}

			XmlNodeList selectedFacilityIDNodes = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_SelectedFacilityIDs, XMLNames._E_FacilityID));
			_selectedFacilityIDs = new List<int>(selectedFacilityIDNodes.Count);
			if (selectedFacilityIDNodes.Count > 0)
			{
				foreach (XmlNode facilityIDNode in selectedFacilityIDNodes)
				{
					_selectedFacilityIDs.Add(Utility.XMLHelper.GetAttributeIntNonNull(facilityIDNode, XMLNames._A_ID));
				}
			}

			XmlNodeList owningFacilityIDNodes = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_OwningFacilityIDs, XMLNames._E_FacilityID));
			_owningFacilityIDs = new List<int>(owningFacilityIDNodes.Count);
			if (owningFacilityIDNodes.Count > 0)
			{
				foreach (XmlNode facilityIDNode in owningFacilityIDNodes)
				{
					_owningFacilityIDs.Add(Utility.XMLHelper.GetAttributeIntNonNull(facilityIDNode, XMLNames._A_ID));
				}
			}
		}

		#endregion

		#region Build XML

		public override void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			base.Build(xmlDoc, termNode, bValidate);

			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_MultiSelect, _multiSelect);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_UseUserSecurity, _useUserSecurity);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_IncludeChildren, _includeChildren);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_IsPrimary, _isPrimary);
			Utility.XMLHelper.AddAttributeInt(xmlDoc, termNode, XMLNames._A_FacilityStatus, (int)_facilityStatus);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_FacilitySort, _sort.Field.ToString());

			XmlNode elementFacilityTypes = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_FacilityTypes, XMLNames._M_NameSpaceURI);
			//Pull from list of FacilityType's to build sub nodes
			foreach (int facilityType in _facilityTypes)
			{
				XmlNode elementFacilityType = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_FacilityType, XMLNames._M_NameSpaceURI);
				Utility.XMLHelper.AddAttributeInt(xmlDoc, elementFacilityType, XMLNames._A_TypeID, facilityType);
				elementFacilityTypes.AppendChild(elementFacilityType);
			}
			termNode.AppendChild(elementFacilityTypes);

			if (_selectedFacilityIDs != null)
			{
				XmlNode elementSelectedFacilityIDs = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_SelectedFacilityIDs, XMLNames._M_NameSpaceURI);
				foreach (int facilityID in _selectedFacilityIDs)
				{
					XmlNode elementFacilityID = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_FacilityID, XMLNames._M_NameSpaceURI);
					Utility.XMLHelper.AddAttributeInt(xmlDoc, elementFacilityID, XMLNames._A_ID, facilityID);
					elementSelectedFacilityIDs.AppendChild(elementFacilityID);
				}
				termNode.AppendChild(elementSelectedFacilityIDs);
			}

			if (_owningFacilityIDs != null)
			{
				XmlNode elementOwningFacilityIDs = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_OwningFacilityIDs, XMLNames._M_NameSpaceURI);
				foreach (int facilityID in _owningFacilityIDs)
				{
					XmlNode elementFacilityID = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_FacilityID, XMLNames._M_NameSpaceURI);
					Utility.XMLHelper.AddAttributeInt(xmlDoc, elementFacilityID, XMLNames._A_ID, facilityID);
					elementOwningFacilityIDs.AppendChild(elementFacilityID);
				}
				termNode.AppendChild(elementOwningFacilityIDs);
			}
		}

		#endregion

        protected override bool MapTransform()
        {
            bool includeChildren = false;
            bool.TryParse(_originalTerm.TermTransforms.Find(tr => tr.TransformType == TermTransformType.Attribute && tr.Name == XMLNames._XFRM_IncludeChildren).Value, out includeChildren);
            _includeChildren = includeChildren;

            _selectedFacilityIDs = new List<int>();
            string selectedFacilityIDLookupName = _originalTerm.TermTransforms.Find(tr => tr.TransformType == TermTransformType.Reference && tr.Name == XMLNames._XFRM_SelectedFacilityIDs).Value;
            if (!string.IsNullOrEmpty(selectedFacilityIDLookupName))
            {
                List<string> selectedFacilityIDs = _originalTerm.GetTransformList(selectedFacilityIDLookupName);
                foreach (string facilityId in selectedFacilityIDs)
                    _selectedFacilityIDs.Add(int.Parse(facilityId));
            }
            return true;
        }

		public override string DisplayValue(string termPartSpecifier)
		{
			List<int> facIds = SelectedFacilityIDs;
			FacilityCollection facs = FacilityCollection.FacilityList(facIds, false);
			List<string> values = new List<string>();
			foreach (Data.FacilityDataRow fdr in facs.Values)
			{
				string formattedFacilityName;
				if (termPartSpecifier == FacilityTerm.DefaultDisplayValueFormat)
					formattedFacilityName = string.Format("{0} - {1}", GetFacilityDisplayValue(fdr, Business.XMLNames._TPS_SAPID), GetFacilityDisplayValue(fdr, Business.XMLNames._TPS_FacilityName));
				else
					formattedFacilityName = GetFacilityDisplayValue(fdr, termPartSpecifier);
				if (!values.Contains(formattedFacilityName))
					values.Add(formattedFacilityName);
			}
			switch (values.Count)
			{
				case 0:
					return string.Empty;
				case 1:
					return values[0];
				case 2:
					return values[0] + " and " + values[1];
				default:
					System.Text.StringBuilder sb = new StringBuilder();
					for (int i = 0; i < values.Count - 1; i++)
					{
						sb.Append(values[i]);
						sb.Append(", ");
					}
					sb.Append("and ");
					sb.Append(values[values.Count - 1]);
					return sb.ToString();
			}
		}

		private string GetFacilityDisplayValue(Data.FacilityDataRow fdr, string termPartSpecifier)
		{
			switch (termPartSpecifier)
			{
				case XMLNames._TPS_SAPID:
					return fdr.SapFacilityId;
				case XMLNames._TPS_FacilityType:
					return fdr.FacilityType;
				case XMLNames._TPS_Address:
					return fdr.Address;
				case XMLNames._TPS_City:
					return fdr.City;
				case XMLNames._TPS_County:
					return fdr.County;
				case XMLNames._TPS_State:
					return fdr.State;
				case XMLNames._TPS_StateCode:
					return fdr.StateCode;
				case XMLNames._TPS_Zip:
					return fdr.Zip;
				case XMLNames._TPS_Phone:
					return Utility.TextHelper.FormatAsPhone(fdr.AreaCode + fdr.Phone);
				case XMLNames._TPS_Fax:
					return Utility.TextHelper.FormatAsPhone(fdr.Fax);
				case XMLNames._TPS_LegalEntityName:
					return fdr.LegalEntityName;
				case XMLNames._TPS_FacilityName:
				default:
					return fdr.FacilityName;
			}
		}

        public override List<string> Validate(bool includeTab, string filterTermTabName)
		{
			List<string> rtn = new List<string>();

			if (Runtime.Required)
				if (this.SelectedFacilityIDs.Count == 0)
					if (this.MultiSelect ?? false)
                        rtn.Add(string.Format("\"{0}\"{1} is a required term, but no facilities were selected.", this.Name, includeTab ? string.Format(" (tab {0})", TermGroupName) : string.Empty));
					else
                        rtn.Add(string.Format("\"{0}\"{1} is a required term, but no facility was selected.", this.Name, includeTab ? string.Format(" (tab {0})", TermGroupName) : string.Empty));

			return rtn;
		}

        public override List<string> CheckType(bool includeTab, string filterTermTabName)
        {
            return null;
        }

		public override bool EmitPDFXML(XmlWriter writer)
		{
			PdfHelper.AddPDFXMLHeader(writer);
			bool bFirstOne = true;

			using (DataSet ds = Data.ManagedItem.GetFacilityInfo(GetXml(SelectedFacilityIDs)))
			{
				if ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
				{
					foreach (DataRow row in ds.Tables[0].Rows)
					{
						string sSAPID = row[Data.DataNames._SP_FacilitySAPID].ToString();
						string sFacilityName = row[Data.DataNames._SP_FacilityName].ToString();
						if (bFirstOne)
							PdfHelper.AddPDFXMLRow(writer, Name, sSAPID + " -- " + sFacilityName);
						else
							PdfHelper.AddPDFXMLRow(writer, "", sSAPID + " -- " + sFacilityName);
						bFirstOne = false;
					}
				}
				else
				{
					PdfHelper.AddPDFXMLRow(writer, Name, "");
				}
			}
			PdfHelper.AddPDFXMLFooter(writer);
			return true;
		}


		//This call is used to generate an xml format for database updates
		public static string GetXml(List<int> facilities)
		{
			if (facilities == null)
				facilities = new List<int>();

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<?xml version='1.0' encoding='UTF-8' ?><Facilities />");
			XmlElement elementRoot = xmlDoc.DocumentElement;

			foreach (int facility in facilities)
			{
				XmlNode nodeFacility = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Facility, XMLNames._M_NameSpaceURI);
				Utility.XMLHelper.AddAttributeString(xmlDoc, nodeFacility, XMLNames._A_ID, facility.ToString());
				elementRoot.AppendChild(nodeFacility);
			}

			return xmlDoc.OuterXml;
		}


		public override void SetDefaultValue()
		{
			// do nothing       Previously:  _selectedFacilityIDs.Clear();
		}

        public override void Clear()
		{
			_selectedFacilityIDs.Clear();
		}

        public override void Migrate(Term term)
        {
            if (term == null)
                base.Migrate(term);
            if (!(term is FacilityTerm))
                return;
            else
            {
                _selectedFacilityIDs = (term as FacilityTerm).SelectedFacilityIDs;
                _owningFacilityIDs = (term as FacilityTerm).OwningFacilityIDs;
                term.Runtime.Migrated = true;
            }
            Runtime.Migrated = true;
        }

        public override void Load(string value, string pattern, char? delimiter)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string sOwningFacilityID = value.Split(delimiter.Value)[0];
                string sSelectedFacilityIDs = value.Split(delimiter.Value)[1];
                _owningFacilityIDs = new List<int>();
                if (!string.IsNullOrEmpty(sOwningFacilityID))
                {
                    string[] sOwningFacilityIDs = sOwningFacilityID.Split(_INTEGER_DELIMITER);
                    foreach (string sID in sOwningFacilityIDs)
                        _owningFacilityIDs.Add(int.Parse(sID));
                }
                _selectedFacilityIDs = new List<int>();
                if (!string.IsNullOrEmpty(sSelectedFacilityIDs))
                {
                    string[] arrSelectedFacilityIDs = sSelectedFacilityIDs.Split(_INTEGER_DELIMITER);
                    foreach (string sID in arrSelectedFacilityIDs)
                        _selectedFacilityIDs.Add(int.Parse(sID));
                }
            }
        }

        public static TermStore CreateStore(string termName, XmlReader reader)
        {
            using (reader)
            {
                TermStore termStore = new TermStore(termName, TermType.Facility);
                bool termProcessed = false;
                bool abort = false;
                reader.Read();
                reader.Read();

                int loopCount = 0;

                ReadPastSubTree(reader, XMLNames._E_validateOn);

                while (!abort && !termProcessed)
                {
                    if (loopCount++ > TermStore.maxLoopCount)
                    {
                        throw new Exception(string.Format("CreateStore stopped at loopCount {0:D} for Facility term '{1}'", loopCount, termName));
                    }
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            abort = true;
                            break;

                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case XMLNames._E_FacilityTypes:
                                case XMLNames._E_OwningFacilityIDs:
                                    reader.Skip();
                                    break;

                                case XMLNames._E_SelectedFacilityIDs:
                                    using (XmlReader facilityReader = reader.ReadSubtree())
                                    {
                                        while (facilityReader.Read())
                                        {
                                            switch (facilityReader.NodeType)
                                            {
                                                case XmlNodeType.Element:
                                                    switch (facilityReader.Name)
                                                    {
                                                        case XMLNames._E_FacilityID:
                                                            termStore.AddMultiValue(facilityReader.GetAttribute(XMLNames._A_ID));
                                                            break;

                                                        default:
                                                            break;
                                                    }
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    termProcessed = true;
                                    break;

                                default:
                                    //reader.Read();
                                    break;
                            }
                            break;

                        default:
                            break;
                    }
                }
                return termStore;
            }
        }

	}
}
