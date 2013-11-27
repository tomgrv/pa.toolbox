using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;
using PA.Plugin;
using PA.Components;
using PA.Configuration;
using System.ComponentModel;

namespace UnitTests.Plugin
{
    [TestClass]
    public class LoaderTest : Component
    {
        [Import("TestSection/TestString")]
        private string ValueToTest { get; set; }

        [Import("TestSection/TestUrl")]
        private Uri UrlToTest { get; set; }

        [TestMethod]
        public void LoaderConfiguration()
        {
            PluginLoader loader = new PluginLoader();
            loader.BeginInit();
            loader.Configuration = new IniConfigurationSource();
            loader.Parent = this;
            loader.EndInit();

            Assert.AreEqual("Correct", this.ValueToTest,"String");
            Assert.AreEqual(new Uri("http://www.google.fr", true), this.UrlToTest, "Url");
        }
    }
}
