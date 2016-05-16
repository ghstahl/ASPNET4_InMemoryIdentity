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
using P5.IdentityServer3.Common;
using P5.MSTest.Common;

namespace P5.IdentityServer3.BiggyJson.Test
{
    [TestClass]
    public class TokenHandleStoreTest:TestBase
    {
        public static TokenHandle MakeTokenHandle(string subjectSeed,int i)
        {
            var subjectId = subjectSeed + i % 2;
            TokenHandle tokenHandle = new TokenHandle
            {
                Key = Guid.NewGuid().ToString(),
                ClientId = "CLIENTID:" + i,
                Claims = new List<ClaimTypeRecord>()
                    {
                        new ClaimTypeRecord()
                        {
                            Type = Constants.ClaimTypes.Subject,
                            Value = subjectId,
                            ValueType = "ValueType:" + i
                        }
                    },
                CreationTime = DateTimeOffset.UtcNow,
                Issuer = "ISSUER:" + i,
                Lifetime = 5,
                Type = "Type:" + i,
                SubjectId = subjectId
            };
            return tokenHandle;
        }
        public static List<TokenHandleRecord> InsertTestData(ClientStore clientStore,ScopeStore scopeStore,TokenHandleStore store, int count = 1)
        {
            var subjectSeed = Guid.NewGuid().ToString();
            var clientInsert = ClientStoreTest.InsertTestData(clientStore, scopeStore,1);
            List<TokenHandleRecord> result = new List<TokenHandleRecord>();
            for (int i = 0; i < count; ++i)
            {

                var tokenHandle = MakeTokenHandle(subjectSeed,i);
                tokenHandle.ClientId = clientInsert[0].Record.ClientId;
                var tokenHandleRecord = new TokenHandleRecord(tokenHandle);
                store.CreateAsync(tokenHandleRecord.Record);
                result.Add(tokenHandleRecord);

            }
            return result;
        }

        private ClientStore _clientStore;
        private TokenHandleStore _tokenHandleStore;
        private ScopeStore _scopeStore;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
            _clientStore = new ClientStore(StoreSettings.UsingFolder(TargetFolder));
            _tokenHandleStore = new TokenHandleStore(StoreSettings.UsingFolder(TargetFolder));
            _scopeStore = new ScopeStore(StoreSettings.UsingFolder(TargetFolder));
        }
        [TestMethod]
         public void TestCreateAsync()
        {
            var insert = InsertTestData(_clientStore,_scopeStore, _tokenHandleStore, 1);
            Guid id = insert[0].Id;
            var result = _tokenHandleStore.RetrieveAsync(id);
            var tokenHandleRecord = new TokenHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);
        }
        [TestMethod]
         public void TestUpdateAsync()
        {

            var insert = InsertTestData(_clientStore,_scopeStore, _tokenHandleStore, 1);
            Guid id = insert[0].Id;
            var result = _tokenHandleStore.RetrieveAsync(id);
            var tokenHandleRecord = new TokenHandleRecord(result.Result);


            Assert.AreEqual(tokenHandleRecord.Id, id);

            var testValue = Guid.NewGuid().ToString();
            tokenHandleRecord.Record.Type = testValue;

            _tokenHandleStore.UpdateAsync(tokenHandleRecord.Record);
            result = _tokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new TokenHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);
            Assert.AreEqual(tokenHandleRecord.Record.Type, testValue);

        }

        [TestMethod]
         public async Task TestStoreAsync()
        {
            try
            {
                var subjectSeed = Guid.NewGuid().ToString();
                var th = MakeTokenHandle(subjectSeed,0);
                TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(th);
                Guid id = tokenHandleRecord.Id;

                var key = th.Key;
                var result = await _tokenHandleStore.GetAsync(key);
                Assert.IsNull(result);



                Token token = await th.MakeIdentityServerTokenAsync(_clientStore);
                await _tokenHandleStore.StoreAsync(key, token);
                result = await _tokenHandleStore.GetAsync(key);
                tokenHandleRecord = new TokenHandleRecord(new TokenHandle(key, result));

                Assert.AreEqual(tokenHandleRecord.Id, id);
            }
            catch (Exception e)
            {

            }


        }
        [TestMethod]
         public void TestGetAsync()
        {
            var insert = InsertTestData(_clientStore,_scopeStore, _tokenHandleStore, 1);
            Guid id = insert[0].Id;
            var key = insert[0].Record.Key;
            var result = _tokenHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, insert[0].Record.ClientId);
        }
        [TestMethod]
         public void TestRemoveAsync()
        {

            var insert = InsertTestData(_clientStore, _scopeStore, _tokenHandleStore, 1);
            Guid id = insert[0].Id;
            var key = insert[0].Record.Key;
            var result = _tokenHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, insert[0].Record.ClientId);

            _tokenHandleStore.RemoveAsync(key);
            result = _tokenHandleStore.GetAsync(key);

            Assert.IsNull(result.Result);
        }
        [TestMethod]
         public void TestGetAllAsync()
        {
            var insert = InsertTestData(_clientStore, _scopeStore, _tokenHandleStore, 10);
            Guid id = insert[0].Id;

            var subject = insert[0].Record.SubjectId;
            var result = _tokenHandleStore.GetAllAsync(subject);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.Count(),5);

        }

        [TestMethod]
         public void TestRevokeAsync()
        {

            var insert = InsertTestData(_clientStore, _scopeStore, _tokenHandleStore, 10);
            Guid id = insert[0].Id;
            var key = insert[0].Record.Key;
            var result = _tokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, insert[0].Record.ClientId);

            var subject = insert[0].Record.SubjectId;
            var clientId = result.Result.ClientId;
            _tokenHandleStore.RevokeAsync(subject, clientId);
            result = _tokenHandleStore.GetAsync(key);
            Assert.IsNull(result.Result);

        }
    }
}
