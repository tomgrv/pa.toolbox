using PA.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace UnitTests.Plugin
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
