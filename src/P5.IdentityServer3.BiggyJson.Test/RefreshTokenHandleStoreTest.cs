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
    public class RefreshTokenHandleStoreTest
    {
        static void InsertTestData(RefreshTokenHandleStore store, int count = 1)
        {

            for (int i = 0; i < count; ++i)
            {

                RefreshTokenHandle tokenHandle = new RefreshTokenHandle
                {

                    ClientId = "CLIENTID:" + i,
                    AccessToken = TokenHandleStoreTest.MakeTokenHandle(i),
                    CreationTime = DateTimeOffset.UtcNow,
                    Key = "KEY:" + i,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5),
                    LifeTime = 5,
                    SubjectId = "SUBJECTID:" + i%2,
                    Version = 1
                };


                var tokenHandleRecord = new RefreshTokenHandleRecord(tokenHandle);
                store.CreateAsync(tokenHandleRecord.Record);

            }
        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestCreateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            var clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            var tokenHandleStore = RefreshTokenHandleStore.NewFromDefaultSetting(targetFolder);

            InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);

            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;
            var result = tokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new RefreshTokenHandleRecord(result.Result);


            Assert.AreEqual(tokenHandleRecord.Id, id);

        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestUpdateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            var clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            var tokenHandleStore = RefreshTokenHandleStore.NewFromDefaultSetting(targetFolder);

            InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);

            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;
            var result = tokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new RefreshTokenHandleRecord(result.Result);


            Assert.AreEqual(tokenHandleRecord.Id, id);

            var testValue = Guid.NewGuid().ToString();
            tokenHandleRecord.Record.SubjectId = testValue;

            tokenHandleStore.UpdateAsync(tokenHandleRecord.Record);
            result = tokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new RefreshTokenHandleRecord(result.Result);
            Assert.AreEqual(tokenHandleRecord.Id, id);
            Assert.AreEqual(tokenHandleRecord.Record.SubjectId, testValue);

        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestStoreAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            var clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            var tokenHandleStore = RefreshTokenHandleStore.NewFromDefaultSetting(targetFolder);

            InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);

            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = tokenHandleStore.GetAsync(key);

            key = "KEY:" + 10;
            tokenHandleRecord = new RefreshTokenHandleRecord(new RefreshTokenHandle()
            {
                Key = key
            });
            id = tokenHandleRecord.Id;
            RefreshToken token = result.Result;
            tokenHandleStore.StoreAsync(key, token);
            result = tokenHandleStore.GetAsync(key);
            tokenHandleRecord = new RefreshTokenHandleRecord(new RefreshTokenHandle(key, result.Result));

            Assert.AreEqual(tokenHandleRecord.Id, id);

        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestGetAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            var clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            var tokenHandleStore = RefreshTokenHandleStore.NewFromDefaultSetting(targetFolder);

            InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);

            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = tokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);
        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestRemoveAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            RefreshTokenHandleStore tokenHandleStore = RefreshTokenHandleStore.NewFromDefaultSetting(targetFolder);

            RefreshTokenHandleStoreTest.InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);


            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
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
            RefreshTokenHandleStore tokenHandleStore = RefreshTokenHandleStore.NewFromDefaultSetting(targetFolder);

            RefreshTokenHandleStoreTest.InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);


            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var subject = "SUBJECTID:" + 0;
            var result = tokenHandleStore.GetAllAsync(subject);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.Count(), 5);


        }


        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestRevokeAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore clientStore = ClientStore.NewFromDefaultSetting(targetFolder);
            RefreshTokenHandleStore tokenHandleStore = RefreshTokenHandleStore.NewFromDefaultSetting(targetFolder);

            RefreshTokenHandleStoreTest.InsertTestData(tokenHandleStore, 10);
            ClientStoreTest.InsertTestData(clientStore, 10);


            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = tokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);

            var subject = "SUBJECTID:" + 0;
            var clientId = result.Result.ClientId;
            tokenHandleStore.RevokeAsync(subject, clientId);
            result = tokenHandleStore.GetAsync(key);
            Assert.IsNull(result.Result);

        }
    }
}
