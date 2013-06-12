using System;
using EPSCoR.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPSCoR.Tests.Controllers
{
    [TestClass]
    public class AboutControllerTest
    {
        private AboutController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _controller = new AboutController();
        }

        [TestMethod]
        public void IndexTest()
        {
            var result = _controller.Index();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HowToExportTest()
        {
            var result = _controller.HowToExport();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HowToUpload()
        {
            var result = _controller.HowToUpload();

            Assert.IsNotNull(result);
        }
    }
}
