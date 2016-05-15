using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.IdentityServer3.Common;
using P5.MSTest.Common;

namespace P5.IdentityServer3.BiggyJson.Test
{
    [TestClass]

    public class AuthorizationHandleStoreTest : TestBase
    {
        static List<AuthorizationCodeHandleRecord> InsertTestData(ClientStore clientStore,
            ScopeStore scopeStore,
            AuthorizationCodeStore authorizationCodeStore,
            TokenHandleStore ths,int count = 1)
        {
            var tokenInsert = TokenHandleStoreTest.InsertTestData(clientStore, scopeStore, ths,10);

            var clientId = tokenInsert[0].Record.ClientId;
            string subjectSeed = Guid.NewGuid().ToString();
            List<AuthorizationCodeHandleRecord> result = new List<AuthorizationCodeHandleRecord>();
            int i = 0;
            foreach (var tokenRecord in tokenInsert)
            {

                var client = clientStore.FindClientByIdAsync(tokenRecord.Record.ClientId);

                AuthorizationCodeHandle handle = new AuthorizationCodeHandle
                {
                    ClientId = tokenRecord.Record.ClientId,
                    SubjectId = tokenRecord.Record.SubjectId,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5),
                    CreationTime = DateTimeOffset.UtcNow,
                    IsOpenId = true,
                    RedirectUri = "REDIRECTURI/" + i,
                    WasConsentShown = true,
                    Nonce = "NONCE:" + i,

                    ClaimIdentityRecords = new List<ClaimIdentityRecord>()
                    {
                        new ClaimIdentityRecord()
                        {
                            AuthenticationType = Constants.PrimaryAuthenticationType,
                            ClaimTypeRecords = new List<ClaimTypeRecord>()
                            {
                                new ClaimTypeRecord()
                                {
                                    Type = Constants.ClaimTypes.Subject,
                                    Value = tokenRecord.Record.SubjectId,
                                    ValueType = "VALUETYPE:" + i
                                }
                            }
                        }
                    },
                    RequestedScopes = client.Result.AllowedScopes,
                    Key = Guid.NewGuid().ToString(),
                };

                var  handleRecord = new AuthorizationCodeHandleRecord(handle);
                authorizationCodeStore.CreateAsync(handleRecord.Record);
                result.Add(handleRecord);
                ++i;
            }
            return result;
        }

        private ClientStore _clientStore;
        private ScopeStore _scopeStore;
        private AuthorizationCodeStore _authorizationCodeHandleStore;
        private TokenHandleStore _tokenHandleStore;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
            _clientStore = new ClientStore(StoreSettings.UsingFolder(TargetFolder));
            _scopeStore = new ScopeStore(StoreSettings.UsingFolder(TargetFolder));
            _authorizationCodeHandleStore = new AuthorizationCodeStore(StoreSettings.UsingFolder(TargetFolder));
            _tokenHandleStore = new TokenHandleStore(StoreSettings.UsingFolder(TargetFolder));
        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestCreateAsync()
        {
            var insert = InsertTestData(_clientStore, _scopeStore, _authorizationCodeHandleStore, _tokenHandleStore, 10);
            var key = insert[0].Record.Key;
            var result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, insert[0].Record.ClientId);

        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestUpdateAsync()
        {
            var insert = InsertTestData(_clientStore, _scopeStore, _authorizationCodeHandleStore, _tokenHandleStore, 1);
            var key = insert[0].Record.Key;
            var result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, insert[0].Record.ClientId);
            var nonce = Guid.NewGuid().ToString();

            insert[0].Record.Nonce = nonce;
            _authorizationCodeHandleStore.UpdateAsync(insert[0].Record);
            result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.Nonce, nonce);
        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestStoreAsync()
        {
            var insert = InsertTestData(_clientStore, _scopeStore, _authorizationCodeHandleStore, _tokenHandleStore, 10);


            var key = insert[0].Record.Key;
            var result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);

            key = Guid.NewGuid().ToString();
            var tokenHandleRecord = new AuthorizationCodeHandleRecord(new AuthorizationCodeHandle()
            {
                Key = key
            });
            var id = tokenHandleRecord.Id;
            AuthorizationCode token = result.Result;
            _authorizationCodeHandleStore.StoreAsync(key, token);
            result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);

        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestGetAsync()
        {

            var insert = InsertTestData(_clientStore, _scopeStore, _authorizationCodeHandleStore, _tokenHandleStore, 10);
            var key = insert[0].Record.Key;
            var result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, insert[0].Record.ClientId);
        }
        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestRemoveAsync()
        {

            var insert = InsertTestData(_clientStore, _scopeStore, _authorizationCodeHandleStore, _tokenHandleStore, 10);
            var key = insert[0].Record.Key;
            var result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, insert[0].Record.ClientId);


            _authorizationCodeHandleStore.RemoveAsync(key);
            result = _authorizationCodeHandleStore.GetAsync(key);


            Assert.IsNull(result.Result);
        }
        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestGetAllAsync()
        {

            var insert = InsertTestData(_clientStore, _scopeStore, _authorizationCodeHandleStore, _tokenHandleStore, 10);
            var key = insert[0].Record.Key;
            var result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, insert[0].Record.ClientId);

            var subject = insert[0].Record.SubjectId;
            var allResult = _authorizationCodeHandleStore.GetAllAsync(subject);


            Assert.IsNotNull(allResult.Result);
            Assert.AreEqual(allResult.Result.Count(), 5);
        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestRevokeAsync()
        {

            var insert = InsertTestData(_clientStore, _scopeStore, _authorizationCodeHandleStore, _tokenHandleStore, 10);
            var key = insert[0].Record.Key;
            var clientId = insert[0].Record.ClientId;

            var result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, clientId);
            var subjectId = result.Result.SubjectId;



            _authorizationCodeHandleStore.RevokeAsync(subjectId, clientId);
            result = _authorizationCodeHandleStore.GetAsync(key);
            Assert.IsNull(result.Result);

        }

    }
}
