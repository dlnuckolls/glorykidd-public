using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Kindred.Common.Data;


namespace Kindred.Knect.ITAT.Interfaces
{

	/// <summary>
	/// A class to retrieve data from the MSO database
	/// </summary>
	public class MSO
	{


		#region private members
		private string _criteria;
		private string _msoFacCode;
		#endregion


		#region  Properties
		public string Criteria
		{
			get { return _criteria; }
			set { _criteria = value; }
		}
		#endregion


		#region constructors
		public MSO()
		{
			_criteria = string.Empty;
			_msoFacCode = string.Empty;
		}

		public MSO(string criteria)
		{
			_criteria = criteria;
			_msoFacCode = string.Empty;
		}
		#endregion



		#region public methods

		public bool ValidateCriteria()
		{
			if (string.IsNullOrEmpty(Criteria))
				return false;
			int facilityId;
			if (int.TryParse(Criteria, out facilityId))
			{
				_msoFacCode = GetMsoFacCode(facilityId);
				return (!string.IsNullOrEmpty(_msoFacCode));
			}
			else
				return false;
		}

		
		public DataSet GetData()
		{
			if (!ValidateCriteria())
				throw new System.InvalidOperationException(string.Format("Criteria has not been set or is invalid.   Criteria = '{0}'.", Criteria));
			DataSet dsIndividuals = null;
			DataSet dsGroups = null;
			using (SqlConnection sqlConn = Kindred.Common.Data.SqlHelper.GetConnection("MSO"))
			{
				SqlParameter[] p = new SqlParameter[1];
				p[0] = new SqlParameter("@FacCode", SqlDbType.VarChar, 3);
				p[0].Value = _msoFacCode;

				dsGroups = SqlHelper.ExecuteDataset(sqlConn, "GetPhysicianGroups", p);
				dsIndividuals = SqlHelper.ExecuteDataset(sqlConn, "GetIndividualPhysicians", p);
			}

			DataTable resultset = new DataTable("MSOProviders");
			resultset.Columns.Add("Name", typeof(string));
			resultset.Columns.Add("Address 1", typeof(string));
			resultset.Columns.Add("Address 2", typeof(string));
			resultset.Columns.Add("City", typeof(string));
			resultset.Columns.Add("ST", typeof(string));
			resultset.Columns.Add("Zip", typeof(string));

			foreach (DataRow row in dsGroups.Tables[0].Rows)
			{
				DataRow newRow = resultset.NewRow();
				newRow.ItemArray = new object[] {
						row["office_name"].ToString(),
						row["address_1"].ToString(),
						row["address_2"].ToString(),
						row["city"].ToString(),
						row["state"].ToString(),
						row["zip_code"].ToString()
				};
				resultset.Rows.Add(newRow);
			}

			foreach (DataRow row in dsIndividuals.Tables[0].Rows)
			{
				DataRow newRow = resultset.NewRow();
				newRow.ItemArray = new object[] {
						row["first_name"].ToString() + " " + row["middle_initial"].ToString() + " " + row["last_name"].ToString(),
						row["address_1"].ToString(),
						row["address_2"].ToString(),
						row["city"].ToString(),
						row["state"].ToString(),
						row["zip_code"].ToString()
				};
				resultset.Rows.Add(newRow);
			}

			DataSet rtn = new DataSet();
			rtn.Tables.Add(resultset);
			return rtn;				
		}


		#endregion


		#region private methods

        private string GetMsoFacCode(int facilityId)
		{
			int msoFacId = facilityId;
			string msoFacCode = string.Empty;

            using (SqlConnection sqlConn = Common.Data.SqlHelper.GetConnection(Data.Common.DataFactoryAppName))
			{
				SqlParameter[] p = new SqlParameter[1];
				p[0] = 	new SqlParameter("@FacilityID", SqlDbType.Int);
				p[0].Value = facilityId;
				object result = SqlHelper.ExecuteScalar(sqlConn, "IFC_MSO_GetSatelliteParentFacility", p);
				if (result != null)
					msoFacId = (int)result;
			}
			using (SqlConnection sqlConn2 = Kindred.Common.Data.SqlHelper.GetConnection("FM"))
			{
				SqlParameter[] p = new SqlParameter[2];
				p[0] = new SqlParameter("@facility_id", SqlDbType.Int);
				p[0].Value = msoFacId;
				p[1] = new SqlParameter("@facility_alias_type_code", SqlDbType.Int);
				p[1].Value = 3;   //3 == SAP Alias
				SqlDataReader dr = SqlHelper.ExecuteReader(sqlConn2, "Fetch_Facility_Alias_Id", p);
				try
				{
					if (dr.Read())
						msoFacCode = ((string)dr["facility_alias_id"]).Substring(1, 3);
				}
				finally
				{
					dr.Close();
				}
			}
			return msoFacCode;
		}


		#endregion


	}

}
