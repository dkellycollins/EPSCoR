using System;
using EPSCoR.Database.Services.FileConverter;
using EPSCoR.Database.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPSCoR.Database.Test.Services.FileConverter
{
    [TestClass]
    public class FileConverterFactoryTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            
        }

        [TestMethod]
        public void GetCsvConverter()
        {
            string testFileName = "test.csv";
            string testUserName = "tester";

            var result = FileConverterFactory.GetConverter(testFileName, testUserName);

            Assert.IsInstanceOfType(result, typeof(CSVFileConverter));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFileException))]
        public void GetConverterFail()
        {
            string testFileName = "test.aaa";
            string testUserName = "tester";

            var result = FileConverterFactory.GetConverter(testFileName, testUserName);

            Assert.Fail("InvalidFileException not thrown.");
        }
    }
}
