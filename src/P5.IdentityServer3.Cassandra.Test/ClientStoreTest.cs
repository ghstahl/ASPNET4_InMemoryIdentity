using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;


using P5.MSTest.Common;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class ClientStoreTest : TestBase
    {
        private IdentityServer3CassandraDao _store;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }


        [TestMethod]
        public async Task TestCreateClientAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(insert[0].Record.ClientName,result.ClientName);
        }

    }
}
