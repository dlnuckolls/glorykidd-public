using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{
	public enum FacilitySortField
	{
		FacilityId,
		SapId,
		FacilityName
	}

	[Serializable]
	public class FacilitySort
	{

		#region Private fields
		private FacilitySortField _field;
		#endregion

		#region static members and methods
		//private static Dictionary<FacilitySortField, string> _dbFields;
		private static Dictionary<FacilitySortField, string> _displayValues;
		private static int enumCount = 2;

		static FacilitySort()
		{
			InitializeDictionaries();
		}

		private static void InitializeDictionaries()
		{
			//_dbFields = new Dictionary<FacilitySortField, string>(enumCount);
			//_dbFields.Add(FacilitySortField.FacilityId, "facility_id");
			//_dbFields.Add(FacilitySortField.SapId, "facility_name");
			//_dbFields.Add(FacilitySortField.FacilityName, "sap_facility_id");

			_displayValues = new Dictionary<FacilitySortField, string>(enumCount);
			_displayValues.Add(FacilitySortField.FacilityId, "By Facility ID");
			_displayValues.Add(FacilitySortField.SapId, "By SAP ID");
			_displayValues.Add(FacilitySortField.FacilityName, "By Facility Name");
		}

		public static string DisplayValue(FacilitySortField facilitySortField)
		{
			return _displayValues[facilitySortField];
		}

		#endregion


		#region constructors

		public FacilitySort()
		{
			_field = FacilitySortField.FacilityName;
		}

		public FacilitySort(FacilitySortField facilitySortField)
		{
			_field = facilitySortField;
		}

		#endregion


		#region properties
		public FacilitySortField Field
		{
			get { return _field; }
			set { _field = value; }
		}

		//public string DbField
		//{
		//   get { return _dbFields[_field]; }
		//}


		#endregion


	}

}
