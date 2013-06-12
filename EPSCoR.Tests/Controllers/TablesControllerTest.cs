using System;
using System.Collections.Generic;
using System.Linq;
using EPSCoR.Controllers;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPSCoR.Tests.Controllers
{
    [TestClass]
    public class TablesControllerTest
    {
        private List<TableIndex> MockData = new List<TableIndex>()
        {
            new TableIndex()
            {
                ID = 1,
                Processed = true,
                Type = TableTypes.CALC
            },
            new TableIndex()
            {
                ID = 2,
                Processed = true,
                Type = TableTypes.ATTRIBUTE
            },
            new TableIndex()
            {
                ID = 3,
                Processed = true,
                Type = TableTypes.UPSTREAM
            },
            new TableIndex()
            {
                ID = 4,
                Processed = false,
                Type = TableTypes.CALC
            },
            new TableIndex()
            {
                ID = 5,
                Processed = false,
                Type = TableTypes.ATTRIBUTE
            },
            new TableIndex()
            {
                ID = 6,
                Processed = false,
                Type = TableTypes.UPSTREAM
            },
        };

        private Mock<IModelRepository<TableIndex>> _tableIndexRepo;
        private Mock<ITableRepository> _tableRepo;
        private Mock<IDatabaseCalc> _dbCalc;

        [TestInitialize]
        public void TestInitialize()
        {
            _tableIndexRepo = new Mock<IModelRepository<TableIndex>>();
            _tableRepo = new Mock<ITableRepository>();
            _dbCalc = new Mock<IDatabaseCalc>();
        }

        [TestMethod]
        public void GetAllProcessedTables()
        {
            TablesController controller = indexSetup();
            int expected = 3;

            dynamic view = controller.Index();
            int result = view.ViewBag.AllTables.Count;

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetAttributeTables()
        {
            TablesController controller = indexSetup();
            int expectedLength = 1;
            TableIndex expectedTableIndex = MockData[1];

            dynamic view = controller.Index();
            int actualLength = view.ViewBag.AttributeTables.Count;
            TableIndex actualTableIndex = view.ViewBag.AttributeTables[0];

            Assert.AreEqual(expectedLength, actualLength);
            Assert.AreEqual(expectedTableIndex, actualTableIndex);
        }

        [TestMethod]
        public void GetUpstreamTables()
        {
            TablesController controller = indexSetup();
            int expectedLength = 1;
            TableIndex expectedTableIndex = MockData[2];

            dynamic view = controller.Index();
            int actualLength = view.ViewBag.AttributeTables.Count;
            TableIndex actualTableIndex = view.ViewBag.UpstreamTables[0];

            Assert.AreEqual(expectedLength, actualLength);
            Assert.AreEqual(expectedTableIndex, actualTableIndex);
        }

        private TablesController indexSetup()
        {
            _tableIndexRepo.Setup(repo => repo.GetAll()).Returns(MockData.AsQueryable());

            return new TablesController(_tableIndexRepo.Object, _tableRepo.Object, _dbCalc.Object);
        }

        [TestMethod]
        public void DetailsSuccessTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DetailsFailTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DeleteGetTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DeletePostSuccessTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DeletePostFailTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CalcTest()
        {
            Assert.Inconclusive();
        }
    }
}
