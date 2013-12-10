using PA.Plugin.Components.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Plugin.Components.Interfaces
{
    public interface IPluginHelper
    {
         IPluginHelper NextHelper { get; set; }
         bool Help<T>(T p) where T : IPlugin;
    }
}
