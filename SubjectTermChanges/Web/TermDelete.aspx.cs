using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web
{
	public partial class TermDelete : BasePage
	{

		#region private members
		//private const string QSKEY_TERMNAME = "Term";
		//private const string QSKEY_ISCOMPLEXLIST = "ComplexList";

		#endregion


		#region base class overrides

		internal override HtmlGenericControl HTMLBody()
		{
			return this.body;
		}

		internal override Control ResizablePanel()
		{
			return editBody;
		}

		#endregion


		#region page and button event handlers


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				//string s = _itatSystem.ManagedItemName;
				//lblItemName1.Text = s;
				//lblItemName2.Text = s;
			}
		}


		protected void btnYes_Click(object sender, EventArgs e)
		{
			DeleteTerm();
			CloseDialog();
		}


		protected void btnNo_Click(object sender, EventArgs e)
		{
			CloseDialog();
		}

		#endregion


		#region private methods

		private void DeleteTerm()
		{
			////Get query string values
			//string qsTermName = Request.QueryString[QSKEY_TERMNAME];
			//string qsIsComplexList = Request.QueryString[QSKEY_ISCOMPLEXLIST];

			////validate query string
			//if (string.IsNullOrEmpty(qsTermName))
			//   throw new Exception(string.Format("\"{0}\" not specified in the query string.", QSKEY_TERMNAME));
			//if (string.IsNullOrEmpty(qsIsComplexList))
			//   throw new Exception(string.Format("\"{0}\" not specified in the query string.", QSKEY_ISCOMPLEXLIST));

			//bool isComplexList;
			//bool.TryParse(qsIsComplexList, out isComplexList);
			//if (isComplexList)
			//   _template.DeleteComplexTerm(qsTermName);
			//else
			//   _template.DeleteBasicTerm(qsTermName);
		}

		#endregion

	}
}
