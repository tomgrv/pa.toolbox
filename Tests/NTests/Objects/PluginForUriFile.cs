using PA.Plugin;
using PA.Plugin.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace PA.Plugin.Tests.Objects
{
    [ExportWithLabel(typeof(IPlugin<Uri>), "file")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class PluginForUriFile : IPlugin<Uri>
    {
        public Uri  Value { get; set; }

        public PluginForUriFile()
        {
          

        }

        public PluginForUriFile(Uri value)
        {
            this.Value = value;
            
        }

        public void Dispose()
        {
            
        }
    }
}
