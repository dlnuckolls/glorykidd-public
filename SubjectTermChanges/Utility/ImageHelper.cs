using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Text;

namespace Kindred.Knect.ITAT.Utility
{
	public static class ImageHelper
	{


/// Call the CreateImage() function to save the text entered into the text box to a stream which will be
/// drawn in the CreateImage() function:
///  CreateImage( txt_Image.Text, font ).Save( Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif );

		static public Bitmap CreateImage(string text, FontInfo fontInfo, Color color)
		{
			Bitmap bmpImage = new Bitmap(1, 1);

			int iWidth = 0;
			int iHeight = 0;

			float emSize = Convert.ToSingle(fontInfo.Size.Unit.Value);
			emSize = (emSize == 0 ? 12 : emSize);
			Font stringFont = new Font(fontInfo.Name, emSize);

			// Create a graphics object to measure the text's width and height.
			Graphics MyGraphics = Graphics.FromImage(bmpImage);

			// This is where the bitmap size is determined.
			iWidth = (int)MyGraphics.MeasureString(text, stringFont).Width;
			iHeight = (int)MyGraphics.MeasureString(text, stringFont).Height;

			// Create the bmpImage again with the correct size for the text and font.
			bmpImage = new Bitmap(bmpImage, new Size(iWidth, iHeight));

			// Add the colors to the new bitmap.
			MyGraphics = Graphics.FromImage(bmpImage);
			MyGraphics.Clear(Color.Navy);
			MyGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			MyGraphics.DrawString(text, stringFont, new SolidBrush(color), 0, 0);
			MyGraphics.Flush();
			return (bmpImage);
		}

		/// <summary>
		/// Determines the number of spaces (&nbsp;) needed to occupy the specified width using the specified font, rounded to the nearest integer
		/// </summary>
		/// <param name="width">desired width in pixels</param>
		/// <param name="f">a font</param>
		/// <returns>The number of spaces (using the specified font) needed to fill "width" pixels, rounded to the nearest integer</returns>
		static public int SpacesNeeded(int width, Font f)
		{
			SizeF size = GetImageSize(" ", f);
			return (int)Math.Round((float)width / size.Width, 0);
		}

		static public SizeF GetImageSize(string text, Font f)
		{
			Bitmap bmpImage = new Bitmap(1, 1);
			Graphics g = Graphics.FromImage(bmpImage);
			//Note - this call returns the same 'width' whether or not trailing spaces are included.
			return g.MeasureString(text, f);
		}


		//static public SizeF MeasureString(string text, Font font)
		//{
		//   float emSize = Convert.ToSingle(fontInfo.Size.Unit.Value);
		//   emSize = (emSize == 0 ? 12 : emSize);
		//   Bitmap bmp = new Bitmap(1000, 100);
		//   Graphics g = Graphics.FromImage(bmp);
		//   SizeF size = g.MeasureString(text, font);
		//   g.Dispose();
		//   return size;
		//}



	}
}
