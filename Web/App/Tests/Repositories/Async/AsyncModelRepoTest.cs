using System;
using System.Collections.Generic;
using EPSCoR.Database;
using EPSCoR.Database.Context;
using EPSCoR.Database.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using EPSCoR.Tests.Repositories.Basic;
using EPSCoR.Repositories.Basic;
using EPSCoR.Repositories.Async;

namespace EPSCoR.Tests.Repositories
{
    [TestClass]
    public class AsyncModelRepoTest
    {
        private static readonly List<TableIndex> TEST_DATA = new List<TableIndex>()
        {
            new TableIndex()
            {
                ID = 1,
            },
            new TableIndex()
            {
                ID = 2,
            },
            new TableIndex()
            {
                ID = 3,
            }
        };

        private AsyncModelRepo<TableIndex> repo;

        [TestInitialize]
        public void TestInitalize()
        {
            Mock<ModelDbContext> mockModelContext = new Mock<ModelDbContext>();

            mockModelContext.Setup(context => context.GetAllModels<TableIndex>())
                .Returns(TEST_DATA.AsQueryable());
            mockModelContext.Setup(context => context.GetModel<TableIndex>(1))
                .Returns(TEST_DATA[0]);
            mockModelContext.Setup(context => context.GetModel<TableIndex>(2))
                .Returns(TEST_DATA[1]);
            mockModelContext.Setup(context => context.GetModel<TableIndex>(3))
                .Returns(TEST_DATA[2]);

            repo = new AsyncModelRepo<TableIndex>(mockModelContext.Object);
        }

        [TestMethod]
        public async void GetSucessfulTest()
        {
            TableIndex expected = TEST_DATA[0];
            TableIndex actual = await repo.GetAsync(1);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async void GetFailTest()
        {
            TableIndex result = await repo.GetAsync(-1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async void GetAllTest()
        {
            int expectedLegnth = 3;
            IEnumerable<TableIndex> actual = await repo.GetAllAsync();

            Assert.AreEqual(expectedLegnth, actual.Count());
        }

        [TestMethod]
        public void CreateTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void UpdateTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void RemoveTest()
        {
            Assert.Inconclusive();
        }
    }
}
