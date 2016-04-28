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
        static void InsertTestData(TokenHandleStore store, int count = 1)
        {

            for (int i = 0; i < count; ++i)
            {

                TokenHandle tokenHandle = new TokenHandle
                {
                    Key = "KEY:"+i,
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
                    SubjectId = "SUBJECT:"+i%2
                };


                var tokenHandleRecord = new TokenHandleRecord(tokenHandle);
                store.CreateAsync(tokenHandleRecord.Record);

            }
        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestCreateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            TokenHandleStore tokenHandleStore = TokenHandleStore.NewFromDefaultSetting(targetFolder);

            TokenHandleStoreTest.InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;
            var result = tokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new TokenHandleRecord(result.Result);


            Assert.AreEqual(tokenHandleRecord.Id, id);

        }
        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestUpdateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            TokenHandleStore tokenHandleStore = TokenHandleStore.NewFromDefaultSetting(targetFolder);

            TokenHandleStoreTest.InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;
            var result = tokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new TokenHandleRecord(result.Result);


            Assert.AreEqual(tokenHandleRecord.Id, id);

            var testValue = Guid.NewGuid().ToString();
            tokenHandleRecord.Record.Type = testValue;

            tokenHandleStore.UpdateAsync(tokenHandleRecord.Record);
            result = tokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new TokenHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);
            Assert.AreEqual(tokenHandleRecord.Record.Type, testValue);

        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestStoreAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            TokenHandleStore tokenHandleStore = TokenHandleStore.NewFromDefaultSetting(targetFolder);

            TokenHandleStoreTest.InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = tokenHandleStore.GetAsync(key);

            key = "KEY:" + 10;
            tokenHandleRecord = new TokenHandleRecord(new TokenHandle()
            {
                Key = key
            });
            id = tokenHandleRecord.Id;
            Token token = result.Result;
            tokenHandleStore.StoreAsync(key,token);
            result = tokenHandleStore.GetAsync(key);
            tokenHandleRecord = new TokenHandleRecord(new TokenHandle(key,result.Result));
           
            Assert.AreEqual(tokenHandleRecord.Id, id);

        }
        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestGetAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            TokenHandleStore tokenHandleStore = TokenHandleStore.NewFromDefaultSetting(targetFolder);

            TokenHandleStoreTest.InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);


            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = tokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId,"CLIENTID:" + 0);
        }
        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestRemoveAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            TokenHandleStore tokenHandleStore = TokenHandleStore.NewFromDefaultSetting(targetFolder);

            TokenHandleStoreTest.InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);


            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = tokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);

            tokenHandleStore.RemoveAsync(key);
            result = tokenHandleStore.GetAsync(key);


            Assert.IsNull(result.Result);
           

        }
        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestGetAllAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            TokenHandleStore tokenHandleStore = TokenHandleStore.NewFromDefaultSetting(targetFolder);

            TokenHandleStoreTest.InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);


            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var subject = "SUBJECT:" + 0;
            var result = tokenHandleStore.GetAllAsync(subject);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.Count(),5);
           

        }


        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestRevokeAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            TokenHandleStore tokenHandleStore = TokenHandleStore.NewFromDefaultSetting(targetFolder);

            TokenHandleStoreTest.InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);


            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = tokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);

            var subject = "SUBJECT:"+0;
            var clientId = result.Result.ClientId;
            tokenHandleStore.RevokeAsync(subject, clientId);
            result = tokenHandleStore.GetAsync(key);
            Assert.IsNull(result.Result);

        }
    }
}
