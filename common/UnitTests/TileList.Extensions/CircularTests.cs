using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList.Circular;
using PA.TileList.Quantified;
using PA.TileList.Contextual;
using PA.TileList.Extensions;
using PA.TileList.Drawing;
using System.Drawing;
using PA.TileList;
using PA.File.Extensions;
using System.Collections.Generic;
using UnitTests.TileList;
using System.IO;
using System.Security.Cryptography;

namespace UnitTests.Drawing
{
    [TestClass]
    public class CircularTests
    {
        [TestMethod]
        public void ProfileWith0()
        {
            CircularProfile search = new CircularProfile(1000);

            for (int a = 0; a < 4; a++)
            {
                double a0 = -13f * Math.PI / 12f + a * Math.PI / 2f;
                double a1 = -11f * Math.PI / 12f + a * Math.PI / 2f;
                search.AddProfileStep(a0, 0);
                search.AddProfileStep(a1, 1000);
            }

            string signature = search.GetImage(1000, 1000).Item.GetSignature();
            Assert.AreEqual("8F615A0FB128A718C2AD74857E45D72CAF108C6B5FC11EB41565437FC952E1B0", signature, "Image hash");
        }

        [TestMethod]
        public void Profile()
        {
            CircularProfile p = new CircularProfile(1500);

            RectangleD<Image> i = p.GetImage(1000, 1000, new RectangleF(-2000, -2000, 4000, 4000));

            string signature = i.Item.GetSignature();
            Assert.AreEqual("AD15FBB9C02516913B3D962FB2095872B3A01A7B41CBFE2A2F6EA48C17884386", signature, "Image hash");
        }

        [TestMethod]
        public void SelectionSmallTile()
        {
            float factor = 1f;

            IQuantifiedTile<IContextual<TileTests.Item>> tile = TileTests.GetTile(factor)
                .Flatten<TileTests.SubTile, TileTests.Item>();

            Assert.AreEqual(3025, tile.Count(), "Initial item count");

            CircularProfile p = GetTestProfile(1400);

            bool change = true;

            IQuantifiedTile<IContextual<TileTests.Item>> q = tile.Take(p, new CircularConfiguration(1f, 1f, CircularConfiguration.SelectionFlag.Inside), ref change);

            Assert.AreEqual(false, change, "Reference Changed");
            Assert.AreEqual(1800, q.Count(), "Selected item count");

            RectangleD<Image> pi = p.GetImage(1000, 1000, tile.GetBounds());

            RectangleD<Image> i = q.GetImage(pi, z => z.Item.Context.ToBitmap(50, 50, z.Item.X + "\n" + z.Item.Y));

            string signature = i.Item.GetSignature();
            Assert.AreEqual("A0D730CAEB492786F539B437A98201D0427F7D27E4BDBCC02E59F47AA7F1F2A1", signature, "Image hash");
        }

        [TestMethod]
        public void SelectionMediumTile()
        {
            float factor = 5f;

            IQuantifiedTile<IContextual<TileTests.Item>> tile = TileTests.GetTile(factor)
                .Flatten<TileTests.SubTile, TileTests.Item>();

            Assert.AreEqual(65025, tile.Count(), "Initial item count");

            CircularProfile p = GetTestProfile(1400);

            IEnumerable<IContextual<TileTests.Item>> l = tile.Take(p, new CircularConfiguration(1f, 1f, CircularConfiguration.SelectionFlag.Inside));

            Assert.AreEqual(47860, l.Count(), "Selected item count");

            IQuantifiedTile<IContextual<TileTests.Item>> q = l.AsTile(tile.Area).AsQuantified(50f / factor, 50f / factor, 55f / factor, 55f / factor);

            RectangleD<Image> pi = p.GetImage(5000, 5000, tile.GetBounds());

            RectangleD<Image> i = q.GetImage(pi, z => z.Item.Context.ToBitmap(50, 50, z.Item.X + "\n" + z.Item.Y));

            string file = "SelectionMediumTile_" + DateTime.Now.Ticks + ".png";

            string signature = i.Item.GetSignature();
            Assert.AreEqual("0FC687D0C5567AABD690D1AECA13E9EE93BC2933FF43A66F9B6D2E3BE85084A5", signature, "Image hash");
        }

        private CircularProfile GetTestProfile(float radius, double stepping = 1f, double resolution = 1f)
        {
            CircularProfile p = new CircularProfile(radius);

            p.AddProfileFlat(-Math.PI / 2, 100, 100, stepping);
            p.AddProfileFlat(7 * Math.PI / 4, 200, 100, stepping);
            p.AddProfileFlat(0, 300, 100, stepping, resolution);
            p.AddProfileFlat(Math.PI / 3f, 400, 200, stepping, resolution);
            p.AddProfileFlat(2f * Math.PI / 3f, 500, 400, stepping, resolution);

            return p;
        }
    }

}
