using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;
using PA.TileList.Contextual;
using PA.TileList.Drawing;
using PA.TileList.Geometrics.Circular;
using System;
using System.Drawing;
using System.Linq;
using UnitTests.TileList;

namespace UnitTests.Drawing
{
    [TestClass]
    public class CircularTests
    {
        [TestMethod, TestCategory("Image hash")]
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
            Assert.AreEqual("98AE8580E2596469A774C97BEE234564E96281C519BFFED621FBB8CC2A63F6D8", signature, "Image hash");
        }

        [TestMethod, TestCategory("Image hash")]
        public void ProfileWithFlat()
        {
            CircularProfile search = new CircularProfile(1000);

            search.AddProfileFlatByLength(0, 1500);
            search.AddProfileFlatByLength(Math.PI, 500, 0.0001, 500);
            search.AddProfileFlat(Math.PI / 2f, 200, 1000);
            search.AddProfileStep(-Math.PI / 4, 1000);
            search.AddProfileStep(-3 * Math.PI / 4, 800);

            string signature = search.GetImage(1000, 1000).Item.GetSignature();
            Assert.AreEqual("EED4365394FDB98CE5A4566244C50FA9925A28F54F8561533295FAC5E4B91FE4", signature, "Image hash");
        }

        [TestMethod, TestCategory("Image hash")]
        public void Profile()
        {
            CircularProfile p = new CircularProfile(1500);

            RectangleD<Bitmap> i = p.GetImage(1000, 1000, new RectangleF(-2000, -2000, 4000, 4000));

            string signature = i.Item.GetSignature();
            Assert.AreEqual("B1FF0A62F65DD493C2781D6D9FB57C4F588F9B0E767EEBAC6219E01EA5A5DF4D", signature, "Image hash");
        }

        [TestMethod]
        public void ProfileForTest()
        {
            CircularProfile p = GetTestProfile(1400);

            RectangleD<Bitmap> i = p.GetImage(1000, 1000);

            string signature = i.Item.GetSignature();
            Assert.AreEqual("DAA3296DC2EE2A6682DFFBD8425ED029E34004676D6AB80E67DBB691E85CD2E0", signature, "Image hash");
        }

        [TestMethod, TestCategory("Image hash")]
        public void SelectionSmallTile()
        {
            float factor = 1f;

            IQuantifiedTile<IContextual<TileTests.Item>> tile = TileTests.GetTile(factor)
                .Flatten<TileTests.SubTile, TileTests.Item>();

            Assert.AreEqual(3025, tile.Count(), "Initial item count");

            CircularProfile p = GetTestProfile(1400);

            bool change = true;

            IQuantifiedTile<IContextual<TileTests.Item>> q = tile.Take(p, new CircularConfiguration(1f, CircularConfiguration.SelectionFlag.Inside), ref change);

            Assert.AreEqual(false, change, "Reference Changed");
            Assert.AreEqual(1799, q.Count(), "Selected item count");

            RectangleD<Bitmap> pi = p.GetImage(1000, 1000, tile.GetBounds());

            RectangleD<Bitmap> i = q.GetImage(pi, (z, s) => z.Context.ToBitmap(50, 50, z.X + "\n" + z.Y));

            string signature1 = q.GetSignature(it => it.Context.Color.GetHashCode().ToString());
            Assert.AreEqual("ADE22DBF99F378AEE20F993BF51705756AFFF2539CA8D6CC5CCA7266C9F2B551", signature1, "Signature");

            string signature = i.Item.GetSignature();
            Assert.AreEqual("ADE22DBF99F378AEE20F993BF51705756AFFF2539CA8D6CC5CCA7266C9F2B551", signature, "Image hash");
        }

        [TestMethod, TestCategory("Image hash")]
        public void SelectionMediumTile()
        {
            float factor = 5f;

            IQuantifiedTile<IContextual<TileTests.Item>> tile = TileTests.GetTile(factor)
                .Flatten<TileTests.SubTile, TileTests.Item>();

            Assert.AreEqual(65025, tile.Count(), "Initial item count");

            CircularProfile p = GetTestProfile(1000);

            foreach (IContextual<TileTests.Item> tt in tile.Except(tile.Take(p, new CircularConfiguration(1f, CircularConfiguration.SelectionFlag.Inside)).ToArray()).ToArray())
            {
                tt.Context.Color = Color.Beige;
            }

            //Assert.AreEqual(47860, tile.Count(), "Selected item count");

            //RectangleD<Bitmap> pi = p.GetImage(5000, 5000, tile.GetBounds());
            //RectangleD<Bitmap> i = tile.GetImage(pi, (z, s) => z.Context.ToBitmap(50, 50, z.X + "\n" + z.Y));
            //string signature_1 = i.Item.GetSignature();
            //Assert.AreEqual("E63318A4278EED31907E0374B728F045285D43B6FBE0955A1622BFCFBB7AF5B8", signature_1, "Image hash");

            RectangleD<Bitmap> j = tile.GetImage(5000, 5000, (z, s) => z.Context.ToBitmap(50, 50, z.X + "\n" + z.Y));
            RectangleD<Bitmap> pj = p.GetImage(j);
            string signature_2 = pj.Item.GetSignature();


            Assert.AreEqual("E63318A4278EED31907E0374B728F045285D43B6FBE0955A1622BFCFBB7AF5B8", signature_2, "Image hash");
        }

        private CircularProfile GetTestProfile(double radius, double stepping = 1f, double resolution = 1f)
        {
            CircularProfile p = new CircularProfile(radius);

            p.AddProfileFlat(-Math.PI / 2, radius - 100, 100, stepping);
            p.AddProfileFlat(7 * Math.PI / 4, radius - 200, 100, stepping);
            p.AddProfileFlat(0, radius - 300, 100, stepping, resolution);
            p.AddProfileFlat(Math.PI / 3f, radius - 400, 200, stepping, resolution);
            p.AddProfileFlat(2f * Math.PI / 3f, radius - 500, 400, stepping, resolution);

            return p;
        }
    }

}
