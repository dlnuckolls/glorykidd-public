using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Utility
{

	public enum DocumentStorageType
	{
		Unknown = 0,
		Documentum = 1,
		FileSystem = 2
	}


	/// <summary>
	/// An class containing methods which will store documents in, and 
	/// retrieve documents from, a document store such as Documentum.
	/// </summary>
	public abstract class DocumentStorage
	{
		protected DocumentStorageType _documentStorageType;
		protected string _rootPath;

		public DocumentStorageType DocumentStorageType
		{
			get { return _documentStorageType; }
		}

		public string RootPath
		{
			get { return _rootPath; }
			set { _rootPath = value; }
		}

		public static DocumentStorage GetDocumentStorageObject(DocumentStorageType documentStorageType)
		{
			switch (documentStorageType)
			{
				case DocumentStorageType.Documentum:
					return new DocumentumDocumentStorage();
				case DocumentStorageType.FileSystem:
					return new FileSystemDocumentStorage();
				case DocumentStorageType.Unknown:
				default:
					throw new Exception("A valid document storage type must be supplied.");
			}
		}


		/// <summary>
		/// Saves a document to the document store.
		/// </summary>
		/// <param name="filename">The full path of the file (minus the extension) in the format required by the data store system</param>
		/// <param name="contentType">The contentType (a.k.a Mime Type) of the document</param>
		/// <param name="documentContents">A byte array containing the contents of the file</param>
		/// <returns>A string uniquely identifying this document within the datastore.  In Documentum this would be the  ChronicleID (or ObjectID if there is no document lifecycle).</returns>
		public abstract string SaveDocument(string filename, string contentType, byte[] documentContents);


		/// <summary>
		/// Retrieves the document from the data store
		/// </summary>
		/// <param name="systemId">The ITAT system ID</param>
		/// <param name="documentStoreId">The unique identifier that the data store uses to identify the document when SaveDocument() was called. </param>
		/// <returns>A byte array containing the document contents</returns>
		public abstract byte[] GetDocument(Guid systemId, string documentStoreId, out string contentType);

		/// <summary>
		/// Deletes a document from the document storage system
		/// </summary>
		/// <param name="documentStoreId">The unique identifier that the data store used to identify the document when SaveDocument() was called. </param>
		/// <returns></returns>
		public abstract bool DeleteDocument(string documentStoreId);


	}
}
