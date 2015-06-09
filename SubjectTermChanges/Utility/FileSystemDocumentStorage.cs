using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Utility
{
	public class FileSystemDocumentStorage : DocumentStorage
	{
		public FileSystemDocumentStorage()
		{
			_documentStorageType = DocumentStorageType.FileSystem;
		}

		public override string SaveDocument(string filename, string contentType, byte[] documentContents)
		{
			if (string.IsNullOrEmpty(_rootPath))
				throw new Exception("FileSystemDocumentStorage RootPath property has not been set.");
			string domain = Utility.EnvironmentHelper.GetEnvironment(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine);
			if (domain == "LOCAL")
				domain = "T1";
			string fullPath = string.Format(Names.FS_FilePathPattern, domain, _rootPath, filename);
			string folder = System.IO.Path.GetDirectoryName(fullPath);
			if (!System.IO.Directory.Exists(folder))
				System.IO.Directory.CreateDirectory(folder);
			System.IO.File.WriteAllBytes(fullPath, documentContents);
			return fullPath;
		}


		public override byte[] GetDocument(Guid systemId, string documentStoreId, out string contentType)
		{
			byte[] bytes = System.IO.File.ReadAllBytes(documentStoreId);
			contentType = string.Empty;
			return bytes;
		}


		public override bool DeleteDocument(string documentStoreId)
		{
			try
			{
				System.IO.File.Delete(documentStoreId);
				return true;
			}
			catch
			{
				return false;
			}
		}

	}
}
