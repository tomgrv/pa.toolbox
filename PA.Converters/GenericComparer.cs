using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Converters
{
    public class GenericCompare<T> : IEqualityComparer<T> where T : class
    {
        private Func<T, object> select { get; set; }

        public GenericCompare(Func<T, object> expr)
        {
            this.select = expr;
        }

        public bool Equals(T x, T y)
        {
            var a = select.Invoke(x);
            var b = select.Invoke(y);

            return a != null && a.Equals(b);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}