using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.TileList;

namespace PA.TileList.Problems
{
    public static class Geometry
    {
        public static IArea GetBiggestArea<T>(this IEnumerable<T> tile)
            where T : ICoordinate
        {
            T[] data = tile.ToArray();
            int maxA = 0;
            IArea area = null;

            foreach (T t0 in data.OrderBy(t => t.X).OrderBy(t => t.Y))
            {
                foreach (T tx in data.Where(t => t.X == t0.X && t.Y > t0.Y).OrderByDescending(t => t.Y))
                {
                    foreach (T ty in data.Where(t => t.Y == t0.Y && t.X > t0.X).OrderByDescending(t => t.X))
                    {
                        var tR = data.FirstOrDefault(t => t.Y == tx.Y && t.X == ty.X);
                        if (tR is T)
                        {
                            int s = Math.Abs((tR.X - t0.X) * (tR.Y - t0.Y));
                            if (s > maxA)
                            {
                                maxA = s;
                                area = new Area(t0.X, t0.Y, tR.X, tR.Y);
                            }
                        }
                    }
                }
            }

            return area;
        }
    }
}
