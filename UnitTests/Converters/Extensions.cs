using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.Converters.Extensions;

namespace UnitTests.Converters
{
    [TestClass]
    public class Extensions
    {
        [TestMethod]
        public void ArrayConvertion()
        {
            string input = ":a:b:c:d";
            string[] array = (string[])input.ParseTo(typeof(string[]));

            Assert.IsTrue(array.Length == 4 && array[0] == "a" && array[1] == "b" && array[2] == "c" && array[3] == "d");
        }

        [TestMethod]
        public void UriConvertion()
        {
            string input = "http://www.test.fr/app/index.php?q=valid";
            Uri url = (Uri)input.ParseTo(typeof(Uri));

            Assert.IsTrue(url.Authority == "www.test.fr");
        }

     
    }
}
