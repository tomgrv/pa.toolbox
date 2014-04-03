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

            t1.GetImage(5000, 5000, z => z.Item.Context.ToBitmap(100, 100, z.Item.X + "\n" + z.Item.Y)).Item.Save("TopLeft.png");

            Assert.AreEqual("3B6438BFF33561F88678CB99059E524C73D09E03C1B7E06323C2609669B0B47D", File.OpenRead("TopLeft.png").GetSignature());

            IQuantifiedTile<IContextual<IContextual<TileTests.Item>>> t2 = tile
                 .Flatten<TileTests.SubTile, TileTests.Item>()
                 .ChangeQuadrant(Quadrant.TopLeft, Quadrant.BottomLeft);

            t2.GetImage(5000, 5000, z => z.Item.Context.Context.ToBitmap(100, 100, z.Item.Context.X + "\n" + z.Item.Context.Y)).Item.Save("BottomLeft.png");

            Assert.AreEqual("EEE628FE50280186EE13B6ED5FA1DD92BD49818E35EB1CB742B44D6328E48AF1", File.OpenRead("BottomLeft.png").GetSignature());

            IQuantifiedTile<IContextual<IContextual<TileTests.Item>>> t3 = tile
               .Flatten<TileTests.SubTile, TileTests.Item>()
               .ChangeQuadrant(Quadrant.TopLeft, Quadrant.BottomRight);

            t3.GetImage(5000, 5000, z => z.Item.Context.Context.ToBitmap(100, 100, z.Item.Context.X + "\n" + z.Item.Context.Y)).Item.Save("BottomRight.png");

            Assert.AreEqual("F01A71D8E9576A12F532540CC082F00DF1AC540328FF2DDA70DE9056B6EBE163", File.OpenRead("BottomRight.png").GetSignature());

            IQuantifiedTile<IContextual<IContextual<TileTests.Item>>> t4 = tile
             .Flatten<TileTests.SubTile, TileTests.Item>()
             .ChangeQuadrant(Quadrant.TopLeft, Quadrant.TopRight);

            t4.GetImage(5000, 5000, z => z.Item.Context.Context.ToBitmap(100, 100, z.Item.Context.X + "\n" + z.Item.Context.Y)).Item.Save("TopRight.png");

            Assert.AreEqual("DFB20F2C276C221488717113B0F6866FC5E64EA100C40B33F1B0DFD523D36FDE", File.OpenRead("TopRight.png").GetSignature());
        }

    }
}
