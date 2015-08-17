using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList.Drawing;
using PA.TileList.Quantified;
using PA.TileList.Geometrics;
using System;
using System.Drawing;
using UnitTests.TileList;

namespace UnitTests.Drawing
{
    [TestClass]
    public class BitmapTest
    {
        [TestMethod]
        public void TestBitmap()
        {
            float factor = 1f;

            TileTests.MainTile t2 = TileTests.GetTile(factor);

            //var z = t2.Reference
            //    .AsQuantified()
            //    .GetImage(100, 100,
            //            (p2, s2) => p2.ToBitmap(20, 20, p2.X + "|" + p2.Y)
            //            );

            //z.Item.GetSignature();

            string signature =
                t2
                .GetImage(1000, 1000,
                 (p1, s1) => AjouterDetourage(
                     p1
                     .AsQuantified()
                     .GetImage(100, 100,
                        (p2, s2) => p2.ToBitmap(20, 20, p2.X + "|" + p2.Y)
                     ), p1.X, p1.Y).Item
                ).Item.GetSignature();

            Assert.AreEqual("0428457778FC3ACBAABEBB93953AB330", signature, "Image hash");
        }

        public static RectangleD<U> AjouterDetourage<U>(RectangleD<U> image, int x, int y, ImageExtentions.ScaleMode mode = ImageExtentions.ScaleMode.ALL)
            where U : Image
        {
            using (GraphicsD g = image.GetGraphicsD(mode))
            {

                g.Graphics.DrawRectangle(Pens.Black, (int)g.Portion.Inner.X, (int)g.Portion.Inner.Y, (int)g.Portion.Inner.Width - 1, (int)g.Portion.Inner.Height - 1);
                g.Graphics.DrawLine(Pens.Black, g.Portion.Inner.Left, g.OffsetY, g.Portion.Inner.Right, g.OffsetY);
                g.Graphics.DrawLine(Pens.Black, g.OffsetX, g.Portion.Inner.Top, g.OffsetX, g.Portion.Inner.Bottom);
                g.Graphics.DrawLine(Pens.Black, g.Portion.Inner.Left, g.Portion.Inner.Top, g.Portion.Inner.Right, g.Portion.Inner.Bottom);
                g.Graphics.DrawLine(Pens.Black, g.Portion.Inner.Right, g.Portion.Inner.Top, g.Portion.Inner.Left, g.Portion.Inner.Bottom);

                g.Graphics.DrawString(x.ToString() + "|" + y.ToString(), SystemFonts.DefaultFont, Brushes.Black, PointF.Empty);
            }

            return image;
        }
    }
}
