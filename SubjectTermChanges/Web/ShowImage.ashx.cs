using System;
using System.Drawing;
using System.IO;
using System.Web;


namespace Kindred.Knect.ITAT.Web
{
	public partial class ShowImage : IHttpHandler
	{
		private Guid _imageId;

		/// <summary>
		/// Creates an empty bitmap image and returns it as an array of bytes
		/// </summary>
		/// <returns></returns>
		private byte[] GetEmptyImage(int height, int width)
		{
            using (Bitmap empty = new Bitmap(width, height))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    empty.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                    return ms.ToArray();
                }
            }
		}


		private byte[] GetImage(Guid templateID)
		{
            byte[] rtn = Data.ITATSystem.GetSystemImage(_imageId);
			if (rtn == null)
				rtn = GetEmptyImage(32, 32);
			return rtn;
		}



		private void Render(HttpContext context, string name, byte[] bytes, string contentType)
		{
			context.Response.Clear();
			context.Response.ContentType = contentType;
			context.Response.AddHeader("Content-Header", bytes.Length.ToString());
			context.Response.AddHeader("Content-Disposition", string.Format("inline; filename={0}", name));
			context.Response.OutputStream.Write(bytes, 0, bytes.Length);
		}


		private void ProcessQueryString(HttpContext context)
		{
			string qsID = context.Request.QueryString["id"];
			if (string.IsNullOrEmpty(qsID))
				throw new Exception("'id' query string parameter not found");
			_imageId = new Guid(qsID);
		}




		#region IHttpHandler Members

		public bool IsReusable
		{
			get { return false; }
		}


        public void ProcessRequest(HttpContext context)
        {
            ProcessQueryString(context);
            byte[] image = GetImage(Guid.Empty);
            Render(context, "myImage", image, "image/gif");
        }



		#endregion
	}
}
