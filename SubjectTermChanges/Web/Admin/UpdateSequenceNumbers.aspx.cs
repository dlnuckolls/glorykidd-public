using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Xml;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.IO;


namespace Kindred.Knect.ITAT.Web
{
	public partial class UpdateSequenceNumbers : BaseSystemPage
	{

		protected override string GetApplicationFunction()
		{
			return Business.XMLNames._AF_ImportData;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			ClientScript.RegisterClientScriptInclude(this.GetType(), "_kh_jsITAT", "Scripts/itat.js");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (IsPostBack)
			{
				if (ImportData())
					RegisterAlert("Complete.");
			}
		}



		private bool ImportData()
		{
			if (filUpload.PostedFile == null || filUpload.PostedFile.ContentLength == 0)
			{
				RegisterAlert("A valid input file must be selected.");
				return false;
			}
			else
			{
				using (System.Transactions.TransactionScope transScope = new System.Transactions.TransactionScope(Data.Common.TransactionScopeOption))
				{
					if (chkDeleteFirst.Checked)
					{
						Data.ITATSystem.DeleteSequenceNumbers(_itatSystem.ID);
					}
					StreamReader reader = new StreamReader(filUpload.PostedFile.InputStream);
					Data.ITATSystem.UpdateSequenceNumbers(_itatSystem.ID, reader.ReadToEnd());
					transScope.Complete();
					return true;
				}
			}
		}



		internal override HtmlGenericControl HTMLBody()
		{
			return htmlBody;
		}

		internal override Control ResizablePanel()
		{
			return null;
		}
	}
}
