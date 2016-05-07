using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.IdentityServer3.Common;
using P5.MSTest.Common;

namespace P5.IdentityServer3.BiggyJson.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class AuthorizationHandleStoreTest : TestBase
    {
        static void InsertTestData(AuthorizationCodeStore store, int count = 1)
        {

            for (int i = 0; i < count; ++i)
            {

                AuthorizationCodeHandle handle = new AuthorizationCodeHandle
                {
                    ClientId = "CLIENTID:" + i,
                    SubjectId = "SUBJECTID:" + i%2,
                    Expires = DateTimeOffset.UtcNow.AddSeconds(5),
                    CreationTime = DateTimeOffset.UtcNow,
                    IsOpenId = true,
                    RedirectUri = "REDIRECTURI/" + i,
                    WasConsentShown = true,
                    Nonce = "NONCE:" + i,
                    ClaimIdentityRecords = new List<ClaimIdentityRecord>()
                    {
                        new ClaimIdentityRecord()
                        {
                            AuthenticationType = "AuthenticationType:" + i,
                            ClaimTypeRecords = new List<ClaimTypeRecord>()
                            {
                                new ClaimTypeRecord()
                                {
                                    Type = "TYPE:" + 1,
                                    Value = "VALUE:" + i,
                                    ValueType = "VALUETYPE:" + i
                                }
                            }
                        }
                    },
                    RequestedScopes = new List<string>() {"REQUESTEDSCOPES:" + i},
                    Key = "KEY:" + i,
                };

                var  handleRecord = new AuthorizationCodeHandleRecord(handle);
                store.CreateAsync(handleRecord.Record);

            }
        }

        private ClientStore _clientStore;
        private ScopeStore _scopeStore;
        private AuthorizationCodeStore _authorizationCodeHandleStore;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
            _clientStore = new ClientStore(StoreSettings.UsingFolder(TargetFolder));
            _scopeStore = new ScopeStore(StoreSettings.UsingFolder(TargetFolder));
            _authorizationCodeHandleStore = new AuthorizationCodeStore(StoreSettings.UsingFolder(TargetFolder));
            InsertTestData(_authorizationCodeHandleStore, 10);
            ClientStoreTest.InsertTestData(_clientStore, 10);
            ScopeStoreTest.InsertTestData(_scopeStore,10);
        }

        [TestMethod]
        public void TestCreateAsync()
        {
            AuthorizationCodeHandle handle = new AuthorizationCodeHandle()
            {
                Key = "KEY:" + 0
            };
            AuthorizationCodeHandleRecord handleRecord = new AuthorizationCodeHandleRecord(handle);
            Guid id = handleRecord.Id;
            var result = _authorizationCodeHandleStore.RetrieveAsync(id);
            handleRecord = new AuthorizationCodeHandleRecord(result.Result);
            Assert.AreEqual(handleRecord.Id, id);
        }

        [TestMethod]
        public void TestUpdateAsync()
        {

            AuthorizationCodeHandle handle = new AuthorizationCodeHandle()
            {
                Key = "KEY:" + 0
            };
            AuthorizationCodeHandleRecord tokenHandleRecord = new AuthorizationCodeHandleRecord(handle);
            Guid id = tokenHandleRecord.Id;
            var result = _authorizationCodeHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new AuthorizationCodeHandleRecord(result.Result);


            Assert.AreEqual(tokenHandleRecord.Id, id);

            var testValue = Guid.NewGuid().ToString();
            tokenHandleRecord.Record.SubjectId = testValue;

            _authorizationCodeHandleStore.UpdateAsync(tokenHandleRecord.Record);
            result = _authorizationCodeHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new AuthorizationCodeHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);
            Assert.AreEqual(tokenHandleRecord.Record.SubjectId, testValue);

        }

        [TestMethod]
        public void TestStoreAsync()
        {

            AuthorizationCodeHandle record = new AuthorizationCodeHandle()
            {
                Key = "KEY:" + 0
            };
            AuthorizationCodeHandleRecord tokenHandleRecord = new AuthorizationCodeHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _authorizationCodeHandleStore.GetAsync(key);

            key = "KEY:" + 10;
            tokenHandleRecord = new AuthorizationCodeHandleRecord(new AuthorizationCodeHandle()
            {
                Key = key
            });
            id = tokenHandleRecord.Id;
            AuthorizationCode token = result.Result;
            _authorizationCodeHandleStore.StoreAsync(key, token);
            result = _authorizationCodeHandleStore.GetAsync(key);
            tokenHandleRecord = new AuthorizationCodeHandleRecord(new AuthorizationCodeHandle(key, result.Result));

            Assert.AreEqual(tokenHandleRecord.Id, id);
        }

        [TestMethod]
        public void TestGetAsync()
        {

            AuthorizationCodeHandle record = new AuthorizationCodeHandle()
            {
                Key = "KEY:" + 0
            };
            var tokenHandleRecord = new AuthorizationCodeHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _authorizationCodeHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);
        }
        [TestMethod]
        public void TestRemoveAsync()
        {

            AuthorizationCodeHandle record = new AuthorizationCodeHandle()
            {
                Key = "KEY:" + 0
            };
            var tokenHandleRecord = new AuthorizationCodeHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _authorizationCodeHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);

            _authorizationCodeHandleStore.RemoveAsync(key);
            result = _authorizationCodeHandleStore.GetAsync(key);


            Assert.IsNull(result.Result);
        }
        [TestMethod]
        public void TestGetAllAsync()
        {

            AuthorizationCodeHandle record = new AuthorizationCodeHandle()
            {
                Key = "KEY:" + 0
            };
            var tokenHandleRecord = new AuthorizationCodeHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var subject = "SUBJECTID:" + 0;
            var result = _authorizationCodeHandleStore.GetAllAsync(subject);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.Count(), 5);
        }

        [TestMethod]
        public void TestRevokeAsync()
        {

            AuthorizationCodeHandle record = new AuthorizationCodeHandle()
            {
                Key = "KEY:" + 0
            };
            var tokenHandleRecord = new AuthorizationCodeHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _authorizationCodeHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);

            var subject = "SUBJECTID:" + 0;
            var clientId = result.Result.ClientId;
            _authorizationCodeHandleStore.RevokeAsync(subject, clientId);
            result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNull(result.Result);

        }

    }
}
