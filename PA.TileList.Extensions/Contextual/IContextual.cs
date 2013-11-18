using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.TileList.Contextual
{
    public interface IContextual<T> : ICoordinate, IDisposable
        where T : ICoordinate, IDisposable
    {
        T Context { get; }
    }
}
