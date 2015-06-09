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
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Web
{
	public partial class TemplateExtensions : BaseTemplatePage
	{


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!IsPostBack)
			{
				InitializeForm(-1);
			}
		}


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			SuppressChangeNotification(grd);
			RegisterValidateAddScripts();
		}


		protected void filAddExtension_Upload(object sender, FileUploadEventArgs e)
		{
			//get the uploaded file and its metadata
			HttpPostedFile postedFile = e.PostedFile;
			string filepath = postedFile.FileName;
			string filename = filepath.Substring(filepath.LastIndexOf('\\') + 1);

			if (ValidateFile(filename))
			{
				//separate the file extension from the filename
				string fileExt = System.IO.Path.GetExtension(filename);
				string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(filename);

				int contentLength = postedFile.ContentLength;
				string contentType = postedFile.ContentType;

				System.IO.BinaryReader reader = new System.IO.BinaryReader(postedFile.InputStream);
				byte[] bytes = new byte[contentLength];
				reader.Read(bytes, 0, contentLength);

				//store the file into the data store
				Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(_itatSystem.DocumentStorageType);
				documentStorageObject.RootPath = _itatSystem.DocumentStorageRootPath;
				string objectId = documentStorageObject.SaveDocument(fileNameWithoutExt, fileExt, bytes);

				//add metadata about the extension to the template object
				Business.Extension extension = new Kindred.Knect.ITAT.Business.Extension();
				extension.FileName = filename;
				extension.ObjectID = objectId;
				_template.Extensions.Add(extension);

				//update the form
				InitializeForm(_template.Extensions.Count - 1);  //select newly added row (last row in the grid)
			}
		}


		private bool ValidateFile(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				RegisterAlert("A file must be selected.");
				return false;
			}

			if (!filename.ToUpper().EndsWith(".PDF"))
			{
				RegisterAlert("A Template Extension must be a PDF document.");
				return false;
			}

			return true;
		}


		internal override Control ResizablePanel()
		{
			return pnlGridBody;
		}

		internal override HtmlGenericControl HTMLBody()
		{
			return this.htmlBody;
		}

		protected override TemplateHeader HeaderControl()
		{
			return (TemplateHeader)header;
		}

		protected void OnHeaderEvent(object sender, HeaderEventArgs e)
		{
			switch (e.CommandName)
			{
				case Common.Names._HEADER_EVENT_Save:
                    _template.SaveExtensions(null, false);
					if (_template.IsManagedItem)
						RegisterAlert(string.Format("{0} Extensions have been saved.", this._itatSystem.ManagedItemName));
					else
						RegisterAlert("Template Extensions have been saved.");
					IsChanged = false;
					break;
				case Common.Names._HEADER_EVENT_Reset:
					GetTemplate(true);
					InitializeForm(-1);
					IsChanged = false;
					break;
				default:
					base.HandleHeaderEvent(sender, e);
					break;
			}
		}


		private void InitializeForm(int rowToSelect)
		{
			LoadGrid();
			if (rowToSelect > -1)
				grd.SelectedIndex = rowToSelect;
			else
				grd.SelectedIndex = -1;
			SetMoveUpDownButtonEvents();
		}


		private void LoadGrid()
		{
			grd.DataSource = _template.Extensions;
			grd.DataBind();
		}


		private void SwapRows(int selectedRow, int otherRow)
		{
			_template.Extensions.Reverse(Math.Min(selectedRow, otherRow), 2);
			InitializeForm(otherRow);
			IsChanged = true;
		}


		private void SetMoveUpDownButtonEvents()
		{
			int selectedRow = grd.SelectedIndex;
			if (selectedRow > 0) //enable move up button
			{
				imgMoveUp.ImageUrl = "~/Images/MoveUp.gif";
				imgMoveUp.Enabled = true;
				imgMoveUp.Style["cursor"] = "pointer";
			}
			else
			{
				imgMoveUp.ImageUrl = "~/Images/MoveUpDisabled.gif";
				imgMoveUp.Enabled = false;
				imgMoveUp.Style["cursor"] = "default";
			}

			if (selectedRow > -1 && selectedRow < grd.Rows.Count - 1)
			{
				imgMoveDown.ImageUrl = "~/Images/MoveDown.gif";
				imgMoveDown.Enabled = true;
				imgMoveDown.Style["cursor"] = "pointer";
			}
			else
			{
				imgMoveDown.ImageUrl = "~/Images/MoveDownDisabled.gif";
				imgMoveDown.Enabled = false;
				imgMoveDown.Style["cursor"] = "default";
			}
		}


		protected void imgMoveUp_OnClick(object sender, ImageClickEventArgs e)
		{
			SwapRows(grd.SelectedIndex, grd.SelectedIndex - 1);
		}


		protected void imgMoveDown_OnClick(object sender, ImageClickEventArgs e)
		{
			SwapRows(grd.SelectedIndex, grd.SelectedIndex + 1);
		}


		protected void grd_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				int rowIndex = e.Row.RowIndex;
				//Assign each row an ID containing the RowIndex
				e.Row.ID = "R" + rowIndex.ToString();
				//Set the command arguments for the View and Delete buttons 
				//Set up client-side script to prompt the user if they click the Delete button 

				LinkButton btnView = (LinkButton)e.Row.Cells[1].Controls[0];
				string documentId = _template.Extensions[rowIndex].ObjectID;
				string filename = _template.Extensions[rowIndex].FileName;
				btnView.OnClientClick = string.Format("javascript:window.open('ShowDoc.ashx{0}','_blank',''); return false;", Utility.TextHelper.QueryString(true, "type","extension", Common.Names._QS_ITAT_SYSTEM_ID, _itatSystem.ID.ToString(), Common.Names._QS_DOCUMENT_ID, documentId, Common.Names._QS_FILENAME, filename));

				//do not allow the deletion of Extension to managed item INSTANCES.
				LinkButton btnDelete = (LinkButton)e.Row.Cells[2].Controls[0];
				if (_template.IsManagedItem)
				{
					btnDelete.Visible = false;
				}
				else
				{
					btnDelete.CommandArgument = rowIndex.ToString();
					btnDelete.OnClientClick = "javascript:return confirm('Are you sure you want to delete this extension?');";
				}
			}
		}


		protected void grd_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "DeleteRow":
					int rowIndex = Convert.ToInt32(e.CommandArgument);
					DeleteExtension(rowIndex);
					IsChanged = true;
					break;

				case "singleClick":
				case "doubleClick":
					SetMoveUpDownButtonEvents();
					break;

				default:
					break;
			}
		}


		private void DeleteExtension(int rowIndex)
		{
			//NOTE: We do NOT want to delete the document from the Document Store, because the extension may be used in existing Managed Items!
			//remove from the attachment collection and refresh the screen
			_template.Extensions.RemoveAt(rowIndex);
			InitializeForm(-1);
		}

		private void RegisterValidateAddScripts()
		{
			Type t = this.GetType();
			string scriptName = "_kh_ValidateAdd";
			if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
			{
				//txtFake
				System.IO.StringWriter swValidate = new System.IO.StringWriter();
				swValidate.WriteLine("function ValidateAdd()");
				swValidate.WriteLine("{");
				swValidate.WriteLine("	var txtBox = document.getElementById('txtFake');");
				swValidate.WriteLine("	if (txtBox)");
				swValidate.WriteLine("	{");
				swValidate.WriteLine("		var txtstring = txtBox.value;");
				swValidate.WriteLine("		var lowtxtstring = txtstring.toLowerCase();");
				swValidate.WriteLine("		if (lowtxtstring.lowtxtstring.length == 0)");
				swValidate.WriteLine("		{");
				swValidate.WriteLine("			alert('You must select a file to add.');");
				swValidate.WriteLine("			return false;");
				swValidate.WriteLine("		}");
				swValidate.WriteLine("		if (lowtxtstring.indexOf('.pdf') != lowtxtstring.length - 4)");
				swValidate.WriteLine("		{");
				swValidate.WriteLine("			alert('You are only allowed to add PDF files.');");
				swValidate.WriteLine("			return false;");
				swValidate.WriteLine("		}");
				swValidate.WriteLine("	}");
				swValidate.WriteLine("	else");
				swValidate.WriteLine("	{");
				swValidate.WriteLine("		return false;");
				swValidate.WriteLine("	}");
				swValidate.WriteLine("	return true;");
				swValidate.WriteLine("}");
				ClientScript.RegisterClientScriptBlock(t, scriptName, swValidate.ToString(), true);
			}
		}

	
	}
}
