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

            var result = FileConverterFactory.GetConverter(testFileName);

            Assert.IsInstanceOfType(result, typeof(CSVFileConverter));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidFileException))]
        public void GetConverterFail()
        {
            string testFileName = "test.aaa";

            var result = FileConverterFactory.GetConverter(testFileName);

            Assert.Fail("InvalidFileException not thrown.");
        }
    }
}
