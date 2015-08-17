using PA.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace PA.Plugin.Tests.Objects
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class PluginForGenericArrayTest : IPlugin
    {
        [Import("#/Param")]
        public string Parameter { get; set; }

        public PluginForGenericArrayTest()
        {
        }

        public void Dispose()
        {
            
        }
    }
}
