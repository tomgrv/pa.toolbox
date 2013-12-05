using PA.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace UnitTests.Plugin
{
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class PluginForGenericImportTest : IPlugin
    {
        [Import("GenericImport/Param")]
        public string Parameter { get; set; }

        public PluginForGenericImportTest()
        {
        }

        public void Dispose()
        {
            
        }
    }
}
