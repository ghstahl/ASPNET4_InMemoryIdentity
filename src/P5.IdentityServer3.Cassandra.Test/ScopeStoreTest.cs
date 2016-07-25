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


        [TestInitialize]
        public async void Setup()
        {
/*
            base.Setup();
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();


            await dao.CreateTablesAsync();
            await dao.TruncateTablesAsync();
 * */
        }


        [TestMethod]
        public async Task Test_GetScopesAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                select item.Record.Name;
            var nameList = queryNames.ToList();
            var result = await dao.FindScopesAsync(true);
            Assert.IsTrue(result.Any());
        }


        [TestMethod]
        public async Task Test_FindScopesAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                select item.Record.Name;
            var nameList = queryNames.ToList();
            var result = await dao.FindScopesByNamesAsync(nameList);
            Assert.AreEqual(result.Count(), insertResult.Count);

        }

        [TestMethod]
        public async Task Test_Find_Scope_by_Name_Async()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            var actualScope1 = new Scope()
            {
                Name = Guid.NewGuid().ToString(),
                Type = ScopeType.Identity
            };
            await dao.UpsertScopeAsync(new FlattenedScopeRecord(new FlattenedScopeHandle(actualScope1)));
             var actualScope2 = new Scope()
            {
                Name = Guid.NewGuid().ToString(),
                Type = ScopeType.Resource
            };
            await dao.UpsertScopeAsync(new FlattenedScopeRecord(new FlattenedScopeHandle(actualScope2)));

            var scope = await dao.FindScopeByNameAsync(actualScope1.Name);
            Assert.AreEqual(scope.Type, actualScope1.Type);
            Assert.AreEqual(scope.Name, actualScope1.Name);

            scope = await dao.FindScopeByNameAsync(actualScope2.Name);
            Assert.AreEqual(scope.Type, actualScope2.Type);
            Assert.AreEqual(scope.Name, actualScope2.Name);
        }
        [TestMethod]
        public async Task Test_Find_Scope_by_ID_Async()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            var actualScope1 = new Scope()
            {
                Name = Guid.NewGuid().ToString(),
                Type = ScopeType.Identity,
                AllowUnrestrictedIntrospection = true,
                ClaimsRule = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                Emphasize = true,
                Enabled = true,
                IncludeAllClaimsForUser = true,
                Required = true,
                ShowInDiscoveryDocument = true
            };
            var rec1 = new FlattenedScopeRecord(new FlattenedScopeHandle(actualScope1));
            await dao.UpsertScopeAsync(rec1);

            var actualScope2 = new Scope()
            {
                Name = Guid.NewGuid().ToString(),
                Type = ScopeType.Resource,
                AllowUnrestrictedIntrospection = true,
                ClaimsRule = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                Emphasize = true,
                Enabled = true,
                IncludeAllClaimsForUser = true,
                Required = true,
                ShowInDiscoveryDocument = true
            };
            var rec2 = new FlattenedScopeRecord(new FlattenedScopeHandle(actualScope2));
            await dao.UpsertScopeAsync(rec2);

            var scope = await dao.FindScopeByIdAsync(rec1.Id);
            Assert.IsTrue(ScopeComparer.OrdinalIgnoreCase.Equals(scope,actualScope1));

            scope = await dao.FindScopeByIdAsync(rec2.Id);
            Assert.IsTrue(ScopeComparer.OrdinalIgnoreCase.Equals(scope, actualScope2));
        }
        [TestMethod]
        public async Task Test_Create_Find_Delete_Scope_Async()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                             select item.Record.Name;
            var nameList = queryNames.ToList();
            var result = await dao.FindScopesByNamesAsync(nameList);
            Assert.AreEqual(result.Count(), insertResult.Count);

            var scope = await dao.FindScopeByNameAsync(nameList[0]);
            Assert.IsNotNull(scope);
            Assert.AreEqual(scope.Name, nameList[0]);
            FlattenedScopeRecord fsr = new FlattenedScopeRecord(new FlattenedScopeHandle(scope));

            scope = await dao.FindScopeByIdAsync(fsr.Id);
            Assert.IsNotNull(scope);
            Assert.AreEqual(scope.Name, nameList[0]);

            await dao.DeleteScopeAsync(scope);

            scope = await dao.FindScopeByNameAsync(scope.Name);
            Assert.IsNull(scope);

            scope = await dao.FindScopeByIdAsync(fsr.Id);
            Assert.IsNull(scope);



        }
        [TestMethod]
        public async Task Test_Create_Add_Delete_ScopesSecretsAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                select item.Record.Name;
            var nameList = queryNames.ToList();

            var adminStore = new IdentityServer3AdminStore();
            var stored = await adminStore.FindScopesAsync(nameList);

            Assert.AreEqual(stored.Count(), insertResult.Count);
            var secretComparer = SecretComparer.OrdinalIgnoreCase;
            var scopeComparer = ScopeComparer.OrdinalIgnoreCase;
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

            List<Secret> expected = storedScope.ScopeSecrets.Union(secrets, SecretComparer.OrdinalIgnoreCase).ToList();

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
        public async Task Test_Create_Add_ScopesSecretsAsync()
        {
            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                             select item.Record.Name;
            var nameList = queryNames.ToList();

            var adminStore = new IdentityServer3AdminStore();
            var stored = await adminStore.FindScopesAsync(nameList);

            Assert.AreEqual(stored.Count(), insertResult.Count);
            var secretComparer = SecretComparer.OrdinalIgnoreCase;
            var scopeComparer = ScopeComparer.OrdinalIgnoreCase;

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
            List<Secret> expected = storedScope.ScopeSecrets.Union(secrets, SecretComparer.OrdinalIgnoreCase).ToList();
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
        public async Task Test_Create_Add_ScopeClaimsAsync()
        {

            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                             select item.Record.Name;
            var nameList = queryNames.ToList();

            var adminStore = new IdentityServer3AdminStore();
            var stored = await adminStore.FindScopesAsync(nameList);

            Assert.AreEqual(stored.Count(), insertResult.Count);
            var scopeClaimComparer = ScopeClaimComparer.MinimalScopeClaimComparer;
            var scopeComparer = ScopeComparer.OrdinalIgnoreCase;
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
            List<ScopeClaim> expected = storedScope.Claims.Union(claims, ScopeClaimComparer.MinimalScopeClaimComparer).ToList();

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
        public async Task Test_Create_Add_Delete_ScopeClaimsAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();


            await dao.CreateTablesAsync();
            await dao.TruncateTablesAsync();

            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                             select item.Record.Name;
            var nameList = queryNames.ToList();

            var adminStore = new IdentityServer3AdminStore();
            var stored = await adminStore.FindScopesAsync(nameList);

            Assert.AreEqual(stored.Count(), insertResult.Count);
            var scopeClaimComparer = ScopeClaimComparer.MinimalScopeClaimComparer;
            var scopeComparer = ScopeComparer.OrdinalIgnoreCase;
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
            var originalClaims = storedScope.Claims;
            List<ScopeClaim> expected = storedScope.Claims.Union(claims, ScopeClaimComparer.MinimalScopeClaimComparer).ToList();

            await adminStore.AddScopeClaimsAsync(insertResult[0].Record.Name, claims);
            stored = await adminStore.FindScopesAsync(nameList);
            storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));

            var query = from item in storedScope.Claims
                        where !expected.Contains(item, scopeClaimComparer)
                        select item;
            var finalList = query.ToList();
            Assert.IsTrue(finalList.Count == 0);

            await adminStore.DeleteScopeClaimsAsync(insertResult[0].Record.Name, claims);
            stored = await adminStore.FindScopesAsync(nameList);
            storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));

            query = from item in storedScope.Claims
                    where !originalClaims.Contains(item, scopeClaimComparer)
                        select item;
            finalList = query.ToList();
            Assert.IsTrue(finalList.Count == 0);

        }

        [TestMethod]
        public async Task Test_Create_Add_Update_ScopeClaimsAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();



            await dao.CreateTablesAsync();
            await dao.TruncateTablesAsync();

            var insertResult = await CassandraTestHelper.InsertTestData_Scopes(1);
            var queryNames = from item in insertResult
                             select item.Record.Name;
            var nameList = queryNames.ToList();

            var adminStore = new IdentityServer3AdminStore();
            var stored = await adminStore.FindScopesAsync(nameList);

            Assert.AreEqual(stored.Count(), insertResult.Count);
            var scopeClaimComparer = ScopeClaimComparer.MinimalScopeClaimComparer;
            var scopeComparer = ScopeComparer.OrdinalIgnoreCase;
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
            await adminStore.UpdateScopeByNameAsync(storedScope.Name, new List<PropertyValue>()
            {
                new PropertyValue()
                {
                    Name = "Claims",
                    Value = claims
                }
            });


            stored = await adminStore.FindScopesAsync(nameList);
            storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));


            var query = from item in storedScope.Claims
                        where !claims.Contains(item, ScopeClaimComparer.DeepScopeClaimComparer)
                        select item;
            var finalList = query.ToList();
            Assert.IsTrue(finalList.Count == 0);

            // Do NOT update name
            foreach (var claim in claims)
            {

                claim.Description = Guid.NewGuid().ToString();
                claim.AlwaysIncludeInIdToken = !claim.AlwaysIncludeInIdToken;
            }

            await adminStore.UpdateScopeClaimsAsync(storedScope.Name, claims);
            stored = await adminStore.FindScopesAsync(nameList);
            storedScope = stored.FirstOrDefault();
            Assert.IsTrue(scopeComparer.Equals(scope, storedScope));


            query = from item in storedScope.Claims
                    where !claims.Contains(item, ScopeClaimComparer.DeepScopeClaimComparer)
                        select item;
            finalList = query.ToList();
            Assert.IsTrue(finalList.Count == 0);
        }


        [TestMethod]
        public async Task Test_CreateAsync()
        {
            await CreateAsync(1);
        }


        public async Task<Scope> CreateAsync(int i)
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

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

            var result = await dao.UpsertScopeAsync(scopeRecord);
            Assert.IsTrue(result);

            var result2 = await dao.FindScopeByIdAsync(id);
            Assert.IsNotNull(result2);

            scopeRecord = new FlattenedScopeRecord(new FlattenedScopeHandle(result2));

            Assert.AreEqual(scopeRecord.Record.Name, name);
            Assert.AreEqual(scopeRecord.Id, id);

            var scope = await scopeRecord.Record.GetScopeAsync();
            Assert.AreEqual(record.Claims.Count, scope.Claims.Count);

            var differences = record.Claims.Except(scope.Claims, ScopeClaimComparer.MinimalScopeClaimComparer);
            Assert.IsTrue(!differences.Any());
            return scope;

        }
        [TestMethod]
        public async Task Test_Create_Page_ByScopeIdAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            await dao.TruncateTablesAsync();
            List<Scope> addedScopes= new List<Scope>();
            int nNumber = 100;
            var adminStore = new IdentityServer3AdminStore();
            for (int i = 0; i < nNumber; ++i)
            {
                addedScopes.Add(await CreateAsync(nNumber));
            }

            var pageSize = 9;
            byte[] pagingState = null;
            int runningCount = 0;
            do
            {
                var items = await adminStore.PageScopesAsync(pageSize, pagingState);
                pagingState = items.PagingState;
                runningCount += items.Count();
            } while (pagingState != null);
            Assert.AreEqual(100, runningCount);


        }
    }
}
