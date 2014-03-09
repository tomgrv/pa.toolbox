using System;
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
        internal class Item : Coordinate
        {
            public Color Color { get; set; }

            public Item(int x, int y, Color c)
                : base(x, y)
            {
                this.Color = c;
            }

            public Bitmap ToBitmap(int w, int h)
            {
                Bitmap b = new Bitmap(w, h);

                using (Graphics g = Graphics.FromImage(b))
                {
                    g.DrawRectangle(Pens.Pink, 0, 0, w - 1, h - 1);
                    g.FillRectangle(new SolidBrush(this.Color), 1, 1, w - 2, h - 2);
                }

                return b;
            }
        }


        [TestMethod]
        public void TestBitmap()
        {
            IArea a1 = new Area(1, 1, 5, 5);
            IArea a2 = new Area(-5, -5, 5, 5);


            Item t0 = new Item(1, 1, Color.Green);

            Tile<Item> t1 = new Tile<Item>(a1, t0);

            t1.Fill<Item>(c => c.X + c.Y == 6 ? t0.Clone() as Item : new Item(c.X, c.Y, Color.Yellow));

            Tile<Tile<Item>> t2 = new Tile<Tile<Item>>(a2, t1);
            t2.Fill<Tile<Item>>(c => c.X + c.Y == 0 ? new Tile<Item>(t1) : new Tile<Item>(c.X, c.Y, a1, new Item(2, 2, Color.Blue)));


            AjouterDetourage(
                t2.AsQuantified(50, 50, 55, 55)
                .GetImage(700, 700, BitmapExtentions.ScaleMode.CENTER | BitmapExtentions.ScaleMode.EXACTPIXEL,
                 (ti, r1) => this.AjouterDetourage(
                     ti.AsQuantified()
                     .GetImage((int)Math.Round(r1.Width), (int)Math.Round(r1.Height), BitmapExtentions.ScaleMode.CENTER | BitmapExtentions.ScaleMode.SCALE | BitmapExtentions.ScaleMode.CENTERSTEP,
                        (j, r2) => j.ToBitmap((int)r2.Width, (int)r2.Height)
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
