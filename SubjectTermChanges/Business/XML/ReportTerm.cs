using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public enum ReportTermSortOrder
	{
		None = 0,
		Primary = 1,
		Secondary = 2
	}

	[Serializable]
	public enum ReportTermType
	{
		//Note - based on TermType
		None = 0,
        NotReported,   //Added with the 1.6 release to denote term types that will not be reported on
		Text,
		Date,
		MSO,
		Renewal,
		Facility,
		//ComplexList,		Not supported
		PickList,
		Link,
		External,   // ???
		//Note - additional types used for sorting purposes only
		ManagedItemNumber,
		Status,
		TemplateName
	}

	[Serializable]
	public class ReportTerm
	{
		//The ReportTerm class is used to do several things, including
		//		1. Store the array of terms (name and type) selected by the user to be used in reporting
		//		2. Store the array of terms (term info) based on each ManagedItem - store as a collection
		//			within the Report class
		//Each ReportTerm must therefore be able to convert the term to and from xml, as well
		//as store enought term info to aid in sorting when rearranging the array of managedItems
		//which are stored and managed within the Report class.
		//It is not expected that we will need to recover decoded (unformatted) term value info
		//from the xml.  The xml will basically not store actual term values - just the array of
		//term names and term types, as needed by the Report class for configuring reports and
		//report search criteria, ordering, etc.

		#region private members
		private string _name;				//Equivalent to basic term 'name' - cannot be modified
		private string _displayName;		//For use when 'name' needs to be modified
		private ReportTermType _reportTermType;
		private bool? _visible;
		private bool? _systemTerm;
		private string _value;				//Not stored in the xml
		private DateTime? _dateTimeValue;	//Not stored in the xml
		private ReportTermSortOrder _sortOrder;

		#endregion

		#region Properties

		public string Name
		{
			get { return Utility.XMLHelper.GetXMLText(_name); }
			set { _name = Utility.XMLHelper.SetXMLText(value); }
		}

		public string DisplayName
		{
			get { return Utility.XMLHelper.GetXMLText(_displayName); }
			set { _displayName = Utility.XMLHelper.SetXMLText(value); }
		}

		public ReportTermType ReportTermType
		{
			get { return _reportTermType; }
			set { _reportTermType = value; }
		}

		public bool? Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		public bool? SystemTerm
		{
			get { return _systemTerm; }
			set { _systemTerm = value; }
		}

		public string Value
		{
			get { return Utility.XMLHelper.GetXMLText(_value); }
			set { _value = Utility.XMLHelper.SetXMLText(value); }
		}

		public DateTime? DateTimeValue
		{
			get { return _dateTimeValue; }
			set { _dateTimeValue = value; }
		}

		public ReportTermSortOrder SortOrder
		{
			get { return _sortOrder; }
			set { _sortOrder = value; }
		}

		#endregion


		#region Constructors

		public ReportTerm(XmlNode node)
		{
			_name = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Name);
			_displayName = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_DisplayName);
			_visible = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_Visible);
			_systemTerm = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_SystemTerm);
			//_value = Utility.XMLHelper.GetText(node);

			try
			{
				_sortOrder = (ReportTermSortOrder)Enum.Parse(typeof(ReportTermSortOrder), Utility.XMLHelper.GetAttributeString(node, XMLNames._A_SortOrder));
			}
			catch
			{
				_sortOrder = ReportTermSortOrder.None;
			}

			try
			{
				_reportTermType = (ReportTermType)Enum.Parse(typeof(ReportTermType), Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Type));
			}
			catch
			{
				_reportTermType = ReportTermType.None;
			}
		}


        //AccessibleTermGroupIDs was added in order to restrict access to the data contained within the term.
        //If AccessibleTermGroupIDs is null, then no restriction applies.
        public ReportTerm(List<Guid> AccessibleTermGroupIDs, Term term)
		{
			_name = term.Name;
			_displayName = term.Name;
			_reportTermType = GetReportTermType(term.TermType);
			_systemTerm = term.SystemTerm;
			_visible = false;
			_sortOrder = ReportTermSortOrder.None;

            if (AccessibleTermGroupIDs != null)
            {
                if (!AccessibleTermGroupIDs.Contains(term.TermGroupID))
                {
                    _value = string.Empty;
                    return;
                }
            }

			switch (_reportTermType)
			{
				case ReportTermType.ManagedItemNumber:
				case ReportTermType.Status:
				case ReportTermType.TemplateName:
					//Not handled here....
					break;
				case ReportTermType.Text:
					_value = (term as TextTerm).Value;
					break;
				case ReportTermType.Date:
					_dateTimeValue = (term as DateTerm).Value;
					_value = (term as DateTerm).DisplayValue(XMLNames._TPS_None);
					break;
				case ReportTermType.MSO:
					_value = (term as MSOTerm).MSOValue;
					break;
				case ReportTermType.Renewal:
					RenewalTerm renewalTerm = term as RenewalTerm;					
					switch (renewalTerm.DisplayedDate)
					{
						case DisplayedDate.EffectiveDate:
							_dateTimeValue = renewalTerm.EffectiveDate;
							_value = renewalTerm.DisplayValue(XMLNames._TPS_EffectiveDate);
							break;
						case DisplayedDate.ExpirationDate:
						default:
							_dateTimeValue = renewalTerm.ExpirationDate;
							_value = renewalTerm.DisplayValue(XMLNames._TPS_ExpirationDate);
							break;
					}
					break;
				case ReportTermType.Facility:
					_value = (term as FacilityTerm).DisplayValue(XMLNames._TPS_None);
					break;
				case ReportTermType.PickList:
					_value = (term as PickListTerm).DisplayValue(XMLNames._TPS_None);
					break;
				case ReportTermType.Link:
					_value = (term as LinkTerm).DisplayValue(XMLNames._TPS_None);
					break;
				case ReportTermType.External:
					_value = (term as ExternalTerm).DisplayValue(XMLNames._TPS_None);
					break;
				default:
					throw new Exception(string.Format("ReportTerm type '{0}' not handled", _reportTermType.ToString()));
			}
		}


		public ReportTerm(string name,
							ReportTermType reportTermType,
							string value,
							bool visible)
		{
			_name = name;
			_displayName = name;
			_reportTermType = reportTermType;
			_systemTerm = true;
			_value = value;
			_visible = visible;
			_sortOrder = ReportTermSortOrder.None;
		}

		#endregion


		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode node)
		{
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Name, _name);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_DisplayName, _displayName);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_Visible, _visible);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Type, _reportTermType.ToString());
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_SystemTerm, _systemTerm);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_SortOrder, ((int)_sortOrder).ToString());
			//Utility.XMLHelper.AddText(xmlDoc, node, _value);
		}

		#endregion

		public static ReportTermType GetReportTermType(TermType termType)
		{
			switch (termType)
			{
				case TermType.Text: return ReportTermType.Text;
				case TermType.Date: return ReportTermType.Date;
				case TermType.MSO: return ReportTermType.MSO;
				case TermType.Renewal: return ReportTermType.Renewal;
				case TermType.Facility: return ReportTermType.Facility;
				case TermType.PickList: return ReportTermType.PickList;
				case TermType.Link: return ReportTermType.Link;
				case TermType.External: return ReportTermType.External;
                case TermType.PlaceHolderAttachments: return ReportTermType.NotReported;
                case TermType.PlaceHolderComments: return ReportTermType.NotReported;
			}
			return ReportTermType.None;
		}


		public static string GetTermType(ReportTermType reportTermType)
		{
			switch (reportTermType)
			{
				case ReportTermType.Text: return TermType.Text.ToString();
				case ReportTermType.Date: return TermType.Date.ToString();
				case ReportTermType.MSO: return TermType.MSO.ToString();
				case ReportTermType.Renewal: return TermType.Renewal.ToString();
				case ReportTermType.Facility: return TermType.Facility.ToString();
				case ReportTermType.PickList: return TermType.PickList.ToString();
				case ReportTermType.Link: return TermType.Link.ToString();
				case ReportTermType.External: return TermType.External.ToString();
			}
			return null;
		}


		public string ColumnHeaderText(string systemManagedItemName)
		{
			switch (_reportTermType)
			{
				case ReportTermType.ManagedItemNumber:
					if (string.IsNullOrEmpty(systemManagedItemName))
						return "Number";
					else
						return systemManagedItemName + " Number";

				case ReportTermType.TemplateName:
					return "Template";

				default:
					return this.DisplayName;
			}
		}


	}
}
