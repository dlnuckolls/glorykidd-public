using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Kindred.Common.Data;

namespace Kindred.Knect.ITAT.Data
{
	public static class Common
	{
		public static string DataFactoryAppName = "ITAT";
        private static System.Transactions.TransactionScopeOption _transactionScopeOption = System.Transactions.TransactionScopeOption.Suppress;

        //When this static class is first initialized, determine whether we can use the TransactionScope object (TransactionScopeOption.Required),
		// or not (TransactionScopeOption.Suppress).  
		static Common()
		{
			try
			{
				using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
				{
					Guid g = GetPersonId("");
					ts.Complete();
					_transactionScopeOption = System.Transactions.TransactionScopeOption.Required;
				}
			}
			catch
			{
				_transactionScopeOption = System.Transactions.TransactionScopeOption.Suppress;
			}
		}


		public static System.Transactions.TransactionScopeOption TransactionScopeOption
		{
			get { return _transactionScopeOption; }
		}

		public static DataSet GetSessionParameter(Guid sessionParameterID)
		{
			DataSet rtn = null;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					p[0] = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
					p[0].Direction = ParameterDirection.ReturnValue;
					p[1] = new SqlParameter("@SessionParameterID", SqlDbType.UniqueIdentifier);
					p[1].Value = sessionParameterID;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetSessionParameter", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve SessionParameter for id={0},   Error: {1}", sessionParameterID.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}


        public static Guid InsertSessionParameter(string Value, int purgeKeepDays)
		{
			Guid rtn = Guid.Empty;

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[3];
					p[0] = new SqlParameter("@Value", SqlDbType.Text);
					p[0].Value = Value;
                    p[1] = new SqlParameter("@PurgeKeepDays", SqlDbType.Int);
                    p[1].Value = purgeKeepDays;
					p[2] = new SqlParameter("@SessionParameterID", SqlDbType.UniqueIdentifier);
					p[2].Direction = ParameterDirection.Output;
					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertSessionParameter", p);
					rtn = (Guid)p[2].Value;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to insert Value for SessioParameter,   Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

		public static Guid InsertUpdateSessionParameter(Guid oldSessionParameterID, string Value)
		{
			Guid rtn = Guid.Empty;

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[3];
					p[0] = new SqlParameter("@OldSessionParameterID", SqlDbType.UniqueIdentifier);
					if (oldSessionParameterID.Equals(Guid.Empty))
						p[0].Value = null;
					else
						p[0].Value = oldSessionParameterID;
					p[1] = new SqlParameter("@Value", SqlDbType.Text);
					p[1].Value = Value;
					p[2] = new SqlParameter("@NewSessionParameterID", SqlDbType.UniqueIdentifier);
					p[2].Direction = ParameterDirection.Output;
					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertUpdateSessionParameter", p);
					rtn = (Guid)p[2].Value;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to insert/update Value for SessioParameter,   Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}



		public static Guid GetPersonId(string loginId)
		{
			Guid rtn = Guid.Empty;
			using (SqlConnection cnn = SqlHelper.GetConnection("People"))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					p[0] = new SqlParameter("@domainID", SqlDbType.VarChar, 20);
					p[0].Value = loginId;
					p[1] = new SqlParameter("@iErrorCode", SqlDbType.Int);
					p[1].Direction = ParameterDirection.Output;
					DataSet ds = SqlHelper.ExecuteDataset(cnn, "Public_Person_SelectByDomainID", p);
					if (ds.Tables[0].Rows.Count > 0)
						rtn = new Guid(ds.Tables[0].Rows[0]["PersonId"].ToString());
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Error calling Public_Person_SelectByDomainID.   Message: {0}", ex.Message));
				}
			}
			return rtn;
		}


        internal static DataTable GetRoles(IEnumerable<string> roles)
        {
            DataTable dtRoles = new DataTable();
            dtRoles.Columns.Add("Role");
            if (roles != null)
                foreach (string role in roles)
                {
					dtRoles.Rows.Add(role);
                }
            return dtRoles;
        }


		internal static DataTable GetFacilities(IEnumerable<int> facilityIds)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("FacilityID");
			if (facilityIds != null)
				foreach (int facId in facilityIds)
				{
					dt.Rows.Add(facId);
				}
			return dt;
		}

		internal static DataTable GetStatuses(IEnumerable<string> statuses)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Status");
			if (statuses != null)
				foreach (string status in statuses)
				{
					dt.Rows.Add(status);
				}
			return dt;
		}

        internal static DataTable GetIDs(IEnumerable<Guid> guids)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            foreach (Guid guid in guids)
            {
                dt.Rows.Add(guid);
            }
            return dt;
        }

	}

}
