using PA.Plugin;
using PA.Plugin.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace PA.Plugin.Tests.Objects
{
    [ExportWithLabel(typeof(IPluginUri), "http")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class PluginForUriHttp : IPluginUri
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
