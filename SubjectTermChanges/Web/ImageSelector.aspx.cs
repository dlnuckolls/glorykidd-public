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
using System.IO;
using System.Drawing;

namespace Kindred.Knect.ITAT.Web
{
	public partial class ImageSelector : System.Web.UI.Page
	{
		private Guid _systemId = Guid.Empty;

		public Guid SystemId
		{
			get
			{
				if (_systemId == Guid.Empty)
				{
					string qsSystemId = Request.QueryString["system"];
					if (string.IsNullOrEmpty(qsSystemId))
						throw new Exception("Query String paramter 'system' not found or blank");
					_systemId = new Guid(qsSystemId);
				}
				return _systemId;				
			}
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				GetImages();
			}
		}


		private void GetImages()
		{
			DataTable dt = Data.ITATSystem.GetSystemImages(SystemId);
			if (imageList == null)
				throw new NullReferenceException("imageList is null");
			if (dt == null)
				throw new NullReferenceException("dt is null");
			imageList.DataSource = dt;
			imageList.DataBind();
		}


		private void AddImage(string imageName, System.Drawing.Bitmap image)
		{
            using (MemoryStream strm = new MemoryStream())
            {
                image.Save(strm, System.Drawing.Imaging.ImageFormat.Gif);
                byte[] imageBytes = strm.ToArray();
                Data.ITATSystem.InsertSystemImage(SystemId, imageName, image.Height, image.Width, imageBytes);
                GetImages();
            }
		}


		private void DeleteImage(Guid imageId)
		{
            Data.ITATSystem.DeleteSystemImage(imageId);
			GetImages();
		}


		protected void imageUploadOnUpload(object sender, FileUploadEventArgs e)
		{
			string imageName = Path.GetFileNameWithoutExtension(e.PostedFile.FileName);
			int imageSize = e.PostedFile.ContentLength;
            using (System.Drawing.Bitmap img = new System.Drawing.Bitmap(e.PostedFile.InputStream))
            {
                AddImage(imageName, img);
            }
		}

		private Size ConstrainedImageDimensions(Size originalImageDimensions)
		{
			const int MAX_IMAGE_DIMENSION = 130;
			Size rtn = new Size();
			int maxDimension = Math.Max(originalImageDimensions.Width, originalImageDimensions.Height);
			if (maxDimension > MAX_IMAGE_DIMENSION)
			{				
				double scaleFactor = (double)MAX_IMAGE_DIMENSION / (double)maxDimension;
				rtn.Width = (int)(originalImageDimensions.Width * scaleFactor);
				rtn.Height = (int)(originalImageDimensions.Height * scaleFactor);
			}
			else
			{
				rtn.Width = originalImageDimensions.Width;
				rtn.Height = originalImageDimensions.Height;
			}
			return rtn;
		}

		protected void imageListItemDataBound(object sender, DataListItemEventArgs e)
		{
			DataRowView row = (DataRowView)e.Item.DataItem;
			Guid imageId = (Guid)row["ImageID"];
			string imageName = (string)row["ImageName"];
			Size imageDimensions = ConstrainedImageDimensions(new Size((int)row["ImageWidth"], (int)row["ImageHeight"]));
			bool canDelete = ((bool)(row["InUse"]) == false);

			System.Web.UI.WebControls.Image image = e.Item.FindControl("itemImage") as System.Web.UI.WebControls.Image;
			if (image != null)
			{
				image.Width = Unit.Pixel(imageDimensions.Width);
				image.Height = Unit.Pixel(imageDimensions.Height);
				image.ImageUrl = string.Format("ShowImage.ashx?id={0}", imageId);
				string onclick = string.Concat("window.returnValue={id:'", imageId.ToString(), "', name:'", imageName ,"'}; window.close();");
				image.Attributes["onclick"] = onclick;
			}

			Label lblName = e.Item.FindControl("itemName") as Label;
			if (lblName != null)
			{
				lblName.Text = imageName;
			}

			LinkButton lnkDelete = e.Item.FindControl("itemDelete") as LinkButton;
			if (lnkDelete != null)
			{
				if (canDelete)
				{
					lnkDelete.Visible = true;
					lnkDelete.CommandName = "Delete";
					lnkDelete.CommandArgument = imageId.ToString("D");
					lnkDelete.OnClientClick = "return confirm('Are you sure you want to delete this image?');";
				}
				else
				{
					lnkDelete.Visible = false;
				}
			}

		}


		protected void imageListItemCommand(object sender, DataListCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
				DeleteImage(new Guid(e.CommandArgument.ToString()));
		}


	}
}
