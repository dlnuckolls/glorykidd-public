using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using Kindred.Common.Logging;

namespace Kindred.Knect.ITAT.Services
{
	/// <summary>
	/// Summary description for Imaging
	/// </summary>
	[WebService(Namespace = "http://kindredhealthcare.com/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class Imaging : System.Web.Services.WebService, IImaging
	{

		#region IImaging Members

		[WebMethod]
		public bool ValidateManagedItemNumber(Guid systemId, string managedItemNumber)
		{
			ILog log = LogManager.GetLogger(this.GetType());
			DataSet ds = null;
			int rowCount = 0;
			try
			{
				ds = Data.ManagedItem.FindByNumber(systemId, managedItemNumber, null);
				if (ds.Tables.Count == 0)
				{
					string msg = string.Format("Data.ManagedItem.FindByNumber() returned no tables -- ManagedItem=\"{0}\".", managedItemNumber);
					ThrowAndLogException(msg, new Exception(msg), log);
				}
				if (ds.Tables[0].Rows.Count == 0)
				{
					string msg = string.Format("Data.ManagedItem.FindByNumber() returned no rows -- ManagedItem=\"{0}\".", managedItemNumber);
					ThrowAndLogException(msg, new Exception(msg), log);
				}
				rowCount = ds.Tables[0].Rows.Count;
			}
			catch (Exception ex)
			{
				ThrowAndLogException(string.Format("Error calling Data.ManagedItem.FindByNumber() -- ManagedItem=\"{0}\"", managedItemNumber), ex, log);
			}
			log.Debug(string.Format("Validating ManagedItem '{0}' -- {1} instances found.", managedItemNumber, rowCount));
			return (rowCount > 0);
		}

		[WebMethod]
		public string StoreDocument(Guid systemId, string managedItemNumber, string filePath)
		{
			ILog log = LogManager.GetLogger(this.GetType());
			log.Debug(string.Format("Entering StoreDocument(\"{0}\", \"{1}\", \"{2}\").", systemId, managedItemNumber, filePath));
			string displayedFileName = "Document Scanned " + Utility.DateHelper.FormatDate(DateTime.Now, "MMM dd, yyyy hh:mm tt");
			string contentType = System.IO.Path.GetExtension(filePath).TrimStart('.');   // contentType = file extension without the "."
			
			Business.ITATSystem itatSystem = null;
			try
			{
				itatSystem = Business.ITATSystem.Get(systemId);
				if (itatSystem == null)
				{
					string msg = string.Format("Business.ITATSystem.Get() returned null -- systemId=\"{0}\"", systemId);
					ThrowAndLogException(msg, new Exception(msg), log);
				}
			}
			catch (Exception ex)
			{
				ThrowAndLogException(string.Format("Error calling Business.ITATSystem.Get() -- systemId=\"{0}\"", systemId), ex, log);
			}
			
			Utility.DocumentStorage documentStorageObject = null;
			try
			{
				documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(itatSystem.DocumentStorageType);
			}
			catch (Exception ex)
			{
				ThrowAndLogException("Error calling Utility.DocumentStorage.GetDocumentStorageObject()", ex, log);
			}

			documentStorageObject.RootPath = itatSystem.DocumentStorageRootPath;
			string filename = System.IO.Path.GetFileName(filePath);
			string fileExt = System.IO.Path.GetExtension(filename).ToLowerInvariant();
			string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(filename);
			byte[] bytes = System.IO.File.ReadAllBytes(filePath);
			string documentStoreId = null;
			try
			{
				documentStoreId = documentStorageObject.SaveDocument(fileNameWithoutExt, fileExt, bytes);
				if (string.IsNullOrEmpty(documentStoreId))
				{
					string msg = string.Format("Utility.DocumentStorage.SaveDocument() returned null -- ManagedItem=\"{0}\", FilePath=\"{1}\".", managedItemNumber, filePath);
					ThrowAndLogException(msg, new Exception(msg), log);
				}
			}
			catch (Exception ex)
			{
				ThrowAndLogException(string.Format("Error calling Utility.DocumentStorage.SaveDocument() -- ManagedItem=\"{0}\", FilePath=\"{1}\"", managedItemNumber, filePath), ex, log);
			}
			
			DataSet ds = null;
			Guid managedItemId = Guid.Empty;
			try
			{
				ds = Data.ManagedItem.FindByNumber(systemId, managedItemNumber, null);
				if (ds.Tables.Count == 0)
				{
					string msg = string.Format("Data.ManagedItem.FindByNumber() returned no tables -- ManagedItem=\"{0}\".", managedItemNumber);
					ThrowAndLogException(msg, new Exception(msg), log);
				}
				if (ds.Tables[0].Rows.Count == 0)
				{
					string msg = string.Format("Data.ManagedItem.FindByNumber() returned no rows -- ManagedItem=\"{0}\".", managedItemNumber);
					ThrowAndLogException(msg, new Exception(msg), log);
				}
				managedItemId = (Guid)ds.Tables[0].Rows[0][Data.DataNames._C_ManagedItemID];
			}
			catch (Exception ex)
			{
				ThrowAndLogException(string.Format("Error calling Data.ManagedItem.FindByNumber() -- ManagedItem=\"{0}\"", managedItemNumber), ex, log);
			}

			Business.ManagedItem mi = null;
			try
			{
				mi = Business.ManagedItem.Get(managedItemId, false);
			}
			catch (Exception ex)
			{

				ThrowAndLogException(string.Format("Error calling Business.ManagedItem.Get() -- managedItemId=\"{0}\"", managedItemId), ex, log);
			}

			Business.Attachment att = null;
			try
			{
				att = new Business.Attachment(Guid.NewGuid(), displayedFileName, "", documentStoreId, new Kindred.Knect.ITAT.Business.DocumentType(""), true);
				mi.Attachments.Add(att);
				mi.SaveAttachments();
			}
			catch (Exception ex)
			{
				ThrowAndLogException(string.Format("Error saving attachment -- managedItemId=\"{0}\", documentStoreId=\"{1}\".", managedItemId, documentStoreId), ex, log);
			}
			try
			{
				System.IO.File.Delete(filePath);
			}
			catch (Exception ex)
			{
				ThrowAndLogException(string.Format("Error deleting temp file \"{0}\".", filePath), ex, log);
			}
			return documentStoreId;
		}

		#endregion


		private void ThrowAndLogException(string contextMessage, Exception ex, ILog log)
		{
			log.Error(contextMessage, ex);
			throw ex;
		}

	}
}
