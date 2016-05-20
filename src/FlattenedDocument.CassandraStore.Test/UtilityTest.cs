using System;
using System.Threading.Tasks;
using FlattenedDocument.CassandraStore.DAO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlattenedDocument.CassandraStore.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class UtilityTest
    {
        [TestMethod]
        public async Task TestTruncateTables()
        {
            // create Tables, in case they don't exist
            await FlattenedDocumentCassandraDao.CreateFlattenedDocumentTablesAsync();
            await FlattenedDocumentCassandraDao.TruncateTablesAsync();
        }
        [TestMethod]
        public async Task TestCreateTables()
        {
            await FlattenedDocumentCassandraDao.CreateFlattenedDocumentTablesAsync();
        }
    }
}
