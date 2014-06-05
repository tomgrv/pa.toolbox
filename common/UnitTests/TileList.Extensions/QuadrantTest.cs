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
            Assert.AreEqual("7D26EB8C9A43C5A80901C93013EE573C6A732EDA96BC50605B2300FCFE5F1DC2", signature1,"TopLeft");

            IQuantifiedTile<IContextual<IContextual<TileTests.Item>>> t2 = tile
                 .Flatten<TileTests.SubTile, TileTests.Item>()
                 .ChangeQuadrant(Quadrant.TopLeft, Quadrant.BottomLeft);

            string signature2 = t2.GetImage(5000, 5000, z => z.Item.Context.Context.ToBitmap(100, 100, z.Item.Context.X + "\n" + z.Item.Context.Y)).Item.GetSignature("BottomLeft");
            Assert.AreEqual("2407B38641F28F13196AECB9B8E19B625319712288C6E91AD6533EF5DA1698E3", signature2, "BottomLeft");

            IQuantifiedTile<IContextual<IContextual<TileTests.Item>>> t3 = tile
               .Flatten<TileTests.SubTile, TileTests.Item>()
               .ChangeQuadrant(Quadrant.TopLeft, Quadrant.BottomRight);

            string signature3 = t3.GetImage(5000, 5000, z => z.Item.Context.Context.ToBitmap(100, 100, z.Item.Context.X + "\n" + z.Item.Context.Y)).Item.GetSignature("BottomRight");

            Assert.AreEqual("68C94ECC7077829ABDD3C064AC54EC709018553EFFF596E404A400062C98071E", signature3, "BottomRight");

            IQuantifiedTile<IContextual<IContextual<TileTests.Item>>> t4 = tile
             .Flatten<TileTests.SubTile, TileTests.Item>()
             .ChangeQuadrant(Quadrant.TopLeft, Quadrant.TopRight);

            string signature4 = t4.GetImage(5000, 5000, z => z.Item.Context.Context.ToBitmap(100, 100, z.Item.Context.X + "\n" + z.Item.Context.Y)).Item.GetSignature("TopRight");

            Assert.AreEqual("3D7210B4561E628184F033755C353947F93E588402883BC18DD6D83D5012B967", signature4, "TopRight");
        }

    }
}
