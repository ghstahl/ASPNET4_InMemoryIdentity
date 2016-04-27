using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
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
                            Type = "Type:" + i,
                            Value = "Value:" + i,
                            ValueType = "ValueType:" + i
                        }
                    },
                    CreationTime = DateTimeOffset.UtcNow,
                    Issuer = "ISSUER:" + i,
                    Lifetime = 5,
                    Type = "Type:" + i
                };


                var tokenHandleRecord = new TokenHandleRecord(tokenHandle);
                store.CreateAsync(tokenHandleRecord.Record);

            }
        }

        [TestMethod]
        [DeploymentItem("source", "targetFolder")]
        public void TestCreateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"targetFolder");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);

            var store = new TokenHandleStore(clientStore, targetFolder);

            InsertTestData(store);

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;
            var result = store.RetrieveAsync(id);
            tokenHandleRecord = new TokenHandleRecord(result.Result);


            Assert.AreEqual(tokenHandleRecord.Id, id);

        }
        [TestMethod]
        [DeploymentItem("source", "targetFolder")]
        public void TestUpdateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"targetFolder");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            var store = new TokenHandleStore(clientStore, targetFolder);

            InsertTestData(store);

            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;
            var result = store.RetrieveAsync(id);
            tokenHandleRecord = new TokenHandleRecord(result.Result);


            Assert.AreEqual(tokenHandleRecord.Id, id);

            var testValue = Guid.NewGuid().ToString();
            tokenHandleRecord.Record.Type = testValue;

            store.UpdateAsync(tokenHandleRecord.Record);
            result = store.RetrieveAsync(id);
            tokenHandleRecord = new TokenHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);
            Assert.AreEqual(tokenHandleRecord.Record.Type, testValue);

        }

        [TestMethod]
        [DeploymentItem("source", "targetFolder")]
        public void TestGetAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"targetFolder");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            var store = new TokenHandleStore(clientStore, targetFolder);

            InsertTestData(store,10);
            ClientStoreTest.InsertTestData(clientStore,10);


            TokenHandle record = new TokenHandle()
            {
                Key = "KEY:" + 0
            };
            TokenHandleRecord tokenHandleRecord = new TokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = store.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId,"CLIENTID:" + 0);
        }

        /*
        [TestMethod]
        [DeploymentItem("source", "targetFolder")]
        public void TestGetScopesAsync_publicOnly()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"targetFolder");

            ScopeStore store = new ScopeStore(targetFolder);
            InsertTestData(store, 10);
            List<string> scopeNames = new List<string>();
            for (int i = 0; i < 10; ++i)
            {
                scopeNames.Add("SCOPENAME:" + i);
            }
            var result = store.GetScopesAsync(true);

            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.Result.Any());
            Assert.AreEqual(result.Result.Count(), 5);
        }
        [TestMethod]
        [DeploymentItem("source", "targetFolder")]
        public void TestGetScopesAsync_all()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"targetFolder");

            ScopeStore store = new ScopeStore(targetFolder);
            InsertTestData(store, 10);
            List<string> scopeNames = new List<string>();
            for (int i = 0; i < 10; ++i)
            {
                scopeNames.Add("SCOPENAME:" + i);
            }
            var result = store.GetScopesAsync(false);

            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.Result.Any());
            Assert.AreEqual(result.Result.Count(), 10);
        }
         * */
    }
}
