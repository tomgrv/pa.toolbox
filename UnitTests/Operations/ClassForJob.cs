using PA.Plugin.Operations;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.Operations
{
    class ClassForJob: IJobPlugin
    {
    
        void IDisposable.Dispose()
        {
           
        }

        void IJobPlugin.Execute(PA.Plugin.Operations.Core.Context context)
        {
            context.Result.Add("Text","JOB_DONE");
        }

        bool IJobPlugin.CanExecute(PA.Plugin.Operations.Core.Context data)
        {
            return (string) data.Data.First().Value == "JOB_TEST";
        }
    }
}
