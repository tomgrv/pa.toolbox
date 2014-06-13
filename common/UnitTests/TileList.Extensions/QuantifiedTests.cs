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

           RectangleD<Bitmap> i1 = t1.GetImage(2000, 2000, z => z.Item.Context.ToBitmap(100, 50, z.Item.X + "\n" + z.Item.Y));
            
            string signature = i1.Item.GetSignature();
            Assert.AreEqual("A56EBC8E87772EA73D38342AF45FF00B5489A22DB73E7ED5996C6AF7EEE3DE0A", signature, "Image hash");
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

            RectangleD<Bitmap> i1 = t1.GetImage(2000, 2000, z => z.Item.Context.ToBitmap(100, 100, z.Item.X + "\n" + z.Item.Y));

            CircularProfile p = new CircularProfile(1000);

            RectangleD<Bitmap> i2 = p.GetImage(i1);

            string file = "Rulers_" + DateTime.Now.Ticks + ".png";

            string signature = t1.GetRulers(i2, new float[] { 100f, 500f }).Item.GetSignature();
            Assert.AreEqual("9272D2C42A039C2122B649DAD516B390A3A2A3C51BA861B6E615F27BA0F1BDA3", signature, "Image hash");
        }

      
    }
}


