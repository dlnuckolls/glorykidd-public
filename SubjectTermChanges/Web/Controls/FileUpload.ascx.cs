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


	/// <summary>
	/// Class to contain the EventArgs needed for the FileUpload.Upload event
	/// </summary>
	public class FileUploadEventArgs : EventArgs
	{
		private readonly HttpPostedFile _postedFile;

		public HttpPostedFile PostedFile
		{
			get { return _postedFile; }
		}

		public FileUploadEventArgs(HttpPostedFile postedFile)
		{
			_postedFile = postedFile;
		}
	}


	/// <summary>
	/// A control to encapsulate an HtmlInputFile control (with a Browse... button), another to start the upload, all with the Kindred Knect look-and-feel
	/// </summary>
	public partial class FileUpload : System.Web.UI.UserControl
	{
		#region private properties
		
		private string _caption;
		private string _buttonText;
		private string _cssClass;

		#endregion


		#region
		
		public string Caption
		{
			get { return _caption; }
			set { _caption = value; }
		}

		public string ButtonText
		{
			get { return _buttonText; }
			set { _buttonText = value; }
		}

		public string CssClass
		{
			get { return _cssClass; }
			set {_cssClass = value; }
		}

		#endregion


		#region events and related methods

		public event EventHandler<FileUploadEventArgs>  Upload;

		protected virtual void OnUpload(FileUploadEventArgs e)
		{
			EventHandler<FileUploadEventArgs> temp = Upload;
			if (temp != null)
				temp(this, e);
		}

		#endregion


		#region event handlers

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			btnUpload.Command += new CommandEventHandler(btnUpload_Command);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			container.Attributes["class"] = _cssClass;
			divFake.Attributes["class"] = _cssClass;
			lblCaption.Text = _caption;
			btnUpload.Text = _buttonText;
			RegisterResizeScript();
			SetClientAttributes();
		}

		protected void btnUpload_Command(object sender, CommandEventArgs e)
		{
			FileUploadEventArgs fuea = new FileUploadEventArgs(inputFile.PostedFile);
			OnUpload(fuea);
		}

		#endregion

		#region private methods

		private void SetClientAttributes()
		{
			string script = string.Format("javascript:FixFileUploadText('{0}', '{1}');", txtFake.ClientID, inputFile.ClientID);
			inputFile.Attributes["onfilterchange"] = script;
			inputFile.Attributes["onkeyup"] = script;
			inputFile.Attributes["onselectstart"] = "javascript:return false;";
			inputFile.Style["z-index"] = "1000";
			inputFile.Style["margin"] = "0 104px 0 0";
			inputFile.Style["filter"] = "alpha(opacity:0)";
			inputFile.Style["position"] = "relative"; 
			inputFile.Style["top"] = "-2px";
			inputFile.Style["left"] = "1px";
			inputFile.Style["width"] = "1px"; 
			inputFile.Style["height"] = "19px";
		}


		private void RegisterResizeScript()
		{
			Type t = Page.GetType();

			string scriptName = "_kh_FileUploadAutoResize";
			if (!Page.ClientScript.IsClientScriptBlockRegistered(t, scriptName))
			{
				System.IO.StringWriter swResize = new System.IO.StringWriter();
				swResize.WriteLine("");
				swResize.WriteLine("function AutoResize()			");
				swResize.WriteLine("{			");
				swResize.WriteLine("	ResizeFileUpload('{0}', '{1}', '{2}');		",  container.ClientID, txtFake.ClientID, btnFake.ClientID);
				swResize.WriteLine("}			");
				Page.ClientScript.RegisterClientScriptBlock(t, scriptName, swResize.ToString(), true);
			}

			scriptName = "_kh_FileUploadStartup";
			if (!Page.ClientScript.IsStartupScriptRegistered(t, scriptName))
			{
				System.IO.StringWriter swResize = new System.IO.StringWriter();
				swResize.WriteLine("");
				swResize.WriteLine("AutoResize(); 			");
				swResize.WriteLine("var o = document.getElementById('{0}')", container.ClientID);
				swResize.WriteLine("if (o) ");
				swResize.WriteLine("	o.onresize = AutoResize;", this.ClientID);
				swResize.WriteLine("else ");
				swResize.WriteLine("	alert(\"Error: document.getElementById('{0}') not found!\" )", this.ClientID);
				Page.ClientScript.RegisterStartupScript(t, scriptName, swResize.ToString(), true);
			}
		}

		#endregion

	}
}