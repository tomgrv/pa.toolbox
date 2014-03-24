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
        public void Profile()
        {
            CircularProfile p = new CircularProfile(1500);
            
            RectangleD<Image> i = p.GetImage(1000, 1000, new RectangleF(-2000, -2000, 4000, 4000));
            
            i.Item.Save("Profile.png");

            Assert.AreEqual("81E88BED0A483EE7E88FDC6BAC0E0E800D3E0420D2F2916B8413A562E6529EB9", this.GetHash("Profile.png"), "Image hash");
        }

        [TestMethod]
        public void SelectionSmallTile()
        {
            float factor = 1f;

            IQuantifiedTile<IContextual<TileTests.Item>> tile = TileTests.GetTile(factor)
                .Flatten<TileTests.SubTile, TileTests.Item>()
                .AsQuantified(50f / factor, 50f / factor, 55f / factor, 55f / factor, -55f * 2f / factor, -55f * 2f / factor);

            Assert.AreEqual(3025, tile.Count(), "Initial item count");

            CircularProfile p = GetTestProfile(1400);

            IEnumerable<IContextual<TileTests.Item>> l = tile.Take(p, new CircularConfiguration(1f, 1f, CircularConfiguration.SelectionFlag.Inside));

            Assert.AreEqual(1800, l.Count(), "Selected item count");

            IQuantifiedTile<IContextual<TileTests.Item>> q = l.AsTile(tile.Area).AsQuantified(50f / factor, 50f / factor, 55f / factor, 55f / factor);

            RectangleD<Image> pi = p.GetImage(1000, 1000, tile.GetBounds());

            RectangleD<Image> i = q.GetImage(pi, z => z.Item.Context.ToBitmap(50, 50, z.Item.X + "|" + z.Item.Y));

            i.Item.Save("SelectionSmallTile.png");

            Assert.AreEqual("63FC3E364774AB7492106359F15957B0E925355953BCF32B558124D466A6CBE3", this.GetHash("SelectionSmallTile.png"), "Image hash");
        }

        [TestMethod]
        public void SelectionMediumTile()
        {
            float factor = 5f;

            IQuantifiedTile<IContextual<TileTests.Item>> tile = TileTests.GetTile(factor)
                .Flatten<TileTests.SubTile, TileTests.Item>()
                .AsQuantified(50f / factor, 50f / factor, 55f / factor, 55f / factor, -55f * 2f / factor, -55f * 2f / factor);

            Assert.AreEqual(65025, tile.Count(), "Initial item count");

            CircularProfile p = GetTestProfile(1400);

            IEnumerable<IContextual<TileTests.Item>> l = tile.Take(p, new CircularConfiguration(1f, 1f, CircularConfiguration.SelectionFlag.Inside));

            Assert.AreEqual(47860, l.Count(), "Selected item count");

            IQuantifiedTile<IContextual<TileTests.Item>> q = l.AsTile(tile.Area).AsQuantified(50f / factor, 50f / factor, 55f / factor, 55f / factor);

            RectangleD<Image> pi = p.GetImage(5000, 5000, tile.GetBounds());

            RectangleD<Image> i = q.GetImage(pi, z => z.Item.Context.ToBitmap(50, 50, z.Item.X + "|" + z.Item.Y));

            i.Item.Save("SelectionMediumTile.png");

            Assert.AreEqual("68FC8370B131B421E3DB81FBB255209671D3036AC3302B33BE132C9D668C48D6", this.GetHash("SelectionMediumTile.png"), "Image hash");
        }

        private string GetHash(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return stream.GetSignature();
            }
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
