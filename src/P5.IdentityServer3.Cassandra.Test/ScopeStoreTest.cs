using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.MSTest.Common;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class ScopeStoreTest : TestBase
    {
        private IdentityServer3CassandraDao _store;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }


        [TestMethod]
        public async Task TestGetScopesAsync()
        {
            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                select item.Record.Name;
            var nameList = queryNames.ToList();
            var result = await IdentityServer3CassandraDao.FindScopesAsync(true);
            Assert.IsTrue(result.Any());
        }


        [TestMethod]
        public async Task TestFindScopesAsync()
        {
            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                select item.Record.Name;
            var nameList = queryNames.ToList();
            var result = await IdentityServer3CassandraDao.FindScopesByNamesAsync(nameList);
            Assert.AreEqual(result.Count(), insertResult.Count);

        }

        [TestMethod]
        public async Task TestCreateAsync()
        {
            int i = 0;

            var name = Guid.NewGuid().ToString();
            global::IdentityServer3.Core.Models.Scope record = new global::IdentityServer3.Core.Models.Scope()
            {
                AllowUnrestrictedIntrospection = true,
                Name = name,
                ClaimsRule = "ClaimRule:" + i,
                Description = "Description:" + i,
                DisplayName = "DisplayName:" + i,
                Enabled = true,
                Emphasize = true,
                IncludeAllClaimsForUser = true,
                Required = true,
                Type = ScopeType.Identity,
                ScopeSecrets = new List<Secret>()
                {
                    new Secret
                    {
                        Type = "Type1:" + i,
                        Description = "Decription1:" + i,
                        Expiration = DateTimeOffset.UtcNow,
                        Value = "Value1:" + i
                    },
                    new Secret
                    {
                        Type = "Type2:" + i,
                        Description = "Decription2:" + i,
                        Expiration = DateTimeOffset.UtcNow,
                        Value = "Value2:" + i
                    }
                },
                ShowInDiscoveryDocument = true,
                Claims = new List<ScopeClaim>()
                {
                    new ScopeClaim
                    {
                        AlwaysIncludeInIdToken = true,Description = "Decription1:" + i,
                        Name = "Name1:" + i
                    },
                      new ScopeClaim
                    {
                        AlwaysIncludeInIdToken = true,Description = "Decription2:" + i,
                        Name = "Name2:" + i
                    }
                }
            };
            var scopeRecord = new FlattenedScopeRecord(new FlattenedScopeHandle(record));
            Guid id = scopeRecord.Id;

            var result = await IdentityServer3CassandraDao.CreateScopeAsync(scopeRecord);
            Assert.IsTrue(result);

            var result2 = await IdentityServer3CassandraDao.FindScopeByIdAsync(id);
            Assert.IsNotNull(result2);

            scopeRecord = new FlattenedScopeRecord(new FlattenedScopeHandle(result2));

            Assert.AreEqual(scopeRecord.Record.Name, name);
            Assert.AreEqual(scopeRecord.Id, id);

            var scope = await scopeRecord.Record.GetScopeAsync();
            Assert.AreEqual(record.Claims.Count, scope.Claims.Count);

            var differences = record.Claims.Except(scope.Claims, new ScopeClaimComparer());
            Assert.IsTrue(!differences.Any());

        }

    }
}
