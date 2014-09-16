using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.TileList;
using PA.TileList.Contextual;
using PA.TileList.Quantified;
using PA.TileList.Extensions;

namespace PA.TileList.Contextual
{
    public static class ContextualExtensions
    {
        public static IContextual<T> Contextualize<T>(this ITile t, T item)
          where T : ICoordinate
        {
            return t.Contextualize<T>(item, t.Area.SizeX, t.Area.SizeY);
        }

        public static IContextual<T> Contextualize<T>(this ITile t, T item, IArea a)
            where T : ICoordinate
        {
            return t.Contextualize<T>(item, a.SizeX, a.SizeY);
        }

        public static IContextual<T> Contextualize<T>(this ITile t, T item, ushort sizeX, ushort sizeY)
            where T : ICoordinate
        {
            return new Contextual<T>(item.X + t.X * sizeX, item.Y + t.Y * sizeY, item);
        }

        public static IEnumerable<T> ToContext<T>(this IEnumerable<IContextual<T>> t)
           where T : ICoordinate
        {
            foreach (IContextual<T> c in t)
            {
                yield return c.Context;
            }
        }

        public static IQuantifiedTile<IContextual<T>> Flatten<U, T>(this IQuantifiedTile<U> t, Func<U, bool> predicate = null)
            where U : ITile<T>
            where T : ICoordinate
        {
            IContextual<T> reference = t.Reference.Contextualize(t.Reference.Reference);

            // Sizes in flattened output Tile
            double sizeX = t.ElementSizeX / t.Reference.Area.SizeX;
            double sizeY = t.ElementSizeY / t.Reference.Area.SizeY;
            double stepX = t.ElementStepX / t.Reference.Area.SizeX;
            double stepY = t.ElementStepY / t.Reference.Area.SizeY;

            // Convert offset<U> (relative to <U> center) to Offset<T> (relative to  <T> center), expressed in {number of <T>} 
            double distX = (t.Reference.Reference.X - t.Reference.Area.Min.X) - ((t.Reference.Area.SizeX - 1) / 2f);
            double distY = (t.Reference.Reference.Y - t.Reference.Area.Min.Y) - ((t.Reference.Area.SizeY - 1) / 2f);

            QuantifiedTile<IContextual<T>> list = new QuantifiedTile<IContextual<T>>(ContextualExtensions.Flatten<U, T>(t as ITile<U>, predicate),
               sizeX, sizeY, stepX, stepY, distX * stepX + t.RefOffsetX, distY * stepY + t.RefOffsetY
            );

            list.SetReference(list.Find(reference.X, reference.Y));

            return list;
        }


        public static ITile<IContextual<T>> Flatten<U, T>(this ITile<U> t, Func<U, bool> predicate = null)
            where U : ITile<T>
            where T : ICoordinate
        {
            IContextual<T> reference = t.Reference.Contextualize(t.Reference.Reference);

            IEnumerable<IContextual<T>> list;

            if (predicate is Func<U, bool>)
            {
                list = t.Where<U>(predicate).SelectMany<U, IContextual<T>>(subtile => subtile.Select(c => subtile.Contextualize(c)));
            }
            else
            {
                list = t.SelectMany<U, IContextual<T>>(subtile => subtile.Select(c => subtile.Contextualize(c)));
            }

            Tile<IContextual<T>> tile = new Tile<IContextual<T>>(t.GetArea(), list);
            tile.UpdateArea();
            tile.SetReference(tile.Find(reference.X, reference.Y));
            return tile;
        }
    }
}
