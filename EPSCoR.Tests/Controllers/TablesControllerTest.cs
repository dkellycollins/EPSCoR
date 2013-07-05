using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
        private List<TableIndex> MockTableIndexes = new List<TableIndex>()
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

        private DataTable MockDataTable = new DataTable()
        {
            TableName = "TestTable",
        };

        private Mock<IModelRepository<TableIndex>> _tableIndexRepo;
        private Mock<IAsyncTableRepository> _tableRepo;
        private Mock<IAsyncDatabaseCalc> _dbCalc;

        [TestInitialize]
        public void TestInitialize()
        {
            _tableIndexRepo = new Mock<IModelRepository<TableIndex>>();
            _tableRepo = new Mock<IAsyncTableRepository>();
            _dbCalc = new Mock<IAsyncDatabaseCalc>();
        }

        #region IndexTests

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
            TableIndex expectedTableIndex = MockTableIndexes[1];

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
            TableIndex expectedTableIndex = MockTableIndexes[2];

            dynamic view = controller.Index();
            int actualLength = view.ViewBag.AttributeTables.Count;
            TableIndex actualTableIndex = view.ViewBag.UpstreamTables[0];

            Assert.AreEqual(expectedLength, actualLength);
            Assert.AreEqual(expectedTableIndex, actualTableIndex);
        }

        private TablesController indexSetup()
        {
            _tableIndexRepo.Setup(repo => repo.GetAll()).Returns(MockTableIndexes.AsQueryable());

            return new TablesController(_tableIndexRepo.Object, _tableRepo.Object, _dbCalc.Object);
        }

        #endregion IndexTests

        #region Details Tests

        [TestMethod]
        public void GetTableDetailsView()
        {
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.Setup(r => r.Headers).Returns(
                new System.Net.WebHeaderCollection {
                });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.Setup(c => c.Request).Returns(request.Object);
            _tableRepo.Setup(repo => repo.Read(MockDataTable.TableName)).Returns(MockDataTable);
            TablesController controller = new TablesController(_tableIndexRepo.Object, _tableRepo.Object, _dbCalc.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            dynamic result = controller.Details(MockDataTable.TableName);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(MockDataTable, result.Model);
        }

        [TestMethod]
        public void GetTableDetailsPartialView()
        {
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.Setup(r => r.Headers).Returns(
                new System.Net.WebHeaderCollection {
                    {"X-Requested-With", "XMLHttpRequest"}
                });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.Setup(c => c.Request).Returns(request.Object);
            _tableRepo.Setup(repo => repo.Read(MockDataTable.TableName)).Returns(MockDataTable);
            TablesController controller = new TablesController(_tableIndexRepo.Object, _tableRepo.Object, _dbCalc.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            dynamic result = controller.Details(MockDataTable.TableName);

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.AreEqual(MockDataTable, result.Model);
        }

        [TestMethod]
        public void GetTableDetailsJsonResult()
        {
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.Setup(r => r.Headers).Returns(
                new System.Net.WebHeaderCollection {
                    {"format", "json"}
                });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.Setup(c => c.Request).Returns(request.Object);
            _tableRepo.Setup(repo => repo.Read(MockDataTable.TableName)).Returns(MockDataTable);
            TablesController controller = new TablesController(_tableIndexRepo.Object, _tableRepo.Object, _dbCalc.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            dynamic result = controller.Details(MockDataTable.TableName);

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.AreEqual(MockDataTable, result.Data);
        }

        [TestMethod]
        public void GetNonexisitantTableDetailsView()
        {
            _tableRepo.Setup(repo => repo.Read(MockDataTable.TableName)).Returns(MockDataTable);
            TablesController controller = new TablesController(_tableIndexRepo.Object, _tableRepo.Object, _dbCalc.Object);

            var actual = controller.Details("X");

            Assert.IsInstanceOfType(actual, typeof(HttpNotFoundResult));
        }

        #endregion Details Tests

        #region Delete Tests

        [TestMethod]
        public void DeletePostSuccessTest()
        {
            TablesController controller = new TablesController(_tableIndexRepo.Object, _tableRepo.Object, _dbCalc.Object);

            var result = controller.Delete(MockDataTable.TableName);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void DeletePostFailTest()
        {
            TablesController controller = new TablesController(_tableIndexRepo.Object, _tableRepo.Object, _dbCalc.Object);

            var result = controller.Delete("");

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        #endregion Delete Tests

        [TestMethod]
        public void CalcTest()
        {
            Assert.Inconclusive();
        }
    }
}
