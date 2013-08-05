using System;
using System.Web.Mvc;
using EPSCoR.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPSCoR.Tests.Controllers
{
    [TestClass]
    public class DataProcessorControllerTest
    {
        private DataProcessorController _controller;

        [TestInitialize]
        public void Initalize()
        {
            _controller = new DataProcessorController();
        }

        [TestMethod]
        public void GetIndex()
        {
            ActionResult result = _controller.Index();

            Assert.IsNotNull(result);
        }
    }
}
