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
	public class Report
	{
		#region private members
		private string _name;				
		private string _description;
		private Guid? _itatSystemId;
		private List<ReportTerm> _reportTerms;
		private SearchCriteria _searchCriteria;

		private Guid? _reportId;

		/*
		 * <?xml version='1.0' encoding='UTF-8' ?>
		 * <Report Name="" ITATSystemID="" >
		 *		<Description></Description>
		 *		<Rules>
		 *				<Rule Name="Statuses" >
		 *						<RuleItem>OPEN</RuleItem>
		 *						<RuleItem>CLOSED</RuleItem>
		 *						<RuleItem>ETC</RuleItem>
		 *				</Rule>
		 *				<Rule Name="FacilityIDs" >
		 *						<RuleItem>492</RuleItem>
		 *						<RuleItem>123</RuleItem>
		 *						<RuleItem>456</RuleItem>
		 *				</Rule>
		 *				<Rule Name="ManagedItemID" >123-456-0000-3333</Rule>
		 *				<Rule Name="TemplateID" >123-456-0000-3333</Rule>
		 *		</Rules>
		 *		<Terms>
		 *				<Term Name="Contract Term" Type="Renewal" Visible="true" SystemTerm="false" />
		 *				<Term Name="Primary Facility" Type="Facility" Visible="false" SystemTerm="false" />
		 *				<Term Name="External Party" Type="MSO" Visible="true" SystemTerm="false" />
		 *		</Terms>
		 * </Report>
		 */



		#endregion

		#region Properties

		public string Name
		{
			get { return Utility.XMLHelper.GetXMLText(_name); }
			set { _name = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Description
		{
			get { return Utility.XMLHelper.GetXMLText(_description); }
			set { _description = Utility.XMLHelper.SetXMLText(value); }
		}

		public Guid? ITATSystemId
		{
			get { return _itatSystemId; }
			set { _itatSystemId = value; }
		}

		public Guid? ReportId
		{
			get { return _reportId; }
			set { _reportId = value; }
		}

		public List<ReportTerm> ReportTerms
		{
			get { return _reportTerms; }
			set { _reportTerms = value; }
		}

		public SearchCriteria SearchCriteria
		{
			get { return _searchCriteria; }
			set { _searchCriteria = value; }
		}

		public ReportTerm PrimaryTerm
		{
			get
			{
				if (_reportTerms == null)
					return null;
				Predicate<ReportTerm> p = delegate(ReportTerm rt) {return rt.SortOrder == ReportTermSortOrder.Primary;};
				List<ReportTerm> reportTerms = _reportTerms.FindAll(p);
				if (reportTerms == null)
					return null;
				if (reportTerms.Count == 0)
					return null;
				if (reportTerms.Count > 1)
					throw new Exception(string.Format("More than one Primary sort term defined ({0} terms)", reportTerms.Count.ToString()));
				return reportTerms[0];
			}
			set
			{
				if (_reportTerms == null)
					return;
				foreach (ReportTerm reportTerm in _reportTerms)
				{
					if (reportTerm.SortOrder == ReportTermSortOrder.Primary)
						reportTerm.SortOrder = ReportTermSortOrder.None;
				}
				if (value == null)
					return;
				Predicate<ReportTerm> p = delegate(ReportTerm rt) { return ((rt.Name == value.Name) && (rt.ReportTermType == value.ReportTermType)); };
				ReportTerm foundTerm = _reportTerms.Find(p);
				if (foundTerm != null)
					foundTerm.SortOrder = ReportTermSortOrder.Primary;
			}
		}

		public ReportTerm SecondaryTerm
		{
			get
			{
				if (_reportTerms == null)
					return null;
				Predicate<ReportTerm> p = delegate(ReportTerm rt) { return (rt.SortOrder == ReportTermSortOrder.Secondary); };
				List<ReportTerm> reportTerms = _reportTerms.FindAll(p);
				if (reportTerms == null)
					return null;
				if (reportTerms.Count == 0)
					return null;
				if (reportTerms.Count > 1)
					throw new Exception(string.Format("More than one Secondary sort term defined ({0} terms)", reportTerms.Count.ToString()));
				return reportTerms[0];
			}
			set
			{
				if (_reportTerms == null)
					return;
				foreach (ReportTerm reportTerm in _reportTerms)
				{
					if (reportTerm.SortOrder == ReportTermSortOrder.Secondary)
						reportTerm.SortOrder = ReportTermSortOrder.None;
				}
				if (value == null)
					return;
				Predicate<ReportTerm> p = delegate(ReportTerm rt) { return ((rt.Name == value.Name) && (rt.ReportTermType == value.ReportTermType)); };
				ReportTerm foundTerm = _reportTerms.Find(p);
				if (foundTerm != null)
					foundTerm.SortOrder = ReportTermSortOrder.Secondary;
			}
		}

		public List<int> Visibility
		{
			get
			{
				if (_reportTerms == null)
					return null;
				List<int> visibility = new List<int>();
				for (int nIndex = 0; nIndex < _reportTerms.Count; nIndex++)
				{
					if (_reportTerms[nIndex].Visible ?? false)
						visibility.Add(nIndex);
				}
				return visibility;
			}
			set 
			{ 
				if (value == null)
					return;
				if (_reportTerms == null)
					return;
				foreach (ReportTerm reportTerm in _reportTerms)
				{
					reportTerm.Visible = false;
				}
				foreach (int nIndex in value)
				{
					if (_reportTerms.Count > nIndex)
						_reportTerms[nIndex].Visible = true;
				}
			}
		}

		public bool IsValid
		{
			get {
				if (_reportTerms == null)
					return false;
				if (_reportTerms.Count == 0)
					return false;
				return true; 
				}
		}

		#endregion

		#region Constructors

		public Report()
		{
            _searchCriteria = new SearchCriteria();
		}

		public Report(Guid itatSystemId, Guid reportID, string sName, string sDescription)
		{
			_itatSystemId = itatSystemId;
			_reportId = reportID;
			_name = sName;
			_description = sDescription;
            _searchCriteria = new SearchCriteria();
		}

		public static Report Create(string xml, ITATSystem system)
		{
			if (xml == null)
				return null;
			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(xml);

			XmlNode nodeReport = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_Report));
			if (nodeReport == null)
				return null;

            Report report = new Report();
			report._name = Utility.XMLHelper.GetAttributeString(nodeReport, XMLNames._A_Name);
			report._reportId = new Guid(Utility.XMLHelper.GetAttributeString(nodeReport, XMLNames._A_ReportID));
			report._itatSystemId = new Guid(Utility.XMLHelper.GetAttributeString(nodeReport, XMLNames._A_ITATSystemID));

			XmlNode descriptionNode = nodeReport.SelectSingleNode(XMLNames._E_Description);
			report._description = Utility.XMLHelper.GetText(descriptionNode);

            report._searchCriteria = new SearchCriteria(nodeReport);

			XmlNodeList listTerms = nodeReport.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Terms, XMLNames._E_Term));
			if (listTerms != null)
			{
				if (listTerms.Count > 0)
				{
					report._reportTerms = new List<ReportTerm>(listTerms.Count);
					foreach (XmlNode nodeTerm in listTerms)
					{

						ReportTerm reportTerm = new ReportTerm(nodeTerm);
						report._reportTerms.Add(reportTerm);
					}
				}
			}

			return report;
		}

		public static Report Create(Guid reportID, ITATSystem system)
		{
            return Create(GetReportXml(reportID), system);
		}

		//This call is meant to be used in place of code such as 'reportTerms = new List<ReportTerm>(count)'
		//This is a standardized way of creating a list of ReportTerm that already includes the '3 special terms'
        public static List<ReportTerm> CreateReportTerms(List<Guid> AccessibleTermGroupIDs, List<Term> terms, string ManagedItemNumber, string ManagedItemStatus, string ManagedItemName)
		{
			List<ReportTerm> reportTerms;
			int nSize = 0;
			if (terms != null)
				nSize = terms.Count;
			if (nSize > 0)
			{
				nSize += 3;
				reportTerms = new List<ReportTerm>(nSize);
				reportTerms.Add(new ReportTerm(ReportTermType.ManagedItemNumber.ToString(), ReportTermType.ManagedItemNumber, ManagedItemNumber, false));
				reportTerms.Add(new ReportTerm(ReportTermType.Status.ToString(), ReportTermType.Status, ManagedItemStatus, false));
				reportTerms.Add(new ReportTerm(ReportTermType.TemplateName.ToString(), ReportTermType.TemplateName, ManagedItemName, false));

				if (terms != null)
				{
					foreach (Term term in terms)
					{
                        if (ReportTerm.GetReportTermType(term.TermType) != ReportTermType.NotReported)
                            reportTerms.Add(new ReportTerm(AccessibleTermGroupIDs, term));
					}
				}
			}
			else
			{
				reportTerms = new List<ReportTerm>(0);
			}

			return reportTerms;
		}

		#endregion

		#region Build XML

		public string GetXml()
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(string.Format("<?xml version='1.0' encoding='UTF-8' ?><{0} />",XMLNames._E_Report));
			XmlElement nodeReport = xmlDoc.DocumentElement;
			Build(xmlDoc, nodeReport);
			return xmlDoc.OuterXml;
		}

		private void Build(XmlDocument xmlDoc, XmlNode node)
		{
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Name, _name);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_ITATSystemID, _itatSystemId.ToString());
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_ReportID, _reportId.ToString());

			XmlNode elementDescription = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Description, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.AddText(xmlDoc, elementDescription, _description);
			node.AppendChild(elementDescription);

			XmlNode elementRules = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Rules, XMLNames._M_NameSpaceURI);
			_searchCriteria.Build(xmlDoc, elementRules);
			node.AppendChild(elementRules);

			XmlNode elementTerms = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Terms, XMLNames._M_NameSpaceURI);
			if (_reportTerms != null)
			{
				foreach (ReportTerm reportTerm in _reportTerms)
				{
					XmlNode elementTerm = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Term, XMLNames._M_NameSpaceURI);
					reportTerm.Build(xmlDoc, elementTerm);
					elementTerms.AppendChild(elementTerm);
				}
			}
			node.AppendChild(elementTerms);
		}

		#endregion

		#region Define Terms


		public bool DefineTerms(SearchResults searchResults)
		{
			//Get a unique collection of Templates from the search results ManagedItems
			List<Template> templates = new List<Template>();
			foreach (SearchResultItem searchResultItem in searchResults)
			{
				Predicate<Template> p = delegate(Template t) { return t.ID == searchResultItem.TemplateId; };
				Template foundTemplate = templates.Find(p);
				if (foundTemplate == null)
				{
					Template template = new Business.Template(searchResultItem.TemplateId, DefType.Final);
					templates.Add(template);
				}
			}

			//Get a unique collection of Terms from the list of Templates
			List<Term> terms = new List<Term>();
			foreach (Template template in templates)
			{
				foreach (Term templateTerm in template.BasicTerms)
				{
					//exclude External Terms from this list
					switch (templateTerm.TermType)
					{
						case TermType.None:
							break;
						default:
							Predicate<Term> p = delegate(Term t) { return ((t.Name == templateTerm.Name) && (t.TermType == templateTerm.TermType)); };
							Term foundTerm = terms.Find(p);
							if (foundTerm == null)
								terms.Add(templateTerm);
							break;
					}
				}
			}

			//Now build the _reportTerms collection.  If it was previously defined, want to keep the original
			//order, but also want to incorporate any new basic-terms found in the 'terms' collection.
            if (_reportTerms != null)
            {
                List<ReportTerm> newReportTerms = new List<ReportTerm>(terms.Count);

                foreach (ReportTerm reportTerm in _reportTerms)
                {
                    if (ReportTerm.GetTermType(reportTerm.ReportTermType) == null)
                    {
                        //This is a 'special' term - so keep it
                        newReportTerms.Add(reportTerm);
                    }
                    else
                    {
                        TermType termType = (TermType)Enum.Parse(typeof(TermType), ReportTerm.GetTermType(reportTerm.ReportTermType));
                        Predicate<Term> p = delegate(Term t) { return ((t.Name == reportTerm.Name) && (t.TermType == termType)); };
                        Term foundTerm = terms.Find(p);
                        if (foundTerm != null)
                        {
                            //This is a reportTerm found in the basic terms, so keep it.
                            newReportTerms.Add(reportTerm);
                            terms.Remove(foundTerm);
                        }
                        else
                        {
                            //This report term was not found in the basic terms collection, so it will not be saved.
                            //TODO - In the future, may want to warn the user that this is the case.
                        }
                    }
                }
                //All done - now add new ReportTerms for all the basic terms that are left (not previously found in ReportTerms)
                foreach (Term term in terms)
                {
                    if (ReportTerm.GetReportTermType(term.TermType) != ReportTermType.NotReported)
                        newReportTerms.Add(new ReportTerm(null, term));
                }

                _reportTerms = newReportTerms;
            }
            else
            {
                //Convert the unique list of BasicTerms into a list of ReportTerms
                _reportTerms = CreateReportTerms(null,terms, null, null, null);
            }

			SetDisplayNames();
			return true;
		}


		//Need to review the resulting list of ReportTerms - if a duplicate name is found, need to modify the ReportTerm's 'display name'
		private bool SetDisplayNames()
		{
			//TODO Report - Incorporate this call later if deemed necessary.
			return false;

			//if (_reportTerms == null)
			//   return true;

			//if (_reportTerms.Count == 0)
			//   return true;

			//foreach (ReportTerm reportTerm in _reportTerms)
			//{
			//   //Mark all reportTerm's as unmodified
			//   reportTerm.DisplayName = null;
			//}

			//foreach (ReportTerm reportTerm in _reportTerms)
			//{
			//   Predicate<ReportTerm> p = delegate(ReportTerm rt) { return (rt.Name == reportTerm.Name) && string.IsNullOrEmpty(rt.DisplayName); };
			//   List<ReportTerm> foundTerms = _reportTerms.FindAll(p);
			//   if (foundTerms != null)
			//   {
			//      if (foundTerms.Count > 1)
			//      {
			//         foreach (ReportTerm foundTerm in foundTerms)
			//         {
			//            //TODO - Use a different format here, if needed
			//            foundTerm.DisplayName = string.Format("{0}-{1}", foundTerm.Name, foundTerm.ReportTermType.ToString());
			//         }
			//      }
			//   }
			//}
			//return true;
		}


		//Used by the interface
		public DataSet GetTerms()
		{
			//Columns = TermName, TermType, Visible
			DataTable dt = new DataTable();
			dt.Columns.Add("Index");
			dt.Columns.Add("TermName");
			dt.Columns.Add("TermType");
			dt.Columns.Add("Visible", typeof(bool));

			if (_reportTerms != null)
			{
				int nIndex = 0;
				foreach (ReportTerm reportTerm in _reportTerms)
				{
					DataRow dr = dt.NewRow();
					dr["Index"] = nIndex++;
					dr["TermName"] = reportTerm.Name;
					dr["TermType"] = reportTerm.ReportTermType.ToString();
					dr["Visible"] = reportTerm.Visible ?? false;
					dt.Rows.Add(dr);
				}
			}

			DataSet ds = new DataSet();
			ds.Tables.Add(dt);
			return ds;
		}

		public bool SetPrimary(string name, string sReportTermType)
		{
			ReportTermType reportTermType;
			try
			{
				reportTermType = (ReportTermType)Enum.Parse(typeof(ReportTermType), sReportTermType);
			}
			catch
			{
				reportTermType = ReportTermType.None;
			}

			try
			{
				PrimaryTerm = new ReportTerm(name, reportTermType, "", false);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public bool SetSecondary(string name, string sReportTermType)
		{
			ReportTermType reportTermType;
			try
			{
				reportTermType = (ReportTermType)Enum.Parse(typeof(ReportTermType), sReportTermType);
			}
			catch
			{
				reportTermType = ReportTermType.None;
			}

			try
			{
				SecondaryTerm = new ReportTerm(name, reportTermType, "", false);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public bool Save()
		{
			return UpdateReport(_reportId ?? Guid.Empty,
								_itatSystemId ?? Guid.Empty,
								_name,
								_description,
								GetXml());
		}

		#endregion

		#region Static Database Calls



		public static string NewReportName(Guid itatSystemId, string oldName)
		{
			string baseNewName = string.Concat("Copy of ", oldName);
			string newName = baseNewName;
			//TODO: if there already exists a report called newName, append (2) or (3) or ... to the end to make it unique
			DataRowCollection rows = Business.Report.GetSystemReports(itatSystemId).Tables[0].Rows;
			if (rows.Count > 0)
			{
				List<string> reportNames = new List<string>(rows.Count);
				for (int i = 0; i < rows.Count; i++)
					reportNames.Add((string)rows[i]["ReportName"]);
				int suffixIndex = 2;
				while (reportNames.Contains(newName))
					newName = string.Format("{0} ({1})", baseNewName, suffixIndex++);
			}
			return newName;
		}


		private static string GetReportXml(Guid reportID)
		{
			string sReportXml = null;
			DataSet ds;
			try
			{
				ds = Data.Report.GetReport(null, reportID);
				sReportXml = (string)ds.Tables[0].Rows[0][Data.DataNames._C_ReportConfigXML];
			}
			catch
			{
				return null;
			}
			return sReportXml;
		}

		public static DataSet GetSystemReports(Guid itatSystemID)
		{
			return Data.Report.GetReport(itatSystemID, null);
		}

		public static DataSet GetReport(Guid reportID)
		{
			return Data.Report.GetReport(null, reportID);
		}

		public static bool UpdateReport(
									Guid reportID, 
									Guid itatSystemID, 
									string reportName, 
									string reportDescription,
									string reportConfigXML)
		{
			if (string.IsNullOrEmpty(reportDescription))
				reportDescription = string.Empty;
			return Data.Report.UpdateReport(reportID, 
									itatSystemID, 
									reportName, 
									reportDescription,
									reportConfigXML);
		}

		public static bool DeleteReport(Guid reportID)
		{
            return Data.Report.DeleteReport(reportID);
		}

		public static bool CopyReport(Guid oldReportID,
												Guid newReportID,
												string newName, 
												string newDescription,
												string configXml)
		{
            return Data.Report.CopyReport(oldReportID, newReportID, newName, newDescription, configXml);
		}

		#endregion

		#region Sorting

		public ReportManagedItemSort GetReportManagedItemSort(SearchResults searchResults, List<string> userRoles)
		{
			List<ReportManagedItem> reportManagedItems = new List<ReportManagedItem>(searchResults.Count);
			foreach (SearchResultItem searchResultItem in searchResults)
			{
				//Note - the _reportTerms collection should already contain the '3 special terms'
                ReportManagedItem reportManagedItem = new ReportManagedItem(searchResultItem.ManagedItemId, _reportTerms, userRoles);
				reportManagedItems.Add(reportManagedItem);
			}
			
			ReportManagedItemSort reportManagedItemSort = new ReportManagedItemSort(reportManagedItems);
			return reportManagedItemSort;
		}

		#endregion

	}
}
