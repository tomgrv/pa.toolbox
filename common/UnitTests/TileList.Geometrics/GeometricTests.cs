using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList.Geometrics.Circular;
using PA.TileList.Quantified;
using PA.TileList.Contextual;
using PA.TileList.Extensions;
using PA.TileList.Geometrics;
using PA.TileList.Drawing;
using System.Drawing;
using PA.TileList;
using PA.File.Extensions;
using System.Collections.Generic;
using UnitTests.TileList;
using System.IO;
using System.Security.Cryptography;

namespace UnitTests.Geometrics
{
    [TestClass]
    public class GeometricTests
    {
        [TestMethod]
        public void InsidePolygon()
        {
            Coordinate[] polygon1 = new Coordinate[] { new Coordinate(0, 0), new Coordinate(10, 0), new Coordinate(10, 10), new Coordinate(0, 10) };
            Coordinate p1 = new Coordinate( 20, 20 );
            
            Assert.AreEqual(false,p1.IsInside(polygon1));

            Coordinate p2 = new Coordinate(5, 5);
            Assert.AreEqual(true, p2.IsInside(polygon1));

            Coordinate[] polygon2 = new Coordinate[] { new Coordinate(0, 0), new Coordinate(5, 5), new Coordinate(5, 0)};

            Coordinate p3 = new Coordinate(3, 3);
            Assert.AreEqual(true, p3.IsInside(polygon2));

            Coordinate p4 = new Coordinate(5, 1);
            Assert.AreEqual(true, p4.IsInside(polygon2));

            Coordinate p5 = new Coordinate(8, 1);
            Assert.AreEqual(false, p5.IsInside(polygon2));

        }


        internal static Tile<Coordinate> GetGeoTile()
        {
            IArea a1 = new Area(0, 0, 100, 100);

            Tile<Coordinate> t0 = new Tile<Coordinate>(a1, new Coordinate(0, 0));

            for (int i = 0; i < 100; i++)
            {

            }


            return t0;
        }
    }

}
