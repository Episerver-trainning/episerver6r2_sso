#region Copyright
// Copyright © EPiServer AB.  All rights reserved.
// 
// This code is released by EPiServer AB under the Source Code File - Specific License Conditions, published August 20, 2007. 
// See http://www.episerver.com/Specific_License_Conditions for details.
#endregion
using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace EPiServer.Templates.AlloyTech.Units.Placeable
{
    /// <summary>
    /// Returns an image with a distorted text.
    /// </summary>
    public class CaptchaImageHandler : IHttpHandler
    {
        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            
            Random random = new Random();

            // Create a Bitmap and setup brushes and text formatting to use.
            Image bitmap = new Bitmap(200, 50);
            Graphics graphic = Graphics.FromImage(bitmap);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            HatchBrush backgroundBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Gray, Color.White);
            HatchBrush drawingBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Gray, Color.Black);
            Pen thinPen = new Pen(drawingBrush);
            Font font = new Font("Arial", 30, FontStyle.Bold);
            
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.Word;
            
            // Fill background of image
            graphic.SmoothingMode = SmoothingMode.AntiAlias;
            graphic.FillRectangle(backgroundBrush, rect);

            // Get the text to draw.
            string encryptedTextToDraw = context.Request.QueryString["text"];
            string textToDraw = String.Empty;
            if (!String.IsNullOrEmpty(encryptedTextToDraw))
            {
                textToDraw = CaptchaSecurity.Decrypt(encryptedTextToDraw);
            }
            
            // Create a path using the text and warp it randomly.
            GraphicsPath path = new GraphicsPath();
            path.AddString(textToDraw, font.FontFamily, (int)font.Style, font.Size, rect, format);
            
            float warpfactor = 5; // The texts gets more distorted with higher warpfactor 
            PointF[] points =
			{
				new PointF(random.Next(rect.Width) / warpfactor, random.Next(rect.Height) / warpfactor),
				new PointF(rect.Width - random.Next(rect.Width) / warpfactor, random.Next(rect.Height) / warpfactor),
				new PointF(random.Next(rect.Width) / warpfactor, rect.Height - random.Next(rect.Height) / warpfactor),
				new PointF(rect.Width - random.Next(rect.Width) / warpfactor, rect.Height - random.Next(rect.Height) / warpfactor)
			};

            path.Warp(points, rect);
            graphic.FillPath(drawingBrush, path);

            // Fill the image with random dots.
            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 30); i++)
            {
                graphic.FillEllipse(drawingBrush, random.Next(rect.Width), random.Next(rect.Height), random.Next(m / 50), random.Next(m / 50));
            }
            // Fill the image with random lines.
            for (int i = 0; i < 8; i++)
            {
                graphic.DrawBezier(thinPen,
                    new Point(random.Next(rect.Width), random.Next(rect.Height)),
                    new Point(random.Next(rect.Width), random.Next(rect.Height)),
                    new Point(random.Next(rect.Width), random.Next(rect.Height)),
                    new Point(random.Next(rect.Width), random.Next(rect.Height))
                );
            }

            context.Response.Clear();
            context.Response.ContentType = "image/jpg";

            bitmap.Save(context.Response.OutputStream, ImageFormat.Jpeg);

            // Make sure we release all drawing resources
            bitmap.Dispose();
            graphic.Dispose();
            thinPen.Dispose();
            backgroundBrush.Dispose();
            drawingBrush.Dispose();
            font.Dispose();
            format.Dispose();
            path.Dispose();
            

            context.Response.End();
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
