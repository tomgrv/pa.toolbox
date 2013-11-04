using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Configuration
{
    public interface IConfigurationProvider
    {
        IConfigurationSource Source { get; }
    }
}
