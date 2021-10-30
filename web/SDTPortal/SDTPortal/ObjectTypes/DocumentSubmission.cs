using System;
using System.Collections.Generic;
using System.Data;
using Telerik.Web.UI;

namespace SDTPortal {
  public class DocumentSubmission {
    public string Id { get; set; }
    public string FromUser { get; set; }
    public string ToEmail { get; set; }
    public DateTime SubmitDate { get; set; }
    public string SubjectLine { get; set; }
    public List<UploadedFile> Attachments { get; set; }
    public List<SavedDocument> SavedDocs { get; set; }

    public void SaveDocument(string savePath) {
      Id = SqlHelpers.InsertScalar(SqlStatements.SQL_SAVE_MESSAGE.FormatWith(FromUser, ToEmail, SubmitDate.ConvertSqlDateTime(), SubjectLine.FixSqlString()));
      SavedDocs = new List<SavedDocument>();
      Attachments.ForEach(doc => {
        var docId = SqlHelpers.InsertScalar(SqlStatements.SQL_SAVE_MESSAGE_ATTACHMENT.FormatWith(Id, ToEmail, SubmitDate.ConvertSqlDateTime(), doc.GetName()));
        doc.SaveAs("{0}/{1}.{2}".FormatWith(savePath, docId, doc.GetExtension()));
        SavedDocs.Add(new SavedDocument() { FileLocation = "{0}/{1}.{2}".FormatWith(savePath, docId, doc.GetExtension()), FileName = doc.GetName() });
      });
    }
  }

  public struct SavedDocument {
    public string FileLocation;
    public string FileName;
  }

  public class DocumentAttachmentLinks {
    public string SubjectLine;
    public List<SavedDocument> Attachments;
    public DocumentAttachmentLinks(string documentId) {
      Attachments = new List<SavedDocument>();
      var files = SqlHelpers.Select(SqlStatements.SQL_GET_DOCUMENT_ATTACHMENTS.FormatWith(documentId));
      foreach(DataRow dr in files.Rows) {
        SubjectLine = dr["SubjectLine"].ToString();
        var doc = dr["Attachment"].ToString();
        Attachments.Add(new SavedDocument() { FileLocation = "https://sdp.aisweb.com/docUploads/{0}.{1}".FormatWith(dr["Id"].ToString(), doc.Substring(doc.LastIndexOf('.'))), FileName = doc });
      }
    }
  }
}