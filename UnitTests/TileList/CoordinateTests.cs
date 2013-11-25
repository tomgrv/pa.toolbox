using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;

namespace UnitTests
{
    [TestClass]
    public class CoordinateTests
    {
        [TestMethod]
        public void GetPerimeter()
        {
            Coordinate c1 = new Coordinate(0, 0);
            Coordinate c2 = new Coordinate(1, 0);
            Coordinate c3 = new Coordinate(1, 1);
            Coordinate c4 = new Coordinate(0, 1);

            Assert.AreEqual(4.0f, PA.TileList.Coordinate.GetPerimeter(c1, c2, c3, c4));
        }

        [TestMethod]
        public void GetLength()
        {
            Coordinate c1 = new Coordinate(0, 0);
            Coordinate c2 = new Coordinate(1, 1);

            Assert.AreEqual(Math.Sqrt(2), PA.TileList.Coordinate.GetLength(c1, c2));
        }

    }
}
