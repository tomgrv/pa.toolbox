using PA.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace UnitTests.Plugin
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [PluginDescription("DESC")]
    public class PluginForSpecificImportTest : IPlugin
    {
        [Import("SpecificImport/Param")]
        public string Parameter { get; set; }

        public PluginForSpecificImportTest()
        {
        }

        public void Dispose()
        {
            
        }
    }
}
