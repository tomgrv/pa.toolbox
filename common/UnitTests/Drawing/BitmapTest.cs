using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;
using PA.TileList.Drawing;
using PA.TileList.Quantified;
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

            AjouterDetourage(
                t2
                .AsQuantified((int)(50 / factor), (int)(50 / factor), (int)(55 / factor),(int)( 55 / factor))
                .GetImage(1000, 1000,
                 p1 => AjouterDetourage(
                     p1.Item
                     .AsQuantified()
                     .GetImage(100, 100,
                        p2 => p2.Item.ToBitmap(20, 20, p2.Item.X + "|" + p2.Item.Y)
                     ).Item
                    )
                ).Item
                ).Save("Tile.png");


        }

        public static Image AjouterDetourage(Image i)
        {
            using (Graphics g = Graphics.FromImage(i))
            {
                g.DrawRectangle(Pens.Black, 0, 0, i.Width - 1, i.Height - 1);
                g.DrawLine(Pens.Black, 0, 0, i.Width - 1, i.Height - 1);
                g.DrawLine(Pens.Black, i.Width - 1, 0, 0, i.Height - 1);
            }

            return i;
        }
    }
}
