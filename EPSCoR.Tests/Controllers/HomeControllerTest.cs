using System;
using EPSCoR.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPSCoR.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private HomeController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _controller = new HomeController();
        }

        [TestMethod]
        public void IndexTest()
        {
            var result = _controller.Index();

            Assert.IsNotNull(result);
        }
    }
}
