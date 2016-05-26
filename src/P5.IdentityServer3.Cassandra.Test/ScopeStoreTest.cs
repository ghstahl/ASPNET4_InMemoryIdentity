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
        public async void Setup()
        {

            base.Setup();
            await IdentityServer3CassandraDao.CreateTablesAsync();
            await IdentityServer3CassandraDao.TruncateTablesAsync();
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
        public async Task TestCreateAddAndDeleteScopesSecretsAsync()
        {
            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                select item.Record.Name;
            var nameList = queryNames.ToList();

            var adminStore = new IdentityServer3AdminStore();
            var stored = await adminStore.FindScopesAsync(nameList);

            Assert.AreEqual(stored.Count(), insertResult.Count);
            var secretComparer = new SecretComparer();
            var scopeComparer = new ScopeComparer();
            var scope = await insertResult[0].Record.MakeIdentityServerScopeAsync();
            var storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));

            List<Secret> secrets = new List<Secret>();
            for (int i = 0; i < 2; ++i)
            {
                secrets.Add(new Secret()
                {
                    Value = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Expiration = DateTimeOffset.UtcNow.AddHours(1),
                    Type = Guid.NewGuid().ToString()
                });
            }
            List<Secret> original = storedScope.ScopeSecrets;

            List<Secret> expected = storedScope.ScopeSecrets.Union(secrets, new SecretComparer()).ToList();

            await adminStore.AddScopeSecretsAsync(insertResult[0].Record.Name, secrets);
            stored = await adminStore.FindScopesAsync(nameList);
            storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));

            var query = from item in storedScope.ScopeSecrets
                        where !expected.Contains(item, secretComparer)
                        select item;
            var finalList = query.ToList();
            Assert.IsTrue(finalList.Count == 0);


            //DELETE IT
            await adminStore.DeleteScopeSecretsAsync(insertResult[0].Record.Name, secrets);
            stored = await adminStore.FindScopesAsync(nameList);
            storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));

            query = from item in storedScope.ScopeSecrets
                        where !original.Contains(item, secretComparer)
                        select item;
            finalList = query.ToList();
            Assert.IsTrue(finalList.Count == 0);



        }

        [TestMethod]
        public async Task TestCreateAndAddScopesSecretsAsync()
        {
            await IdentityServer3CassandraDao.CreateTablesAsync();
            await IdentityServer3CassandraDao.TruncateTablesAsync();

            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                             select item.Record.Name;
            var nameList = queryNames.ToList();

            var adminStore = new IdentityServer3AdminStore();
            var stored = await adminStore.FindScopesAsync(nameList);

            Assert.AreEqual(stored.Count(), insertResult.Count);
            var secretComparer = new SecretComparer();
            var scopeComparer = new ScopeComparer();
            var scope = await insertResult[0].Record.MakeIdentityServerScopeAsync();
            var storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));

            List<Secret> secrets = new List<Secret>();
            for (int i = 0; i < 2; ++i)
            {
                secrets.Add(new Secret()
                {
                    Value = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Expiration = DateTimeOffset.UtcNow.AddHours(1),
                    Type = Guid.NewGuid().ToString()
                });
            }
            List<Secret> expected = storedScope.ScopeSecrets.Union(secrets, new SecretComparer()).ToList();
            await adminStore.AddScopeSecretsAsync(insertResult[0].Record.Name, secrets);
            stored = await adminStore.FindScopesAsync(nameList);
            storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));

            var query = from item in storedScope.ScopeSecrets
                        where !expected.Contains(item, secretComparer)
                        select item;
            var finalList = query.ToList();
            Assert.IsTrue(finalList.Count == 0);

        }
        [TestMethod]
        public async Task TestCreateAndAddScopeClaimsAsync()
        {
            await IdentityServer3CassandraDao.CreateTablesAsync();
            await IdentityServer3CassandraDao.TruncateTablesAsync();

            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                             select item.Record.Name;
            var nameList = queryNames.ToList();

            var adminStore = new IdentityServer3AdminStore();
            var stored = await adminStore.FindScopesAsync(nameList);

            Assert.AreEqual(stored.Count(), insertResult.Count);
            var scopeClaimComparer = new ScopeClaimComparer();
            var scopeComparer = new ScopeComparer();
            var scope = await insertResult[0].Record.MakeIdentityServerScopeAsync();
            var storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));

            List<ScopeClaim> claims = new List<ScopeClaim>();
            for (int i = 0; i < 2; ++i)
            {
                claims.Add(new ScopeClaim
                {
                   Name = Guid.NewGuid().ToString(),
                   AlwaysIncludeInIdToken = true,
                   Description = Guid.NewGuid().ToString()
                });
            }
            List<ScopeClaim> expected = storedScope.Claims.Union(claims, new ScopeClaimComparer()).ToList();

            await adminStore.AddScopeClaimsAsync(insertResult[0].Record.Name, claims);
            stored = await adminStore.FindScopesAsync(nameList);
            storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));

            var query = from item in storedScope.Claims
                        where !expected.Contains(item, scopeClaimComparer)
                        select item;
            var finalList = query.ToList();
            Assert.IsTrue(finalList.Count == 0);


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

            var result = await IdentityServer3CassandraDao.UpsertScopeAsync(scopeRecord);
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
