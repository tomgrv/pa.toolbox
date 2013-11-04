using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Configuration
{
    public interface IConfigurationItem
    {
        string Contract { get; }
    }

    public interface IConfigurationItem<T> : IConfigurationItem
    {
        T Value { get; set; }
        void Refresh();
        void Save();
    }
}
