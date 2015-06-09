//#define _NO_KDMS
using System;
using System.Collections.Generic;
using System.Text;
using Kindred.Common.Logging;
using System.IO;
using System.Reflection;
using Kindred.Common.WebServices;
using System.Configuration;

namespace Kindred.Knect.ITAT.Utility
{
	public class DocumentumDocumentStorage : DocumentStorage
	{
		public DocumentumDocumentStorage()
		{
			#if _NO_KDMS
				#warning *********_NO_KDMS is defined - this should be the case only if Documentum is NOT available************
			#endif
			_documentStorageType = DocumentStorageType.Documentum;
		}

		public override string SaveDocument(string filename, string contentType, byte[] documentContents)
		{
			#if _NO_KDMS
				return "9999999999999999";
			#else
				if (string.IsNullOrEmpty(_rootPath))
					throw new Exception("DocumentumDocumentStorage RootPath property has not been set.");
				if (!ValidDocumentType(contentType))
					throw new ArgumentException(string.Format("{0} '{1}'.", Names._EM_UnrecognizedExtension, contentType));

			    KDMS.ImportConfiguration ic = new Kindred.Knect.ITAT.Utility.KDMS.ImportConfiguration();
				ic.DocuType = Names.KDMS_DocType;
				System.Collections.Hashtable ht = new System.Collections.Hashtable();
				ht.Add(Names.KDMS_Property_DocumentName, filename);
				ic.PropertyList = Utility.WebServiceHelper.ToJaggedArray(ht);
				ic.LifeCyclePolicyName = "ITAT Scanned Document Lifecycle";
				ic.DocuFolderPath = _rootPath;
				try
				{
					KDMS.Service svc = new KDMS.Service();
					svc.Credentials = System.Net.CredentialCache.DefaultCredentials;
					svc.Url = ConfigurationManager.AppSettings[string.Format("{0}.{1}", Utility.EnvironmentHelper.GetEnvironment(EnvironmentDetectionMode.Machine), svc.GetType())];
					return svc.ImportDocumentContent(documentContents, contentType, ic);
				}
				catch (Exception e)
				{
					string error = string.Format("Error uploading file '{0}' to KDMS : {1}",filename,e.ToString());
					ILog log = LogManager.GetLogger(this.GetType());
					log.Error(error);
					throw new Exception(error, e);
				}
			#endif
		}

		public override byte[] GetDocument(Guid systemId, string documentStoreId, out string contentType)
		{
			#if _NO_KDMS
				contentType = "pdf";
				try
				{
					//Note - Refer to:  http://support.microsoft.com/kb/319292
					Assembly assembly = Assembly.GetExecutingAssembly();
					Stream imageStream = assembly.GetManifestResourceStream("Kindred.Knect.ITAT.Utility.NoKDMS.pdf");
					BinaryReader reader = new BinaryReader(imageStream);
					byte[] bytes = reader.ReadBytes((int)imageStream.Length);
					reader.Close();
					return bytes;
				}
				catch
				{
					return null;
				}
			#else
			KDMS.Service svc = new KDMS.Service();
			svc.Credentials = System.Net.CredentialCache.DefaultCredentials;
			svc.Url = ConfigurationManager.AppSettings[string.Format("{0}.{1}", Utility.EnvironmentHelper.GetEnvironment(EnvironmentDetectionMode.Machine), svc.GetType())];
			return svc.ExportDocumentContentByChronicleID(documentStoreId, out contentType); 
			#endif
		}


		public override bool DeleteDocument(string documentStoreId)
		{
			#if _NO_KDMS
				return true;
			#else
				bool bReturn = false;
				try 
				{
					KDMS.Service svc = new KDMS.Service();
					svc.Credentials = System.Net.CredentialCache.DefaultCredentials;
					svc.Url = ConfigurationManager.AppSettings[string.Format("{0}.{1}", Utility.EnvironmentHelper.GetEnvironment(EnvironmentDetectionMode.Machine), svc.GetType())];
					bReturn = svc.DeleteDocument(documentStoreId, true);
				}
				catch (Exception e)
				{
					ILog log = LogManager.GetLogger(this.GetType());
					log.Error(string.Format("Error deleting document '{0}' from KDMS:{1}", documentStoreId, e.ToString()));
				}
				return bReturn;
			#endif
		}

		private bool ValidDocumentType(string extension)
		{
			string mimeType = WebServiceHelper.ExtensionToMimeType(extension);
			if (string.IsNullOrEmpty(mimeType))
				return false;
			return true;
		}


	}
}
