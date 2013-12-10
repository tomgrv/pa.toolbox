using PA.Plugin.Components.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace PA.Plugin.Components.Interfaces
{
    public interface IPluginSource: IPartImportsSatisfiedNotification
    {
        PluginLoader Loader { get; set; }
        IPlugin Plugin { get; }
        event EventHandler<PluginEventArgs> PluginChanged;
    }
}
