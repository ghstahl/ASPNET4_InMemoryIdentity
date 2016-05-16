using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.IdentityServer3.Common.RefreshToken;
using P5.MSTest.Common;

namespace P5.IdentityServer3.BiggyJson.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class RefreshTokenHandleStoreTest:TestBase
    {
        public static List<RefreshTokenHandleRecord> InsertTestData(ClientStore clientStore,ScopeStore scopeStore, TokenHandleStore ths, RefreshTokenStore store, int count = 1)
        {
            List<RefreshTokenHandleRecord> result = new List<RefreshTokenHandleRecord>();
            var insert = TokenHandleStoreTest.InsertTestData(clientStore, scopeStore,ths, count);
            var subjectSeed = Guid.NewGuid().ToString();
            var clientId = insert[0].Record.ClientId;
            foreach (var item in insert)
            {
                RefreshTokenHandle tokenHandle = new RefreshTokenHandle
                {

                    ClientId = clientId,
                    AccessToken = item.Record,
                    CreationTime = DateTimeOffset.UtcNow,
                    Key = Guid.NewGuid().ToString(),
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5),
                    LifeTime = 5,
                    SubjectId = item.Record.SubjectId,
                    Version = 1
                };
                var tokenHandleRecord = new RefreshTokenHandleRecord(tokenHandle);
                store.CreateAsync(tokenHandleRecord.Record);
                result.Add(tokenHandleRecord);
            }
            return result;
        }

        private static ClientStore _clientStore;
        private static TokenHandleStore _tokenHandleStore;
        private static RefreshTokenStore _refreshTokenHandleStore;
        private ScopeStore _scopeStore;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
            _clientStore = new ClientStore(StoreSettings.UsingFolder(TargetFolder));
            _tokenHandleStore = new TokenHandleStore(StoreSettings.UsingFolder(TargetFolder));
            _refreshTokenHandleStore = new RefreshTokenStore(StoreSettings.UsingFolder(TargetFolder));
            _scopeStore = new ScopeStore(StoreSettings.UsingFolder(TargetFolder));
        }
        [TestMethod]
        public void TestCreateAsync()
        {
            var insert = InsertTestData(_clientStore, _scopeStore, _tokenHandleStore, _refreshTokenHandleStore, 1);
            Guid id = insert[0].Id;
            var result = _refreshTokenHandleStore.RetrieveAsync(id);
            var tokenHandleRecord = new RefreshTokenHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);
        }

        [TestMethod]
         public void TestUpdateAsync()
        {
            var insert = InsertTestData(_clientStore, _scopeStore, _tokenHandleStore, _refreshTokenHandleStore, 1);
            Guid id = insert[0].Id;
            var result = _refreshTokenHandleStore.RetrieveAsync(id);
            var tokenHandleRecord = new RefreshTokenHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);

            var testValue = Guid.NewGuid().ToString();
            tokenHandleRecord.Record.SubjectId = testValue;

            _refreshTokenHandleStore.UpdateAsync(tokenHandleRecord.Record);
            result = _refreshTokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new RefreshTokenHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);
            Assert.AreEqual(tokenHandleRecord.Record.SubjectId, testValue);

        }

        [TestMethod]
         public async Task TestStoreAsync()
        {
            var insert = InsertTestData(_clientStore, _scopeStore, _tokenHandleStore, _refreshTokenHandleStore, 1);
            Guid id = insert[0].Id;
            var result = await _refreshTokenHandleStore.RetrieveAsync(id);
            var tokenHandleRecord = new RefreshTokenHandleRecord(result);
            Assert.AreEqual(tokenHandleRecord.Id, id);

            tokenHandleRecord.Record.SubjectId = Guid.NewGuid().ToString();
            tokenHandleRecord.Record.AccessToken.SubjectId = tokenHandleRecord.Record.SubjectId;
            tokenHandleRecord.Record.Key = Guid.NewGuid().ToString();
            var key = tokenHandleRecord.Record.Key;
            var rt = await tokenHandleRecord.Record.MakeRefreshTokenAsync(_clientStore);


            await _refreshTokenHandleStore.StoreAsync(key, rt);

            var storedResult = await _refreshTokenHandleStore.GetAsync(key);
            Assert.IsNotNull(storedResult);


            Assert.AreEqual(rt.SubjectId, storedResult.SubjectId);

        }

        [TestMethod]
         public void TestGetAsync()
        {
            var insert = InsertTestData(_clientStore, _scopeStore, _tokenHandleStore, _refreshTokenHandleStore, 1);
            Guid id = insert[0].Id;
            var result = _refreshTokenHandleStore.RetrieveAsync(id);
            var tokenHandleRecord = new RefreshTokenHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);


            var storedResult = _refreshTokenHandleStore.GetAsync(tokenHandleRecord.Record.Key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, insert[0].Record.ClientId);
        }

        [TestMethod]
         public void TestRemoveAsync()
        {

            var insert = InsertTestData(_clientStore, _scopeStore, _tokenHandleStore, _refreshTokenHandleStore, 1);
            Guid id = insert[0].Id;
            var result = _refreshTokenHandleStore.RetrieveAsync(id);
            var tokenHandleRecord = new RefreshTokenHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);


            var storedResult = _refreshTokenHandleStore.GetAsync(tokenHandleRecord.Record.Key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, insert[0].Record.ClientId);


            _refreshTokenHandleStore.RemoveAsync(tokenHandleRecord.Record.Key);
            storedResult = _refreshTokenHandleStore.GetAsync(tokenHandleRecord.Record.Key);

            Assert.IsNull(storedResult.Result);

        }

        [TestMethod]
         public void TestGetAllAsync()
        {

            var insert = InsertTestData(_clientStore, _scopeStore, _tokenHandleStore, _refreshTokenHandleStore, 10);
            Guid id = insert[0].Id;

            var subject = insert[0].Record.SubjectId;
            var result = _refreshTokenHandleStore.GetAllAsync(subject);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.Count(), 5);


        }


        [TestMethod]
         public void TestRevokeAsync()
        {

            var insert = InsertTestData(_clientStore, _scopeStore, _tokenHandleStore, _refreshTokenHandleStore, 1);
            Guid id = insert[0].Id;

            var subject = insert[0].Record.SubjectId;
            var clientId = insert[0].Record.ClientId;
            var result = _refreshTokenHandleStore.GetAsync(insert[0].Record.Key);

            Assert.IsNotNull(result.Result);

            _refreshTokenHandleStore.RevokeAsync(subject, clientId);
            result = _refreshTokenHandleStore.GetAsync(insert[0].Record.Key);

            Assert.IsNull(result.Result);

        }
    }
}
