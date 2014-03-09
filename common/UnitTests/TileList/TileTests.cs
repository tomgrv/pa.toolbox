using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;
using System.Drawing;

namespace UnitTests.TileList
{
    [TestClass]
    public class TileTests
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
        public void Tile()
        {
        }


        internal static ITile<ITile<Item>> GetTile()
        {
            IArea a1 = new Area(1, 1, 5, 5);
            IArea a2 = new Area(-5, -5, 5, 5);

            Item t0 = new Item(1, 1, Color.Green);

            Tile<Item> t1 = new Tile<Item>(a1, t0);

            t1.Fill<Item>(c => c.X + c.Y == 6 ? t0.Clone() as Item : new Item(c.X, c.Y, Color.Yellow));

            Tile<ITile<Item>> t2 = new Tile<ITile<Item>>(a2, t1);
            t2.Fill<Tile<Item>>(c => c.X + c.Y == 0 ? new Tile<Item>(t1) : new Tile<Item>(c.X, c.Y, a1, new Item(2, 2, Color.Blue)));

            return t2;
        }
    }
}
