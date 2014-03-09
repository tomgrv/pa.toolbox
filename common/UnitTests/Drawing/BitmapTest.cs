using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;
using PA.TileList.Drawing;
using PA.TileList.Quantified;
using System.Drawing;

namespace UnitTests.Drawing
{
    [TestClass]
    public class BitmapTest
    {



        [TestMethod]
        public void TestBitmap()
        {

            ITile<ITile<UnitTests.TileList.TileTests.Item>> t2 = UnitTests.TileList.TileTests.GetTile();

            AjouterDetourage(
                t2.AsQuantified(50, 50, 55, 55)
                .GetImage(700, 700, ImageExtentions.ScaleMode.CENTER | ImageExtentions.ScaleMode.EXACTPIXEL,
                 p1 => this.AjouterDetourage(
                     p1.Item.AsQuantified()
                     .GetImage((int)Math.Round(p1.Portion.Width), (int)Math.Round(p1.Portion.Height), ImageExtentions.ScaleMode.CENTER | ImageExtentions.ScaleMode.SCALE | ImageExtentions.ScaleMode.CENTERSTEP,
                        p2 => p2.Item.ToBitmap((int)p2.Portion.Width, (int)p2.Portion.Height)
                     )
                    )
                )
                ).Save("Tile.png");


        }

        public Image AjouterDetourage(Image i)
        {
            using (Graphics g = Graphics.FromImage(i))
            {
                g.DrawRectangle(Pens.Black, 0, 0, i.Width - 1, i.Height - 1);
            }

            return i;
        }


    }
}
