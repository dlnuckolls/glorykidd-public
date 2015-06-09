using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Kindred.Common.Data;
using System.Transactions;


namespace Kindred.Knect.ITAT.Data
{
	public static class ExternalTerm
	{
		public static DataTable GetValues(Guid managedItemId, string interfaceConfigName)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				SqlParameter[] p = new SqlParameter[2];
				p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
				p[0].Value = managedItemId;
				p[1] = new SqlParameter("@InterfaceConfigName", SqlDbType.VarChar, 100);
				p[1].Value = interfaceConfigName;
				return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetExternalTermValues", p).Tables[0];
			}
		}


		public static void SaveValues(Guid managedItemId, string interfaceConfigName, string valueDescriptions)
		{
			using (TransactionScope transScope = new TransactionScope(Common.TransactionScopeOption))
			{
                using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
				{
					cnn.Open();
					SqlParameter[] p = new SqlParameter[3];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemId;
					p[1] = new SqlParameter("@InterfaceConfigName", SqlDbType.VarChar, 100);
                    if (string.IsNullOrEmpty(interfaceConfigName))
					    p[1].Value = DBNull.Value;
                    else
                        p[1].Value = interfaceConfigName;
                    p[2] = new SqlParameter("@ItemsXML", SqlDbType.Text);
                    if (string.IsNullOrEmpty(valueDescriptions))
                        p[2].Value = DBNull.Value;
                    else
                        p[2].Value = valueDescriptions;
                    SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertExternalTermValues", p);
				}
				transScope.Complete();
			}
		}


		public static bool HasValues(Guid managedItemId, string interfaceConfigName)
		{
			bool rtn = false;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				SqlParameter[] p = new SqlParameter[3];
				p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
				p[0].Value = managedItemId;
				p[1] = new SqlParameter("@InterfaceConfigName", SqlDbType.VarChar, 100);
				p[1].Value = interfaceConfigName;
				p[2] = new SqlParameter("@Exists", SqlDbType.TinyInt);
				p[2].Direction = ParameterDirection.Output;
				SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.ExternalTermValueExists", p);
				rtn = ((int)p[2].Value == 1);
			}
			return rtn;
		}


	}

}