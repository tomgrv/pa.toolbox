using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace PA.Data.Interfaces
{
    public interface IContextUser<T> where T : ObjectContext
    {
        void UseContext(IContextHandler<T> entities);
    }
}
