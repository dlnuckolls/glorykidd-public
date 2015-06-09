using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Kindred.Common.Data;

namespace Kindred.Knect.ITAT.Data
{
	public class Report
	{
		public static DataSet GetReport(Guid? itatSystemID, Guid? reportID)
		{
			DataSet rtn = null;

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					p[0] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
					p[0].Value = itatSystemID ?? SqlGuid.Null;
					p[1] = new SqlParameter("@ReportID", SqlDbType.UniqueIdentifier);
					p[1].Value = reportID ?? SqlGuid.Null;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetReport", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get the Report, Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

		public static bool UpdateReport(
											Guid reportID, 
											Guid itatSystemID, 
											string reportName, 
											string reportDescription,
											string reportConfigXML)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					int nIndex = 0;
					SqlParameter[] p = new SqlParameter[5];
					p[nIndex] = new SqlParameter("@ReportID", SqlDbType.UniqueIdentifier);
					p[nIndex++].Value = reportID;
					p[nIndex] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
					p[nIndex++].Value = itatSystemID;
					p[nIndex] = new SqlParameter("@ReportName", SqlDbType.VarChar, 100);
					p[nIndex++].Value = reportName;
					p[nIndex] = new SqlParameter("@ReportDescription", SqlDbType.VarChar, 255);
					p[nIndex++].Value = reportDescription;
					p[nIndex] = new SqlParameter("@ReportConfigXML", SqlDbType.Text);
					p[nIndex++].Value = reportConfigXML;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateReport", p);
					return true;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to Update Report, Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		public static bool DeleteReport(Guid reportID)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ReportID", SqlDbType.UniqueIdentifier);
					p[0].Value = reportID;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.DeleteReport", p);
					return true;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to delete Report {0}, Error: {1}", reportID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		public static bool CopyReport(Guid copyFrom, Guid copyTo, string newName, string newDescription, string configXml)
		{
			Guid rtn = Guid.Empty;

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				int nReturn = 0;
				try
				{
					SqlParameter[] p = new SqlParameter[5];
					int nIndex = 0;
					p[nIndex] = new SqlParameter("@OldReportID", SqlDbType.UniqueIdentifier);
					p[nIndex++].Value = copyFrom;
					p[nIndex] = new SqlParameter("@NewReportID", SqlDbType.UniqueIdentifier);
					p[nIndex++].Value = copyTo;
					p[nIndex] = new SqlParameter("@NewReportName", SqlDbType.VarChar, 100);
					p[nIndex++].Value = newName;
					p[nIndex] = new SqlParameter("@NewReportDecription", SqlDbType.VarChar, 255);
					p[nIndex++].Value = newDescription;
					p[nIndex] = new SqlParameter("@ReportConfigXml", SqlDbType.Text);
					p[nIndex++].Value = configXml;
					nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.CopyReport", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to copy Report for ReportID = {0},   Error: {1}", copyFrom.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
				return true;
			}
		}

	}
}
