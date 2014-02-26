using PA.Plugin;
using PA.Plugin.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace UnitTests.Plugin
{
    [ExportWithLabel(typeof(IPlugin<Uri>), "http")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class PluginForUriHttp : IPlugin<Uri>
    {
        public Uri Value { get; set; }

        [ImportingConstructor]
        public PluginForUriHttp(Uri value)
        {
            this.Value = value;
        }

       


        public void Dispose()
        {
            
        }
    }
}
