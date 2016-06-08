using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.ResponseHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.Extensions;
using P5.MSTest.Common;
using ClaimComparer = P5.IdentityServer3.Common.ClaimComparer;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class ClientStoreTest : TestBase
    {
        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }
        private IdentityServer3CassandraDao _store;

        [TestInitialize]
        public async void Setup()
        {
            base.Setup();
            //       await dao.CreateTablesAsync();
            //       await dao.TruncateTablesAsync();
        }


        [TestMethod]
        public async Task Test_CreateClientAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);
        }
        [TestMethod]
        public async Task Test_Create_Delete_ByIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            await adminStore.DeleteClientAsync(insert[0].Id);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNull(result);
        }
        [TestMethod]
        public async Task Test_Create_Delete_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            await adminStore.DeleteClientAsync(insert[0].Record.ClientId);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNull(result);
        }
        [TestMethod]
        public async Task Test_Create_Add_AllowedCustomGrantTypesByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);

            var originalAllowedCustomGrantTypes = result.AllowedCustomGrantTypes;
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<string> newAllowedCustomGrantTypes = new List<string>()
            {
                Guid.NewGuid().ToString()
            };
            var finalList = new List<string>();
            finalList.AddRange(originalAllowedCustomGrantTypes);
            finalList.AddRange(newAllowedCustomGrantTypes);

            await adminStore.AddAllowedCustomGrantTypesToClientAsync(insert[0].Record.ClientId, newAllowedCustomGrantTypes);


            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.AllowedCustomGrantTypes.Count(), finalList.Count);


            var ff = result.AllowedCustomGrantTypes.Except(finalList);
            Assert.IsFalse(ff.Any());
        }
        [TestMethod]
        public async Task Test_Create_Add_Delete_AllowedCustomGrantTypesByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);

            var originalAllowedCustomGrantTypes = result.AllowedCustomGrantTypes;
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<string> newAllowedCustomGrantTypes = new List<string>()
            {
                Guid.NewGuid().ToString()
            };
            var finalList = new List<string>();
            finalList.AddRange(originalAllowedCustomGrantTypes);
            finalList.AddRange(newAllowedCustomGrantTypes);

            await adminStore.AddAllowedCustomGrantTypesToClientAsync(insert[0].Record.ClientId, newAllowedCustomGrantTypes);


            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.AllowedCustomGrantTypes.Count(), finalList.Count);


            var ff = result.AllowedCustomGrantTypes.Except(finalList);
            Assert.IsFalse(ff.Any());

            await
                adminStore.DeleteAllowedCustomGrantTypesFromClientAsync(insert[0].Record.ClientId,
                    result.AllowedCustomGrantTypes);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.AllowedCustomGrantTypes.Any());

        }
        [TestMethod]
        public async Task Test_Create_Add_AllowedCorsOrigins_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<string> allowedCorsOrigins = new List<string>()
            {
                Guid.NewGuid().ToString()
            };

            await adminStore.AddAllowedCorsOriginsToClientAsync(insert[0].Record.ClientId, allowedCorsOrigins);

            var finalList = new List<string>();
            finalList.AddRange(allowedCorsOrigins);
            finalList.AddRange(result.AllowedCorsOrigins);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.AreEqual(result.AllowedCorsOrigins.Count(), finalList.Count);


            var ff = result.AllowedCorsOrigins.Except(finalList);
            Assert.IsFalse(ff.Any());
        }

        [TestMethod]
        public async Task Test_Create_Add_Delete_AllowedCorsOrigins_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<string> allowedCorsOrigins = new List<string>()
            {
                Guid.NewGuid().ToString()
            };
            var originalList = result.AllowedCorsOrigins;
            await adminStore.AddAllowedCorsOriginsToClientAsync(insert[0].Record.ClientId, allowedCorsOrigins);

            var finalList = new List<string>();
            finalList.AddRange(allowedCorsOrigins);
            finalList.AddRange(result.AllowedCorsOrigins);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.AreEqual(result.AllowedCorsOrigins.Count(), finalList.Count);

            var ff = result.AllowedCorsOrigins.Except(finalList);
            Assert.IsFalse(ff.Any());


            await adminStore.DeleteAllowedCorsOriginsFromClientAsync(insert[0].Record.ClientId, allowedCorsOrigins);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.AreEqual(result.AllowedCorsOrigins.Count(), originalList.Count);
            ff = result.AllowedCorsOrigins.Except(originalList);
            Assert.IsFalse(ff.Any());
        }
        [TestMethod]
        public async Task Test_Create_Add_ScopeByClientIdAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var adminStore = new IdentityServer3AdminStore();
           // await adminStore.CleanupClientByIdAsync(insert[0].Record.ClientId);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
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



            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.AllowedScopes.Count(), finalExpected.Count);


            var ff = result.AllowedScopes.Except(finalExpected);
            Assert.IsFalse(ff.Any());
        }
        [TestMethod]
        public async Task Test_Create_Add_Delete_Scope_ByClientIdAsync()
        {
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var adminStore = new IdentityServer3AdminStore();
            // await adminStore.CleanupClientByIdAsync(insert[0].Record.ClientId);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
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



            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.AllowedScopes.Count(), finalExpected.Count);


            var ff = result.AllowedScopes.Except(finalExpected);
            Assert.IsFalse(ff.Any());


            await adminStore.DeleteScopesFromClientAsync(insert[0].Record.ClientId, result.AllowedScopes);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsFalse(result.AllowedScopes.Any());


        }

        [TestMethod]
        public async Task Test_Create_Modify_ClientAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            var adminStore = new IdentityServer3AdminStore();
            global::IdentityServer3.Core.Models.Client dd = new global::IdentityServer3.Core.Models.Client();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
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

            await dao.UpdateClientByIdAsync(result.ClientId, propertyList);

            var result2 = await dao.FindClientIdAsync(insert[0].Id);

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
                        propertyValue.Except(resultValue, SecretComparer.OrdinalIgnoreCase);
                    Assert.IsFalse(except.Any());
                }
                else if (castTo == typeof (List<Claim>))
                {
                    var propertyValue = (List<Claim>) Convert.ChangeType(property.Value, castTo);
                    var resultValue = (List<Claim>) Convert.ChangeType(valueRaw, castTo);
                    Assert.AreEqual(propertyValue.Count, resultValue.Count);

                    IEnumerable<Claim> except =
                        propertyValue.Except(resultValue, ClaimComparer.MinimalComparer);
                    Assert.IsFalse(except.Any());
                }

                else
                {
                    Assert.AreEqual(valueActual, valueExpected);

                }
            }


        }
        [TestMethod]
        public async Task Test_Create_Add_IdentityProviderRestrictions_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            var originalIdentityProviderRestrictions = result.IdentityProviderRestrictions;

            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<string> newIdentityProviderRestrictions = new List<string>()
            {
                Guid.NewGuid().ToString()
            };

            await adminStore.AddIdentityProviderRestrictionsToClientAsync(insert[0].Record.ClientId, newIdentityProviderRestrictions);

            var finalList = new List<string>();
            finalList.AddRange(newIdentityProviderRestrictions);
            finalList.AddRange(originalIdentityProviderRestrictions);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.IdentityProviderRestrictions.Count(), finalList.Count);


            var ff = result.IdentityProviderRestrictions.Except(finalList);
            Assert.IsFalse(ff.Any());
        }

        [TestMethod]
        public async Task Test_Create_Add_Delete_IdentityProviderRestrictions_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            var originalIdentityProviderRestrictions = result.IdentityProviderRestrictions;

            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<string> newIdentityProviderRestrictions = new List<string>()
            {
                Guid.NewGuid().ToString()
            };

            await adminStore.AddIdentityProviderRestrictionsToClientAsync(insert[0].Record.ClientId, newIdentityProviderRestrictions);

            var finalList = new List<string>();
            finalList.AddRange(newIdentityProviderRestrictions);
            finalList.AddRange(originalIdentityProviderRestrictions);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.IdentityProviderRestrictions.Count(), finalList.Count);


            var ff = result.IdentityProviderRestrictions.Except(finalList);
            Assert.IsFalse(ff.Any());


            await adminStore.DeleteIdentityProviderRestrictionsFromClientAsync(insert[0].Record.ClientId, result.IdentityProviderRestrictions);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);

            Assert.IsFalse(result.IdentityProviderRestrictions.Any());
        }

        [TestMethod]
        public async Task Test_Create_Add_PostLogoutRedirectUris_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            var original = result.PostLogoutRedirectUris;

            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<string> newData = new List<string>()
            {
                Guid.NewGuid().ToString()
            };

            await adminStore.AddPostLogoutRedirectUrisToClientAsync(insert[0].Record.ClientId, newData);

            var finalList = new List<string>();
            finalList.AddRange(original);
            finalList.AddRange(newData);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.PostLogoutRedirectUris.Count(), finalList.Count);


            var ff = result.PostLogoutRedirectUris.Except(finalList);
            Assert.IsFalse(ff.Any());
        }

        [TestMethod]
        public async Task Test_Create_Add_Delete_PostLogoutRedirectUris_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            var original = result.PostLogoutRedirectUris;

            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<string> newData = new List<string>()
            {
                Guid.NewGuid().ToString()
            };

            await adminStore.AddPostLogoutRedirectUrisToClientAsync(insert[0].Record.ClientId, newData);

            var finalList = new List<string>();
            finalList.AddRange(original);
            finalList.AddRange(newData);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.PostLogoutRedirectUris.Count(), finalList.Count);


            var ff = result.PostLogoutRedirectUris.Except(finalList);
            Assert.IsFalse(ff.Any());


            await adminStore.DeletePostLogoutRedirectUrisFromClientAsync(insert[0].Record.ClientId, result.PostLogoutRedirectUris);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);

            Assert.IsFalse(result.PostLogoutRedirectUris.Any());
        }

        [TestMethod]
        public async Task Test_Create_Add_RedirectUris_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            var original = result.RedirectUris;

            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            List<string> newData = new List<string>()
            {
                Guid.NewGuid().ToString()
            };

            await adminStore.AddRedirectUrisToClientAsync(insert[0].Record.ClientId, newData);

            var finalList = new List<string>();
            finalList.AddRange(original);
            finalList.AddRange(newData);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.RedirectUris.Count(), finalList.Count);


            var ff = result.RedirectUris.Except(finalList);
            Assert.IsFalse(ff.Any());
        }

        [TestMethod]
        public async Task Test_Create_Add_Delete_RedirectUris_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            var original = result.RedirectUris;

            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);
            List<string> newData = new List<string>()
            {
                Guid.NewGuid().ToString()
            };

            await adminStore.AddRedirectUrisToClientAsync(insert[0].Record.ClientId, newData);

            var finalList = new List<string>();
            finalList.AddRange(original);
            finalList.AddRange(newData);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.RedirectUris.Count(), finalList.Count);


            var ff = result.RedirectUris.Except(finalList);
            Assert.IsFalse(ff.Any());


            await adminStore.DeleteRedirectUrisFromClientAsync(insert[0].Record.ClientId, result.RedirectUris);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);

            Assert.IsFalse(result.RedirectUris.Any());
        }

        [TestMethod]
        public async Task Test_Create_Add_ClientSecrets_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            var original = result.ClientSecrets;

            List<Secret> newSecrets = new List<Secret>();
            for (int i = 0; i < 2; ++i)
            {
                newSecrets.Add(new Secret()
                {
                    Value = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Expiration = DateTimeOffset.UtcNow.AddHours(1),
                    Type = Guid.NewGuid().ToString()
                });
            }
            var finalList = new List<Secret>();
            finalList.AddRange(original);
            finalList.AddRange(newSecrets);


            await adminStore.AddClientSecretsToClientAsync(insert[0].Record.ClientId, newSecrets);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ClientSecrets.Count(), finalList.Count);

            var ff = result.ClientSecrets.Except(finalList, SecretComparer.OrdinalIgnoreCase);
            Assert.IsFalse(ff.Any());

        }

        [TestMethod]
        public async Task Test_Create_Add_Delete_ClientSecrets_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            var original = result.ClientSecrets;

            List<Secret> newSecrets = new List<Secret>();
            for (int i = 0; i < 2; ++i)
            {
                newSecrets.Add(new Secret()
                {
                    Value = Guid.NewGuid().ToString(),
                    Description = Guid.NewGuid().ToString(),
                    Expiration = DateTimeOffset.UtcNow.AddHours(1),
                    Type = Guid.NewGuid().ToString()
                });
            }
            var finalList = new List<Secret>();
            finalList.AddRange(original);
            finalList.AddRange(newSecrets);

            await adminStore.AddClientSecretsToClientAsync(insert[0].Record.ClientId, newSecrets);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ClientSecrets.Count(), finalList.Count);

            var ff = result.ClientSecrets.Except(finalList, SecretComparer.OrdinalIgnoreCase);
            Assert.IsFalse(ff.Any());

            await adminStore.DeleteClientSecretsFromClientAsync(insert[0].Record.ClientId, result.ClientSecrets);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);

            Assert.IsFalse(result.ClientSecrets.Any());
        }
        [TestMethod]
        public async Task Test_Create_Add_ClientClaims_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            var original = result.Claims;

            var newClaims = new List<Claim>{
                new Claim(Constants.ClaimTypes.Subject, Guid.NewGuid().ToString()),
                new Claim(Constants.ClaimTypes.PreferredUserName, Guid.NewGuid().ToString()),
            };

            var finalList = new List<Claim>();
            finalList.AddRange(original);
            finalList.AddRange(newClaims);


            await adminStore.AddClaimsToClientAsync(insert[0].Record.ClientId, newClaims);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Claims.Count(), finalList.Count);

            var ff = result.Claims.Except(finalList, ClaimComparer.DeepComparer);
            Assert.IsFalse(ff.Any());

        }

        [TestMethod]
        public async Task Test_Create_Add_Delete_ClientClaims_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            var original = result.Claims;

            var newClaims = new List<Claim>{
                new Claim(Constants.ClaimTypes.Subject, Guid.NewGuid().ToString()),
                new Claim(Constants.ClaimTypes.PreferredUserName, Guid.NewGuid().ToString()),
            };

            var finalList = new List<Claim>();
            finalList.AddRange(original);
            finalList.AddRange(newClaims);


            await adminStore.AddClaimsToClientAsync(insert[0].Record.ClientId, newClaims);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Claims.Count(), finalList.Count);

            var ff = result.Claims.Except(finalList, ClaimComparer.DeepComparer);
            Assert.IsFalse(ff.Any());

            await adminStore.DeleteClaimsFromClientAsync(insert[0].Record.ClientId, result.Claims);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);

            Assert.IsFalse(result.Claims.Any());
        }

        [TestMethod]
        public async Task Test_Create_Add_Update_ClientClaims_ByClientIdAsync()
        {
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            var original = result.Claims;

            var newClaims = new List<Claim>{
                new Claim(Constants.ClaimTypes.Subject, Guid.NewGuid().ToString()),
                new Claim(Constants.ClaimTypes.PreferredUserName, Guid.NewGuid().ToString()),
            };

            var finalList = new List<Claim>();
            finalList.AddRange(original);
            finalList.AddRange(newClaims);


            await adminStore.AddClaimsToClientAsync(insert[0].Record.ClientId, newClaims);

            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Claims.Count(), finalList.Count);

            var ff = result.Claims.Except(finalList, ClaimComparer.DeepComparer);
            Assert.IsFalse(ff.Any());

            newClaims = new List<Claim>{
                new Claim(Constants.ClaimTypes.Subject, Guid.NewGuid().ToString()),
                new Claim(Constants.ClaimTypes.PreferredUserName, Guid.NewGuid().ToString()),
            };
            finalList = new List<Claim>();
            finalList.AddRange(original);
            finalList.AddRange(newClaims);

            await adminStore.UpdateClaimsInClientAsync(insert[0].Record.ClientId, newClaims);
            result = await adminStore.FindClientByIdAsync(insert[0].Record.ClientId);
            Assert.IsNotNull(result);


            ff = result.Claims.Except(finalList, ClaimComparer.DeepComparer);
            Assert.IsFalse(ff.Any());
        }
        [TestMethod]
        public async Task Test_Create_Page_ByClientIdAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            await dao.TruncateTablesAsync();
            int nNumber = 100;
            var adminStore = new IdentityServer3AdminStore();
            var insert = await CassandraTestHelper.InsertTestData_Clients(nNumber);
            foreach (var item in insert)
            {
                var result = await adminStore.FindClientByIdAsync(item.Record.ClientId);
                Assert.IsNotNull(result);
                Assert.AreEqual(item.Record.ClientName, result.ClientName);
            }


            var pageSize = 9;
            byte[] pagingState = null;
            int runningCount = 0;
            do
            {
                var items = await adminStore.PageClientsAsync(pageSize, pagingState);
                pagingState = items.PagingState;
                runningCount += items.Count();
            } while (pagingState != null);
            Assert.AreEqual(100, runningCount);


        }
    }
}
