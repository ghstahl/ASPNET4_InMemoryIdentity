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
using P5.IdentityServer3.Cassandra.Client;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;


using P5.MSTest.Common;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class TokenHandleStoreTest : TestBase
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
            var insert = await CassandraTestHelper.InsertTestData_Tokens(10);

            ITokenHandleStore ths = new TokenHandleStore();

            var find_metadata = await ths.GetAllAsync(insert[0].SubjectId);
            Assert.AreEqual(find_metadata.Count(), insert.Count);
        }

        [TestMethod]
        public async Task TestRemoveAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Tokens(1);

            ITokenHandleStore ths = new TokenHandleStore();
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
            var insert = await CassandraTestHelper.InsertTestData_Tokens(10);

            ITokenHandleStore ths = new TokenHandleStore();
            var subjectId = insert[0].SubjectId;
            var clientId = insert[0].ClientId;

            var find_metadata = await ths.GetAllAsync(subjectId);
            Assert.AreEqual(find_metadata.Count(), insert.Count);

            await ths.RevokeAsync(subjectId, clientId);
            find_metadata = await ths.GetAllAsync(subjectId);

            Assert.AreEqual(find_metadata.Count(), 0);

        }

        [TestMethod]
        public async Task TestCreateTokenHandleAsync()
        {
            int i = 0;
            var claims = new List<Claim>()
            {
                new Claim("Type 0:" + i, "Value 0:" + i),
                new Claim("Type 1:" + i, "Value 1:" + i)

            };
            var json = JsonConvert.SerializeObject(claims);

            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var flat = new FlattenedTokenHandle
            {
                Key = Guid.NewGuid().ToString(),
                Audience = "Audience:"+i,
                Claims = JsonConvert.SerializeObject(claims),
                ClientId = insert[0].Record.ClientId,
                CreationTime = DateTimeOffset.UtcNow,
                Expires = DateTimeOffset.UtcNow,
                Issuer = "Issuer:"+i,
                Lifetime = 1,
                SubjectId = "SubjectId:"+i,
                Type = "Type:"+i,
                Version = 1
            };
            IClientStore cs = new ClientStore();
            ITokenHandleStore ths = new TokenHandleStore();
            var result = await IdentityServer3CassandraDao.CreateTokenHandleAsync(flat);
            var result_of_find = await IdentityServer3CassandraDao.FindTokenByKey(flat.Key,cs);
            Token tt = result_of_find;
            Assert.AreEqual(flat.ClientId,result_of_find.ClientId);

            var newKey = Guid.NewGuid().ToString();
            await ths.StoreAsync(newKey, tt);
            result_of_find = await ths.GetAsync(newKey);

            Assert.AreEqual(flat.ClientId, result_of_find.ClientId);

            await ths.RemoveAsync(newKey);
            result_of_find = await ths.GetAsync(newKey);
            Assert.AreEqual(result_of_find, null);
        }

    }
}
