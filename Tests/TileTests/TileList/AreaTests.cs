using System;
using PA.TileList;
using PA.TileList.Geometrics;
using NUnit.Framework;

namespace PA.TileList
{
    [TestFixtureAttribute]
    public class AreaTests
    {
        [Test]
        public void CreateArea()
        {
            var a = new Area(-1, -1,10,10);
            var b = new Area(new Coordinate(-1, -1),  new Coordinate(10,10));

            Assert.AreEqual(a.SizeX , b.SizeX);
            Assert.AreEqual(a.SizeY , b.SizeY);

        }

    }
}
