using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Kindred.Common.Data;
using System.Threading;

namespace Kindred.Knect.ITAT.Data
{
	public static class Template
	{

		/// <summary>
		/// Returns a recordset containing a list of Templates (of any status)
		/// </summary>
		/// <param name="ITATSystemID">Indicates which the ITAT system the templates should be selected from.</param>
		/// <returns>A DataSet containing the template id, template name, and template status, in alphabetical order</returns>
        public static DataSet GetTemplateList(Guid ITATSystemID, List<string> userRoles)
		{
            DataSet ds = null;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[4];
					p[0] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
					p[0].Value = ITATSystemID;
					p[1] = new SqlParameter("@TemplateStatusXML", SqlDbType.Text);
					p[1].Value = DBNull.Value;
                    p[2] = new SqlParameter("@RoleList", SqlDbType.Structured);
                    p[2].Value = Common.GetRoles(userRoles);
                    p[2].TypeName = "dbo.Role";
                    p[3] = new SqlParameter("@UseRoleList", SqlDbType.Bit);
                    p[3].Value = userRoles != null;
                    ds = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetTemplateList", p);
                }
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve Template list.   Error: {1}", ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return ds;
		}

        public static DataTable GetActiveTemplates(Guid ITATSystemID)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[1];
                    p[0] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
                    p[0].Value = ITATSystemID;
                    return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetActiveTemplates", p).Tables[0];
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve Template list.   Error: {1}", ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

		/// <summary>
		/// Returns a recordset containing a list of Templates
		/// </summary>
		/// <param name="ITATSystemID">Indicates which the ITAT system the templates should be selected from.</param>
		/// <param name="statusXML">An XML structure indicating the status(es) to search for.</param>
		/// <returns>A DataSet containing the template id, template name, and template status, in alphabetical order</returns>
        public static DataSet GetTemplateListWithStatus(Guid ITATSystemID, string statusXML, List<string> userRoles)
		{
			DataSet ds = null;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[4];
					p[0] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
					p[0].Value = ITATSystemID;
					p[1] = new SqlParameter("@TemplateStatusXML", SqlDbType.Text);
					p[1].Value = statusXML;
                    p[2] = new SqlParameter("@RoleList", SqlDbType.Structured);
                    p[2].Value = Common.GetRoles(userRoles);
                    p[2].TypeName = "dbo.Role";
                    p[3] = new SqlParameter("@UseRoleList", SqlDbType.Bit);
                    p[3].Value = userRoles != null;
                    ds = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetTemplateList", p);
                }
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve Template list for these statuses: {0},   Error: {1}", statusXML, ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return ds;
		}

        public static DataSet GetRetroTemplateList(Guid ITATSystemID)
        {
            DataSet ds = null;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[1];
                    p[0] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
                    p[0].Value = ITATSystemID;
                    ds = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetRetroTemplateList", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve Template list,   Error: {0}", ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
            return ds;
        }

        /// <summary>
        /// Returns a recordset containing the Template History
        /// </summary>
        public static DataSet GetTemplateHistory(Guid TemplateID, DataTable AuditTypeFilter)
         {

            //TODO:Will need to change the AuditTypeFilter to be of type "Business.Retro.AuditType". Can't do that at this time due to circular reference.
            DataSet ds = null;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {                    
                    SqlParameter[] p = new SqlParameter[2];
                    p[0] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
                    p[0].Value = TemplateID;
                    p[1] = new SqlParameter("@AuditTypeFilter", SqlDbType.Structured);
                    p[1].Value = AuditTypeFilter;
                    p[1].TypeName = "dbo.UDTAuditType";
                    ds = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetTemplateHistory", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve the TemplateHistory for this TemplateID: {0},   Error: {1}", TemplateID.ToString(), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
            return ds;
        }

        /// <summary>
		/// Adds a new template to the database
		/// </summary>
		/// <param name="name">Template Name</param>
		/// <param name="description">Template Description</param>
		/// <param name="generatesDocument">Indicates whether the items created from this template will have the ability to generate documents (i.e. have clauses and extensions)</param>
		/// <returns>The Guid (database ID) for the new record</returns>
		public static Guid InsertTemplate(Guid ITATSystemID, string name)
		{
			Guid rtn = Guid.Empty;

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[5];
					p[0] = new SqlParameter("@ITATSystemID", SqlDbType.UniqueIdentifier);
					p[0].Value = ITATSystemID;
					p[1] = new SqlParameter("@TemplateName", SqlDbType.VarChar, 255);
					p[1].Value = name;
					p[2] = new SqlParameter("@Status", SqlDbType.SmallInt);
					p[2].Value = 0;
					p[3] = new SqlParameter("@DraftTemplateDef", SqlDbType.Text);
					p[3].Value = "";
                    p[4] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[4].Direction = ParameterDirection.Output;
					int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertTemplate", p);
					rtn = (Guid)p[4].Value;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to insert Template for ITATSystemID = {0},   Error: {1}", ITATSystemID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}


		/// <summary>
		/// Adds a new templateaudit row to the database
		/// </summary>
		public static void InsertTemplateAudit(Guid templateID, Guid personID, short status, string templateDef, int auditTypeID)
		{
			Guid rtn = Guid.Empty;

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[5];
					p[0] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[0].Value = templateID;
					p[1] = new SqlParameter("@PersonID", SqlDbType.UniqueIdentifier);
					p[1].Value = personID;
					p[2] = new SqlParameter("@Status", SqlDbType.SmallInt);
					p[2].Value = status;
					p[3] = new SqlParameter("@TemplateDef", SqlDbType.Text);
					p[3].Value = templateDef;
                    p[4] = new SqlParameter("@AuditTypeID", SqlDbType.Int);
                    p[4].Value = auditTypeID;
                    int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.InsertTemplateAudit", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to insert TemplateAudit for TemplateID = {0},   Error: {1}", templateID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
					//TODO Note - this small wait is added since the GetDate() value is used as a key
					//and a really fast server might hit this call multiple times, too frequently.
					//Long term - need to alter how saves are done.
					Thread.Sleep(50);
				}
			}
		}

		/// <summary>
		/// Creates a copy of an existing template, using a new name and description
		/// </summary>
		/// <param name="copyFrom">The database ID indicating the template to be copied</param>
		/// <param name="newName">The name of the new template</param>
		/// <param name="newDescription">The description of the new template</param>
		/// <param name="newStatus">The status of the new template</param>
		public static Guid CopyTemplate(Guid copyFrom, string newName, string newDescription, short newStatus)
		{
			Guid rtn = Guid.Empty;

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				int nReturn = 0;
				try
				{
					SqlParameter[] p = new SqlParameter[4];
					p[0] = new SqlParameter("@OldTemplateID", SqlDbType.UniqueIdentifier);
					p[0].Value = copyFrom;
					p[1] = new SqlParameter("@NewTemplateName", SqlDbType.VarChar, 255);
					p[1].Value = newName;
					p[2] = new SqlParameter("@NewTemplateStatus", SqlDbType.SmallInt);
					p[2].Value = newStatus;
					p[3] = new SqlParameter("@NewTemplateID", SqlDbType.UniqueIdentifier);
					p[3].Direction = ParameterDirection.Output;
					nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.CopyTemplate", p);
					rtn = (Guid)p[3].Value;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to copy Template for TemplateID = {0},   Error: {1}", copyFrom.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
				return rtn;
			}
		}

		/// <summary>
		/// Updates the summary information (info on the Template Main page) for a template
		/// </summary>
		/// <param name="templateID">The database ID indicating the template to be updated</param>
		/// <param name="name">The name of the template</param>
		/// <param name="description">The description of the template</param>
		/// <param name="generatesDocument">Indicates whether the items created from this template will have the ability to generate documents (i.e. have clauses and extensions)</param>
		/// <param name="active">Indicates whether the template is currently active</param>
		public static void UpdateTemplateSummary(Guid templateID, string name, short status, DateTime? retroDate)
		{
			//NOTE:  Most of the DraftTemplateDef field should remain the same; except for the Description and generatesDocument information.

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				int nReturn = 0;
				try
				{
					SqlParameter[] p = new SqlParameter[4];
					p[0] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[0].Value = templateID;
					p[1] = new SqlParameter("@TemplateName", SqlDbType.VarChar, 255);
					p[1].Value = name;
					p[2] = new SqlParameter("@Status", SqlDbType.SmallInt);
					p[2].Value = status;
                    p[3] = new SqlParameter("@RetroDate", SqlDbType.DateTime);
                    if (retroDate.HasValue)
                        p[3].Value = retroDate;
                    else
                        p[3].Value = DBNull.Value;
                    nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateTemplateSummary", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update Template for TemplateID = {0},   Error: {1}", templateID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		/// <summary>
		/// Deletes a template from the database
		/// </summary>
		/// <param name="templateID">The database ID indicating the template to be deleted</param>
		public static void DeleteTemplate(Guid templateID, bool includeTemplate)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				int nReturn = 0;
				try
				{
					SqlParameter[] p = new SqlParameter[2];
					p[0] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[0].Value = templateID;
                    p[1] = new SqlParameter("@IncludeTemplate", SqlDbType.Bit);
                    p[1].Value = includeTemplate;
                    nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.DeleteTemplate", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to delete Template for TemplateID = {0},   Error: {1}", templateID.ToString(), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}

		/// <summary>
		/// Returns a recordset containing the summary information for the specified template
		/// </summary>
		/// <param name="templateID">The database ID of the template</param>
		/// <returns>A SqlDataReader contining 1 row with template's summary info </returns>
        public static DataSet GetTemplateSummary(Guid templateID, string templateDef)
        {
            DataSet ds = null;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[1];
                    p[0] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
                    p[0].Value = templateID;
                    ds = SqlHelper.ExecuteDataset(cnn, "dbo.GetTemplateSummary", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve Template summary info for TemplateID={0},   Error: {1}", templateID.ToString(), ex.Message));
                }
                finally
                {
                }

                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(templateDef);
                    XmlElement templateRoot = doc.DocumentElement;

                    ds.Tables[0].Columns.Add(Data.DataNames._C_Description);
                    ds.Tables[0].Columns.Add(Data.DataNames._C_GenerateDocument);
                    ds.Tables[0].Columns.Add(Data.DataNames._C_GenerateUserDocuments);
                    ds.Tables[0].Columns.Add(Data.DataNames._C_Attachments);
                    ds.Tables[0].Columns.Add(Data.DataNames._C_AllowComments);
                    ds.Tables[0].Columns.Add(Data.DataNames._C_UseDetailedDescription);
                    ds.Tables[0].Columns.Add(Data.DataNames._C_SecurityModel);
                    ds.Tables[0].Columns.Add(Data.DataNames._C_RetroModel);

                    ds.Tables[0].Rows[0][Data.DataNames._C_Description] = GetAttributeValue(templateRoot, Data.DataNames._C_Description);
                    ds.Tables[0].Rows[0][Data.DataNames._C_GenerateDocument] = GetAttributeValue(templateRoot, Data.DataNames._C_GenerateDocument);
                    ds.Tables[0].Rows[0][Data.DataNames._C_GenerateUserDocuments] = GetAttributeValue(templateRoot, Data.DataNames._C_GenerateUserDocuments);
                    ds.Tables[0].Rows[0][Data.DataNames._C_Attachments] = GetAttributeValue(templateRoot, Data.DataNames._C_Attachments);
                    ds.Tables[0].Rows[0][Data.DataNames._C_AllowComments] = GetAttributeValue(templateRoot, Data.DataNames._C_AllowComments);
                    ds.Tables[0].Rows[0][Data.DataNames._C_UseDetailedDescription] = GetAttributeValue(templateRoot, Data.DataNames._C_UseDetailedDescription);
                    ds.Tables[0].Rows[0][Data.DataNames._C_SecurityModel] = GetAttributeValue(templateRoot, Data.DataNames._C_SecurityModel);
                    ds.Tables[0].Rows[0][Data.DataNames._C_RetroModel] = GetAttributeValue(templateRoot, Data.DataNames._C_RetroModel);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to parse Template summary info for TemplateID={0},   Error: {1}", templateID.ToString(), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
            return ds;
        }

		public static Guid GetTemplateIdByName(Guid systemId, string templateName)
		{
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[3];
					p[0] = new SqlParameter("@ITATSystemId", SqlDbType.UniqueIdentifier);
					p[0].Value = systemId;
					p[1] = new SqlParameter("@TemplateName", SqlDbType.VarChar, 50);
					p[1].Value = templateName;
					p[2] = new SqlParameter("@TemplateId", SqlDbType.UniqueIdentifier);
					p[2].Direction = ParameterDirection.Output;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.GetTemplateIdByName", p);
					return (Guid)p[2].Value;
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve TemplateId for name={0},   Error: {1}", templateName, ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
		}


		private static object GetAttributeValue(XmlNode node, string attributeName)
		{
			if (node.Attributes[attributeName] == null)
				return string.Empty;
			return node.Attributes[attributeName].Value;
		}



		public static DataSet GetDraftTemplateDef(Guid templateID)
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
					p[1] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[1].Value = templateID;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetDraftTemplateDef", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to retrieve DraftTemplateDef for id={0},   Error: {1}", templateID.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}

        public static DataSet GetFinalTemplateDef(Guid templateID)
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
                    p[1] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
                    p[1].Value = templateID;
                    rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetFinalTemplateDef", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve FinalTemplateDef for id={0},   Error: {1}", templateID.ToString("D"), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
            return rtn;
        }

        public static DataTable GetTemplateAudit(Guid templateAuditID)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[1];
                    p[0] = new SqlParameter("@TemplateAuditID", SqlDbType.UniqueIdentifier);
                    p[0].Value = templateAuditID;
                    return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetTemplateAudit", p).Tables[0];
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve TemplateAudit for id={0},   Error: {1}", templateAuditID.ToString("D"), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

        public static DataTable GetTemplateRetroDetails(Guid templateID)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[1];
                    p[0] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
                    p[0].Value = templateID;
                    return SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.GetTemplateRetroDetails", p).Tables[0];
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retrieve TemplateRetroDate for id={0},   Error: {1}", templateID.ToString("D"), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }


        public static void UpdateDraftTemplateDef(Guid templateID, string templateDef)
		{

            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[3];
					p[0] = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
					p[0].Direction = ParameterDirection.ReturnValue;
					p[1] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[1].Value = templateID;
					p[2] = new SqlParameter("@DraftTemplateDef", SqlDbType.Text);
					p[2].Value = templateDef;
					SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateDraftTemplateDef", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update DraftTemplateDef for id={0},   Error: {1}", templateID.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			
		}

		public static DataSet UpdateFinalTemplateDef(Guid templateID, string templateDef)
		{
			DataSet rtn = null;
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
			{
				cnn.Open();
				try
				{
					SqlParameter[] p = new SqlParameter[3];
					p[0] = new SqlParameter("RETURN_VALUE", SqlDbType.Int);
					p[0].Direction = ParameterDirection.ReturnValue;
					p[1] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
					p[1].Value = templateID;
					p[2] = new SqlParameter("@TemplateDef", SqlDbType.Text);
					p[2].Value = templateDef;
					rtn = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "dbo.UpdateFinalTemplateDef", p);
				}
				catch (Exception ex)
				{
					throw new Exception(String.Format("Unable to update FinalTemplateDef for id={0},   Error: {1}", templateID.ToString("D"), ex.Message));
				}
				finally
				{
					cnn.Close();
				}
			}
			return rtn;
		}
  
        public static void UpdateTemplateBaseStateRole(Guid templateId, List<string> roles)
        {
            using (SqlConnection cnn = SqlHelper.GetConnection(Common.DataFactoryAppName))
            {
                cnn.Open();
                try
                {
                    SqlParameter[] p = new SqlParameter[2];
                    p[0] = new SqlParameter("@TemplateID", SqlDbType.UniqueIdentifier);
                    p[0].Value = templateId;
                    p[1] = new SqlParameter("@RoleList", SqlDbType.Structured);
                    p[1].Value = Common.GetRoles(roles);
                    p[1].TypeName = "dbo.Role";
                    int nReturn = SqlHelper.ExecuteNonQuery(cnn, CommandType.StoredProcedure, "dbo.UpdateTemplateBaseStateRole", p);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to update the TemplateBaseStateRole for TemplateID = {0}, Error: {1}", templateId.ToString(), ex.Message));
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

	}
}
