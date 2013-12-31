using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PA.Plugin;

namespace UnitTests.Operations
{
    [TestClass]
    public class TestRunner
    {
        [TestMethod]
        public void TestJob()
        {
            using (PA.Plugin.Operations.Controls.PluginRunner runner = new PA.Plugin.Operations.Controls.PluginRunner())
            {
                runner.RunOnce(new ClassForJob(), new Dictionary<string, object>() { { "name", "JOB_TEST" } });
            }
        }

    }
}
