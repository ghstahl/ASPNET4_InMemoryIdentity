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
using P5.IdentityServer3.Cassandra.AuthorizationCode;
using P5.IdentityServer3.Cassandra.Client;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.MSTest.Common;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class AuthorizationHandleStoreTest : TestBase
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
            var insert = await CassandraTestHelper.InsertTestData_AuthorizationCode(10);
            var subjectId = insert[0].SubjectId;
            var clientId = insert[0].ClientId;
            IAuthorizationCodeStore authStore = new AuthorizationCodeStore();
            var find_metadata = await authStore.GetAllAsync(subjectId);
            Assert.AreEqual(find_metadata.Count(), insert.Count);
        }

        [TestMethod]
        public async Task TestRemoveAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_AuthorizationCode(1);
            var subjectId = insert[0].SubjectId;
            var clientId = insert[0].ClientId;
            var key = insert[0].Key;
            IAuthorizationCodeStore authStore = new AuthorizationCodeStore();
            var find_metadata = await authStore.GetAsync(key);
            Assert.IsNotNull(find_metadata);

            await authStore.RemoveAsync(key);
            find_metadata = await authStore.GetAsync(key);
            Assert.IsNull(find_metadata);
        }

        [TestMethod]
        public async Task TestRevokeAsync()
        {

            var insert = await CassandraTestHelper.InsertTestData_AuthorizationCode(10);
            var subjectId = insert[0].SubjectId;
            var clientId = insert[0].ClientId;
            IAuthorizationCodeStore authStore = new AuthorizationCodeStore();
            var find_metadata = await authStore.GetAllAsync(subjectId);
            Assert.AreEqual(find_metadata.Count(), insert.Count);



            await authStore.RevokeAsync(subjectId, clientId);
            find_metadata = await authStore.GetAllAsync(subjectId);

            Assert.AreEqual(find_metadata.Count(), 0);

        }

        
    }
}
