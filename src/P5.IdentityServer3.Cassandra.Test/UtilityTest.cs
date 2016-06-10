using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
 
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.MSTest.Common;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class UtilityTest : TestBase
    {

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public async Task TestUpdateAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await dao.FindClientByIdAsync(insert[0].Id);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            await dao.TruncateTablesAsync();
            result = await dao.FindClientByIdAsync(insert[0].Id);
            Assert.IsNull(result);

        }
        [TestMethod]
        public async Task Test_CreateTablesAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();
            await dao.CreateTablesAsync();
        }
        [TestMethod]
        public async Task Test_TruncateTablesAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();
            await dao.TruncateTablesAsync();
        }

        
    }
}
