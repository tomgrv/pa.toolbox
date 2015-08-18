﻿using System;
using System.Linq;
using System.ComponentModel.Composition;
using PA.Plugin;
using PA.Plugin.Extensions;
using PA.Plugin.Components;
using PA.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using PA.Plugin.Configuration;
using NUnit.Framework;
using PA.Plugin.Tests.Objects;

namespace PA.Plugin.Tests
{
    [TestFixtureAttribute]
    public class TestLoader : Component
    {
        [Import("TestSection/TestString", AllowRecomposition=true)]
        private string ValueToTest { get; set; }

        [Import("TestSection/TestUrl", AllowRecomposition = true)]
        private Uri UrlToTest { get; set; }

        [Import("TestSection/Date", AllowRecomposition = true)]
        private DateTime Date1 { get; set; }

        [Import("TestSection/Date", AllowRecomposition = true)]
        private ConfigurationItem<DateTime> Date2 { get; set; }

        [Import("TestSection/TestUrl", AllowRecomposition = true)]
        private ConfigurationItem<string> UrlItemToTest { get; set; }

        [Import("TestSection/TestUrl", AllowRecomposition = true)]
        public IPluginUri PluginForUri { get; set; }

        [Import("TestSection/TestUrls", AllowRecomposition = true)]
        public Uri[] Uris { get; set; }

        [Import("TestSection/TestUrls", AllowRecomposition = true)]
        public IPlugin<Uri>[] PluginForUris { get; set; }

        [ImportMany("TestSection/Many", AllowRecomposition = true)]
        public IEnumerable<Lazy<IPlugin, IPluginIndex>> ManyPluginToLoad1 { get; set; }

        [ImportMany("TestSection/Many", AllowRecomposition = true)]
        public IEnumerable<Lazy<IPlugin>> ManyPluginToLoad1bis { get; set; }


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

        [ImportMany("TestLoader/AnotherArray")]
        public IEnumerable<IPlugin> AnotherPluginToLoad1 { get; set; }

        [ImportMany("TestLoader/AnotherArray")]
        public IEnumerable<Lazy<IPlugin>> AnotherPluginToLoad2 { get; set; }


        //[ImportMany("TestLoader/AnotherArray")]
        //public IEnumerable<IPlugin> AnotherPluginToLoad1 { get; set; }

        [Test]
        public void Attributes()
        {
            Assert.AreEqual("DESC", PluginManager.GetAttribute<PluginDescriptionAttribute>(typeof(PluginForSpecificImportTest)).Description);

            using (PA.Plugin.Components.Controls.PluginLoader loader = new PA.Plugin.Components.Controls.PluginLoader())
            {
                var ics = new IniConfigurationSource();
                ics.BeginInit();
                ics.EndInit();

                loader.BeginInit();
                loader.Configuration = ics;
                loader.Parent = this;
                loader.EndInit();

                Assert.AreEqual("DESC", PluginManager.GetAttribute<PluginDescriptionAttribute>(this.SpecificPluginToTest).Description);
                Assert.AreEqual("DESC", this.SpecificPluginToTest.GetDescription());
            }
        }

        //[TestMethod]
        public void LoadingUnloading()
        {
            using (PA.Plugin.Components.Controls.PluginLoader loader = new PA.Plugin.Components.Controls.PluginLoader())
            {
                var ics = new IniConfigurationSource();
                ics.BeginInit();
                ics.EndInit();

                loader.BeginInit();
                loader.Configuration = ics;
                loader.Parent = this;
                loader.EndInit();

                Assert.AreEqual(1, this.ManyPluginToLoad2.Count(), "TestLoader/ManyPluginToLoad Plugin Loading");
                Assert.IsInstanceOf<IPlugin>(this.ManyPluginToLoad2.ElementAt(0).Value);

                loader.BeginEdit();

                Assert.AreEqual(1, this.ManyPluginToLoad2.Count(), "TestLoader/ManyPluginToLoad Plugin Loading");
                Assert.IsInstanceOf<IPlugin>(this.ManyPluginToLoad2.ElementAt(0).Value);

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

        [Test]
        public void ConfigurationLoading()
        {
            using (PA.Plugin.Components.Controls.PluginLoader loader = new PA.Plugin.Components.Controls.PluginLoader())
            {
                var ics = new IniConfigurationSource();
                ics.BeginInit();
                ics.EndInit();

                loader.BeginInit();
                loader.Configuration = ics;
                loader.Parent = this;
                loader.EndInit();

                Assert.AreEqual("Correct", this.ValueToTest, "String", "String Parsing");
                Assert.AreEqual(new Uri("http://www.google.fr"), this.UrlToTest, "Url Parsing");

                Assert.AreEqual(typeof(PluginForUriHttp), this.PluginForUri.GetType(), "Object selection & init");
                Assert.AreEqual(new Uri("http://www.google.fr"), this.PluginForUri.Value, "plugin selection Parsing");


                Assert.AreEqual(1, this.ManyPluginToLoad1.Count(), "TestSection/Many Plugin Loading");
                Assert.IsInstanceOf<PluginForSpecificArrayTest>(this.ManyPluginToLoad1.ElementAt(0).Value);
                Assert.AreEqual("OKMANY1", (this.ManyPluginToLoad1.ElementAt(0).Value as PluginForSpecificArrayTest).Parameter);

                Assert.AreEqual(1, this.ManyPluginToLoad2.Count(), "TestLoader/ManyPluginToLoad Plugin Loading");
                Assert.IsInstanceOf<IPlugin>(this.ManyPluginToLoad2.ElementAt(0).Value);

                Assert.AreEqual(0, this.ManyPluginToLoad3.Count(), "TestSection/Many {PluginForGenericArrayTest} Plugin Loading");

                Assert.AreEqual(1, this.ManyPluginToLoad4.Count(), "TestSection/Many {PluginForSpecificArrayTest} Plugin Loading");
                Assert.IsInstanceOf<PluginForSpecificArrayTest>(this.ManyPluginToLoad4.ElementAt(0).Value);
                Assert.AreEqual("OKMANY1", (this.ManyPluginToLoad4.ElementAt(0).Value as PluginForSpecificArrayTest).Parameter);

                Assert.AreEqual(2, this.ManyPluginToLoad5.Count(), "Default Plugin Loading with Interface");
                Assert.IsInstanceOf<IPlugin>(this.ManyPluginToLoad5.ElementAt(0));

                Assert.AreEqual(1, this.ManyPluginToLoad6.Count(), "Default Plugin Loading with Class");
                Assert.IsInstanceOf<PluginForSpecificImportTest>(this.ManyPluginToLoad6.ElementAt(0));
                Assert.AreEqual(3, this.ManyPluginToLoad6.ElementAt(0).Parameter.Length);

                Assert.IsInstanceOf<PluginForSpecificImportTest>(this.GenericPluginToTest);
                Assert.IsInstanceOf<PluginForSpecificImportTest>(this.SpecificPluginToTest);
                Assert.IsInstanceOf<IPlugin>(this.GenericArrayToTest);

                Assert.IsInstanceOf<PluginForSpecificArrayTest>(this.SpecificArrayToTest);
                Assert.AreEqual("OKSINGLE", this.SpecificArrayToTest.Parameter);
            }
        }
    }
}
