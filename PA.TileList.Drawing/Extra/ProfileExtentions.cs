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
            return p.GetImage(new RectangleD<Bitmap>(new Bitmap(width, height) , PointF.Empty, p.GetSize()));
        }


        /// <summary>
        /// Get profile centered in specified rectangleF
        /// </summary>
        /// <param name="p"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="inner"></param>
        /// <returns></returns>
        public static RectangleD<Bitmap> GetImage(this CircularProfile p, int width, int height, RectangleF inner)
        {
            return p.GetImage(new RectangleD<Bitmap>(new Bitmap(width, height), inner, inner));
        }

        public static RectangleD<U> GetImage<U>(this CircularProfile p, RectangleD<U> image, bool extra = true)
            where U:Image
        {
            float maxsize = (float)p.GetMaxRadius() * 2f;
            float minsize = (float)p.GetMinRadius() * 2f;
            float midsize = (float)p.Radius * 2f;
            float offsetX = image.Inner.Left + image.Inner.Width / 2f;
            float offsetY = image.Inner.Top + image.Inner.Height / 2f;

            float resolution = Math.Min((float)image.Item.Width / image.Outer.Width, (float)image.Item.Height / image.Outer.Height);

            using (Graphics g = Graphics.FromImage(image.Item))
            {
                g.ScaleTransform((float)image.Item.Width / image.Outer.Width, (float)image.Item.Height / image.Outer.Height);
                g.TranslateTransform(-image.Outer.Left, -image.Outer.Top);

                if (extra)
                {
                    g.DrawRectangle(Pens.Black, (int)image.Inner.Location.X, (int)image.Inner.Location.Y, (int)image.Inner.Width - 1, (int)image.Inner.Height - 1);
                    g.DrawLine(Pens.Black, image.Inner.Left, offsetY, image.Inner.Right, offsetY);
                    g.DrawLine(Pens.Black, offsetX, image.Inner.Top, offsetX, image.Inner.Bottom);
                    g.DrawLine(Pens.Black, image.Inner.Left, image.Inner.Top, image.Inner.Right, image.Inner.Bottom);
                    g.DrawLine(Pens.Black, image.Inner.Right, image.Inner.Top, image.Inner.Left, image.Inner.Bottom);
                    g.DrawEllipse(Pens.Blue, offsetX - maxsize / 2f, offsetY - maxsize / 2f, maxsize, maxsize);
                    g.DrawEllipse(Pens.Black, offsetX - midsize / 2f, offsetY - midsize / 2f, midsize, midsize);
                    g.DrawEllipse(Pens.Blue, offsetX - minsize / 2f, offsetY - minsize / 2f, minsize, minsize);
                }

                CircularProfile.ProfileStep last = p.GetFirst();

                foreach (CircularProfile.ProfileStep current in p.Profile)
                {
                    double ad = 180f * -last.Angle /  Math.PI;
                    double sw = 180f * -(current.Angle - last.Angle) /Math.PI;

                    if (last.Radius > 0)
                    {
                        g.DrawArc(Pens.Red, offsetX - (float)last.Radius, offsetY - (float)last.Radius, (float)last.Radius * 2f, (float)last.Radius * 2f, (float)ad, (float)sw);
                    }

                    if (Math.Round(last.Radius ) != Math.Round(current.Radius))
                    {
                        double x1 = offsetX + last.Radius * Math.Cos(current.Angle);
                        double y1 = offsetY - last.Radius * Math.Sin(current.Angle);
                        double x2 = offsetX + current.Radius * Math.Cos(current.Angle);
                        double y2 = offsetY - current.Radius * Math.Sin(current.Angle);
                        g.DrawLine(Pens.Orange, (float)x1, (float)y1, (float)x2, (float)y2);
                    }

                    last = current;
                }
            }

            return image;
        }
    }
}

