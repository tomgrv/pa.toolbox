using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using PA.TileList.Quantified;
using PA.TileList.Contextual;
using PA.TileList.Geometrics.Circular;

namespace PA.TileList.Drawing
{
    public static class ProfileExtentions
    {
        public static SizeF GetSize(this CircularProfile p)
        {
            float max = 2f * (float)p.GetMaxRadius();
            return new SizeF(max, max);
        }

        public static RectangleD<Bitmap> GetImage(this CircularProfile p, int width, int height)
        {
            return p.GetImage(new RectangleD<Bitmap>(new Bitmap(width, height), PointF.Empty, p.GetSize()));
        }

        /// <summary>
        /// Get profile centered in specified rectangleF
        /// </summary>
        /// <param name="p"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="inner"></param>
        /// <returns></returns>
        public static RectangleD<Bitmap> GetImage(this CircularProfile p, int width, int height, RectangleF outer)
        {
            return p.GetImage(new RectangleD<Bitmap>(new Bitmap(width, height), outer, outer));
        }

        public static RectangleD<Bitmap> GetImage(this CircularProfile p, int width, int height, RectangleF outer, RectangleF inner)
        {
            return p.GetImage(new RectangleD<Bitmap>(new Bitmap(width, height), outer, inner));
        }

        public static RectangleD<U> GetImage<U>(this CircularProfile p, RectangleD<U> image, ImageExtentions.ScaleMode mode = ImageExtentions.ScaleMode.ALL, bool extra = true)
            where U : Image
        {
            using (GraphicsD g = image.GetGraphicsD(mode))
            {

                float maxsize = (float)p.GetMaxRadius() * 2f;
                float minsize = (float)p.GetMinRadius() * 2f;
                float midsize = (float)p.Radius * 2f;


                if (extra)
                {
                    g.Graphics.DrawRectangle(Pens.Black, (int)g.Portion.Inner.X, (int)g.Portion.Inner.Y, (int)g.Portion.Inner.Width - 1, (int)g.Portion.Inner.Height - 1);
                    g.Graphics.DrawLine(Pens.Black, g.Portion.Inner.Left, g.OffsetY, g.Portion.Inner.Right, g.OffsetY);
                    g.Graphics.DrawLine(Pens.Black, g.OffsetX, g.Portion.Inner.Top, g.OffsetX, g.Portion.Inner.Bottom);
                    g.Graphics.DrawLine(Pens.Black, g.Portion.Inner.Left, g.Portion.Inner.Top, g.Portion.Inner.Right, g.Portion.Inner.Bottom);
                    g.Graphics.DrawLine(Pens.Black, g.Portion.Inner.Right, g.Portion.Inner.Top, g.Portion.Inner.Left, g.Portion.Inner.Bottom);
                    g.Graphics.DrawEllipse(Pens.Blue, g.OffsetX - maxsize * g.ScaleX / 2f, g.OffsetY - maxsize * g.ScaleY / 2f, maxsize * g.ScaleX, maxsize * g.ScaleY);
                    g.Graphics.DrawEllipse(Pens.Black, g.OffsetX - midsize * g.ScaleX / 2f, g.OffsetY - midsize * g.ScaleY / 2f, midsize * g.ScaleX, midsize * g.ScaleY);
                    g.Graphics.DrawEllipse(Pens.Blue, g.OffsetX - minsize * g.ScaleX / 2f, g.OffsetY - minsize * g.ScaleY / 2f, minsize * g.ScaleX, minsize * g.ScaleY);
                }

                CircularProfile.ProfileStep last = p.GetFirst();

                foreach (CircularProfile.ProfileStep current in p.Profile)
                {
                    double ad = 180f * -last.Angle / Math.PI;
                    double sw = 180f * -(current.Angle - last.Angle) / Math.PI;

                    float lastRadius = (float)last.Radius;

                    if (lastRadius > 0)
                    {
                        g.Graphics.DrawArc(Pens.Red, g.OffsetX - lastRadius * g.ScaleX, g.OffsetY - lastRadius * g.ScaleY, lastRadius * g.ScaleX * 2f, lastRadius * g.ScaleY * 2f, (float)ad, (float)sw);
                    }

                    if (Math.Round(lastRadius) != Math.Round(current.Radius))
                    {
                        double x1 = g.OffsetX + (double)lastRadius * g.ScaleX * Math.Cos(current.Angle);
                        double y1 = g.OffsetY - (double)lastRadius * g.ScaleY * Math.Sin(current.Angle);
                        double x2 = g.OffsetX + (double)g.ScaleX * current.Radius * Math.Cos(current.Angle);
                        double y2 = g.OffsetY - (double)g.ScaleY * current.Radius * Math.Sin(current.Angle);
                        g.Graphics.DrawLine(Pens.Orange, (float)x1, (float)y1, (float)x2, (float)y2);
                    }

                    last = current;
                }
            }

            return image;
        }
    }
}