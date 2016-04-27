using System;
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
    public class ScopeStoreTest
    {
        static void InsertTestData(ScopeStore store,int count = 1)
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

        [TestMethod]
        [DeploymentItem("source", "targetFolder")]
        public void TestCreateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"targetFolder");

            ScopeStore store = new ScopeStore(targetFolder);
            InsertTestData(store);

            Scope record = new Scope()
            {
                Name = "SCOPENAME:" + 0
            };
            ScopeRecord scopeRecord =new ScopeRecord(record);
            Guid id = scopeRecord.Id;
            var result = store.RetrieveAsync(id);
            scopeRecord = new ScopeRecord(result.Result);


            Assert.AreEqual(scopeRecord.Id, id);

        }

        [TestMethod]
        [DeploymentItem("source", "targetFolder")]
        public void TestUpdateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"targetFolder");

            ScopeStore store = new ScopeStore(targetFolder);
            InsertTestData(store);

            Scope record = new Scope()
            {
                Name = "SCOPENAME:" + 0
            };
            ScopeRecord scopeRecord = new ScopeRecord(record);
            Guid id = scopeRecord.Id;
            var result = store.RetrieveAsync(id);
            scopeRecord = new ScopeRecord(result.Result);
            string testData = Guid.NewGuid().ToString();
            scopeRecord.Record.ClaimsRule = testData;

            store.UpdateAsync(scopeRecord.Record);
            result = store.RetrieveAsync(id);

            Assert.AreEqual(testData,result.Result.ClaimsRule);
        }

        [TestMethod]
        [DeploymentItem("source", "targetFolder")]
        public void TestFindScopesAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"targetFolder");

            ScopeStore store = new ScopeStore(targetFolder);
            InsertTestData(store,10);
            List<string> scopeNames = new List<string>();
            for (int i = 0; i < 10; ++i)
            {
                scopeNames.Add("SCOPENAME:" + i);
            }
            var result = store.FindScopesAsync(scopeNames);

            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.Result.Any());
            Assert.AreEqual(result.Result.Count(),10);
        }
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
    }
}
