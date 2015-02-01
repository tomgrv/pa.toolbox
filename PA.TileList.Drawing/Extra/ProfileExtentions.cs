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

        public static RectangleD<U> GetImage<U>(this CircularProfile p, RectangleD<U> image, bool extra = true)
            where U : Image
        {
            float scale = Math.Min((float)image.Item.Width / image.Outer.Width, (float)image.Item.Height / image.Outer.Height);

            RectangleF outerZone = new RectangleF(image.Outer.X * scale, image.Outer.Y * scale, image.Outer.Width * scale, image.Outer.Height * scale);
            RectangleF innerZone = new RectangleF(image.Inner.X * scale, image.Inner.Y * scale, image.Inner.Width * scale, image.Inner.Height * scale);

            float maxsize = scale * (float)p.GetMaxRadius() * 2f;
            float minsize = scale * (float)p.GetMinRadius() * 2f;
            float midsize = scale * (float)p.Radius * 2f;
            float offsetX = innerZone.Left + innerZone.Width / 2f;
            float offsetY = innerZone.Top + innerZone.Height / 2f;

            using (Graphics g = Graphics.FromImage(image.Item))
            {
                //g.ScaleTransform((float)image.Item.Width / image.Outer.Width, (float)image.Item.Height / image.Outer.Height);
                g.TranslateTransform(-outerZone.Left, -outerZone.Top);

                if (extra)
                {
                    g.DrawRectangle(Pens.Black, (int)innerZone.X, (int)innerZone.Y, (int)innerZone.Width - 1, (int)innerZone.Height - 1);
                    g.DrawLine(Pens.Black, innerZone.Left, offsetY, innerZone.Right, offsetY);
                    g.DrawLine(Pens.Black, offsetX, innerZone.Top, offsetX, innerZone.Bottom);
                    g.DrawLine(Pens.Black, innerZone.Left, innerZone.Top, innerZone.Right, innerZone.Bottom);
                    g.DrawLine(Pens.Black, innerZone.Right, innerZone.Top, innerZone.Left, innerZone.Bottom);
                    g.DrawEllipse(Pens.Blue, offsetX - maxsize / 2f, offsetY - maxsize / 2f, maxsize, maxsize);
                    g.DrawEllipse(Pens.Black, offsetX - midsize / 2f, offsetY - midsize / 2f, midsize, midsize);
                    g.DrawEllipse(Pens.Blue, offsetX - minsize / 2f, offsetY - minsize / 2f, minsize, minsize);
                }

                CircularProfile.ProfileStep last = p.GetFirst();

                foreach (CircularProfile.ProfileStep current in p.Profile)
                {
                    double ad = 180f * -last.Angle / Math.PI;
                    double sw = 180f * -(current.Angle - last.Angle) / Math.PI;

                    float lastRadius = scale * (float)last.Radius;

                    if (lastRadius > 0)
                    {
                        g.DrawArc(Pens.Red, offsetX - lastRadius, offsetY - lastRadius, lastRadius * 2f, lastRadius * 2f, (float)ad, (float)sw);
                    }

                    if (Math.Round(lastRadius) != Math.Round(current.Radius))
                    {
                        double x1 = offsetX + (double)lastRadius * Math.Cos(current.Angle);
                        double y1 = offsetY - (double)lastRadius * Math.Sin(current.Angle);
                        double x2 = offsetX + (double)scale * current.Radius * Math.Cos(current.Angle);
                        double y2 = offsetY - (double)scale * current.Radius * Math.Sin(current.Angle);
                        g.DrawLine(Pens.Orange, (float)x1, (float)y1, (float)x2, (float)y2);
                    }

                    last = current;
                }
            }

            return image;
        }
    }
}