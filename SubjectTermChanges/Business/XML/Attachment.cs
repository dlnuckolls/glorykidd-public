using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class Attachment
	{

		private Guid _documentID;
		private string _name;
		private string _description;
		private string _documentStoreID;
		private DocumentType _documentType;
		private bool _isScanned;


		public Guid DocumentID
		{
			get { return _documentID; }
		}

		public string Name
		{
			get { return _name; }
		}

		public string Description
		{
			get { return _description; }
		}

		public string DocumentStoreID
		{
			get { return _documentStoreID; }
		}

		public DocumentType DocumentType
		{
			get { return _documentType; }
		}

		public bool IsScanned
		{
			get { return _isScanned; }
		}


		public Attachment(Guid documentID, string name, string description, string documentStoreID, DocumentType documentType, bool isScanned)
		{
			_documentID = documentID;
			_name = name;
			_description = description;
			_documentStoreID = documentStoreID;
			_documentType = documentType;
			_isScanned = isScanned;
		}


		public byte[] GetContents(ITATSystem itatSystem, out string contentType)
		{
			Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(itatSystem.DocumentStorageType);
			documentStorageObject.RootPath = itatSystem.DocumentStorageRootPath;
			return documentStorageObject.GetDocument(itatSystem.ID, _documentStoreID, out contentType);
		}


		public static Attachment FindAttachmentByDocumentID(List<Attachment> attachments, Guid documentID)
		{
			for (int i = 0; i < attachments.Count; i++)
				if (attachments[i].DocumentID == documentID)
					return attachments[i];
			return null;
		}

		public bool UpdateDocumentType(ITATSystem itatSystem, string documentType)
		{
			_documentType.Name = documentType;
            return Data.Document.UpdateDocumentType(_documentID, documentType);
		}


        public static TermStore CreateStore(string termName, Guid managedItemID)
        {
            TermStore termStore = new TermStore(termName, TermType.PlaceHolderAttachments);
            DataTable dtDocuments = Business.ManagedItem.GetManagedItemDocuments(managedItemID);
            if (dtDocuments.Rows.Count > 0)
            {
                foreach (DataRow drDocument in dtDocuments.Rows)
                {
                    if (!(bool)drDocument[Data.DataNames._C_Deleted])
                        termStore.AddMultiValue(drDocument[Data.DataNames._C_DocumentName].ToString());
                }
            }
            return termStore;
        }




	}
}
