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
    public static class RulersExtentions
    {

        public static RectangleD<Image> GetRulers<T>(this IQuantifiedTile<T> c, int width, int height, float[] steps)
           where T : ICoordinate
        {
            RectangleF b = c.GetBounds();
            return c.GetRulers(new RectangleD<Image>(new Bitmap(width, height), b, b), steps);
        }

        public static RectangleD<Image> GetRulers<T>(this IQuantifiedTile<T> c, RectangleD<Image> image, float[] steps)
           where T : ICoordinate
        {
            using (Graphics g = Graphics.FromImage(image.Item))
            {
                float scaleX = (float)image.Item.Width / image.Outer.Width;
                float scaleY = (float)image.Item.Height / image.Outer.Height;
                float offsetX = image.Inner.Left + image.Inner.Width / 2f;
                float offsetY = image.Inner.Top + image.Inner.Height / 2f;

                g.ScaleTransform(scaleX, scaleY);
                g.TranslateTransform(-image.Outer.Left, -image.Outer.Top);

                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;


                g.DrawSteps(steps, image.Inner.Left, image.Inner.Right, offsetX, Direction.Horizontal, scaleX);
                g.DrawSteps(steps, image.Inner.Top, image.Inner.Bottom, offsetY , Direction.Vertical, scaleY);


            }

            return image;
        }

        private enum Direction
        {
            Vertical,
            Horizontal
        }

        private static void DrawSteps(this Graphics g, float[] steps, float min, float max, float offset, Direction d, float scale = 1)
        {
            switch (d)
            {
                case Direction.Vertical:
                    g.DrawLine(Pens.Black, min, 0, max, 0);
                    break;
                case Direction.Horizontal:
                    g.DrawLine(Pens.Black, 0, min, 0, max);
                    break;
            }

            for (int i = 0; i < steps.Length; i++)
            {
                float start = 0;
                float step = steps[i];
                float size = (i + 1f) / scale;

                while (start < min)
                {
                    start += step;
                }

                while (start > min)
                {
                    start -= step;
                }

                for (float position = start + step; position < max; position += step)
                {
                    switch (d)
                    {
                        case Direction.Vertical:
                            if (i == 0)
                            {
                                g.DrawString(position.ToString(), new Font(FontFamily.GenericSansSerif, 10 / scale), Brushes.Black, offset - size, position + offset);
                            }
                            g.DrawLine(Pens.Black, offset - 10 * size, position + offset, offset + 10 * size, position + offset);
                            break;
                        case Direction.Horizontal:
                            if (i == 0)
                            {
                                g.DrawString(position.ToString(), new Font(FontFamily.GenericSansSerif, 10 / scale), Brushes.Black, position + offset, offset - size);
                            }
                            g.DrawLine(Pens.Black, position + offset, offset - 10 * size, position + offset, offset + 10 * size);
                            break;
                    }
                }
            }
        }
    }
}

