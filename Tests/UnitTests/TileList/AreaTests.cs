using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;
using PA.TileList.Geometrics;

namespace UnitTests
{
    [TestClass]
    public class AreaTests
    {
        [TestMethod]
        public void CreateArea()
        {
            Area a = new Area(-1, -1,10,10);
            Area b = new Area(new Coordinate(-1, -1),  new Coordinate(10,10));

            Assert.AreEqual(a.SizeX , b.SizeX);
            Assert.AreEqual(a.SizeY , b.SizeY);

        }

    }
}
