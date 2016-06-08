using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlattenedDocument.CassandraStore.DAO;
using FlattenedDocument.CassandraStore.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.Store.Core.Models;

namespace FlattenedDocument.CassandraStore.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class CRUDTest
    {
        List<IDocumentRecord> BuidInsertRecords(int count = 1, int versionStart = 0)
        {
            List<IDocumentRecord> result = new List<IDocumentRecord>();
            var myVersion = versionStart;
            for (int i = 0; i < count; ++i, ++myVersion)
            {
                var myPocoV1 = new Models.V1.MyPoco()
                {
                    Name = "PocoName_V1_" + i
                };
                myPocoV1.Services.Add("Service:" + i, "Service:" + i);
                var record = new Models.V1.MyPocoDocumentRecord(new Models.V1.MyPocoDocument(myPocoV1))
                {
                    DocumentVersion = string.Format("1.{0}", myVersion)
                };
                result.Add(record);
            }
            myVersion = versionStart;
            for (int i = 0; i < count; ++i,++myVersion)
            {
                var myPocoV2 = new Models.V2.MyPoco()
                {
                    Name = "PocoName_V2_" + i,
                    City = "PocoCity_V2_" + i
                };
                myPocoV2.Services.Add("Service:" + i, "Service:" + i);
                var record = new Models.V2.MyPocoDocumentRecord(new Models.V2.MyPocoDocument(myPocoV2))
                {
                    DocumentVersion = string.Format("1.{0}", myVersion)
                };
                result.Add(record);
            }
            return result;
        }

        private FlattenedDocumentStore FlattenedDocumentStore { get; set; }

        private async Task CreateAndTruncateTables()
        {
            var dao = new FlattenedDocumentCassandraDao();
            await dao.EstablishConnectionAsync();
            await dao.CreateFlattenedDocumentTablesAsync();
            await dao.TruncateTablesAsync();
        }
        [TestInitialize]
        public void Setup()
        {
            FlattenedDocumentStore = new FlattenedDocumentStore();
        }
        [TestMethod]
        public async Task Test_Find_NoItemByIdAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();

            var q = await FlattenedDocumentStore.GetByIdAsync(Guid.NewGuid());
            Assert.IsNull(q);
        }
        [TestMethod]
        public async Task Test_Find_NoItemsByTypeAndVersionAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();

            var q = await FlattenedDocumentStore.GetByTypeAndVersionAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Assert.IsNull(q);
        }
        [TestMethod]
        public async Task Test_Find_NoItemsByTypeAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();

            var q = await FlattenedDocumentStore.GetAllByTypeAsync(Guid.NewGuid().ToString());
            Assert.IsTrue(!q.Any());
        }
        [TestMethod]
        public async Task TestCreateAndFindByIdAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();
            var toBeInserted = BuidInsertRecords(10);
            await FlattenedDocumentStore.StoreManyAsync(toBeInserted);


            foreach (var insert in toBeInserted)
            {
                var q = await FlattenedDocumentStore.GetByIdAsync(insert.Id);
                Assert.AreEqual(q.DocumentType, insert.DocumentType);
                Assert.AreEqual(q.DocumentVersion, insert.DocumentVersion);
                Assert.AreEqual(q.DocumentJson, insert.DocumentJson);
            }
        }
        [TestMethod]
        public async Task TestCreateAndFindByTypeAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();
            var nCount = 10;
            var toBeInserted = BuidInsertRecords(nCount);
            await FlattenedDocumentStore.StoreManyAsync(toBeInserted);

            foreach (var insert in toBeInserted)
            {
                var q = await FlattenedDocumentStore.GetByIdAsync(insert.Id);
                Assert.AreEqual(q.DocumentType, insert.DocumentType);
                Assert.AreEqual(q.DocumentVersion, insert.DocumentVersion);
                Assert.AreEqual(q.DocumentJson, insert.DocumentJson);
            }
            var item = toBeInserted[0];
            var items = await FlattenedDocumentStore.GetAllByTypeAsync(item.DocumentType);
            Assert.AreEqual(items.Count(), nCount);
        }
        [TestMethod]
        public async Task TestCreateAndPageByTypeAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();
            List<IDocumentRecord> masterInsertList = new List<IDocumentRecord>();
            int recordsPerRun = 9;// to give us an uneven number
            int numberOfRuns = 11;
            for (int i = 0; i < numberOfRuns; ++i)
            {
                var toBeInserted = BuidInsertRecords(recordsPerRun, i * recordsPerRun);
                await FlattenedDocumentStore.StoreManyAsync(toBeInserted);
                masterInsertList.AddRange(toBeInserted);
            }

            foreach (var insert in masterInsertList)
            {
                var q = await FlattenedDocumentStore.GetByIdAsync(insert.Id);
                Assert.AreEqual(q.DocumentType, insert.DocumentType);
                Assert.AreEqual(q.DocumentVersion, insert.DocumentVersion);
                Assert.AreEqual(q.DocumentJson, insert.DocumentJson);
            }
            var item = masterInsertList[0];
            int pagesize = 10;
            byte[] pagingState = null;
            int runningCount = 0;
            do
            {
                var items = await FlattenedDocumentStore.PageGetByTypeAsync(
                    item.DocumentType, pagesize, pagingState);
                pagingState = items.PagingState;
                runningCount += items.Count();
            } while (pagingState != null);
            Assert.AreEqual(numberOfRuns*recordsPerRun, runningCount);

        }
        [TestMethod]
        public async Task TestCreateAndPageAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();
            List<IDocumentRecord> masterInsertList = new List<IDocumentRecord>();
            int recordsPerRun = 9;// to give us an uneven number
            int numberOfRuns = 11;
            for (int i = 0; i < numberOfRuns; ++i)
            {
                var toBeInserted = BuidInsertRecords(recordsPerRun, i * recordsPerRun);
                await FlattenedDocumentStore.StoreManyAsync(toBeInserted);
                masterInsertList.AddRange(toBeInserted);
            }

            foreach (var insert in masterInsertList)
            {
                var q = await FlattenedDocumentStore.GetByIdAsync(insert.Id);
                Assert.AreEqual(q.DocumentType, insert.DocumentType);
                Assert.AreEqual(q.DocumentVersion, insert.DocumentVersion);
                Assert.AreEqual(q.DocumentJson, insert.DocumentJson);
            }
            var item = masterInsertList[0];
            int pagesize = 10;
            byte[] pagingState = null;
            int runningCount = 0;
            do
            {
                var items = await FlattenedDocumentStore.PageGetAsync(
                     pagesize, pagingState);
                pagingState = items.PagingState;
                runningCount += items.Count();
            } while (pagingState != null);
            Assert.AreEqual(masterInsertList.Count, runningCount);

        }
        [TestMethod]
        public async Task TestCreateAndFindByTypeAndVersionAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();
            var nCount = 10;
            var toBeInserted = BuidInsertRecords(nCount);
            await FlattenedDocumentStore.StoreManyAsync(toBeInserted);

            foreach (var insert in toBeInserted)
            {
                var q = await FlattenedDocumentStore.GetByIdAsync(insert.Id);
                Assert.AreEqual(q.DocumentType, insert.DocumentType);
                Assert.AreEqual(q.DocumentVersion, insert.DocumentVersion);
                Assert.AreEqual(q.DocumentJson, insert.DocumentJson);
            }
            var item = toBeInserted[0];
            var result = await FlattenedDocumentStore.GetByTypeAndVersionAsync(item.DocumentType, item.DocumentVersion);
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task TestCreateAndDeleteByIdAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();
            var toBeInserted = BuidInsertRecords(10);
            await FlattenedDocumentStore.StoreManyAsync(toBeInserted);

            foreach (var insert in toBeInserted)
            {
                var q = await FlattenedDocumentStore.GetByIdAsync(insert.Id);
                Assert.AreEqual(q.DocumentType, insert.DocumentType);
                Assert.AreEqual(q.DocumentVersion, insert.DocumentVersion);
                Assert.AreEqual(q.DocumentJson, insert.DocumentJson);
            }

            foreach (var insert in toBeInserted)
            {
                await FlattenedDocumentStore.RemoveByIdAsync(insert.Id);
                var q = await FlattenedDocumentStore.GetByIdAsync(insert.Id);
                Assert.IsNull(q);
            }

        }
        [TestMethod]
        public async Task TestCreateAndDeleteByTypeAndVersionAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();
            var toBeInserted = BuidInsertRecords(10);
            await FlattenedDocumentStore.StoreManyAsync(toBeInserted);

            foreach (var insert in toBeInserted)
            {
                var q = await FlattenedDocumentStore.GetByIdAsync(insert.Id);
                Assert.AreEqual(q.DocumentType, insert.DocumentType);
                Assert.AreEqual(q.DocumentVersion, insert.DocumentVersion);
                Assert.AreEqual(q.DocumentJson, insert.DocumentJson);
            }

            foreach (var insert in toBeInserted)
            {
                await FlattenedDocumentStore.RemoveByTypeAndVersionAsync(insert.DocumentType, insert.DocumentVersion);
                var q = await FlattenedDocumentStore.GetByIdAsync(insert.Id);
                Assert.IsNull(q);
            }

        }
        [TestMethod]
        public async Task TestCreateAndDeleteByTypeAsync()
        {
            // create Tables, in case they don't exist
            await CreateAndTruncateTables();
            var toBeInserted = BuidInsertRecords(10);
            await FlattenedDocumentStore.StoreManyAsync(toBeInserted);

            foreach (var insert in toBeInserted)
            {
                var q = await FlattenedDocumentStore.GetByIdAsync(insert.Id);
                Assert.AreEqual(q.DocumentType, insert.DocumentType);
                Assert.AreEqual(q.DocumentVersion, insert.DocumentVersion);
                Assert.AreEqual(q.DocumentJson, insert.DocumentJson);
            }
            var item = toBeInserted[0];
            await FlattenedDocumentStore.RemoveAllByTypeAsync(item.DocumentType);
            var items = await FlattenedDocumentStore.GetAllByTypeAsync(item.DocumentType);
            Assert.IsFalse(items.Any());


        }
    }
}
