using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Kindred.Common.Data;

namespace Kindred.Knect.ITAT.Data
{
	public static class Document
	{

		public static bool AddCachedDocument(Guid documentId, Guid managedItemId, string documentStoreId, string documentName, string documentDescription, string documentType)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[7];
					p[0] = new SqlParameter("@DocumentID", SqlDbType.UniqueIdentifier);
					p[0].Value = documentId;
					p[1] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[1].Value = managedItemId;
					p[2] = new SqlParameter("@DocumentStoreID", SqlDbType.VarChar, 100);
					p[2].Value = documentStoreId;
					p[3] = new SqlParameter("@DocumentName", SqlDbType.VarChar, 500);
					p[3].Value = documentName;
					p[4] = new SqlParameter("@DocumentDescription", SqlDbType.VarChar, 500);
					p[4].Value = documentDescription;
					p[5] = new SqlParameter("@DocumentType", SqlDbType.VarChar, 100);
					p[5].Value = documentType;
					p[6] = new SqlParameter("@CachedDateTime", SqlDbType.DateTime);
					p[6].Value = DateTime.Now;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertDocumentCache", p);
					return true;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to insert cached document.  DocumentStoreId = {0},   Error: {1}", documentStoreId, ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}


		public static bool DeleteDocumentsByManagedItem(Guid managedItemID)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemID;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.DeleteDocumentsByManagedItemID", p);
					return true;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to delete documents for Managed Item = {0},   Error: {1}", managedItemID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}


		public static bool DeleteCachedDocumentsByManagedItem(Guid managedItemID)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemID;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.DeleteCachedDocumentsByManagedItemID", p);
					return true;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to delete cached documents for Managed Item = {0},   Error: {1}", managedItemID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}


		public static bool RemoveCachedDocument(string documentStoreId)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@DocumentStoreID", SqlDbType.VarChar, 100);
					p[0].Value = documentStoreId;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.DeleteCachedDocument", p);
					return true;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to remove cached document.  DocumentStoreId={0},   Error: {1}", documentStoreId, ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		/// <summary>
		/// Returns a recordset containing the metadata for the documents associated with for the specified ManagedItem
		/// </summary>
		/// <param name="managedItemID">The database ID of the template</param>
		/// <returns>A DataSet contining a row for each document's metadata</returns>
        public static DataSet GetDocumentList(Guid managedItemID, bool includeDeleted)
        {
            DataSet ds = null;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[2];
                    p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
                    p[0].Value = managedItemID;
                    p[1] = new SqlParameter("@IncludeDeleted", SqlDbType.Bit);
                    p[1].Value = includeDeleted;
                    ds = SqlHelper.ExecuteDataset(cnn, "dbo.GetDocumentList", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve DocumentList info for ManagedItemID={0},   Error: {1}", managedItemID.ToString(), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
            return ds;
        }

		public static Guid InsertDocument(Guid managedItemID, string documentName, string documentDescription, string documentStoreID, string documentType, bool isScanned)
		{
			Guid documentID = Guid.Empty;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[7];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemID;
					p[1] = new SqlParameter("@DocumentName", SqlDbType.VarChar, 500);
					p[1].Value = documentName;
					p[2] = new SqlParameter("@DocumentDescription", SqlDbType.VarChar, 500);
					p[2].Value = documentDescription;
					p[3] = new SqlParameter("@DocumentStoreID", SqlDbType.VarChar, 100);
					p[3].Value = documentStoreID;
					p[4] = new SqlParameter("@DocumentType", SqlDbType.VarChar, 100);
					p[4].Value = documentType;
					p[5] = new SqlParameter("@IsScanned", SqlDbType.Bit);
					p[5].Value = (isScanned ? 1 : 0);
					p[6] = new SqlParameter("@DocumentID", SqlDbType.UniqueIdentifier);
					p[6].Direction = ParameterDirection.Output;

					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertDocument", p);
					documentID = (Guid)p[6].Value;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to insert Document for ManagedItem {0},   Error: {1}", managedItemID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return documentID;
		}

        public static void DeleteDocument(Guid managedItemID, string documentName, string documentDescription, string documentStoreID, string documentType, bool isScanned, bool hardDelete)
        {
            Guid documentID = Guid.Empty;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[7];
                    int index = 0;
                    p[index] = new SqlParameter("@DocumentStoreID", SqlDbType.VarChar, 100);
                    p[index++].Value = documentStoreID;
                    p[index] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
                    p[index++].Value = managedItemID;
                    p[index] = new SqlParameter("@DocumentName", SqlDbType.VarChar, 500);
                    p[index++].Value = documentName;
                    p[index] = new SqlParameter("@DocumentDescription", SqlDbType.VarChar, 500);
                    p[index++].Value = documentDescription;
                    p[index] = new SqlParameter("@DocumentType", SqlDbType.VarChar, 100);
                    p[index++].Value = documentType;
                    p[index] = new SqlParameter("@IsScanned", SqlDbType.Bit);
                    p[index++].Value = (isScanned ? 1 : 0);
                    p[index] = new SqlParameter("@HardDelete", SqlDbType.Bit);
                    p[index++].Value = (isScanned ? 1 : 0);

                    SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.DeleteDocument", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to delete Document for ManagedItem {0},   Error: {1}", managedItemID.ToString(), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

		public static bool UpdateDocumentType(Guid documentID, string documentType)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					p[0] = new SqlParameter("@DocumentID", SqlDbType.UniqueIdentifier);
					p[0].Value = documentID;
					p[1] = new SqlParameter("@DocumentType", SqlDbType.VarChar, 100);
					p[1].Value = documentType;

					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateDocumentType", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update Document Type for document ID = {0},   Error: {1}", documentID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return true;
		}




	}
}
