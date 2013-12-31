using System.ComponentModel.Composition;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using PA.Plugin.Operations.Core;
using System.Threading.Tasks;


namespace PA.Plugin.Operations
{
    [InheritedExport]
    public interface IJobPlugin : IPlugin
    {
        Task<DataMap> Execute(Context context);
        Task<bool> CanExecute(Context context);
    }
}
