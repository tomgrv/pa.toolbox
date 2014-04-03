using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using PA.TileList.Quantified;
using PA.TileList.Contextual;
using PA.TileList.Circular;

namespace PA.TileList.Drawing
{
    public static class ProfileExtentions
    {
        public static SizeF GetSize(this CircularProfile p)
        {
            float max = 2f * (float)p.GetMaxRadius();
            return new SizeF(max, max);
        }

        public static RectangleD<Image> GetImage(this CircularProfile p, int width, int height)
        {
            return p.GetImage(new RectangleD<Image>(new Bitmap(width, height), PointF.Empty, p.GetSize()));
        }


        /// <summary>
        /// Get profile centered in specified rectangleF
        /// </summary>
        /// <param name="p"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="inner"></param>
        /// <returns></returns>
        public static RectangleD<Image> GetImage(this CircularProfile p, int width, int height, RectangleF inner)
        {
            return p.GetImage(new RectangleD<Image>(new Bitmap(width, height), inner, inner));
        }

        public static RectangleD<Image> GetImage(this CircularProfile p, RectangleD<Image> image, bool extra = true)
        {
            float maxsize = (float)p.GetMaxRadius() * 2f;
            float minsize = (float)p.GetMinRadius() * 2f;
            float midsize = (float)p.Radius * 2f;
            float offset = image.Inner.Left + image.Inner.Width / 2f;

            using (Graphics g = Graphics.FromImage(image.Item))
            {
                g.ScaleTransform((float)image.Item.Width / image.Outer.Width, (float)image.Item.Height / image.Outer.Height);
                g.TranslateTransform(-image.Outer.Left, -image.Outer.Top);

                if (extra)
                {
                    g.DrawRectangle(Pens.Black, (int)image.Inner.Location.X, (int)image.Inner.Location.Y, (int)image.Inner.Width - 1, (int)image.Inner.Height - 1);
                    g.DrawLine(Pens.Black, image.Inner.Left, offset, image.Inner.Right, offset);
                    g.DrawLine(Pens.Black, offset, image.Inner.Top, offset, image.Inner.Bottom);
                    g.DrawLine(Pens.Black, image.Inner.Left, image.Inner.Top, image.Inner.Right, image.Inner.Bottom);
                    g.DrawLine(Pens.Black, image.Inner.Right, image.Inner.Top, image.Inner.Left, image.Inner.Bottom);
                    g.DrawEllipse(Pens.Blue, offset - maxsize / 2f, offset - maxsize / 2f, maxsize, maxsize);
                    g.DrawEllipse(Pens.Black, offset - midsize / 2f, offset - midsize / 2f, midsize, midsize);
                    g.DrawEllipse(Pens.Blue, offset - minsize / 2f, offset - minsize / 2f, minsize, minsize);
                }

                CircularProfile.ProfileStep last = p.GetFirst();

                foreach (CircularProfile.ProfileStep current in p.Profile)
                {
                    int ad = (int)Math.Round(180 * -last.Angle / Math.PI);
                    int sw = (int)Math.Round(180 * -(current.Angle - last.Angle) / Math.PI);

                    if (last.Radius > 0)
                    {
                        g.DrawArc(new Pen(Color.Red), offset - (float)last.Radius, offset - (float)last.Radius, (float)last.Radius * 2f, (float)last.Radius * 2f, ad, sw);
                    }

                    if (last.Radius != current.Radius)
                    {
                        int x1 = (int)Math.Round(offset + last.Radius * Math.Cos(current.Angle));
                        int y1 = (int)Math.Round(offset - last.Radius * Math.Sin(current.Angle));
                        int x2 = (int)Math.Round(offset + current.Radius * Math.Cos(current.Angle));
                        int y2 = (int)Math.Round(offset - current.Radius * Math.Sin(current.Angle));
                        g.DrawLine(new Pen(Color.Red), x1, y1, x2, y2);
                    }

                    last = current;
                }
            }

            return image;
        }
    }
}

