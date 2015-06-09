using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Kindred.Common.Data;

namespace Kindred.Knect.ITAT.Data
{
    public static class DataStoreDefinitions
    {

        public static DataTable GetDataStoreDefinitionBySystemID(Guid systemID)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[1];
                    p[0] = new SqlParameter("@SystemID", SqlDbType.UniqueIdentifier);
                    p[0].Value = systemID;
                    return SqlHelper.ExecuteDataset(cnn, "dbo.GetDataStoreDefinitionBySystemID", p).Tables[0];
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve DataStoreDefinition info for SystemID={0},   Error: {1}", systemID, ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

        public static bool CheckDataStoreDefinitionExists(string name)
        {
            bool result;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[8];
                    p[0] = new SqlParameter("@Name", SqlDbType.VarChar, 50);
                    p[0].Value = name;
                    int nReturn = Convert.ToInt16(SqlHelper.ExecuteScalar(cnn, CommandType.StoredProcedure, "dbo.CheckDataStoreDefinition", p));
                    if (nReturn > 0)
                        result = true;
                    else
                        result = false;

                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to Check DataStoreDefinition for ,  Error: {0}", ex.Message));
                }
            }
            return result;
        }


        public static Guid  InsertDataStoreDefinition(string name, string description, Guid systemID,
                                                      string definition,
                                                     bool active
                                             )
        {
            Guid rtn = Guid.Empty;

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[8];
                    p[0] = new SqlParameter("@Name", SqlDbType.VarChar, 50);
                    p[0].Value = name;
                    p[1] = new SqlParameter("@Description", SqlDbType.VarChar, 500);
                    p[1].Value = description;
                    p[2] = new SqlParameter("@SystemID", SqlDbType.UniqueIdentifier);
                    p[2].Value = systemID;
                    p[4] = new SqlParameter("@Definition", SqlDbType.Xml);
                    p[4].Value = definition;
                    p[6] = new SqlParameter("@Active", SqlDbType.Bit);
                    p[6].Value = active;
                    p[7] = new SqlParameter("@DataStoreDefinitionID", SqlDbType.UniqueIdentifier);
                    p[7].Direction = ParameterDirection.Output;
                    int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertDataStoreDefinition", p);
                    rtn = (Guid)p[7].Value;
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to insert DataStoreDefinition for SystemID = {0},   Error: {1}", systemID.ToString(), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
            return rtn;
        }

        public static bool UpdateDataStoreDefinition(Guid dataStoreDefinitionID, string definition,string description,bool active)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[4];
                    p[0] = new SqlParameter("@DataStoreDefinitionID", SqlDbType.UniqueIdentifier);
                    p[0].Value = dataStoreDefinitionID;
                    p[1] = new SqlParameter("@definition", SqlDbType.Xml);
                    p[1].Value = definition;
                    p[2] = new SqlParameter("@Description", SqlDbType.VarChar, 500);
                    p[2].Value = description;
                    p[3] = new SqlParameter("@Active", SqlDbType.Bit);
                    p[3].Value = active;
                    int nreturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateDataStoreDefinition", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to update DataStoreDefinition for id={0},   Error: {1}", dataStoreDefinitionID.ToString("D"), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
                return true;
            }

        }



        public static DataTable GetDataStoreDefinitionsByDefinitionID(Guid DataStoreDefinitionID)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[1];
                    p[0] = new SqlParameter("@DataStoreDefinitionID", SqlDbType.UniqueIdentifier);
                    p[0].Value = DataStoreDefinitionID;
                    return SqlHelper.ExecuteDataset(cnn, "dbo.GetDataStoreDefinitionByDefinitionID", p).Tables[0];
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve DataStoreDefinition info for DefinitionID={0},   Error: {1}", DataStoreDefinitionID, ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

        public static bool UpdateDataStoreDefinitionOnRun(Guid dataStoreDefinitionID, string definitionfilepath, DateTime? rundate)
        {

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[3];
                    p[0] = new SqlParameter("@DataStoreDefinitionID", SqlDbType.UniqueIdentifier);
                    p[0].Value = dataStoreDefinitionID;
                    p[1] = new SqlParameter("@DefinitionFilePath", SqlDbType.VarChar, 500);
                    p[1].Value = definitionfilepath;
                    p[2] = new SqlParameter("@LastRunDate", SqlDbType.DateTime);
                    p[2].Value = rundate;
                    int nreturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateDataStoreDefinitionOnRun", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to update DataStoreDefinition for id={0},   Error: {1}", dataStoreDefinitionID.ToString("D"), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
                return true;
            }

        }


        public static DataTable GetDataStoreDefinitionBySystemID(Guid systemID, bool isActive)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[2];
                    p[0] = new SqlParameter("@SystemID", SqlDbType.UniqueIdentifier);
                    p[0].Value = systemID;
                    p[1] = new SqlParameter("@IsActive", SqlDbType.Bit);
                    p[1].Value = isActive ? 1 : 0;
                    return SqlHelper.ExecuteDataset(cnn, "dbo.GetDataStoreDefinitionBySystemIDStatus", p).Tables[0];
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve DataStoreDefinition info for SystemID={0},   Error: {1}", systemID, ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }
    }
}
