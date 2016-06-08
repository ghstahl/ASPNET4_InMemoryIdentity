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
    public class RefreshTokenHandleStoreTest : TestBase
    {
        private IdentityServer3CassandraDao _store;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public async Task TestGetAllAsync()
        {

            IClientStore cs = new ClientStore();
            var insert = await CassandraTestHelper.InsertTestData_RefreshTokens(cs, 10);

            IRefreshTokenStore ths = new RefreshTokenHandleStore();
            var subjectId = insert[0].SubjectId;
            var clientId = insert[0].ClientId;

            var find_metadata = await ths.GetAllAsync(subjectId);
            Assert.AreEqual(find_metadata.Count(), insert.Count);

            await ths.RevokeAsync(subjectId, clientId);
            find_metadata = await ths.GetAllAsync(subjectId);

            Assert.AreEqual(find_metadata.Count(), 0);

        }

        [TestMethod]
        public async Task TestRemoveAsync()
        {
            IClientStore cs = new ClientStore();
            var insert = await CassandraTestHelper.InsertTestData_RefreshTokens(cs, 1);

            IRefreshTokenStore ths = new RefreshTokenHandleStore();
            var subjectId = insert[0].SubjectId;
            var clientId = insert[0].ClientId;
            var key = insert[0].Key;
            var find_metadata = await ths.GetAllAsync(subjectId);
            Assert.AreEqual(find_metadata.Count(), insert.Count);

            await ths.RemoveAsync(key);
            find_metadata = await ths.GetAllAsync(subjectId);

            Assert.AreEqual(find_metadata.Count(), 0);

        }

        [TestMethod]
        public async Task TestRevokeAsync()
        {
            IClientStore cs = new ClientStore();
            var insert = await CassandraTestHelper.InsertTestData_RefreshTokens(cs,10);

            IRefreshTokenStore ths = new RefreshTokenHandleStore();
            var subjectId = insert[0].SubjectId;
            var clientId = insert[0].ClientId;

            var find_metadata = await ths.GetAllAsync(subjectId);
            Assert.AreEqual(find_metadata.Count(), insert.Count);

            await ths.RevokeAsync(subjectId, clientId);
            find_metadata = await ths.GetAllAsync(subjectId);

            Assert.AreEqual(find_metadata.Count(), 0);

        }

        [TestMethod]
        public async Task TestCreateRefreshTokenHandleAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            IClientStore cs = new ClientStore();
            var insert = await CassandraTestHelper.InsertTestData_RefreshTokens(cs);

            foreach (var rth in insert)
            {
                var result = await dao.FindRefreshTokenByKey(rth.Key, cs);
                Assert.AreEqual(result.ClientId,rth.ClientId);
            }
        }
    }
}
