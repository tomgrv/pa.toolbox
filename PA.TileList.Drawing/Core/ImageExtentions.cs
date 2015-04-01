using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using PA.TileList.Quantified;
using PA.TileList.Contextual;
using PA.TileList.Geometrics.Circular;
using System.Security.Cryptography;
using System.IO;
using System.Drawing.Imaging;

namespace PA.TileList.Drawing
{
    public static class ImageExtentions
    {
        [Flags]
        public enum ScaleMode
        {
            NONE = 0x00,
            NOSTRETCH = 0x01,
            CENTER = 0x02,
            //EXACTPIXEL = 0x08,
            ALL = 0xFF
        }

        #region Dimension

        public static SizeF GetSize<T>(this IQuantifiedTile<T> c, ScaleMode mode, float scaleX , float scaleY )
         where T : ICoordinate
        {
            return new SizeF((float)(c.ElementStepX * c.Area.SizeX) * scaleX, (float)(c.ElementStepY * c.Area.SizeY) * scaleY);
        }

        public static PointF GetOrigin<T>(this IQuantifiedTile<T> c, ScaleMode mode, float scaleX, float scaleY )
            where T : ICoordinate
        {
            float x = c.Area.Min.X - (mode.HasFlag(ScaleMode.CENTER) ? c.Reference.X + 0.5f : c.Reference.X);
            float y = c.Area.Min.Y - (mode.HasFlag(ScaleMode.CENTER) ? c.Reference.Y + 0.5f : c.Reference.Y);

            return new PointF((float)(x * c.ElementStepX - c.RefOffsetX) * scaleX, (float)(y * c.ElementStepY - c.RefOffsetY) * scaleY);
        }

        public static RectangleF GetBounds<T>(this IQuantifiedTile<T> c)
            where T : ICoordinate
        {
            return new RectangleF(c.GetOrigin(ScaleMode.ALL, 1f, 1f), c.GetSize(ScaleMode.ALL, 1f, 1f));
        }

        public static RectangleF GetBounds<T>(this IQuantifiedTile<T> c, float scaleX, float scaleY, ScaleMode mode = ScaleMode.ALL)
            where T : ICoordinate
        {
            return new RectangleF(c.GetOrigin(mode, scaleX, scaleY), c.GetSize(mode, scaleX, scaleY));
        }

        #endregion

        #region Image

        private static RectangleD<U> GetBaseImage<T, U>(this IQuantifiedTile<T> c, int width, int height, ScaleMode mode)
            where T : ICoordinate
            where U : Image
        {
            return new RectangleD<U>(new Bitmap(width, height) as U, c.GetOrigin(mode, 1f, 1f), c.GetSize(mode, 1f, 1f));
        }

        public static RectangleD<U> GetImage<T, U>(this IQuantifiedTile<T> c, int width, int height, Func<T, SizeF, U> getImagePart)
            where T : ICoordinate
            where U : Image
        {
            return c.GetImage<T, U>(c.GetBaseImage<T, U>(width, height, ScaleMode.ALL), ScaleMode.ALL, getImagePart);
        }

        public static RectangleD<U> GetImage<T, U>(this IQuantifiedTile<T> c, int width, int height, ScaleMode mode, Func<T, SizeF, U> getImagePart)
            where T : ICoordinate
            where U : Image
        {
            return c.GetImage<T, U>(c.GetBaseImage<T, U>(width, height, mode), mode, getImagePart);
        }

        public static RectangleD<U> GetImage<T, U>(this IQuantifiedTile<T> c, RectangleD<U> image, Func<T, SizeF, U> getImagePart)
            where T : ICoordinate
            where U : Image
        {
            return c.GetImage<T, U>(image, ScaleMode.ALL, getImagePart);
        }


        public static GraphicsD GetGraphicsD<U>(this RectangleD<U> image, ScaleMode mode)
            where U : Image
        {
            float scaleX = (float)image.Item.Width / image.Outer.Width;
            float scaleY = (float)image.Item.Height / image.Outer.Height;

            if (mode.HasFlag(ScaleMode.NOSTRETCH))
            {
                float scale = Math.Min(scaleX, scaleY);
                scaleX = scale;
                scaleY = scale;
            }

            RectangleF outerZone = new RectangleF(image.Outer.X * scaleX, image.Outer.Y * scaleY, image.Outer.Width * scaleX, image.Outer.Height * scaleX);
            RectangleF innerZone = new RectangleF(image.Inner.X * scaleX, image.Inner.Y * scaleY, image.Inner.Width * scaleX, image.Inner.Height * scaleY);

            Graphics g = Graphics.FromImage(image.Item);
            g.TranslateTransform(-outerZone.Left, -outerZone.Top);
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            return new GraphicsD(g, scaleX, scaleY, innerZone, outerZone);
        }

        public static RectangleD<U> GetImage<T, U>(this IQuantifiedTile<T> c, RectangleD<U> image, ScaleMode mode, Func<T, SizeF, U> getImagePortion)
            where T : ICoordinate
            where U : Image
        {
            using (GraphicsD g = image.GetGraphicsD(mode))
            {
                foreach (RectangleD<T> portion in c.GetPortions(g.ScaleX, g.ScaleY, mode))
                {
                    if (portion.Outer.Height >= 1 && portion.Outer.Width >= 1)
                    {
                        U partial = getImagePortion(portion.Item, portion.Inner.Size);

                        if (partial is U)
                        {
                            g.Graphics.DrawImage(partial, portion.Inner);
                        }
                    }
                }
            }

            return image;
        }


        #endregion

        #region Portion

        internal static IEnumerable<RectangleD<T>> GetPortions<T>(this IQuantifiedTile<T> tile, float scaleX, float scaleY, ScaleMode mode)
            where T : ICoordinate
        {
            RectangleF zone = tile.GetBounds(scaleX, scaleY, mode);

            float sizeX = (float)tile.ElementSizeX * scaleX;
            float sizeY = (float)tile.ElementSizeY * scaleY;

            float stepX = (float)tile.ElementStepX * scaleX;
            float stepY = (float)tile.ElementStepY * scaleY;

            float offsetX = (float)tile.RefOffsetX * scaleX;
            float offsetY = (float)tile.RefOffsetY * scaleY;

            float refX = mode.HasFlag(ScaleMode.CENTER) ? tile.Reference.X + 0.5f : tile.Reference.X;
            float refY = mode.HasFlag(ScaleMode.CENTER) ? tile.Reference.Y + 0.5f : tile.Reference.Y;

            float offX = (stepX - sizeX) / 2f;
            float offY = (stepY - sizeY) / 2f;

            foreach (T e in tile)
            {
                RectangleF portionO = new RectangleF((e.X - refX) * stepX + offsetX, (e.Y - refY) * stepY + offsetY, stepX, stepY);
                RectangleF portionI = new RectangleF(portionO.X + offX, portionO.Y + offY, sizeX, sizeX);
                yield return new RectangleD<T>(e, portionO, portionI);
            }
        }

        #endregion

        public static byte[] GetRawData(this Image image)
        {
            ImageConverter converter = new ImageConverter();
            return converter.ConvertTo(image, typeof(byte[])) as byte[];
        }

        public static string GetSignature(this Image image, string tag = null)
        {
            using (MD5CryptoServiceProvider sha = new MD5CryptoServiceProvider())
            {
                byte[] hash = sha.ComputeHash(image.GetRawData());
                string key = BitConverter.ToString(hash).Replace("-", String.Empty);
#if DEBUG

                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                System.Diagnostics.StackFrame sf = st.GetFrames().FirstOrDefault(s => s.GetMethod().GetCustomAttributes(false)
                                                .Any(i => i.ToString().EndsWith("TestMethodAttribute")));

                if (sf is System.Diagnostics.StackFrame)
                {
                    string name = sf.GetMethod().Name + (tag is string ? "_" + tag : string.Empty);
                    image.Save(name + "_" + key + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
#endif
                return key;
            }
        }
    }


}
