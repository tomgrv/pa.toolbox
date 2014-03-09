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
    public static class ImageExtentions
    {
        [Flags]
        public enum ScaleMode
        {
            NONE = 0,
            SCALE = 1,
            CENTER = 2,
            CENTERSTEP = 4,
            EXACTPIXEL = 8,
        }

        public static Image GetImage<T>(this IQuantifiedTile<T> c, Image image, Func<ImagePortion<T>, Image> getImagePortion)
          where T : ICoordinate
        {
            return c.GetImage<T>(image, ScaleMode.CENTER | ScaleMode.SCALE | ScaleMode.CENTERSTEP | ScaleMode.EXACTPIXEL, getImagePortion);
        }

        public static Image GetImage<T>(this IQuantifiedTile<T> c, Image image, ScaleMode mode, Func<ImagePortion<T>, Image> getImagePortion)
            where T : ICoordinate
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                foreach (ImagePortion<T> e in c.GetPortions(image.Size, mode))
                {
                    if (e.Portion.Height >= 1 && e.Portion.Width >= 1)
                    {
                        g.DrawImage(getImagePortion(e), e.Portion);
                    }
                    else
                    {
                        g.FillRectangle(new SolidBrush(Color.Gray), e.Portion);
                    }
                }
            }

            return image;
        }

        public static Image GetImage<T>(this IQuantifiedTile<T> c, int width, int height, Func<ImagePortion<T>, Image> getImagePortion)
            where T : ICoordinate
        {
            return c.GetImage(new Bitmap(width, height), ScaleMode.CENTER | ScaleMode.SCALE | ScaleMode.CENTERSTEP | ScaleMode.EXACTPIXEL, getImagePortion);
        }

        public static Image GetImage<T>(this IQuantifiedTile<T> c, int width, int height, ScaleMode mode, Func<ImagePortion<T>, Image> getImagePortion)
            where T : ICoordinate
        {
            return c.GetImage(new Bitmap(width, height), mode, getImagePortion);
        }

        public static IEnumerable<ImagePortion<T>> GetPortions<T>(this IQuantifiedTile<T> c, Size size, ScaleMode mode = ScaleMode.CENTER| ScaleMode.SCALE)
           where T : ICoordinate
        {
            return c.GetPortions(new RectangleF(PointF.Empty, size), mode);
        }

        public static IEnumerable<ImagePortion<T>> GetPortions<T>(this IQuantifiedTile<T> c, RectangleF bounds, ScaleMode mode = ScaleMode.CENTER | ScaleMode.SCALE)
            where T : ICoordinate
        {
            double scale = (mode & ScaleMode.SCALE) == ScaleMode.SCALE ? c.GetScaleFactor(bounds.Width, bounds.Height) : 1;

            float sizeX = (float)(c.ElementSizeX * scale);
            float sizeY = (float)(c.ElementSizeY * scale);

            float stepX = (float)(c.ElementStepX * scale);
            float stepY = (float)(c.ElementStepY * scale);

            float decX = (float)((mode & ScaleMode.CENTER) == ScaleMode.CENTER ? (bounds.Width - c.Area.SizeX * c.ElementStepX * scale) / 2f : 0);
            float decY = (float)((mode & ScaleMode.CENTER) == ScaleMode.CENTER ? (bounds.Height - c.Area.SizeY * c.ElementStepY * scale) / 2f : 0);


            decX += (float)((mode & ScaleMode.CENTERSTEP) == ScaleMode.CENTERSTEP ? (c.ElementStepX - c.ElementSizeX) * scale / 2f : 0);
            decY += (float)((mode & ScaleMode.CENTERSTEP) == ScaleMode.CENTERSTEP ? (c.ElementStepY - c.ElementSizeY) * scale / 2f : 0);

            foreach (T e in c)
            {
                RectangleF r = new RectangleF(bounds.Left + decX + (e.X - c.Area.Min.X) * stepX, bounds.Top + decY + (e.Y - c.Area.Min.Y) * stepY, sizeX, sizeY);

                if ((mode & ScaleMode.EXACTPIXEL) == ScaleMode.EXACTPIXEL)
                {
                    yield return new ImagePortion<T>(e, Rectangle.Truncate(r));
                }
                else
                {
                    yield return new ImagePortion<T>(e, r);
                }
            }
        }

    }


}
