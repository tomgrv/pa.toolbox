using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;
using PA.Plugin;
using PA.Components;
using PA.Configuration;
using System.ComponentModel;
using System.Collections.Generic;

namespace UnitTests.Plugin
{
    [TestClass]
    public class TestLoader : Component
    {
        [Import("TestSection/TestString")]
        private string ValueToTest { get; set; }

        [Import("TestSection/TestUrl")]
        private Uri UrlToTest { get; set; }

        [ImportMany("TestSection/Many")]
        public IEnumerable<Lazy<IPlugin, IPluginIndex>> ManyPluginToLoad1 { get; set; }

        [ImportMany("TestLoader/ManyPluginToLoad")]
        public IEnumerable<Lazy<IPlugin, IPluginIndex>> ManyPluginToLoad2 { get; set; }

        [ImportMany("TestSection/Many", typeof(PluginForGenericArrayTest))]
        public IEnumerable<Lazy<IPlugin, IPluginIndex>> ManyPluginToLoad3 { get; set; }

        [ImportMany("TestSection/Many", typeof(PluginForSpecificArrayTest))]
        public IEnumerable<Lazy<IPlugin, IPluginIndex>> ManyPluginToLoad4 { get; set; }

        [ImportMany]
        public IEnumerable<IPlugin> ManyPluginToLoad5 { get; set; }

        [ImportMany]
        public IEnumerable<PluginForSpecificImportTest> ManyPluginToLoad6 { get; set; }

        [Import("TestSection/Single")]
        public IPlugin GenericPluginToTest { get; set; }

        [Import("TestSection/Single")]
        public PluginForSpecificImportTest SpecificPluginToTest { get; set; }

        [Import("TestSection/SingleConf")]
        public IPlugin GenericArrayToTest { get; set; }

        [Import("TestSection/SingleConf")]
        public PluginForSpecificArrayTest SpecificArrayToTest { get; set; }

        [TestMethod]
        public void ConfigurationLoading()
        {
            PluginLoader loader = new PluginLoader();
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
