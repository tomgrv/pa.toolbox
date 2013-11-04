using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Objects;
using System.Text;

namespace PA.Data.Interfaces
{
    public interface IContextHandler<T> : IDisposable where T : ObjectContext
    {
        T Context { get; }
    }
}
