using System;
using System.Linq;
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
            string[] array = input.AsArray().ParseTo<string, string>().ToArray();

            Assert.IsTrue(array.Length == 4 && array[0] == "a" && array[1] == "b" && array[2] == "c" && array[3] == "d");
        }

        [TestMethod]
        public void UriConvertion()
        {
            string input = "http://www.test.fr/app/index.php?q=valid";
            Uri url =input.ParseTo<Uri, string>();

            Assert.IsTrue(url.Authority == "www.test.fr");
        }
    }
}
