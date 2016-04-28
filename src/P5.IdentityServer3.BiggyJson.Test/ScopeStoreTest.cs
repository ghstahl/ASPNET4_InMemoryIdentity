﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P5.IdentityServer3.BiggyJson.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class ScopeStoreTest
    {
        public static void InsertTestData(ScopeStore store,int count = 1)
        {

            for (int i = 0; i < count; ++i)
            {

                Scope record = new Scope()
                {
                    Name = "SCOPENAME:" + i,
                    AllowUnrestrictedIntrospection = true,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim()
                        {
                            AlwaysIncludeInIdToken = true,
                            Description = "SCOPECLAIMDESCRIPTION:" + i,
                            Name = "SCOPECLAIMNAME:" + i

                        }
                    },
                    ClaimsRule = "CLAIMSRULE:" + i,
                    Description = "CLAIMSRULE:" + i,
                    DisplayName = "DISPLAYNAME:" + i,
                    Emphasize = true,
                    Enabled = true,
                    IncludeAllClaimsForUser = true,
                    Required = true,
                    ScopeSecrets = new List<Secret>() {new Secret("SECRET:" + i), new Secret("SECRET:" + i)},
                    ShowInDiscoveryDocument = (i%2==1)


                };
                var scopeRecord = new ScopeRecord(record);
                store.CreateAsync(scopeRecord.Record);

            }
        }
        private string _targetFolder;
        private ScopeStore _scopeStore;

        [TestInitialize]
        public void Setup()
        {
            _targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");
            _scopeStore = ScopeStore.NewFromSetting(StoreSettings.UsingFolder(_targetFolder));
            InsertTestData(_scopeStore, 10);
        }
        [TestMethod]
        public void TestCreateAsync()
        {

            Scope record = new Scope()
            {
                Name = "SCOPENAME:" + 0
            };
            ScopeRecord scopeRecord =new ScopeRecord(record);
            Guid id = scopeRecord.Id;
            var result = _scopeStore.RetrieveAsync(id);
            scopeRecord = new ScopeRecord(result.Result);


            Assert.AreEqual(scopeRecord.Id, id);

        }

        [TestMethod]
        public void TestUpdateAsync()
        {

            Scope record = new Scope()
            {
                Name = "SCOPENAME:" + 0
            };
            ScopeRecord scopeRecord = new ScopeRecord(record);
            Guid id = scopeRecord.Id;
            var result = _scopeStore.RetrieveAsync(id);
            scopeRecord = new ScopeRecord(result.Result);
            string testData = Guid.NewGuid().ToString();
            scopeRecord.Record.ClaimsRule = testData;

            _scopeStore.UpdateAsync(scopeRecord.Record);
            result = _scopeStore.RetrieveAsync(id);

            Assert.AreEqual(testData,result.Result.ClaimsRule);
        }

        [TestMethod]
         public void TestFindScopesAsync()
        {

            List<string> scopeNames = new List<string>();
            for (int i = 0; i < 10; ++i)
            {
                scopeNames.Add("SCOPENAME:" + i);
            }
            var result = _scopeStore.FindScopesAsync(scopeNames);

            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.Result.Any());
            Assert.AreEqual(result.Result.Count(),10);
        }
        [TestMethod]
         public void TestGetScopesAsync_publicOnly()
        {

            List<string> scopeNames = new List<string>();
            for (int i = 0; i < 10; ++i)
            {
                scopeNames.Add("SCOPENAME:" + i);
            }
            var result = _scopeStore.GetScopesAsync(true);

            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.Result.Any());
            Assert.AreEqual(result.Result.Count(), 5);
        }
        [TestMethod]
         public void TestGetScopesAsync_all()
        {

            List<string> scopeNames = new List<string>();
            for (int i = 0; i < 10; ++i)
            {
                scopeNames.Add("SCOPENAME:" + i);
            }
            var result = _scopeStore.GetScopesAsync(false);

            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.Result.Any());
            Assert.AreEqual(result.Result.Count(), 10);
        }
    }
}
