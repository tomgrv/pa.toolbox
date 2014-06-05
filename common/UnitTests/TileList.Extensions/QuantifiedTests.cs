using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;
using PA.TileList.Quantified;
using PA.TileList.Contextual;
using PA.TileList.Drawing;
using PA.File.Extensions;
using System.Drawing;
using PA.TileList.Circular;
using System.IO;

namespace UnitTests.TileList.Extensions
{
    [TestClass]
    public class QuantifiedTests
    {
        [TestMethod]
        public void FirstOrDefault()
        {
            TileTests.MainTile tile = TileTests.GetTile(1);

            IQuantifiedTile<IContextual<TileTests.Item>> t1 = tile
               .Flatten<TileTests.SubTile, TileTests.Item>();

            IContextual<TileTests.Item> item = t1.FirstOrDefault(10, 10);
            item.Context.Color = Color.Red;

            string signature = t1.GetImage(2000, 2000, z => z.Item.Context.ToBitmap(1000, 500, z.Item.X + "\n" + z.Item.Y)).Item.GetSignature();
            Assert.AreEqual("B9AB99CBC9FD35ECABBEE8C833B77BA3363BFE18084F3DA9EDAAF25708A8E406", signature, "Image hash");
        }

        [TestMethod]
        public void Coordinates()
        {
            TileTests.MainTile tile = TileTests.GetTile(1);

            IQuantifiedTile<IContextual<TileTests.Item>> t1 = tile
               .Flatten<TileTests.SubTile, TileTests.Item>();

            IContextual<TileTests.Item> item = t1.FirstOrDefault(1000, 500);
            item.Context.Color = Color.Red;

            Coordinate coord = t1.GetCoordinatesAt(1000, 500);

            Assert.AreEqual(item.X, coord.X);
            Assert.AreEqual(item.Y, coord.Y);
        }

        [TestMethod]
        public void Rulers()
        {
            TileTests.MainTile tile = TileTests.GetTile(1);

            IQuantifiedTile<IContextual<TileTests.Item>> t1 = tile
               .Flatten<TileTests.SubTile, TileTests.Item>();

            t1.Reference.Context.Color = Color.Lavender;

            IContextual<TileTests.Item> item = t1.FirstOrDefault(500, 1000);
            item.Context.Color = Color.Red;

            RectangleD<Image> i1 = t1.GetImage(2000, 2000, z => z.Item.Context.ToBitmap(100, 100, z.Item.X + "\n" + z.Item.Y));

            CircularProfile p = new CircularProfile(1000);

            RectangleD<Image> i2 = p.GetImage(i1);

            string file = "Rulers_" + DateTime.Now.Ticks + ".png";

            string signature = t1.GetRulers(i2, new float[] { 100f, 500f }).Item.GetSignature();
            Assert.AreEqual("9E0B91534E9C29934BF8D353666F986606BE968612756DAFE261B2A3C52D97B0", signature, "Image hash");
        }

      
    }
}


