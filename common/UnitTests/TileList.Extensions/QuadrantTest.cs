using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList.Contextual;
using PA.TileList.Quantified;
using PA.TileList.Extensions;
using PA.TileList.Drawing;
using PA.TileList.Quadrant;
using PA.File.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitTests.TileList;
using System.IO;
using PA.TileList;

namespace UnitTests
{
    [TestClass]
    public class QuadrantTest
    {
        [TestMethod]
        public void ChangeQuadrant()
        {
            float factor = 1;

            TileTests.MainTile tile = TileTests.GetTile(factor);

            IQuantifiedTile<IContextual<TileTests.Item>> t1 = tile
               .Flatten<TileTests.SubTile, TileTests.Item>();

            string signature1 = t1.GetImage(5000, 5000, z => z.Item.Context.ToBitmap(100, 100, z.Item.X + "\n" + z.Item.Y)).Item.GetSignature("TopLeft");
            Assert.AreEqual("055FBADECFE4D727D083968D6D2AEA62A2312E303FF48635474E11E5525CEEC3", signature1, "TopLeft");

            IQuantifiedTile<IContextual<IContextual<TileTests.Item>>> t2 = tile
                 .Flatten<TileTests.SubTile, TileTests.Item>()
                 .ChangeQuadrant(Quadrant.TopLeft, Quadrant.BottomLeft);

            string signature2 = t2.GetImage(5000, 5000, z => z.Item.Context.Context.ToBitmap(100, 100, z.Item.Context.X + "\n" + z.Item.Context.Y)).Item.GetSignature("BottomLeft");
            Assert.AreEqual("4B02E3B3619367AB0CCE9AB8648B508FE5611B1D1B46BD225AB62A90F014BA0D", signature2, "BottomLeft");

            IQuantifiedTile<IContextual<IContextual<TileTests.Item>>> t3 = tile
               .Flatten<TileTests.SubTile, TileTests.Item>()
               .ChangeQuadrant(Quadrant.TopLeft, Quadrant.BottomRight);

            string signature3 = t3.GetImage(5000, 5000, z => z.Item.Context.Context.ToBitmap(100, 100, z.Item.Context.X + "\n" + z.Item.Context.Y)).Item.GetSignature("BottomRight");

            Assert.AreEqual("0ED609DCF12112DCFDDAEC61C32DBEB9874B347C3E5305CA545A5D6795F8DA31", signature3, "BottomRight");

            IQuantifiedTile<IContextual<IContextual<TileTests.Item>>> t4 = tile
             .Flatten<TileTests.SubTile, TileTests.Item>()
             .ChangeQuadrant(Quadrant.TopLeft, Quadrant.TopRight);

            string signature4 = t4.GetImage(5000, 5000, z => z.Item.Context.Context.ToBitmap(100, 100, z.Item.Context.X + "\n" + z.Item.Context.Y)).Item.GetSignature("TopRight");

            Assert.AreEqual("70CEDF7E06EE71F13DC5844E3ECC5F897501BD8356B3FF6EE60430B23781ECA6", signature4, "TopRight");
        }

    }
}
