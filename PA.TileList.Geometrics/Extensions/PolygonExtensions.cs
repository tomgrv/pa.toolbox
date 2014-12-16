using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.TileList;

namespace PA.TileList.Geometrics
{
    public static class PolygonExtensions
    {
        public static bool IsInside<T>(this T p, T[] polygon)
           where T : ICoordinate
        {
            // There must be at least 3 vertices in polygon[]
            if (polygon.Length < 3)
            {
                throw new InvalidOperationException("At least 3 Coordinates needed in polygon");
            }

            // Create a point for line segment from p to infinite
            Coordinate extreme = new Coordinate(int.MaxValue, p.Y);
            Segment<T> ray = new Segment<T>(p, extreme);

            // Count intersections of the above line with sides of polygon
            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % polygon.Length;

                Segment<T> segment = new Segment<T>(polygon[i], polygon[next]);

                // Check if the line segment from 'p' to 'extreme' intersects
                // with the line segment from 'polygon[i]' to 'polygon[next]'
                if (!segment.AreCollinear(ray))
                {
                    count++;
                }
                else if (p.AreCollinear(polygon[i], polygon[next]))
                {
                    return segment.Contains(p);
                }

                i = next;

            } while (i != 0);

            // Return true if count is odd, false otherwise
            return count % 2 == 1;
        }

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
