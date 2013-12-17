using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;
using PA.Plugin;
using PA.Plugin.Extensions;
using PA.Plugin.Components;
using PA.Configuration;
using System.ComponentModel;
using System.Collections.Generic;

namespace UnitTests.Plugin
{
    [TestClass]
    public class TestLoader : Component
    {
        [Import("TestSection/TestString", AllowRecomposition=true)]
        private string ValueToTest { get; set; }

        [Import("TestSection/TestUrl", AllowRecomposition = true)]
        private Uri UrlToTest { get; set; }

        [ImportMany("TestSection/Many", AllowRecomposition = true)]
        public IEnumerable<Lazy<IPlugin, IPluginIndex>> ManyPluginToLoad1 { get; set; }

        [ImportMany("TestLoader/ManyPluginToLoad", AllowRecomposition = true)]
        public IEnumerable<Lazy<IPlugin, IPluginIndex>> ManyPluginToLoad2 { get; set; }

        [ImportMany("TestSection/Many", typeof(PluginForGenericArrayTest), AllowRecomposition = true)]
        public IEnumerable<Lazy<IPlugin, IPluginIndex>> ManyPluginToLoad3 { get; set; }

        [ImportMany("TestSection/Many", typeof(PluginForSpecificArrayTest), AllowRecomposition = true)]
        public IEnumerable<Lazy<IPlugin, IPluginIndex>> ManyPluginToLoad4 { get; set; }

        [ImportMany(AllowRecomposition=true)]
        public IEnumerable<IPlugin> ManyPluginToLoad5 { get; set; }

        [ImportMany( AllowRecomposition=true)]
        public IEnumerable<PluginForSpecificImportTest> ManyPluginToLoad6 { get; set; }

        [Import("TestSection/Single", AllowRecomposition = true)]
        public IPlugin GenericPluginToTest { get; set; }

        [Import("TestSection/Single", AllowRecomposition = true)]
        public PluginForSpecificImportTest SpecificPluginToTest { get; set; }

        [Import("TestSection/SingleConf", AllowRecomposition = true)]
        public IPlugin GenericArrayToTest { get; set; }

        [Import("TestSection/SingleConf", AllowRecomposition = true)]
        public PluginForSpecificArrayTest SpecificArrayToTest { get; set; }

        [TestMethod]
        public void Attributes()
        {
            Assert.AreEqual("DESC", PluginManager.GetAttribute<PluginDescriptionAttribute>(typeof(PluginForSpecificImportTest)).Description);

            using (PA.Plugin.Components.Controls.PluginLoader loader = new PA.Plugin.Components.Controls.PluginLoader())
            {
                loader.BeginInit();
                loader.Configuration = new IniConfigurationSource();
                loader.Parent = this;
                loader.EndInit();

                Assert.AreEqual("DESC", PluginManager.GetAttribute<PluginDescriptionAttribute>(this.SpecificPluginToTest).Description);
                Assert.AreEqual("DESC", this.SpecificPluginToTest.GetDescription());
            }
        }

        [TestMethod]
        public void LoadingUnloading()
        {
            using (PA.Plugin.Components.Controls.PluginLoader loader = new PA.Plugin.Components.Controls.PluginLoader())
            {
                loader.BeginInit();
                loader.Configuration = new IniConfigurationSource();
                loader.Parent = this;
                loader.EndInit();

                Assert.AreEqual(1, this.ManyPluginToLoad2.Count(), "TestLoader/ManyPluginToLoad Plugin Loading");
                Assert.IsInstanceOfType(this.ManyPluginToLoad2.ElementAt(0).Value, typeof(IPlugin));

                loader.BeginEdit();

                Assert.AreEqual(1, this.ManyPluginToLoad2.Count(), "TestLoader/ManyPluginToLoad Plugin Loading");
                Assert.IsInstanceOfType(this.ManyPluginToLoad2.ElementAt(0).Value, typeof(IPlugin));

                loader.Location = @"c:\";

                try
                {
                    loader.EndEdit();
                    Assert.Fail();
                }
                catch
                {
                    
                    //Assert.AreEqual(1, this.ManyPluginToLoad2.Count(), "TestLoader/ManyPluginToLoad Plugin Loading");
                }

                loader.Location = null;
                loader.EndEdit();
            }
        }

        [TestMethod]
        public void ConfigurationLoading()
        {
            using (PA.Plugin.Components.Controls.PluginLoader loader = new PA.Plugin.Components.Controls.PluginLoader())
            {
                loader.BeginInit();
                loader.Configuration = new IniConfigurationSource();
                loader.Parent = this;
                loader.EndInit();

                Assert.AreEqual("Correct", this.ValueToTest, "String", "String Parsing");
                Assert.AreEqual(new Uri("http://www.google.fr", true), this.UrlToTest, "Url Parsing");

                Assert.AreEqual(1, this.ManyPluginToLoad1.Count(), "TestSection/Many Plugin Loading");
                Assert.IsInstanceOfType(this.ManyPluginToLoad1.ElementAt(0).Value, typeof(PluginForSpecificArrayTest));
                Assert.AreEqual("OKMANY1", (this.ManyPluginToLoad1.ElementAt(0).Value as PluginForSpecificArrayTest).Parameter);

                Assert.AreEqual(1, this.ManyPluginToLoad2.Count(), "TestLoader/ManyPluginToLoad Plugin Loading");
                Assert.IsInstanceOfType(this.ManyPluginToLoad2.ElementAt(0).Value, typeof(IPlugin));

                Assert.AreEqual(0, this.ManyPluginToLoad3.Count(), "TestSection/Many {PluginForGenericArrayTest} Plugin Loading");

                Assert.AreEqual(1, this.ManyPluginToLoad4.Count(), "TestSection/Many {PluginForSpecificArrayTest} Plugin Loading");
                Assert.IsInstanceOfType(this.ManyPluginToLoad4.ElementAt(0).Value, typeof(PluginForSpecificArrayTest));
                Assert.AreEqual("OKMANY1", (this.ManyPluginToLoad4.ElementAt(0).Value as PluginForSpecificArrayTest).Parameter);

                Assert.AreEqual(2, this.ManyPluginToLoad5.Count(), "Default Plugin Loading with Interface");
                Assert.IsInstanceOfType(this.ManyPluginToLoad5.ElementAt(0), typeof(IPlugin));

                Assert.AreEqual(1, this.ManyPluginToLoad6.Count(), "Default Plugin Loading with Class");
                Assert.IsInstanceOfType(this.ManyPluginToLoad6.ElementAt(0), typeof(PluginForSpecificImportTest));
                Assert.AreEqual("OKIMPORT", this.ManyPluginToLoad6.ElementAt(0).Parameter);

                Assert.IsInstanceOfType(this.GenericPluginToTest, typeof(PluginForSpecificImportTest));
                Assert.IsInstanceOfType(this.SpecificPluginToTest, typeof(PluginForSpecificImportTest));
                Assert.IsInstanceOfType(this.GenericArrayToTest, typeof(IPlugin));

                Assert.IsInstanceOfType(this.SpecificArrayToTest, typeof(PluginForSpecificArrayTest));
                Assert.AreEqual("OKSINGLE", this.SpecificArrayToTest.Parameter);
            }
        }
    }
}
