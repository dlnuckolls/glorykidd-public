using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Kindred.Common.Data;
 

namespace Kindred.Knect.ITAT.Data
{
	public static class ManagedItem
	{

		/// <summary>
		/// Returns a recordset containing the information for the specified ManagedItem
		/// </summary>
		/// <param name="templateID">The database ID of the template</param>
		/// <returns>A SqlDataReader contining 1 row with template's summary info </returns>
		public static DataSet GetManagedItem(Guid managedItemID)
		{
			DataSet ds = null;
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemID;
					ds = SqlHelper.ExecuteDataset(cnn, "dbo.GetManagedItem", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve ManagedItem info for ManagedItemID={0},   Error: {1}", managedItemID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return ds;
		}

		/// <summary>
		/// Returns a recordset containing the information for the specified ManagedItems
		/// </summary>
		/// <param name="ManagedItemID">A table of the database ID's of the ManagedItems</param>
		/// <returns>A DataSet containing rows with ManagedItems' info </returns>
		public static DataTable GetManagedItems(DataTable managedItemIDs)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.Structured);
					p[0].TypeName = "dbo.ManagedItemID";
					p[0].Value = managedItemIDs;
					return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetManagedItems", p).Tables[0];
			   }
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve ManagedItems info for supplied ManagedItemIDs,   Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		public static DataTable GetManagedItemByNumber(string managedItemNumber)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ManagedItemNumber", SqlDbType.VarChar, 20);
					p[0].Value = managedItemNumber;
					return SqlHelper.ExecuteDataset(cnn, "dbo.GetManagedItemByNumber", p).Tables[0];
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve ManagedItem info for ManagedItemID={0},   Error: {1}", managedItemNumber, ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		/// <summary>
		/// Returns a recordset containing the information for the specified ManagedItem
		/// </summary>
		/// <param name="templateID">The database ID of the template</param>
		/// <returns>A SqlDataReader contining 1 row with template's summary info </returns>
		public static DataSet GetManagedItemFacility(Guid managedItemID)
		{
			DataSet ds = null;
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemID;
					ds = SqlHelper.ExecuteDataset(cnn, "dbo.GetManagedItemFacility", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve ManagedItem Facility info for ManagedItemID={0},   Error: {1}", managedItemID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return ds;
		}

		/// <summary>
		/// Adds a new manageditem to the database
		/// </summary>
		/// <param name="name">Template Name</param>
		/// <param name="description">Template Description</param>
		/// <param name="generatesDocument">Indicates whether the items created from this template will have the ability to generate documents (i.e. have clauses and extensions)</param>
		/// <returns>The Guid (database ID) for the new record</returns>
		public static bool InsertManagedItem(	Guid managedItemID,
												Guid templateID,
												string managedItemNumber,
												string status,		
												string state,
												Guid stateID,
												string Term1,
												string Term2,
												DateTime? Term3,
												string Term4,
												string Term5,
												DateTime? Term6,
												DateTime? Term7,
												string selectedFacilityIDXml,
												string owningFacilityIDXml,
												bool isOrphaned
											 )
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[16];
					int index = 0;
					p[index] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[index++].Value = managedItemID;
					p[index] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[index++].Value = templateID;
					p[index] = new SqlParameter("@ManagedItemNumber", SqlDbType.VarChar, 20);
					p[index++].Value = managedItemNumber;
					p[index] = new SqlParameter("@Status", SqlDbType.VarChar, 255);
					p[index++].Value = status;
					p[index] = new SqlParameter("@State", SqlDbType.VarChar, 255);
					p[index++].Value = state;
					p[index] = new SqlParameter("@StateID", SqlDbType.UniqueIdentifier);
					p[index++].Value = stateID;
					p[index] = new SqlParameter("@Term1", SqlDbType.VarChar, Data.DataNames._FL_Term1);
					p[index++].Value = Term1;
					p[index] = new SqlParameter("@Term2", SqlDbType.VarChar, Data.DataNames._FL_Term2);
					p[index++].Value = Term2;
					p[index] = new SqlParameter("@Term3", SqlDbType.DateTime);
					p[index++].Value = Term3;
					p[index] = new SqlParameter("@Term4", SqlDbType.VarChar, Data.DataNames._FL_Term4);
					p[index++].Value = Term4;
					p[index] = new SqlParameter("@Term5", SqlDbType.VarChar, Data.DataNames._FL_Term5);
					p[index++].Value = Term5;
					p[index] = new SqlParameter("@Term6", SqlDbType.DateTime);
					p[index++].Value = Term6;
					p[index] = new SqlParameter("@Term7", SqlDbType.DateTime);
					p[index++].Value = Term7;
					p[index] = new SqlParameter("@SelectedFacilityIDXml", SqlDbType.Text);
					p[index++].Value = selectedFacilityIDXml;
					p[index] = new SqlParameter("@OwningFacilityIDXml", SqlDbType.Text);
					p[index++].Value = owningFacilityIDXml;
					p[index] = new SqlParameter("@IsOrphaned", SqlDbType.Bit);
					p[index++].Value = isOrphaned;
					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertManagedItem", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to insert ManagedItem for TemplateID = {0},   Error: {1}", templateID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return true;
		}

		/// <summary>
		/// Adds a new ManagedItemAudit row to the database
		/// </summary>
		public static void InsertManagedItemAudit(Guid managedItemID, Guid personID, DateTime dateOfChange, string state, Guid stateID, string status, byte[] templateDefZipped, int auditTypeID)
		{
			Guid rtn = Guid.Empty;

			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[8];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemID;
					p[1] = new SqlParameter("@PersonID", SqlDbType.UniqueIdentifier);
					p[1].Value = personID;
					p[2] = new SqlParameter("@DateOfChange", SqlDbType.DateTime);
					p[2].Value = dateOfChange;
					p[3] = new SqlParameter("@State", SqlDbType.VarChar, 255);
					p[3].Value = state;
					p[4] = new SqlParameter("@StateID", SqlDbType.UniqueIdentifier);
					p[4].Value = stateID;
					p[5] = new SqlParameter("@Status", SqlDbType.VarChar, 255);
					p[5].Value = status;
					p[6] = new SqlParameter("@TemplateDefZipped", SqlDbType.VarBinary);
					p[6].Value = templateDefZipped;
					p[7] = new SqlParameter("@AuditTypeID", SqlDbType.Int);
					p[7].Value = auditTypeID;

					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertManagedItemAudit", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to insert ManagedItemAudit for ManagedItemID = {0},   Error: {1}", managedItemID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}


		}

		/// <summary>
		/// Deletes the ManagedItem from the database
		/// </summary>
		public static void DeleteManagedItem(Guid managedItemID)
		{
			Guid rtn = Guid.Empty;

			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemID;
					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.DeleteManagedItem", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to delete ManagedItem for ManagedItemID = {0},   Error: {1}", managedItemID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}


		public static string GetTemplateDef(Guid managedItemID)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				SqlParameter[] p = new SqlParameter[1];
				p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
				p[0].Value = managedItemID;
				return (string)SqlHelper.ExecuteScalar(cnn, CommandType.StoredProcedure, "dbo.GetTemplateDef", p);
			}
		}


		public static void UpdateTemplateDef(Guid managedItemID, string templateDef)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[3];
					p[0] = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
					p[0].Direction = ParameterDirection.ReturnValue;
					p[1] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[1].Value = managedItemID;
					p[2] = new SqlParameter("@TemplateDef", SqlDbType.Text);
					p[2].Value = templateDef;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateTemplateDef", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update TemplateDef for id={0},   Error: {1}", managedItemID.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		public static DataSet UpdateFacilityIDs(Guid managedItemID, string selectedFacilityIDXml, string owningFacilityIDXml)
		{
			DataSet rtn = null;
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[4];
					p[0] = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
					p[0].Direction = ParameterDirection.ReturnValue;
					p[1] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[1].Value = managedItemID;
					p[2] = new SqlParameter("@SelectedFacilityIDXml", SqlDbType.Text);
					p[2].Value = selectedFacilityIDXml;
					p[3] = new SqlParameter("@OwningFacilityIDXml", SqlDbType.Text);
					p[3].Value = owningFacilityIDXml;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.UpdateFacilityIDs", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update FacilityIDs for id={0},   Error: {1}", managedItemID.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

		/// <summary>
		/// Updates an existing manageditem
		/// </summary>
		/// <param name="name">Template Name</param>
		/// <param name="description">Template Description</param>
		/// <param name="generatesDocument">Indicates whether the items created from this template will have the ability to generate documents (i.e. have clauses and extensions)</param>
		/// <returns>The Guid (database ID) for the new record</returns>
		public static bool UpdateManagedItem(	Guid managedItemID,
												string managedItemNumber,
												string status,	
												string state,
												Guid stateID,
												string Term1,
												string Term2,
												DateTime? Term3,
												string Term4,
												string Term5,
												DateTime? Term6,
												DateTime? Term7,
												string keyWords,
												string selectedFacilityIDXml,
												string owningFacilityIDXml,
												bool isOrphaned
			)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[16];
					int index = 0;
					p[index] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[index++].Value = managedItemID;
					p[index] = new SqlParameter("@ManagedItemNumber", SqlDbType.VarChar, 20);
					p[index++].Value = managedItemNumber;
					p[index] = new SqlParameter("@Status", SqlDbType.VarChar, 255);
					p[index++].Value = status;
					p[index] = new SqlParameter("@State", SqlDbType.VarChar, 255);
					p[index++].Value = state;
					p[index] = new SqlParameter("@StateID", SqlDbType.UniqueIdentifier);
					p[index++].Value = stateID;
					p[index] = new SqlParameter("@Term1", SqlDbType.VarChar, Data.DataNames._FL_Term1);
					p[index++].Value = Term1;
					p[index] = new SqlParameter("@Term2", SqlDbType.VarChar, Data.DataNames._FL_Term2);
					p[index++].Value = Term2;
					p[index] = new SqlParameter("@Term3", SqlDbType.DateTime);
					p[index++].Value = Term3;
					p[index] = new SqlParameter("@Term4", SqlDbType.VarChar, Data.DataNames._FL_Term4);
					p[index++].Value = Term4;
					p[index] = new SqlParameter("@Term5", SqlDbType.VarChar, Data.DataNames._FL_Term5);
					p[index++].Value = Term5;
					p[index] = new SqlParameter("@Term6", SqlDbType.DateTime);
					p[index++].Value = Term6;
					p[index] = new SqlParameter("@Term7", SqlDbType.DateTime);
					p[index++].Value = Term7;
					p[index] = new SqlParameter("@KeyWords", SqlDbType.Text);
					p[index++].Value = keyWords;
					p[index] = new SqlParameter("@SelectedFacilityIDXml", SqlDbType.Text);
					p[index++].Value = selectedFacilityIDXml;
					p[index] = new SqlParameter("@OwningFacilityIDXml", SqlDbType.Text);
					p[index++].Value = owningFacilityIDXml;
					p[index] = new SqlParameter("@IsOrphaned", SqlDbType.Bit);
					p[index++].Value = isOrphaned;
					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateManagedItem", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to insert ManagedItem for ManagedItemID = {0},   Error: {1}", managedItemID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return true;
		}

		/// <summary>
		/// Updates an existing manageditem
		/// </summary>
		/// <param name="name">Template Name</param>
		/// <param name="description">Template Description</param>
		/// <param name="generatesDocument">Indicates whether the items created from this template will have the ability to generate documents (i.e. have clauses and extensions)</param>
		/// <returns>The Guid (database ID) for the new record</returns>
		public static bool UpdateManagedItemTemplate(Guid managedItemID, Guid templateID)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					int index = 0;
					p[index] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[index++].Value = managedItemID;
					p[index] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[index++].Value = templateID;
					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateManagedItemTemplate", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update ManagedItem for ManagedItemID = {0},   Error: {1}", managedItemID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return true;
		}

		public static DataSet GetFacilityInfo(string facilityIDXml)
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
					p[1] = new SqlParameter("@FacilityIDXml", SqlDbType.Text);
					p[1].Value = facilityIDXml;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetFacilityInfo", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get Facility info,   Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

		public static int GetSequenceNumber(Guid ITATSystemID, string sPrefix)
		{
			int rtn = -1;

			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[3];
					p[0] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
					p[0].Value = ITATSystemID;
					p[1] = new SqlParameter("@Prefix", SqlDbType.VarChar, 20);
					p[1].Value = sPrefix;
					p[2] = new SqlParameter("@SequenceNumber", SqlDbType.Int);
					p[2].Direction = ParameterDirection.Output;
					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.GetManagedItemSequenceNumber", p);
					rtn = (int)p[2].Value;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get the next sequence number for ITATSystemID = {0}, Prefix = {1},   Error: {2}", ITATSystemID.ToString(), sPrefix, ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

		public static DataTable GetManagedItemAuditList(int maxCount)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlCommand sc = new SqlCommand();
					sc.CommandText = "dbo.GetManagedItemAuditList";
					sc.CommandTimeout = 300;
					sc.CommandType = CommandType.StoredProcedure;
					sc.Connection = cnn;
					sc.Parameters.Add("@MaxCount", SqlDbType.Int).Value = maxCount;


					SqlDataAdapter sda = new SqlDataAdapter(sc);
					DataSet ds = new DataSet();
					sda.Fill(ds);


					if (ds != null && ds.Tables.Count > 0)
						return ds.Tables[0];
					return null;

				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve ManagedItemAudit item list, Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		//Note - this is used by the Compression utility executable.
		public static void UpdateManagedItemAuditList(DataTable dtTemplateDefZipped)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@TemplateDefZipped", SqlDbType.Structured);
					p[0].Value = dtTemplateDefZipped;
					p[0].TypeName = "dbo.TemplateDefZipped";
					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateManagedItemAuditList", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update the ManagedItemAudit table for {0} entries, Error: {1}", dtTemplateDefZipped.Rows.Count.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		public static int GetManagedItemAuditListCount()
		{
			int rtn = -1;

			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();

				try
				{
					SqlParameter p = new SqlParameter("@ManagedItemAuditListCount", SqlDbType.Int);
					p.Direction = ParameterDirection.Output;

					SqlCommand sc = new SqlCommand();
					sc.CommandText = "dbo.GetManagedItemAuditListCount";
					sc.CommandTimeout = 300;
					sc.CommandType = CommandType.StoredProcedure;
					sc.Connection = cnn;
					sc.Parameters.Add(p);


					sc.ExecuteNonQuery();
					rtn = (int)p.Value;

				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get the ManagedItemAuditListCount.  Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}

				return rtn;

			}
		}

		public static DataSet Find(
			Guid systemID, 
			Guid? templateId, 
			IEnumerable<string> statuses, 
			IEnumerable<int> facilityIds, 
			string textTerm1, 
			string textTerm2, 
			DateTime? dateTerm3StartDate, 
			DateTime? dateTerm3EndDate,
			string textTerm4,
			string textTerm5,
			DateTime? dateTerm6StartDate,
			DateTime? dateTerm6EndDate,
			DateTime? dateTerm7StartDate,
			DateTime? dateTerm7EndDate,
			string keyWords, 
			IEnumerable<string> userRoles)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				int nIndex = 0;
				SqlParameter[] p = new SqlParameter[18];
				p[nIndex] = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
				p[nIndex++].Direction = ParameterDirection.ReturnValue;
				p[nIndex] = new SqlParameter("@ItatSystemID", SqlDbType.UniqueIdentifier);
				p[nIndex++].Value = systemID;
				p[nIndex] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
				p[nIndex++].Value = templateId ?? SqlGuid.Null;
				p[nIndex] = new SqlParameter("@StatusList", SqlDbType.Structured);
				p[nIndex].TypeName = "dbo.Status";
				p[nIndex++].Value = Common.GetStatuses(statuses);
				p[nIndex] = new SqlParameter("@FacilityList", SqlDbType.Structured);
				p[nIndex].TypeName = "dbo.Facility";
				p[nIndex++].Value = Common.GetFacilities(facilityIds);

				p[nIndex] = new SqlParameter("@Term1", SqlDbType.VarChar, Data.DataNames._FL_Term1);
				p[nIndex++].Value = (string.IsNullOrEmpty(textTerm1) ? SqlString.Null : textTerm1);
				p[nIndex] = new SqlParameter("@Term2", SqlDbType.VarChar, Data.DataNames._FL_Term2);
				p[nIndex++].Value = (string.IsNullOrEmpty(textTerm2) ? SqlString.Null : textTerm2);

				p[nIndex] = new SqlParameter("@Term3StartDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm3StartDate ?? SqlDateTime.Null;
				p[nIndex] = new SqlParameter("@Term3EndDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm3EndDate ?? SqlDateTime.Null;

				p[nIndex] = new SqlParameter("@Term4", SqlDbType.VarChar, Data.DataNames._FL_Term4);
				p[nIndex++].Value = (string.IsNullOrEmpty(textTerm4) ? SqlString.Null : textTerm4);
				p[nIndex] = new SqlParameter("@Term5", SqlDbType.VarChar, Data.DataNames._FL_Term5);
				p[nIndex++].Value = (string.IsNullOrEmpty(textTerm5) ? SqlString.Null : textTerm5);

				p[nIndex] = new SqlParameter("@Term6StartDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm6StartDate ?? SqlDateTime.Null;
				p[nIndex] = new SqlParameter("@Term6EndDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm6EndDate ?? SqlDateTime.Null;

				p[nIndex] = new SqlParameter("@Term7StartDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm7StartDate ?? SqlDateTime.Null;
				p[nIndex] = new SqlParameter("@Term7EndDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm7EndDate ?? SqlDateTime.Null;

				p[nIndex] = new SqlParameter("@KeyWords", SqlDbType.VarChar, 1000);
				p[nIndex++].Value = (string.IsNullOrEmpty(keyWords) ? SqlString.Null : keyWords);
				p[nIndex] = new SqlParameter("@RoleList", SqlDbType.Structured);
				p[nIndex].Value = Common.GetRoles(userRoles);
				p[nIndex++].TypeName = "dbo.Role";
				p[nIndex] = new SqlParameter("@UseRoleList", SqlDbType.Bit);
				p[nIndex++].Value = userRoles != null;
				return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetManagedItemList", p);
			}
		}

		public static DataTable FindStore(
			Guid systemID, 
			IEnumerable<Guid> templateId, 
			IEnumerable<string> statuses, 
			IEnumerable<int> facilityIds, 
			string textTerm1, 
			string textTerm2, 
			DateTime? dateTerm3StartDate, DateTime? dateTerm3EndDate,
			string textTerm4, 
			string textTerm5, 
			DateTime? dateTerm6StartDate, DateTime? dateTerm6EndDate,
			DateTime? dateTerm7StartDate, DateTime? dateTerm7EndDate
			)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				int nIndex = 0;
				SqlParameter[] p = new SqlParameter[15];
				p[nIndex] = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
				p[nIndex++].Direction = ParameterDirection.ReturnValue;
				p[nIndex] = new SqlParameter("@ItatSystemID", SqlDbType.UniqueIdentifier);
				p[nIndex++].Value = systemID;
				p[nIndex] = new SqlParameter("@TemplateID", SqlDbType.Structured);
				p[nIndex].TypeName = "dbo.Template";
				p[nIndex++].Value = Common.GetIDs(templateId); ;
				p[nIndex] = new SqlParameter("@StatusList", SqlDbType.Structured);
				p[nIndex].TypeName = "dbo.Status";
				p[nIndex++].Value = Common.GetStatuses(statuses);
				p[nIndex] = new SqlParameter("@FacilityList", SqlDbType.Structured);
				p[nIndex].TypeName = "dbo.Facility";
				p[nIndex++].Value = Common.GetFacilities(facilityIds);
				p[nIndex] = new SqlParameter("@Term1", SqlDbType.VarChar, Data.DataNames._FL_Term1);
				p[nIndex++].Value = (string.IsNullOrEmpty(textTerm1) ? SqlString.Null : textTerm1);
				p[nIndex] = new SqlParameter("@Term2", SqlDbType.VarChar, Data.DataNames._FL_Term2);
				p[nIndex++].Value = (string.IsNullOrEmpty(textTerm2) ? SqlString.Null : textTerm2);
				p[nIndex] = new SqlParameter("@Term3StartDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm3StartDate ?? SqlDateTime.Null;
				p[nIndex] = new SqlParameter("@Term3EndDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm3EndDate ?? SqlDateTime.Null;
				p[nIndex] = new SqlParameter("@Term4", SqlDbType.VarChar, Data.DataNames._FL_Term4);
				p[nIndex++].Value = (string.IsNullOrEmpty(textTerm4) ? SqlString.Null : textTerm4);
				p[nIndex] = new SqlParameter("@Term5", SqlDbType.VarChar, Data.DataNames._FL_Term5);
				p[nIndex++].Value = (string.IsNullOrEmpty(textTerm5) ? SqlString.Null : textTerm5);
				p[nIndex] = new SqlParameter("@Term6StartDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm6StartDate ?? SqlDateTime.Null;
				p[nIndex] = new SqlParameter("@Term6EndDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm6EndDate ?? SqlDateTime.Null;
				p[nIndex] = new SqlParameter("@Term7StartDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm7StartDate ?? SqlDateTime.Null;
				p[nIndex] = new SqlParameter("@Term7EndDate", SqlDbType.DateTime);
				p[nIndex++].Value = dateTerm7EndDate ?? SqlDateTime.Null;
				return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetManagedItemStoreList", p).Tables[0];
			}
		}

		public static DataSet FindByNumber(Guid systemId, string managedItemNumber, List<string> userRoles)
		{
			DataSet rtn = null;
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[5];
					p[0] = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
					p[0].Direction = ParameterDirection.ReturnValue;
					p[1] = new SqlParameter("@SystemId", SqlDbType.UniqueIdentifier);
					p[1].Value = systemId;
					p[2] = new SqlParameter("@ManagedItemNumber", SqlDbType.VarChar, 20);
					p[2].Value = managedItemNumber;
					p[3] = new SqlParameter("@RoleList", SqlDbType.Structured);
					p[3].Value = Common.GetRoles(userRoles);
					p[3].TypeName = "dbo.Role";
					p[4] = new SqlParameter("@UseRoleList", SqlDbType.Bit);
					p[4].Value = userRoles != null;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetManagedItemListByNumber", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to find ManagedItems by number.,   Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}


		public static DataSet FindById(Guid managedItemId, List<string> userRoles)
		{
			DataSet rtn = null;

			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[4];
					p[0] = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
					p[0].Direction = ParameterDirection.ReturnValue;
					p[1] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[1].Value = managedItemId;
					p[2] = new SqlParameter("@RoleList", SqlDbType.Structured);
					p[2].Value = Common.GetRoles(userRoles);
					p[2].TypeName = "dbo.Role";
					p[3] = new SqlParameter("@UseRoleList", SqlDbType.Bit);
					p[3].Value = userRoles != null;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetManagedItemListByID", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to find ManagedItems by number.,   Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

		public static void UpdateManagedItemStateRole(Guid managedItemId, List<string> roles)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemId;
					p[1] = new SqlParameter("@RoleList", SqlDbType.Structured);
					p[1].Value = Common.GetRoles(roles);
					p[1].TypeName = "dbo.Role";
					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateManagedItemStateRole", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update the ManagedItemStateRole for ManagedItemID = {0}, Error: {1}", managedItemId.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		public static DataSet GetManagedItemHistory(Guid managedItemId)
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
					p[1] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[1].Value = managedItemId;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetManagedItemHistory", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get ManagedItem history for manageditemid {0},   Error: {1}", managedItemId.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

		public static DataSet GetManagedItemDetailedHistory(Guid managedItemId, bool includeAll)
		{
			DataSet rtn = null;
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemId;
					p[1] = new SqlParameter("@IncludeAll", SqlDbType.Bit);
					p[1].Value = includeAll;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetManagedItemDetailedHistory", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get ManagedItem history for manageditemid {0},   Error: {1}", managedItemId.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}


		public static DataSet GetAllNonExecutedEvents(DateTime eventDate)
		{
			DataSet rtn = null;

			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[3];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = null;
					p[1] = new SqlParameter("@EventDate", SqlDbType.DateTime);
					p[1].Value = eventDate;
					p[2] = new SqlParameter("@Executed", SqlDbType.Bit);
					p[2].Value = 0;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetEvents", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get the non-executed events, Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

		public static DataSet GetSystemEventManagedItems(string sSystem)
		{
			DataSet rtn = null;

			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@System", SqlDbType.VarChar, 255);
					p[0].Value = sSystem;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetSystemEventManagedItems", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get the system manageditemids, Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

		public static DataSet GetTemplateDocumentList(Guid templateID)
		{
			DataSet rtn = null;

			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[0].Value = templateID;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetTemplateDocumentList", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get the document list, Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

		public static DataTable GetSystemTemplateManagedItems(Guid systemID, Guid? templateID)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					p[0] = new SqlParameter("@SystemID", SqlDbType.UniqueIdentifier);
					p[0].Value = systemID;
					p[1] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					if (templateID.HasValue)
						p[1].Value = templateID;
					else
						p[1].Value = DBNull.Value;
					return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetSystemTemplateManagedItems", p).Tables[0];
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get the ManagedItem list, Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		public static DataTable GetRetroManagedItems(Guid templateID, DateTime? retroDate)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					p[0] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[0].Value = templateID;
					p[1] = new SqlParameter("@TemplateRetroDate", SqlDbType.DateTime);
					if (retroDate.HasValue)
						p[1].Value = retroDate.Value;
					else
						p[1].Value = DBNull.Value;

					return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetRetroManagedItems", p).Tables[0];
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to get the ManagedItem list, Error: {0}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		public static DataTable GetManagedItemAudit(Guid managedItemAuditID)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ManagedItemAuditID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemAuditID;
					return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetManagedItemAudit", p).Tables[0];
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve ManagedItemAudit for id={0},   Error: {1}", managedItemAuditID.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		public static DataTable GetManagedItemDocuments(Guid managedItemID)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[1];
					p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
					p[0].Value = managedItemID;
					return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetManagedItemDocuments", p).Tables[0];
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve Documents for id={0},   Error: {1}", managedItemID.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}


		public static DataTable GetScheduledEvents(Guid managedItemId)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				SqlParameter[] p = new SqlParameter[1];
				p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
				p[0].Value = managedItemId;
				return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetScheduledEvents", p).Tables[0];
			}
		}


		public static void UpdateProcessScheduledEvents(Guid managedItemId, DataTable scheduledEvents, bool removeUnmatchedScheduledEvents)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				SqlParameter[] p = new SqlParameter[3];
				p[0] = new SqlParameter("@ManagedItemID", SqlDbType.UniqueIdentifier);
				p[0].Value = managedItemId;
				p[1] = new SqlParameter("@RemoveUnmatchedScheduledEvents", SqlDbType.Bit);
				p[1].Value = removeUnmatchedScheduledEvents;
				p[2] = new SqlParameter("@ScheduledEvents", SqlDbType.Structured);
				p[2].TypeName = "dbo.udtScheduledEvent";
				p[2].Value = scheduledEvents;
				SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateScheduledEvents", p);
			}
		}

		public static DataTable GetOrphanedManagedItems(Guid templateID)
		{
			using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				SqlParameter[] p = new SqlParameter[1];
				p[0] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
				p[0].Value = templateID;
				return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetOrphanedManagedItems", p).Tables[0];
			}
		}

	}
}
