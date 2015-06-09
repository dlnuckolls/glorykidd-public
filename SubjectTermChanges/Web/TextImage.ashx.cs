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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web.Services;
using System.Web.Services.Protocols;


namespace Kindred.Knect.ITAT.Web
{

	[WebService(Namespace = "http://kindredhealthcare.com/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class TextImg : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			string text = GetText(context);
			DrawText(context, text);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}


		private void DrawText(HttpContext context, string text)
		{
			Font f = new Font("verdana", 8, FontStyle.Bold);
			SizeF size = Utility.ImageHelper.GetImageSize(text, f);
			size.Height += 2.0F;
			size.Width += 2.0F;
			RectangleF rectF = new RectangleF(0, 1F, size.Width - 1, size.Height - 2);
			Rectangle rect = new Rectangle(0, 0, (int)size.Width - 1, (int)size.Height - 2);
			MemoryStream strm = new MemoryStream();
			Bitmap pic = new Bitmap((int)size.Width, (int)size.Height, PixelFormat.Format32bppPArgb);
			Pen linePen = new Pen(Color.FromArgb(164, 64, 64), 1);
			SolidBrush textBrush = new SolidBrush(Color.FromArgb(64, 64, 164));
			SolidBrush fillBrush = new SolidBrush(Color.FromArgb(255, 255, 164));
			Graphics g = Graphics.FromImage(pic);
			g.Clear(Color.FromArgb(255, 255, 255, 255));
			g.SmoothingMode = SmoothingMode.AntiAlias;

			g.DrawRectangle(linePen, rect);
			g.FillRectangle(fillBrush, rect);
			g.DrawString(text, f, textBrush, rectF);

			context.Response.ContentType = "image/png";
			pic.Save(strm, ImageFormat.Png);
			strm.WriteTo(context.Response.OutputStream);

			linePen.Dispose();
			textBrush.Dispose();
			g.Dispose();
			pic.Dispose();
		}


		private string GetText(HttpContext context)
		{
			string rtn = context.Request.QueryString["text"];
			if (string.IsNullOrEmpty(rtn))
				rtn = "TERM";
			return rtn;
		}


	}
}
