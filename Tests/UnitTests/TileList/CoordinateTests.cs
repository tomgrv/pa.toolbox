using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;
using PA.TileList.Geometrics;

namespace UnitTests
{
    [TestClass]
    public class CoordinateTests
    {
        [TestMethod]
        public void CreateCoordinate()
        {
            Coordinate a = new Coordinate(0, 0);
            Coordinate b = new Coordinate(-1, -1);
        }
    }


    
}
