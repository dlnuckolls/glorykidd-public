using System;
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Text;
using Kindred.Common.Data;

namespace Kindred.Knect.ITAT.Data
{
	public static class ITATSystem
	{
        public static DataSet GetSystemList()
		{
			DataSet rtn = null;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetITATSystemList");
				}
				finally
				{
					cnn.Close();
				}
				return rtn;
			}
		}

		public static DataSet GetSystem(Guid systemID)
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
					p[1] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
					p[1].Value = systemID;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetITATSystemDef", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve SystemDef for system={0},   Error: {1}", systemID.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}


		public static void UpdateSystemDef(Guid systemID, string systemName, string systemDef)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[4];
					p[0] = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
					p[0].Direction = ParameterDirection.ReturnValue;
					p[1] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
					p[1].Value = systemID;
					p[2] = new SqlParameter("@ITATSystemName", SqlDbType.VarChar, 255);
					p[2].Value = systemName;
					p[3] = new SqlParameter("@ITATSystemDef", SqlDbType.Text);
					p[3].Value = systemDef;
					int rowsAffected = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateITATSystem", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update SystemDef for system={0},   Error: {1}", systemID.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}


		public static void DeleteSequenceNumbers(Guid systemId)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
					p[0].Value = systemId;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.DeleteManagedItemSequenceNumbers", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to delete ManagedItem Sequence Numbers for system={0},   Error: {1}", systemId.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}


		public static void UpdateSequenceNumbers(Guid systemId, string SequenceNumbersXml)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					p[0] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
					p[0].Value = systemId;
					p[1] = new SqlParameter("@SequenceNumberXml", SqlDbType.Text);
					p[1].Value = SequenceNumbersXml;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.SetManagedItemSequenceNumbers", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update ManagedItem Sequence Numbers for system={0},   Error: {1}", systemId.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

        public static void InsertSystemImage(Guid systemId, string imageName, int imageHeight, int imageWidth, byte[] image)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[5];
                    int index = 0;
                    p[index] = new SqlParameter("@SystemID", SqlDbType.UniqueIdentifier);
                    p[index++].Value = systemId;
                    p[index] = new SqlParameter("@ImageName", SqlDbType.VarChar, 100);
                    p[index++].Value = imageName;
                    p[index] = new SqlParameter("@ImageHeight", SqlDbType.Int);
                    p[index++].Value = imageHeight;
                    p[index] = new SqlParameter("@ImageWidth", SqlDbType.Int);
                    p[index++].Value = imageWidth;
                    p[index] = new SqlParameter("@Image", SqlDbType.Image);
                    p[index++].Value = image;
                    SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertSystemImage", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to insert new image system={0},   Error: {1}", systemId.ToString("D"), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }


		public static byte[] GetSystemImage(Guid imageId)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ImageID", SqlDbType.UniqueIdentifier);
					p[0].Value = imageId;
					return (byte[])SqlHelper.ExecuteScalar(cnn, CommandType.StoredProcedure, "dbo.GetSystemImage", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve SystemImage for image id={0},   Error: {1}", imageId.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}



		public static DataTable GetSystemImages(Guid systemID)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				SqlParameter[] p = new SqlParameter[1];
				p[0] = new SqlParameter("@SystemID", SqlDbType.UniqueIdentifier);
				p[0].Value = systemID;
				return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetSystemImages", p).Tables[0];
			}
		}

        public static DataTable GetSystemTemplates(Guid systemID)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@SystemID", SqlDbType.UniqueIdentifier);
                p[0].Value = systemID;
                return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetSystemTemplates", p).Tables[0];
            }
        }

        public static void UpdateSystemImageInUse(Guid imageId, bool inUse)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[2];
                    int index = 0;
                    p[index] = new SqlParameter("@ImageID", SqlDbType.UniqueIdentifier);
                    p[index++].Value = imageId;
                    p[index] = new SqlParameter("@InUse", SqlDbType.Bit);
                    p[index++].Value = inUse;
                    SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateSystemImageInUse", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to update imageid={0},   Error: {1}", imageId.ToString("D"), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

        public static void DeleteSystemImage(Guid imageId)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[1];
                    p[0] = new SqlParameter("@ImageID", SqlDbType.UniqueIdentifier);
                    p[0].Value = imageId;
                    SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.DeleteSystemImage", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to delete imageid={0},   Error: {1}", imageId.ToString("D"), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

	}
}
