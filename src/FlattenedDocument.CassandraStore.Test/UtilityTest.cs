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
            var dao = new FlattenedDocumentCassandraDao();
            await dao.EstablishConnectionAsync();

            // create Tables, in case they don't exist
            await dao.CreateFlattenedDocumentTablesAsync();
            await dao.TruncateTablesAsync();
        }
        [TestMethod]
        public async Task TestCreateTables()
        {
            var dao = new FlattenedDocumentCassandraDao();
            await dao.EstablishConnectionAsync();
            await dao.CreateFlattenedDocumentTablesAsync();
        }
    }
}
