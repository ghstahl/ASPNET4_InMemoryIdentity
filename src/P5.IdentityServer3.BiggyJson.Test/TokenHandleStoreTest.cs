using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P5.IdentityServer3.BiggyJson.Test
{
    [TestClass]
    public class TokenHandleStoreTest
    {
        public static TokenHandle MakeTokenHandle(int i)
        {
            TokenHandle tokenHandle = new TokenHandle
            {
                Key = "KEY:" + i,
                ClientId = "CLIENTID:" + i,
                Claims = new List<ClaimTypeRecord>()
                    {
                        new ClaimTypeRecord()
                        {
                            Type = Constants.ClaimTypes.Subject,
                            Value = "Value:" + i,
                            ValueType = "ValueType:" + i
                        }
                    },
                CreationTime = DateTimeOffset.UtcNow,
                Issuer = "ISSUER:" + i,
                Lifetime = 5,
                Type = "Type:" + i,
                SubjectId = "SUBJECT:" + i % 2
            };
            return tokenHandle;
        }
        static void InsertTestData(TokenHandleStore store, int count = 1)
        {

            for (int i = 0; i < count; ++i)
            {

                TokenHandle tokenHandle = MakeTokenHandle(i);
                var tokenHandleRecord = new TokenHandleRecord(tokenHandle);
                store.CreateAsync(tokenHandleRecord.Record);

            }
        }
        private string _targetFolder;
        private ClientStore _clientStore;
        private TokenHandleStore _tokenHandleStore;

        [TestInitialize]
        public void Setup()
        {
            _targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");
            _clientStore = ClientStore.NewFromSetting(StoreSettings.UsingFolder(_targetFolder));
            _tokenHandleStore = TokenHandleStore.NewFromSetting(StoreSettings.UsingFolder(_targetFolder));
            InsertTestData(_tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(_clientStore, 10);
        }
        [TestMethod]
         public void TestCreateAsync()
        {

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;
            var result = _tokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new TokenHandleRecord(result.Result);


            Assert.AreEqual(tokenHandleRecord.Id, id);

        }
        [TestMethod]
         public void TestUpdateAsync()
        {

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;
            var result = _tokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new TokenHandleRecord(result.Result);


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
         public void TestStoreAsync()
        {

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _tokenHandleStore.GetAsync(key);

            key = "KEY:" + 10;
            tokenHandleRecord = new TokenHandleRecord(new TokenHandle()
            {
                Key = key
            });
            id = tokenHandleRecord.Id;
            Token token = result.Result;
            _tokenHandleStore.StoreAsync(key, token);
            result = _tokenHandleStore.GetAsync(key);
            tokenHandleRecord = new TokenHandleRecord(new TokenHandle(key,result.Result));

            Assert.AreEqual(tokenHandleRecord.Id, id);

        }
        [TestMethod]
         public void TestGetAsync()
        {

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _tokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId,"CLIENTID:" + 0);
        }
        [TestMethod]
         public void TestRemoveAsync()
        {

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _tokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);

            _tokenHandleStore.RemoveAsync(key);
            result = _tokenHandleStore.GetAsync(key);


            Assert.IsNull(result.Result);


        }
        [TestMethod]
         public void TestGetAllAsync()
        {

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var subject = "SUBJECT:" + 0;
            var result = _tokenHandleStore.GetAllAsync(subject);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.Count(),5);

        }

        [TestMethod]
         public void TestRevokeAsync()
        {

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _tokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);

            var subject = "SUBJECT:"+0;
            var clientId = result.Result.ClientId;
            _tokenHandleStore.RevokeAsync(subject, clientId);
            result = _tokenHandleStore.GetAsync(key);
            Assert.IsNull(result.Result);

        }
    }
}
