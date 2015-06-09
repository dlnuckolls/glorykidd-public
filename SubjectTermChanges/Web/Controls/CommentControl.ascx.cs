using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Kindred.Common.Security;

namespace Kindred.Knect.ITAT.Web
{
	public partial class CommentControl : System.Web.UI.UserControl
	{

		#region private members

		private bool _canAddComments;
		private List<Business.Comment> _comments;

		#endregion


		#region properties

		public bool CanAddComments
		{
			get { return _canAddComments; }
			set { _canAddComments = value; }
		}

		public List<Business.Comment> Comments
		{
			get { return _comments; }
			set { _comments = value; }
		}

		#endregion


		#region  base class overrides

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!IsPostBack)
			{
				if (_comments == null)
					throw new NullReferenceException("The Comments property has not been initialized.");
			}
		}


		protected override void OnPreRender(EventArgs e)
		{
			BindData();
			base.OnPreRender(e);
			txtNewComment.Text = string.Empty;
            txtNewComment.Visible = _canAddComments;
		}


		#endregion


		#region private methods


		protected void lstComments_ItemDataBound(object sender, DataListItemEventArgs e)
		{
			switch (e.Item.ItemType)
			{
				case ListItemType.AlternatingItem:
				case ListItemType.Item:
					Business.Comment comment = (Business.Comment)e.Item.DataItem;
					Literal litCommentHeader = FindLiteralControl(e.Item, "litCommentHeader");
					litCommentHeader.Text = string.Format("{0} ({1}) - {2:MMMM dd, yyyy h:mm tt}", comment.UserName, comment.UserID, comment.Created);
					Literal litCommentBody = FindLiteralControl(e.Item, "litCommentBody");
					//20070821_DEG Added the HtmlEncode call since we added 'ValidateRequest="false"' to the aspx page.
					litCommentBody.Text = HttpUtility.HtmlEncode(comment.Text);
					break;
				default:
					break;
			}
		}


		private Literal FindLiteralControl(Control parentControl, string controlName)
		{
			Control c = parentControl.FindControl(controlName);
			if (c == null)
				throw new Exception(string.Format("Unable to locate the control \"{0}\"."));
			Literal rtn = c as Literal;
			if (c == null)
				throw new Exception(string.Format("The control \"{0}\" is not a Literal control.", controlName));
			return rtn;
		}


		private void BindData()
		{
			lstComments.DataSource = _comments;
			lstComments.DataBind();
		}


		private void AddComment(string commentText)
		{
			BaseSystemPage page = Page as BaseSystemPage;
			if (page == null)
				throw new Exception("The CommentControl.ascx control can only be added to a page that derives from BaseSystemPage.");
			Business.Comment newComment = new Kindred.Knect.ITAT.Business.Comment();
			newComment.Created = DateTime.Now;
			ADPerson person = page.SecurityHelper.Person;
			newComment.UserID = person.SAMLogonName;
			newComment.UserName = person.FullName;
			newComment.Text = commentText;
			_comments.Insert(0, newComment);  //insert at index 0 since commens are shown most-recent first.
		}
		
		#endregion


		public void UpdateManagedItemComments(string termGroupContainerName)
		{
			string prefix = (string.IsNullOrEmpty(termGroupContainerName) ? "" : termGroupContainerName + "$");
			string commentText = this.Request.Form[prefix + txtNewComment.UniqueID];
			if (!string.IsNullOrEmpty(commentText))
				AddComment(commentText);
		}


	}
}