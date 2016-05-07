using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.IdentityServer3.Cassandra.DAO;
using P5.MSTest.Common;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class ClientStore4Test : TestBase
    {
        private IdentityServer3CassandraDao _store;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }
        [TestMethod]
        async Task TestCreateAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            Assert.IsFalse(true);
        }
    }
}
