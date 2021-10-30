using System;
using System.Collections.Generic;
using System.Data;
using Telerik.Web.UI;

namespace AISWeb {
  public class DocumentSubmission {
    public string Id { get; set; }
    public string FromUser { get; set; }
    public string ToEmail { get; set; }
    public DateTime SubmitDate { get; set; }
    public string SubjectLine { get; set; }
    public List<UploadedFile> Attachments { get; set; }

    public void SaveDocument() {
      Id = SqlHelpers.InsertScalar(SqlStatements.SQL_SAVE_MESSAGE.FormatWith(FromUser, ToEmail, SubmitDate.ConvertSqlDateTime(), SubjectLine.FixSqlString()));
      Attachments.ForEach(doc => {
        SqlHelpers.Insert(SqlStatements.SQL_SAVE_MESSAGE_ATTACHMENT.FormatWith(Id, ToEmail, SubmitDate.ConvertSqlDateTime(), doc.GetName()));
      });
    }
  }
}