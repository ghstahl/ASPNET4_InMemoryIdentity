using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer3.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.Extensions;
using P5.MSTest.Common;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class ClientStoreTest : TestBase
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
        public async Task TestCreateClientAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);
        }
        [TestMethod]
        public async Task TestCreateAndDeleteByIdAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);
            var adminStore = new IdentityServer3AdminStore();
            await adminStore.DeleteClientAsync(insert[0].Id);
            result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.IsNull(result);
        }
        [TestMethod]
        public async Task TestCreateAndDeleteByClientIdAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);
            var adminStore = new IdentityServer3AdminStore();
            await adminStore.DeleteClientAsync(insert[0].Record.ClientId);
            result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task TestCreateAndAddAllowedCorsOriginsByClientIdAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);
            var adminStore = new IdentityServer3AdminStore();
            List<string> allowedCorsOrigins = new List<string>()
            {
                Guid.NewGuid().ToString()
            };

            await adminStore.AddAllowedCorsOriginsToClientAsync(insert[0].Record.ClientId, allowedCorsOrigins);

            var finalList = new List<string>();
            finalList.AddRange(allowedCorsOrigins);
            finalList.AddRange(result.AllowedCorsOrigins);

            result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(result.AllowedCorsOrigins.Count(), finalList.Count);


            var ff = result.AllowedCorsOrigins.Except(finalList);
            Assert.IsFalse(ff.Any());
        }

        [TestMethod]
        public async Task TestCreateAddAndDeleteAllowedCorsOriginsByClientIdAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);
            var adminStore = new IdentityServer3AdminStore();
            List<string> allowedCorsOrigins = new List<string>()
            {
                Guid.NewGuid().ToString()
            };
            var originalList = result.AllowedCorsOrigins;
            await adminStore.AddAllowedCorsOriginsToClientAsync(insert[0].Record.ClientId, allowedCorsOrigins);

            var finalList = new List<string>();
            finalList.AddRange(allowedCorsOrigins);
            finalList.AddRange(result.AllowedCorsOrigins);

            result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(result.AllowedCorsOrigins.Count(), finalList.Count);

            var ff = result.AllowedCorsOrigins.Except(finalList);
            Assert.IsFalse(ff.Any());


            await adminStore.DeleteAllowedCorsOriginsFromClientAsync(insert[0].Record.ClientId, allowedCorsOrigins);
            result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(result.AllowedCorsOrigins.Count(), originalList.Count);
            ff = result.AllowedCorsOrigins.Except(originalList);
            Assert.IsFalse(ff.Any());
        }
        [TestMethod]
        public async Task TestCreateAndAddScopeByClientIdAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var adminStore = new IdentityServer3AdminStore();
           // await adminStore.CleanupClientByIdAsync(insert[0].Record.ClientId);
            var result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            var originalAllowedScopes = result.AllowedScopes;
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<Scope> scopes = new List<Scope>()
            {
                new Scope()
                {
                    AllowUnrestrictedIntrospection = true,
                    Name = Guid.NewGuid().ToString(),
                    Enabled = true
                },
                 new Scope()
                {
                    AllowUnrestrictedIntrospection = true,
                    Name = Guid.NewGuid().ToString(),
                    Enabled = true
                }
            };
            foreach (var scope in scopes)
            {
                await adminStore.CreateScopeAsync(scope);
            }


            var query = from item in scopes
                let c = item.Name
                select c;

            List<string> addedScopeNames = query.ToList();
            List<string> finalExpected = new List<string>();
            finalExpected.AddRange(addedScopeNames);
            finalExpected.AddRange(result.AllowedScopes);

            await adminStore.AddScopesToClientAsync(insert[0].Record.ClientId, addedScopeNames);



            result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.AllowedScopes.Count(), finalExpected.Count);


            var ff = result.AllowedScopes.Except(finalExpected);
            Assert.IsFalse(ff.Any());
        }
        [TestMethod]
        public async Task TestCreateAndAddDeleteScopeByClientIdAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var adminStore = new IdentityServer3AdminStore();
            // await adminStore.CleanupClientByIdAsync(insert[0].Record.ClientId);
            var result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            var originalAllowedScopes = result.AllowedScopes;
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<Scope> scopes = new List<Scope>()
            {
                new Scope()
                {
                    AllowUnrestrictedIntrospection = true,
                    Name = Guid.NewGuid().ToString(),
                    Enabled = true
                },
                 new Scope()
                {
                    AllowUnrestrictedIntrospection = true,
                    Name = Guid.NewGuid().ToString(),
                    Enabled = true
                }
            };
            foreach (var scope in scopes)
            {
                await adminStore.CreateScopeAsync(scope);
            }


            var query = from item in scopes
                        let c = item.Name
                        select c;

            List<string> addedScopeNames = query.ToList();
            List<string> finalExpected = new List<string>();
            finalExpected.AddRange(addedScopeNames);
            finalExpected.AddRange(result.AllowedScopes);

            await adminStore.AddScopesToClientAsync(insert[0].Record.ClientId, addedScopeNames);



            result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.AllowedScopes.Count(), finalExpected.Count);


            var ff = result.AllowedScopes.Except(finalExpected);
            Assert.IsFalse(ff.Any());


            await adminStore.DeleteScopesFromClientAsync(insert[0].Record.ClientId, result.AllowedScopes);
            result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.IsFalse(result.AllowedScopes.Any());


        }

        [TestMethod]
        public async Task TestCreateAndModifyClientAsync()
        {
            global::IdentityServer3.Core.Models.Client dd = new global::IdentityServer3.Core.Models.Client();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);
            int expectedInt = 1961;
            string expectedString = Guid.NewGuid().ToString();
            List<string> expectedList = new List<string>()
            {
                expectedString
            };
            #region PROPERTY_VALUES

            List<PropertyValue> propertyList = new List<PropertyValue>
            {

                new PropertyValue()
                {
                    Name = "Claims",
                    Value = new List<Claim>()
                    {
                        new Claim("Type:" + expectedString, "Value:" + expectedString, "ValueType:" + expectedString, "Issuer:" + expectedString, "OriginalIssuer:" + expectedString,
                            new ClaimsIdentity(new List<Claim>() {new Claim("Type:" + expectedString, "Value:" + expectedString)}))

                    }
                },
                new PropertyValue()
                {
                    Name = "ClientSecrets",
                    Value = new List<Secret>
                    {
                        new Secret(expectedString.Sha256()),
                    }
                },
                 new PropertyValue()
                {
                    Name = "ClientUri",
                    Value = expectedString
                },
                  new PropertyValue()
                {
                    Name = "ClientName",
                    Value = expectedString
                },
                new PropertyValue()
                {
                    Name = "AbsoluteRefreshTokenLifetime",
                    Value = expectedInt
                },
                new PropertyValue()
                {
                    Name = "AccessTokenLifetime",
                    Value = expectedInt
                },
                new PropertyValue()
                {
                    Name = "AccessTokenType",
                    Value = expectedInt
                },
                new PropertyValue()
                {
                    Name = "AllowAccessToAllCustomGrantTypes",
                    Value = !result.AllowAccessToAllCustomGrantTypes
                },
                new PropertyValue()
                {
                    Name = "AllowAccessToAllScopes",
                    Value = !result.AllowAccessToAllScopes
                },
                new PropertyValue()
                {
                    Name = "AllowAccessTokensViaBrowser",
                    Value = !result.AllowAccessTokensViaBrowser
                },
                new PropertyValue()
                {
                    Name = "AllowClientCredentialsOnly",
                    Value = !result.AllowClientCredentialsOnly
                },
                new PropertyValue()
                {
                    Name = "AllowRememberConsent",
                    Value = !result.AllowRememberConsent
                },
                new PropertyValue()
                {
                    Name = "AlwaysSendClientClaims",
                    Value = !result.AlwaysSendClientClaims
                },
                new PropertyValue()
                {
                    Name = "AuthorizationCodeLifetime",
                    Value = expectedInt
                },
                new PropertyValue()
                {
                    Name = "Enabled",
                    Value = !result.Enabled
                },
                new PropertyValue()
                {
                    Name = "EnableLocalLogin",
                    Value = !result.EnableLocalLogin
                },
                new PropertyValue()
                {
                    Name = "Flow",
                    Value = expectedInt
                },
                new PropertyValue()
                {
                    Name = "IdentityTokenLifetime",
                    Value = expectedInt
                },
                new PropertyValue()
                {
                    Name = "IncludeJwtId",
                    Value = !result.IncludeJwtId
                },
                new PropertyValue()
                {
                    Name = "LogoutSessionRequired",
                    Value = !result.LogoutSessionRequired
                },
                new PropertyValue()
                {
                    Name = "LogoutUri",
                    Value = expectedString
                },
                new PropertyValue()
                {
                    Name = "PrefixClientClaims",
                    Value = !result.PrefixClientClaims
                },
                new PropertyValue()
                {
                    Name = "RefreshTokenExpiration",
                    Value = expectedInt
                },
                new PropertyValue()
                {
                    Name = "RefreshTokenUsage",
                    Value = expectedInt
                },
                new PropertyValue()
                {
                    Name = "RequireConsent",
                    Value = !result.RequireConsent
                },
                new PropertyValue()
                {
                    Name = "RequireSignOutPrompt",
                    Value = !result.RequireSignOutPrompt
                },
                new PropertyValue()
                {
                    Name = "SlidingRefreshTokenLifetime",
                    Value = expectedInt
                },
                new PropertyValue()
                {
                    Name = "UpdateAccessTokenClaimsOnRefresh",
                    Value = !result.UpdateAccessTokenClaimsOnRefresh
                },
                new PropertyValue()
                {
                    Name = "AllowedCorsOrigins",
                    Value = expectedList
                },
                new PropertyValue()
                {
                    Name = "AllowedCustomGrantTypes",
                    Value = expectedList
                },
                new PropertyValue()
                {
                    Name = "AllowedScopes",
                    Value = expectedList
                },
                new PropertyValue()
                {
                    Name = "IdentityProviderRestrictions",
                    Value = expectedList
                },
                new PropertyValue()
                {
                    Name = "PostLogoutRedirectUris",
                    Value = expectedList
                },
                new PropertyValue()
                {
                    Name = "RedirectUris",
                    Value = expectedList
                },
                new PropertyValue()
                {
                    Name = "LogoUri",
                    Value = expectedString
                },
            };
#endregion

            await IdentityServer3CassandraDao.UpdateClientByIdAsync(result.ClientId, propertyList);

            var result2 = await IdentityServer3CassandraDao.FindClientIdAsync(insert[0].Id);

            foreach (var property in propertyList)
            {
                var castTo = property.Value.GetType();
                var valueRaw = result2.GetPropertyValue(property.Name);
                var valueActual = Cast(valueRaw, castTo);

                var valueExpected = property.Value;
                if (castTo == typeof (List<string>))
                {
                    var propertyValue = (List<string>) Convert.ChangeType(property.Value, castTo);
                    var resultValue = (List<string>) Convert.ChangeType(valueRaw, castTo);
                    Assert.AreEqual(propertyValue.Count, resultValue.Count);
                    var v = from x in propertyValue
                        where !resultValue.Contains(x)
                        select x;
                    Assert.IsFalse(v.Any());
                }
                else if (castTo == typeof (List<Secret>))
                {
                    var propertyValue = (List<Secret>) Convert.ChangeType(property.Value, castTo);
                    var resultValue = (List<Secret>) Convert.ChangeType(valueRaw, castTo);
                    Assert.AreEqual(propertyValue.Count, resultValue.Count);

                    IEnumerable<Secret> except =
                        propertyValue.Except(resultValue, new SecretComparer());
                    Assert.IsFalse(except.Any());
                }
                else if (castTo == typeof (List<Claim>))
                {
                    var propertyValue = (List<Claim>) Convert.ChangeType(property.Value, castTo);
                    var resultValue = (List<Claim>) Convert.ChangeType(valueRaw, castTo);
                    Assert.AreEqual(propertyValue.Count, resultValue.Count);

                    IEnumerable<Claim> except =
                        propertyValue.Except(resultValue, new ClaimComparer());
                    Assert.IsFalse(except.Any());
                }

                else
                {
                    Assert.AreEqual(valueActual, valueExpected);

                }
            }


        }
        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }
    }
}
