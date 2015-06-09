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
	public class ReportManagedItem
	{
		#region private members
		private List<ReportTerm> _reportTerms;
		#endregion

		#region Properties

		public List<ReportTerm> ReportTerms
		{
			get { return _reportTerms; }
			set { _reportTerms = value; }
		}

		#endregion

		#region Constructors

		//The ordering of the terms is determined by orderReportTerm, which is from Report.ReportTerms
		//The orderReportTerm list should already contain the '3 special terms'.
        //Pass in the userRoles for determining access.  If userRoles is null, then no restriction.
		public ReportManagedItem(Guid managedItemID, List<ReportTerm> orderReportTerm, List<string> userRoles)
		{
			ManagedItem managedItem = ManagedItem.GetBasic(managedItemID);
            _reportTerms = Report.CreateReportTerms(managedItem.AccessibleTermGroupIDs(userRoles), managedItem.BasicTerms, managedItem.ItemNumber, managedItem.State.Status, managedItem.Name);
			ReOrder(orderReportTerm);
		}

		#endregion

		#region Ordering

		//This call will 'reorder' the current _reportTerms listing.  it will also ensure that
		//the returned List<ReportTerm> will contain a term for each term found in orderReportTerms.
		public bool ReOrder(List<ReportTerm> orderReportTerms)
		{
			if (_reportTerms == null)
				return true;
			//This is one case where we are not compelled to call Report.CreateReportTerms
			if (_reportTerms.Count > 0)
			{
				List<ReportTerm> newReportTerms = new List<ReportTerm>(_reportTerms.Count);
				foreach (ReportTerm orderReportTerm in orderReportTerms)
				{
					ReportTerm foundTerm = FindReportTerm(orderReportTerm);
					if (foundTerm == null)
					{
						//Add a 'blank' term for the ones that are not found.
						//Not every term will be present in every ManagedItem.
						newReportTerms.Add(new ReportTerm(orderReportTerm.Name, orderReportTerm.ReportTermType, "", orderReportTerm.Visible ?? false));
					}
					else
					{
						foundTerm.Visible = orderReportTerm.Visible;
						newReportTerms.Add(foundTerm);
					}
				}
				_reportTerms = newReportTerms;
			}
			return true;
		}


		public ReportTerm FindReportTerm(ReportTerm reportTerm)
		{
			if (reportTerm == null)
				return null;
			if (_reportTerms == null)
				return null;
			Predicate<ReportTerm> p = delegate(ReportTerm rt) { return ((rt.Name == reportTerm.Name) && (rt.ReportTermType == reportTerm.ReportTermType)); };
			return _reportTerms.Find(p);
		}

		#endregion
	}

	[Serializable]
	public class ReportManagedItemSort : List<ReportManagedItem>
	{
		#region Constructors

		private ReportTerm _primaryTerm;
		private ReportTerm _secondaryTerm;
		private bool _sortAscending;

		public ReportManagedItemSort(List<ReportManagedItem> listReportManagedItem)
		{
			this.Clear();
			foreach (ReportManagedItem reportManagedItem in listReportManagedItem)
				this.Add(reportManagedItem);
		}

		#endregion

		//Updates the visibility setting for all ReportManagedItems
		private bool UpdateColumnOrder(List<ReportTerm> reportTerms)
		{
			foreach (ReportManagedItem reportManagedItem in this)
			{
				reportManagedItem.ReOrder(reportTerms);
			}
			return true;
		}

		//Used to convert this collection of ReportManagedItem's into a dataset for use in a gridview
		public DataSet GetDisplay()
		{
			DataSet ds = new DataSet();
			if (this.Count > 0)
			{
				DataTable dt = new DataTable();
				foreach (ReportTerm reportTerm in this[0].ReportTerms)
				{
					if (reportTerm.Visible ?? false)
						dt.Columns.Add(reportTerm.DisplayName);
				}

				foreach (ReportManagedItem reportManagedItem in this)
				{
					DataRow dr = dt.NewRow();
					foreach (ReportTerm reportTerm in reportManagedItem.ReportTerms)
					{
						if (reportTerm.Visible ?? false)
							dr[reportTerm.DisplayName] = reportTerm.Value;
					}
					dt.Rows.Add(dr);
				}
				ds.Tables.Add(dt);
			}

			return ds;
		}

		private int CompareDate(ReportTerm a, ReportTerm b)
		{
			if (a.DateTimeValue.HasValue)
			{
				if (b.DateTimeValue.HasValue)
				{
					return a.DateTimeValue.Value.CompareTo(b.DateTimeValue.Value);
				}
				else
				{
					return -1;		//b is null
				}
			}
			else
			{
				if (b.DateTimeValue.HasValue)
				{
					return 1;		//a is null
				}
				else
				{
					return 0;		//Both null
				}
			}
		}

		private int CompareText(ReportTerm a, ReportTerm b)
		{
			if ((a.Value == null) && (b.Value == null))
				return 0;
			if (a.Value == null)
				return 1;
			if (b.Value == null)
				return -1;
			return a.Value.CompareTo(b.Value);
		}

		//If _secondaryTerm is defined, then _primaryTerm must also be defined.
		//Okay if only _primaryTerm is defined.
		private int CompareTerms(ReportManagedItem a, ReportManagedItem b)
		{
			if (_primaryTerm == null)
			{
				if (_secondaryTerm != null)
					throw new Exception(string.Format("Secondary term defined as '{0}', type '{1}', but Primary term is not defined", _secondaryTerm.Name, _secondaryTerm.ReportTermType.ToString()));
				else
					return 0;
			}

			ReportTerm a1 = a.FindReportTerm(_primaryTerm);
			ReportTerm b1 = b.FindReportTerm(_primaryTerm);
			if ((a1 == null) || (b1 == null))
				throw new Exception(string.Format("Report terms '{0}' type '{1}' not found during sorting", _primaryTerm.Name, _primaryTerm.ReportTermType.ToString()));

			ReportTerm a2 = a.FindReportTerm(_secondaryTerm);
			ReportTerm b2 = b.FindReportTerm(_secondaryTerm);

			if (!_sortAscending)
			{
				//Swap the terms
				ReportTerm temp_a1 = a1;
				ReportTerm temp_b1 = b1;

				a1 = a2;
				b1 = b2;
				a2 = temp_a1;
				b2 = temp_b1;
			}

			int a1_vs_b1 = IsTextType(_primaryTerm) ? CompareText(a1, b1) : CompareDate(a1, b1);

			if (_secondaryTerm == null)
				return a1_vs_b1;
			else
			{
				if (a1_vs_b1 == 0)
					return IsTextType(_secondaryTerm) ? CompareText(a2, b2) : CompareDate(a2, b2);
				else
					return a1_vs_b1;
			}
		}

		private bool IsTextType(ReportTerm reportTerm)
		{
			switch (reportTerm.ReportTermType)
			{
				case ReportTermType.Text:
				case ReportTermType.None:
				case ReportTermType.MSO:
				case ReportTermType.Facility:
				case ReportTermType.PickList:
				case ReportTermType.Link:
				case ReportTermType.ManagedItemNumber:
				case ReportTermType.Status:
				case ReportTermType.TemplateName:
					return true;

				case ReportTermType.Date:
				case ReportTermType.Renewal:
					return false;
			}
			throw new Exception(string.Format("IsTextType call encountered type '{0}' not handled", reportTerm.ReportTermType.ToString()));
		}

		private bool UpdateRowOrder(ReportTerm primaryTerm, ReportTerm secondaryTerm, bool sortAscending)
		{
			_primaryTerm = primaryTerm;
			_secondaryTerm = secondaryTerm;
			_sortAscending = sortAscending;
			Sort(CompareTerms);
			return true;
		}

		public bool Update(List<ReportTerm> reportTerms, ReportTerm primaryTerm, ReportTerm secondaryTerm, bool sortAscending)
		{
			UpdateColumnOrder(reportTerms);
			UpdateRowOrder(primaryTerm, secondaryTerm, sortAscending);
			return true;
		}
	}

}
