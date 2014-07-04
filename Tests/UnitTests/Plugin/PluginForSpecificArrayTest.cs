using PA.Plugin;
using PA.Plugin.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace UnitTests.Plugin
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class PluginForSpecificArrayTest : IPlugin
    {
        [Import("#/Param")]
        public string Parameter { get; set; }

        public PluginForSpecificArrayTest()
        {
            
        }


        public void Dispose()
        {
            
        }
    }
}
