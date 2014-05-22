using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.TileList.Quantified
{
    public static class QuantifiedExtensions
    {
        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> l)
             where T : ICoordinate
        {
            return l as IQuantifiedTile<T> ?? new QuantifiedTile<T>(l);
        }

        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> l, double sizeX, double sizeY)
            where T : ICoordinate
        {
            return new QuantifiedTile<T>(l, sizeX, sizeY);
        }

        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> l, double sizeX, double sizeY, double stepX, double stepY)
           where T : ICoordinate
        {
            return new QuantifiedTile<T>(l, sizeX, sizeY, stepX, stepY);
        }

        public static IQuantifiedTile<T> AsQuantified<T>(this ITile<T> l, double sizeX, double sizeY, double stepX, double stepY, double offsetX, double offsetY)
           where T : ICoordinate
        {
            return new QuantifiedTile<T>(l, sizeX, sizeY, stepX, stepY, offsetX, offsetY);
        }

        public static double GetScaleFactor(this IQuantifiedTile list, double sizeX, double sizeY)
        {
            double ratioX = sizeX / (list.Area.SizeX * list.ElementStepX);
            double ratioY = sizeY / (list.Area.SizeY * list.ElementStepY);

            return Math.Min(ratioX, ratioY);
        }

        public static IQuantifiedTile<T> Scale<T>(this IQuantifiedTile<T> list, double scaleFactor)
              where T : ICoordinate
        {
            return new QuantifiedTile<T>(list, list.ElementSizeX * scaleFactor, list.ElementSizeY * scaleFactor, list.ElementStepX * scaleFactor, list.ElementStepY * scaleFactor, list.RefOffsetX * scaleFactor, list.RefOffsetY * scaleFactor);
        }

        public static T FirstOrDefault<T>(this IQuantifiedTile<T> list, double x, double y)
             where T : ICoordinate
        {
            double xx = (x - list.RefOffsetX) / list.ElementStepX;
            double yy = (y - list.RefOffsetY) / list.ElementStepY;

            return list.FirstOrDefault(t => (t.X - list.Reference.X) <= xx && xx < (t.X - list.Reference.X + 1) && (t.Y - list.Reference.Y) <= yy && yy < (t.Y - list.Reference.Y + 1));
        }

        public static Coordinate GetCoordinatesAt<T>(this IQuantifiedTile<T> list, double x, double y)
             where T : ICoordinate
        {
            double xx = (x - list.RefOffsetX) / list.ElementStepX;
            double yy = (y - list.RefOffsetY) / list.ElementStepY;

            return new Coordinate((int)xx + list.Reference.X, (int)yy + list.Reference.Y);
        }
    }
}
