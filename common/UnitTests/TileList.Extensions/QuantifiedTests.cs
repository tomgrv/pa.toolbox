using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PA.TileList;
using PA.TileList.Quantified;
using PA.TileList.Contextual;

namespace UnitTests.TileList.Extensions
{
    [TestClass]
    public class QuantifiedTests
    {
        [TestMethod]
        public void AsQuantified()
        {
            ITile<ITile<UnitTests.TileList.TileTests.Item>> t2 = UnitTests.TileList.TileTests.GetTile();
            ITile<IContextual<UnitTests.TileList.TileTests.Item>> t3 = t2.Flatten<ITile<UnitTests.TileList.TileTests.Item>, UnitTests.TileList.TileTests.Item>();
            IQuantifiedTile<IContextual<UnitTests.TileList.TileTests.Item>> t4 = t3.AsQuantified(5, 5);

        }
    }
}


