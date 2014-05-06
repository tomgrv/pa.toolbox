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

            string file = "FirstOrDefault_" + DateTime.Now.Ticks + ".png";

            t1.GetImage(2000, 2000, z => z.Item.Context.ToBitmap(1000, 500, z.Item.X + "\n" + z.Item.Y)).Item.Save(file);

            Assert.AreEqual("6B93EB09F16B30AAF482EE2CEBE9F28289D25CFE6FFCBAF646FFAA7178A4FFEE", this.GetHash(file), "Image hash");
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

            t1.GetRulers(i2, new float[] { 100f, 500f }).Item.Save(file);

            Assert.AreEqual("A6313D498F4906070C8D96A20FD3EF19CDADDC73F35BB0E49D4BAF3516BBE80A", this.GetHash(file), "Image hash");
        }

        private string GetHash(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return stream.GetSignature();
            }
        }
    }
}


