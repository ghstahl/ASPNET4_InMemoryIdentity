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
    [DeploymentItem("source", "source")]
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
        private string _targetFolder;
        private ClientStore _clientStore;
        private RefreshTokenHandleStore _refreshTokenHandleStore;

        [TestInitialize]
        public void Setup()
        {
            _targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");
            _clientStore = ClientStore.NewFromSetting(StoreSettings.UsingFolder(_targetFolder));
            _refreshTokenHandleStore = RefreshTokenHandleStore.NewFromSetting(StoreSettings.UsingFolder(_targetFolder));
            InsertTestData(_refreshTokenHandleStore, 10);
            ClientStoreTest.InsertTestData(_clientStore, 10);
        }
        [TestMethod]
        public void TestCreateAsync()
        {
            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;
            var result = _refreshTokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new RefreshTokenHandleRecord(result.Result);


            Assert.AreEqual(tokenHandleRecord.Id, id);

        }

        [TestMethod]
         public void TestUpdateAsync()
        {

            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;
            var result = _refreshTokenHandleStore.RetrieveAsync(id);
            tokenHandleRecord = new RefreshTokenHandleRecord(result.Result);


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
         public void TestStoreAsync()
        {

            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _refreshTokenHandleStore.GetAsync(key);

            key = "KEY:" + 10;
            tokenHandleRecord = new RefreshTokenHandleRecord(new RefreshTokenHandle()
            {
                Key = key
            });
            id = tokenHandleRecord.Id;
            RefreshToken token = result.Result;
            _refreshTokenHandleStore.StoreAsync(key, token);
            result = _refreshTokenHandleStore.GetAsync(key);
            tokenHandleRecord = new RefreshTokenHandleRecord(new RefreshTokenHandle(key, result.Result));

            Assert.AreEqual(tokenHandleRecord.Id, id);

        }

        [TestMethod]
         public void TestGetAsync()
        {

            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _refreshTokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);
        }

        [TestMethod]
         public void TestRemoveAsync()
        {

            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _refreshTokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);

            _refreshTokenHandleStore.RemoveAsync(key);
            result = _refreshTokenHandleStore.GetAsync(key);


            Assert.IsNull(result.Result);


        }

        [TestMethod]
         public void TestGetAllAsync()
        {

            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var subject = "SUBJECTID:" + 0;
            var result = _refreshTokenHandleStore.GetAllAsync(subject);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.Count(), 5);


        }


        [TestMethod]
         public void TestRevokeAsync()
        {

            RefreshTokenHandle record = new RefreshTokenHandle()
            {
                Key = "KEY:" + 0
            };
            RefreshTokenHandleRecord tokenHandleRecord = new RefreshTokenHandleRecord(record);
            Guid id = tokenHandleRecord.Id;

            var key = "KEY:" + 0;
            var result = _refreshTokenHandleStore.GetAsync(key);


            Assert.IsNotNull(result.Result);
            Assert.AreEqual(result.Result.ClientId, "CLIENTID:" + 0);

            var subject = "SUBJECTID:" + 0;
            var clientId = result.Result.ClientId;
            _refreshTokenHandleStore.RevokeAsync(subject, clientId);
            result = _refreshTokenHandleStore.GetAsync(key);
            Assert.IsNull(result.Result);

        }
    }
}
