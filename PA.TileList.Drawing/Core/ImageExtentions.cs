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
            SCALE = 0x01,
            CENTER = 0x02,
            CENTERSTEP = 0x04,
            EXACTPIXEL = 0x08,
            ALL = 0xFF
        }

        #region Dimension

        public static SizeF GetSize<T>(this IQuantifiedTile<T> c, ScaleMode mode, float scale = 1f)
         where T : ICoordinate
        {
            return new SizeF((float)(c.ElementStepX * c.Area.SizeX) * scale, (float)(c.ElementStepY * c.Area.SizeY) * scale);
        }

        public static PointF GetOrigin<T>(this IQuantifiedTile<T> c, ScaleMode mode, float scale = 1f)
            where T : ICoordinate
        {
            float x = c.Area.Min.X - (mode.HasFlag(ScaleMode.CENTERSTEP) ? c.Reference.X + 0.5f : c.Reference.X);
            float y = c.Area.Min.Y - (mode.HasFlag(ScaleMode.CENTERSTEP) ? c.Reference.Y + 0.5f : c.Reference.Y);

            return new PointF((float)(x * c.ElementStepX - c.RefOffsetX) * scale, (float)(y * c.ElementStepY - c.RefOffsetY) * scale);
        }

        public static RectangleF GetBounds<T>(this IQuantifiedTile<T> c, ScaleMode mode = ScaleMode.ALL, float scale = 1f)
            where T : ICoordinate
        {
            return new RectangleF(c.GetOrigin(mode, scale), c.GetSize(mode, scale));
        }

        #endregion

        #region Image

        private static RectangleD<U> GetBaseImage<T, U>(this IQuantifiedTile<T> c, int width, int height, ScaleMode mode)
            where T : ICoordinate
            where U : Image
        {
            return new RectangleD<U>(new Bitmap(width, height) as U, c.GetOrigin(mode), c.GetSize(mode));
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

        public static RectangleD<U> GetImage<T, U>(this IQuantifiedTile<T> c, RectangleD<U> image, ScaleMode mode, Func<T, SizeF, U> getImagePart)
            where T : ICoordinate
            where U : Image
        {
            float scale = Math.Min((float)image.Item.Width / image.Outer.Width, (float)image.Item.Height / image.Outer.Height);

            using (Graphics g = Graphics.FromImage(image.Item))
            {
                g.TranslateTransform(image.Outer.Width * scale / 2f, image.Outer.Height * scale / 2f);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                foreach (Tuple<T, SizeF, RectangleF> e in c.GetPortions(scale, mode))
                {
                    if (e.Item2.Height >= 1 && e.Item2.Width >= 1)
                    {
                        U partial = getImagePart(e.Item1, e.Item2);

                        if (partial is U)
                        {
                            g.DrawImage(partial, e.Item3);
                        }
                    }
                }
            }

            return image;
        }


        #endregion

        #region Portion

        internal static IEnumerable<Tuple<T, SizeF, RectangleF>> GetPortions<T>(this IQuantifiedTile<T> c, float scale, ScaleMode mode = ScaleMode.CENTER | ScaleMode.SCALE)
            where T : ICoordinate
        {
            RectangleF zone = c.GetBounds(mode, scale);

            float sizeX = (float)(c.ElementSizeX * scale);
            float sizeY = (float)(c.ElementSizeY * scale);

            float stepX = (float)(c.ElementStepX * scale);
            float stepY = (float)(c.ElementStepY * scale);

            float offsetX = (float)(c.RefOffsetX * scale);
            float offsetY = (float)(c.RefOffsetY * scale);

            float refX = mode.HasFlag(ScaleMode.CENTERSTEP) ? c.Reference.X + 0.5f : c.Reference.X;
            float refY = mode.HasFlag(ScaleMode.CENTERSTEP) ? c.Reference.Y + 0.5f : c.Reference.Y;

            SizeF drawZone = new SizeF(sizeX, sizeY);

            foreach (T e in c)
            {
                //portion => RectangleF inner = new RectangleF(bounds.Left + p.Inner.Left + (e.X - c.Area.Min.X) * p.Outer.Width, bounds.Top + p.Inner.Top + (e.Y - c.Area.Min.Y) * p.Outer.Height, p.Inner.Width, p.Inner.Height);
                //RectangleF outer = new RectangleF(bounds.Left + p.Outer.Left + (e.X - c.Area.Min.X) * p.Outer.Width, bounds.Top + p.Outer.Top + (e.Y - c.Area.Min.Y) * p.Outer.Height, p.Outer.Width, p.Outer.Height);

                RectangleF portion = new RectangleF((e.X - refX) * stepX + offsetX, (e.Y - refY) * stepY + offsetY, stepX, stepY);
                yield return new Tuple<T, SizeF, RectangleF>(e, drawZone, portion);
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
