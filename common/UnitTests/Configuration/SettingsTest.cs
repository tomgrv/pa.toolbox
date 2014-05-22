using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnitTests.Configuration
{
    [TestClass]
    public class SettingsTest
    {
        [TestMethod]
        public void Structure()
        {
            using (Settings stg = new Settings(Process.GetCurrentProcess().ProcessName + ".ini", Settings.Format.IniFormat))
            {
                string[] keys = stg.AllKeys();
                Assert.AreEqual(24, keys.Length);
                Assert.AreEqual("TestSection/Param", keys[1]);

                string[] chd = stg.ChildKeys();
                Assert.AreEqual(0, chd.Length);

                string[] grp = stg.ChildGroups();
                Assert.AreEqual(4, grp.Length);
                Assert.AreEqual("TestLoader", grp[1]);

                stg.BeginGroup("TestSection");

                keys = stg.AllKeys();
                Assert.AreEqual(12, keys.Length);
                Assert.AreEqual("Param", keys[1]);

                chd = stg.ChildKeys();
                Assert.AreEqual(8, chd.Length);
                Assert.AreEqual("Param", keys[1]);

                grp = stg.ChildGroups();
                Assert.AreEqual(2, grp.Length);
                Assert.AreEqual("Many", grp[1]);

                int size = stg.BeginReadArray("Many");
                Assert.AreEqual(1, size);
                
                stg.SetArrayIndex(0);
                
                Assert.AreEqual("OKMANY1", stg.Value("Param"));
                stg.EndGroup();
            }

        }

        [TestMethod]
        public void Read()
        {
            using (Settings stg = new Settings(Process.GetCurrentProcess().ProcessName + ".ini", Settings.Format.IniFormat))
            {
                stg.BeginGroup("TestLoader");

                Assert.AreEqual("3", stg.Value("AnotherArray/size"));

                stg.EndGroup();
                stg.BeginGroup("TestSection");
                Assert.AreEqual("Faux", stg.Value("Param"));
                Assert.AreEqual("OKSINGLE", stg.Value("SingleConf/Param"));
            }
        }

        [TestMethod]
        public void ReadArray()
        {
            using (Settings stg = new Settings(Process.GetCurrentProcess().ProcessName + ".ini", Settings.Format.IniFormat))
            {
                stg.BeginGroup("TestLoader");
                stg.BeginReadArray("AnotherArray");
                stg.SetArrayIndex(1);
                Assert.AreEqual(">UnitTests.Plugin.PluginForSpecificArrayTest", stg.Value(""));
                stg.SetArrayIndex(0);
                Assert.AreEqual("OKMANY2", stg.Value("Param"));
            }
        }

        [TestMethod]
        public void Group()
        {
            using (Settings stg = new Settings(Process.GetCurrentProcess().ProcessName + ".ini", Settings.Format.IniFormat))
            {
                stg.BeginGroup("a");
                Assert.AreEqual("a", stg.Group);
                stg.BeginGroup("b");
                Assert.AreEqual("a/b", stg.Group);
                stg.BeginGroup("c");
                Assert.AreEqual("a/b/c", stg.Group);
                stg.EndGroup();
                Assert.AreEqual("a/b", stg.Group);
                stg.EndGroup();
                Assert.AreEqual("a", stg.Group);
                stg.EndGroup();
                Assert.AreEqual("", stg.Group);
            }
        }

    }
}
