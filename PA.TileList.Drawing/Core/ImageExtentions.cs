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

        #region Dimension

        public static SizeF GetSize<T>(this IQuantifiedTile<T> c)
         where T : ICoordinate
        {
            return new SizeF((float)c.ElementStepX * c.Area.SizeX, (float)c.ElementStepY * c.Area.SizeY);
        }

        public static PointF GetOrigin<T>(this IQuantifiedTile<T> c)
        where T : ICoordinate
        {
            return new PointF((float)c.ElementStepX * c.Area.Min.X, (float)c.ElementStepY * c.Area.Min.Y);
        }

        public static RectangleF GetBounds<T>(this IQuantifiedTile<T> c)
            where T : ICoordinate
        {
            return new RectangleF(c.GetOrigin(), c.GetSize());
        }

        #endregion

        #region Image

        private static RectangleD<Image> GetBaseImage<T>(this IQuantifiedTile<T> c, int width, int height)
            where T : ICoordinate
        {
            return new RectangleD<Image>(new Bitmap(width, height), c.GetOrigin(), c.GetSize());
        }

        public static RectangleD<Image> GetImage<T>(this IQuantifiedTile<T> c, int width, int height, Func<RectangleD<T>, Image> getImagePart)
            where T : ICoordinate
        {
            return c.GetImage(c.GetBaseImage(width, height), ScaleMode.CENTER | ScaleMode.SCALE | ScaleMode.CENTERSTEP | ScaleMode.EXACTPIXEL, getImagePart);
        }

        public static RectangleD<Image> GetImage<T>(this IQuantifiedTile<T> c, int width, int height, ScaleMode mode, Func<RectangleD<T>, Image> getImagePart)
            where T : ICoordinate
        {
            return c.GetImage(c.GetBaseImage(width, height), mode, getImagePart);
        }

        public static RectangleD<Image> GetImage<T>(this IQuantifiedTile<T> c, RectangleD<Image> image, Func<RectangleD<T>, Image> getImagePart)
          where T : ICoordinate
        {
            return c.GetImage<T>(image, ScaleMode.CENTER | ScaleMode.SCALE | ScaleMode.CENTERSTEP | ScaleMode.EXACTPIXEL, getImagePart);
        }

        public static RectangleD<Image> GetImage<T>(this IQuantifiedTile<T> c, RectangleD<Image> image, ScaleMode mode, Func<RectangleD<T>, Image> getImagePart)
            where T : ICoordinate
        {
            using (Graphics g = Graphics.FromImage(image.Item))
            {
                g.ScaleTransform((float)image.Item.Width / image.Outer.Width, (float)image.Item.Height / image.Outer.Height);
                g.TranslateTransform(-image.Outer.Left, -image.Outer.Top);

                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                foreach (RectangleD<T> e in c.GetPortions(image.Inner, mode))
                {
                    if (e.Outer.Height >= 1 && e.Outer.Width >= 1)
                    {
                        Image partial = getImagePart(e);

                        if (partial is Image)
                        {
                            g.DrawImage(partial, e.Inner);
                        }
                    }
                }
            }

            return image;
        }


        #endregion

        #region Portion

        public static RectangleD GetPortionBase<T>(this IQuantifiedTile<T> c, int width, int height, ScaleMode mode = ScaleMode.CENTER | ScaleMode.SCALE)
          where T : ICoordinate
        {
            return c.GetPortionBase<T>(new RectangleF(0, 0, width, height), mode);
        }

        public static RectangleD GetPortionBase<T>(this IQuantifiedTile<T> c, RectangleF bounds, ScaleMode mode = ScaleMode.CENTER | ScaleMode.SCALE)
           where T : ICoordinate
        {
            double scale = (mode & ScaleMode.SCALE) == ScaleMode.SCALE ? c.GetScaleFactor(bounds.Width, bounds.Height) : 1;

            float sizeX = (float)(c.ElementSizeX * scale);
            float sizeY = (float)(c.ElementSizeY * scale);

            float stepX = (float)(c.ElementStepX * scale);
            float stepY = (float)(c.ElementStepY * scale);

            float outX = (float)((mode & ScaleMode.CENTER) == ScaleMode.CENTER ? (bounds.Width - c.Area.SizeX * stepX) / 2f : 0);
            float outY = (float)((mode & ScaleMode.CENTER) == ScaleMode.CENTER ? (bounds.Height - c.Area.SizeY * stepY) / 2f : 0);

            float intX = outX + (float)((mode & ScaleMode.CENTERSTEP) == ScaleMode.CENTERSTEP ? (stepX - sizeX) / 2f : 0);
            float intY = outY + (float)((mode & ScaleMode.CENTERSTEP) == ScaleMode.CENTERSTEP ? (stepY - sizeY) / 2f : 0);

            return new RectangleD(new RectangleF(outX, outY, stepX, stepY), new RectangleF(intX, intY, sizeX, sizeY));
        }

        public static IEnumerable<RectangleD<T>> GetPortions<T>(this IQuantifiedTile<T> c, RectangleF bounds, ScaleMode mode = ScaleMode.CENTER | ScaleMode.SCALE)
            where T : ICoordinate
        {
            RectangleD p = c.GetPortionBase(bounds, mode);

            foreach (T e in c)
            {
                RectangleF inner = new RectangleF(bounds.Left + p.Inner.Left + (e.X - c.Area.Min.X) * p.Outer.Width, bounds.Top + p.Inner.Top + (e.Y - c.Area.Min.Y) * p.Outer.Height, p.Inner.Width, p.Inner.Width);
                RectangleF outer = new RectangleF(bounds.Left + p.Outer.Left + (e.X - c.Area.Min.X) * p.Outer.Width, bounds.Top + p.Outer.Top + (e.Y - c.Area.Min.Y) * p.Outer.Height, p.Outer.Width, p.Outer.Width);

                if ((mode & ScaleMode.EXACTPIXEL) == ScaleMode.EXACTPIXEL)
                {
                    yield return new RectangleD<T>(e, new RectangleD(outer, inner));
                }
                else
                {
                    yield return new RectangleD<T>(e, new RectangleD(outer, inner));
                }
            }
        }



        #endregion

    }


}
