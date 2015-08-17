using PA.Plugin;
using PA.Plugin.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace PA.Plugin.Tests.Objects
{
    [ExportWithLabel(typeof(IPlugin<Uri>), "http")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class Plugin2ForUriHttp : PluginForUriHttp
    {
        public Plugin2ForUriHttp(Uri value)
            :base(value)
        {
            this.Value = value;
        }
    }
}
