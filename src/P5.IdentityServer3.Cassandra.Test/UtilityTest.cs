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
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            await IdentityServer3CassandraDao.TruncateTablesAsync();
            result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.IsNull(result);

        }
        [TestMethod]
        public async Task Test_CreateTablesAsync()
        {
            await IdentityServer3CassandraDao.CreateTablesAsync();
        }
        [TestMethod]
        public async Task Test_TruncateTablesAsync()
        {
            await IdentityServer3CassandraDao.TruncateTablesAsync();
        }

        
    }
}
