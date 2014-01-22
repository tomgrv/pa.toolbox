
using System.ComponentModel;
using System;
using System.ComponentModel.Composition;

namespace PA.Plugin.Operations.Interfaces
{
    [InheritedExport]
    public interface IPluginOperation : IPlugin
    {
        [Obsolete]
        bool CanExecute<T>(object obj)
            where T : IPluginOperation;
        [Obsolete]
        object Execute<T>(object obj)
            where T : IPluginOperation;

        bool CanExecute(params object[] arguments);
        object Execute(params object[] arguments);
    }
}
