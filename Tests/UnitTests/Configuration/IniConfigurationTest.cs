using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.Configuration;

namespace UnitTests.Configuration
{
    [TestClass]
    public class IniConfigurationTest
    {
      
        [TestMethod]
        public void ContainsSetting()
        {
            IniConfigurationSource src = new IniConfigurationSource();

            Assert.IsTrue(src.ContainsSetting("TestSection/TestString"));
            Assert.IsFalse(src.ContainsSetting("TestSection/CommentedItem"));
        }

        [TestMethod]
        public void Read()
        {
            IniConfigurationSource src = new IniConfigurationSource();

            Assert.AreEqual("Correct", src.GetSetting("TestSection/TestString"));
        }



        [TestMethod]
        public void Write()
        {
            IniConfigurationSource src = new IniConfigurationSource();

            string testString = DateTime.Now.Ticks.ToString();

            src.SetSetting("TestSection/Now", testString);

            Assert.AreEqual(testString, src.GetSetting("TestSection/Now"));
        }
    }
}
