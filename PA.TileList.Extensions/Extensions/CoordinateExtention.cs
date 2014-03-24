using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PA.TileList.Extensions
{
    public static class CoordinateExtention
    {
        public static string ToString<T>(this T c)
            where T : ICoordinate
        {
            return c.X + "," + c.Y;
        }

        public static Coordinate GetCoordinate<T>(this T c)
           where T : ICoordinate
        {
            return new Coordinate(c.X, c.Y);
        }

        public static Tile<T> AsTile<T>(this IEnumerable<T> l, int referenceIndex = 0)
           where T : ICoordinate
        {
            return new Tile<T>(l, referenceIndex);
        }

        public static Tile<T> AsTile<T>(this IEnumerable<T> l, IArea a, int referenceIndex = 0)
           where T : ICoordinate
        {
            return new Tile<T>(a, l, referenceIndex);
        }
    }


}
