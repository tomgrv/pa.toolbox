using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace PA.Plugin.Components.Interfaces
{
    [InheritedExport]
    public interface IActionPlugin : IPlugin
    {
        bool CanExecute(IDictionary<string, object> data);
        object Execute(IDictionary<string, object> data);
    }
}
