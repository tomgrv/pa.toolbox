using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.TileList;
using PA.TileList.Contextual;
using PA.TileList.Quantified;

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
           where T : ICoordinate, IDisposable
        {
            foreach (IContextual<T> c in t)
            {
                yield return c.Context;
            }
        }

        public static IQuantifiedTile<IContextual<T>> Flatten<U, T>(this IQuantifiedTile<U> t)
            where U : ITile<T>
            where T : ICoordinate, IDisposable
        {
            IContextual<T> reference = t.Reference.Contextualize(t.Reference.Reference);

            QuantifiedTile<IContextual<T>> list = new QuantifiedTile<IContextual<T>>((t as ITile<U>).Flatten<U,T>(),
                                                               t.ElementSizeX, t.ElementSizeY, t.ElementStepX, t.ElementStepY, t.RefOffsetX, t.RefOffsetY);

            list.SetReference(list.Find(reference.X, reference.Y));

            return list;
        }

        public static IQuantifiedTile<IContextual<T>> Flatten<U, T>(this IQuantifiedTile<U> t, Func<U, bool> predicate)
            where U : ITile<T>
            where T : ICoordinate, IDisposable
        {
            return new QuantifiedTile<IContextual<T>>(ContextualExtensions.Flatten<U,T>(t as ITile<U>,predicate),
                t.ElementSizeX, t.ElementSizeY, t.ElementStepX, t.ElementStepY, t.RefOffsetX, t.RefOffsetY);
        }

        public static ITile<IContextual<T>> Flatten<U, T>(this ITile<U> t, Func<U, bool> predicate)
            where U : ITile<T>
            where T : ICoordinate, IDisposable
        {
            IContextual<T> reference = t.Reference.Contextualize(t.Reference.Reference);

            Tile<IContextual<T>> list = new Tile<IContextual<T>>(t.GetArea(), t.Where<U>(predicate).SelectMany<U, IContextual<T>>(subtile => subtile.Select(c => subtile.Contextualize(c))));

            list.SetReference(list.Find(reference.X, reference.Y));

            return list;
        }

        public static ITile<IContextual<T>> Flatten<U, T>(this ITile<U> t)
            where T : ICoordinate
            where U : ITile<T>
        {
            IContextual<T> reference = t.Reference.Contextualize(t.Reference.Reference);

            Tile<IContextual<T>> list = new Tile<IContextual<T>>(t.GetArea(), t.SelectMany<U, IContextual<T>>(subtile => subtile.Select(c => subtile.Contextualize(c))));

            list.SetReference(list.Find(reference.X, reference.Y));

            return list;
        }
    }
}
