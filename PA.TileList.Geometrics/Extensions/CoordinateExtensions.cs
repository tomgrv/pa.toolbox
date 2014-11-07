
using PA.TileList.Contextual;
using PA.TileList.Extensions;
using PA.TileList.Quantified;
using PA.TileList.Geometrics.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.TileList.Geometrics
{
    public static class CoordinateExtensions
    {
        public enum Orientation
        {
            ClockWise = -1,
            CounterClockWise = 1,
            Collinear = 0
        }

        public static double DistanceTo<T>(this T p, ICoordinate q)
            where T : ICoordinate
        {
            return Math.Sqrt(Math.Pow(p.X - q.X, 2) + Math.Pow(p.Y - q.Y, 2));
        }

        public static Orientation GetOrientation<T>(this T b, T a, T c)
            where T : ICoordinate
        {
            double area2 = (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

            if (area2 < 0)
            {
                return Orientation.ClockWise;
            }

            if (area2 > 0)
            {
                return Orientation.CounterClockWise;
            }

            return Orientation.Collinear;
        }

        public static bool IsCollinear<T>(this T p, T q, T r)
            where T : ICoordinate
        {
            return p.GetOrientation<T>(q, r) == Orientation.Collinear;
        }

        public static bool OnSegment<T>(this T p, Line<T> l)
            where T : ICoordinate
        {
            return p.X <= Math.Max(l.Coordinate1.X, l.Coordinate2.X)
                && p.X >= Math.Min(l.Coordinate1.X, l.Coordinate2.X)
                && p.Y <= Math.Max(l.Coordinate1.Y, l.Coordinate2.Y)
                && p.Y >= Math.Min(l.Coordinate1.Y, l.Coordinate2.Y);
        }

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
            Line<T> ray = new Line<T>(p, extreme);

            // Count intersections of the above line with sides of polygon
            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % polygon.Length;

                Line<T> segment = new Line<T>(polygon[i], polygon[next]);

                // Check if the line segment from 'p' to 'extreme' intersects
                // with the line segment from 'polygon[i]' to 'polygon[next]'
                if (segment.Intersect(ray))
                {
                    count++;
                }
                else if (p.IsCollinear(polygon[i], polygon[next]))
                {
                    return p.OnSegment(segment);
                }

                i = next;

            } while (i != 0);

            // Return true if count is odd, false otherwise
            return count % 2 == 1;
        }

        #region Translate

        public enum TranslateSource
        {
            Center,
            Min, Max
        }

        /// <summary>
        /// Translate all list so that specified source match coordinate [0,0] 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<IContextual<T>> Translate<T>(this IEnumerable<T> c, TranslateSource source)
            where T : ICoordinate
        {
            switch (source)
            {
                case TranslateSource.Min:
                    return c.Translate(c.GetArea().Min, Coordinate.Zero);
                case TranslateSource.Max:
                    return c.Translate(c.GetArea().Max, Coordinate.Zero);
                default:
                    return c.Translate(c.GetArea().Center(), Coordinate.Zero);
            }
        }

        /// <summary>
        /// Translate all list so that specified source match coordinate [0,0] 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<IContextual<T>> Translate<T>(this IEnumerable<IContextual<T>> c, TranslateSource source)
            where T : ICoordinate
        {
            switch (source)
            {
                case TranslateSource.Min:
                    return c.Translate(c.GetArea().Min, Coordinate.Zero);
                case TranslateSource.Max:
                    return c.Translate(c.GetArea().Max, Coordinate.Zero);
                default:
                    return c.Translate(c.GetArea().Center(), Coordinate.Zero);
            }
        }

        public static ITile<IContextual<T>> Translate<T>(this ITile<T> t, TranslateSource source)
            where T : ICoordinate
        {
            return CoordinateExtensions.Translate<T>(t.AsEnumerable(), source)
                .AsTile(t.IndexOf(t.Reference))
                .RefreshArea();
        }

        public static ITile<IContextual<T>> Translate<T>(this ITile<IContextual<T>> t, TranslateSource source)
           where T : ICoordinate
        {
            return CoordinateExtensions.Translate<T>(t.AsEnumerable(), source)
                .AsTile(t.IndexOf(t.Reference))
                .RefreshArea();
        }

        public static IQuantifiedTile<IContextual<T>> Translate<T>(this IQuantifiedTile<T> t, TranslateSource source)
            where T : ICoordinate
        {
            return CoordinateExtensions.Translate<T>(t.AsTile(), source)
                .AsQuantified(t.ElementSizeX, t.ElementSizeY, t.ElementStepX, t.ElementStepY, t.RefOffsetX, t.RefOffsetY);
        }

        public static IQuantifiedTile<IContextual<T>> Translate<T>(this IQuantifiedTile<IContextual<T>> t, TranslateSource source)
           where T : ICoordinate
        {
            return CoordinateExtensions.Translate<T>(t.AsTile(), source)
                .AsQuantified(t.ElementSizeX, t.ElementSizeY, t.ElementStepX, t.ElementStepY, t.RefOffsetX, t.RefOffsetY);
        }


        /// <summary>
        /// Translate all list so that source match destination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="refOrigin"></param>
        /// <param name="refDest"></param>
        /// <returns></returns>
        public static IEnumerable<IContextual<T>> Translate<T>(this IEnumerable<T> c, ICoordinate refOrigin, ICoordinate refDest)
            where T : ICoordinate
        {
            int offsetX = refDest.X - refOrigin.X;
            int offsetY = refDest.Y - refOrigin.Y;

            foreach (T e in c)
            {
                yield return new Contextual<T>(e.X + offsetX, e.Y + offsetY, e);
            }
        }


        /// <summary>
        /// Translate all list so that source match destination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="refOrigin"></param>
        /// <param name="refDest"></param>
        /// <returns></returns>
        public static IEnumerable<IContextual<T>> Translate<T>(this IEnumerable<IContextual<T>> c, ICoordinate refOrigin, ICoordinate refDest)
            where T : ICoordinate
        {
            int offsetX = refDest.X - refOrigin.X;
            int offsetY = refDest.Y - refOrigin.Y;

            foreach (IContextual<T> e in c)
            {
                e.X += offsetX;
                e.Y += offsetY;
            }

            return c;
        }

        #endregion
    }
}
