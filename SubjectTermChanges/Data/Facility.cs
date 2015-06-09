using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using Kindred.Common.Data;

namespace Kindred.Knect.ITAT.Data
{

	[Serializable]
	public class FacilityDataRow
	{
		private int _facilityId;
		private string _facilityName;
		private string _sapFacilityId;
		private string _facilityOpPath;
		private int _facilityTypeId;
		private string _facilityType;
		private int _facilityStatusId;
		private int _level;
		private string _address;
		private string _city;
		private string _county;
		private string _state;
		private string _stateCode;
		private string _zip;
		private string _areaCode;
		private string _phone;
		private string _fax;
		private string _legalEntityName;


		public int FacilityId
		{
			get { return _facilityId; }
		}

		public string FacilityName
		{
			get { return _facilityName; }
		}

		public string SapFacilityId
		{
			get { return _sapFacilityId; }
		}

		public string SapIdPlusName
		{
			get { return _sapFacilityId + " - " + _facilityName; }
		}

		public string FacilityOpPath
		{
			get { return _facilityOpPath; }
		}

		public int FacilityTypeId
		{
			get { return _facilityTypeId; }
		}

		public string FacilityType
		{
			get { return _facilityType; }
		}

		public int FacilityStatusId
		{
			get { return _facilityStatusId; }
		}

		public int Level
		{
			get { return _level; }
		}

		public string Address
		{
			get { return _address; }
			set { _address = value; }
		}

		public string City
		{
			get { return _city; }
			set { _city = value; }
		}

		public string County
		{
			get { return _county; }
			set { _county = value; }
		}

		public string State
		{
			get { return _state; }
			set { _state = value; }
		}

		public string StateCode
		{
			get { return _stateCode; }
			set { _stateCode = value; }
		}

		public string Zip
		{
			get { return _zip; }
			set { _zip = value; }
		}

		public string AreaCode
		{
			get { return _areaCode; }
			set { _areaCode = value; }
		}

		public string Phone
		{
			get { return _phone; }
			set { _phone = value; }
		}

		public string Fax
		{
			get { return _fax; }
			set { _fax = value; }
		}

		public string LegalEntityName
		{
			get { return _legalEntityName; }
			set { _legalEntityName = value; }
		}


		public string IndentedName
		{
			get {
				System.Text.StringBuilder sb = new StringBuilder();
				for (int i = 0; i<3 * _level; i++)
					sb.Append((char)160);
				sb.Append(_facilityName);
				return sb.ToString(); 
			}
		}


		public FacilityDataRow(int facilityId, string facilityName, string sapFacilityId, string facilityOpPath, int facilityTypeId, string facilityType, int facilityStatusId, int level, string address, string city, string county, string state, string stateCode, string zip, string areaCode, string phone, string fax, string legalEntityName)
		{
			_facilityId = facilityId;
			_facilityName = facilityName;
			_sapFacilityId = sapFacilityId;
			_facilityOpPath = facilityOpPath;
			_facilityTypeId = facilityTypeId;
			_facilityType = facilityType;
			_facilityStatusId = facilityStatusId;
			_level = level;
			_address = address;
			_city = city;
			_county = county;
			_state = state;
			_stateCode = stateCode;
			_zip = zip;
			_areaCode = areaCode;
			_phone = phone;
			_fax = fax;
			_legalEntityName = legalEntityName;
		}


		public static int CompareByFacId(FacilityDataRow a, FacilityDataRow b)
		{
			return a.FacilityId.CompareTo(b.FacilityId);
		}

		public static int CompareByName(FacilityDataRow a, FacilityDataRow b)
		{
			return string.Compare(a.FacilityName, b.FacilityName);
		}

		public static int CompareBySapId(FacilityDataRow a, FacilityDataRow b)
		{
			return string.Compare(a.SapFacilityId , b.SapFacilityId);
		}

	}



	public static class Facility
	{

		public static int CorporateFacilityId
		{
			get { return DataNames._FACID_CORPORATE; }
		}

		/// <summary>
		/// Returns all Kindred facilities (all facilities under "Corporate" -- facid=452)
		/// </summary>
		/// <returns></returns>
		public static DataSet AllFacilities(bool includeChildren)
		{
			return AllFacilities(DataNames._FACID_CORPORATE, includeChildren);
		}

		/// <summary>
		/// Returns all Kindred facilities "under" the specified facility
		/// </summary>
		/// <param name="facilityId"></param>
		/// <returns></returns>
        public static DataSet AllFacilities(int facilityId, bool includeChildren)
		{
			//NOTE: This proc uses the fully-qualified sproc name to ensure that the FacilityMaster database from the same db server as ITAT runs on is used.
			//   This is so that the ITAT account only needs access to that particular FM database
			string procName = "FacilityMaster.dbo.GetHierarchyAll";
            //@@@@@ Note - Changing this would require modifying 'FacilityMaster.dbo.GetHierarchy' to return facility.status
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				DataSet ds;
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[3];
					p[0] = new SqlParameter("@FID", SqlDbType.Int);
					p[0].Value = facilityId;
					p[1] = new SqlParameter("@FAID", SqlDbType.VarChar, 20);
					p[1].Value = null;
					p[2] = new SqlParameter("@Tree", SqlDbType.Int);
					p[2].Value = (includeChildren ? 1 : 0);
					ds = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, procName, p);
				}
				finally
				{
					cnn.Close();
				}
				return ds;
			}
		}


		/// <summary>
		/// Returns info on all Kindred facilities
		/// </summary>
		/// <returns></returns>
        public static DataSet GetFacilityInfoAll()
		{
			string procName = "GetFacilityInfoAll";
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				DataSet ds;
				cnn.Open();
				try
				{
					ds = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, procName);
				}
				finally
				{
					cnn.Close();
				}
				return ds;
			}
		}


	}
}
