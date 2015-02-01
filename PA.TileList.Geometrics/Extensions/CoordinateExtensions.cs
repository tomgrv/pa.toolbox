
using PA.TileList.Contextual;
using PA.TileList.Extensions;
using PA.TileList.Quantified;
using PA.TileList.Geometrics;
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
            ClockWise = 1,
            Collinear = 0,
            CounterClockWise = -1
        }

       

        public static double DistanceTo<T>(this T p, ICoordinate q)
            where T : ICoordinate
        {
            return Math.Sqrt(Math.Pow(q.X - p.X, 2) + Math.Pow(q.Y - p.Y, 2));
        }

        /// <summary>
        /// Dot Product OA.OB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetDotProduct<T>(this T o, ICoordinate a, ICoordinate b)
        where T : ICoordinate
        {
            return (a.X - o.X) * (b.X - o.X) + (a.Y - o.Y) * (b.Y - o.Y);
        }


        /// <summary>
        /// Cross Product Magnitude |OAxOB|
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double GetArea<T>(this T o, ICoordinate a, ICoordinate b)
         where T : ICoordinate
        {
            return (o.X - a.X) * (b.Y - o.Y)  - (o.Y - a.Y) * (b.X - o.X);
        }

        /// <summary>
        /// Determine whether or not a triangle has its vertices ordered clockwise or counterclockwise
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Orientation GetOrientation<T>(this T o, ICoordinate a, ICoordinate b)
            where T : ICoordinate
        {
            double area2 = o.GetArea(a, b);

            if (area2 > 0)
            {
                return Orientation.ClockWise;
            }

            if (area2 < 0)
            {
                return Orientation.CounterClockWise;
            }

            return Orientation.Collinear;
        }

        /// <summary>
        /// Determine whether or not OA and OB are Collinear
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreCollinear<T>(this T o, ICoordinate a, ICoordinate b)
            where T : ICoordinate
        {
            return o.GetOrientation<T>(a, b) == Orientation.Collinear;
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
