using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.TileList.Contextual
{
    public class Contextual<T> : IContextual<T>
        where T : ICoordinate
    {
        public int X { get;  set; }
        public int Y { get;  set; }

        public T Context { get; private set; }

        public Contextual(int x, int y, T context)
        {
            this.X = x;
            this.Y = y;
            this.Context = context;
        }

    }
}
