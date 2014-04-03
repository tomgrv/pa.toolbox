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
        public string[][] Parameter { get; set; }

        [PluginDescription("Parameter String")]
        public string ParamString { get; set; }

        [PluginDescription("Parameter Boolean")]
        public bool ParamBool { get; set; }

        public enum Mode { 
            MODE_A,
            MODE_B
        }

        [PluginDescription("Parameter Enum")]
        public Mode ParamEnum { get; set; }   


        public PluginForSpecificImportTest()
        {
            ParamString = "Hello";
        }

        public void Dispose()
        {
            
        }
    }
}
