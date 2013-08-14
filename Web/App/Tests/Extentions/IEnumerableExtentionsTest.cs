using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPSCoR.Extentions;

namespace EPSCoR.Tests.Extentions
{
    [TestClass]
    public class IEnumerableExtentionsTest
    {
        private static readonly IEnumerable<string> TEST_DATA = new List<string>()
        {
            "testValue1",
            "testValue2",
            "testValue3"
        };

        [TestMethod]
        public void ToCommaSeparatedStringWithoutFormatTest()
        {
            string expected = "testValue1, testValue2, testValue3";
            string actual = TEST_DATA.ToCommaSeparatedString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToCommaSeparatedStringWithFormatTest()
        {
            string expected = "(testValue1), (testValue2), (testValue3)";
            string actual = TEST_DATA.ToCommaSeparatedString("({0})");

            Assert.AreEqual(expected, actual);
        }
    }
}
