using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;
using PA.TileList.Quantified;
using PA.TileList.Contextual;
using PA.TileList.Drawing;
using System.Drawing;

namespace UnitTests.TileList.Extensions
{
    [TestClass]
    public class QuantifiedTests
    {
        [TestMethod]
        public void TestFirstOrDefault()
        {
            TileTests.MainTile tile = TileTests.GetTile(1);

            IQuantifiedTile<IContextual<TileTests.Item>> t1 = tile
               .Flatten<TileTests.SubTile, TileTests.Item>();

            IContextual<TileTests.Item> item = t1.FirstOrDefault(10, 10);
            item.Context.Color = Color.Red;

            t1.GetImage(2000, 2000, z => z.Item.Context.ToBitmap(100, 100, z.Item.Context.X + "\n" + z.Item.Context.Y)).Item.Save("FirstOrDefault.png");
        }
    }
}


