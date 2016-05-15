using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.IdentityServer3.Common;
using P5.MSTest.Common;

namespace P5.IdentityServer3.BiggyJson.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class ClientStoreTest : TestBase
    {
        public static List<ClientRecord> InsertTestData(ClientStore store, ScopeStore scopeStore, int count = 1)
        {
            var scopeInsert = ScopeStoreTest.InsertTestData(scopeStore,10);
            List<ClientRecord> result = new List<ClientRecord>();
            for (int i = 0; i < count; ++i)
            {
                var scopes = from item in scopeInsert
                    select item.Record.Name;

                Client record = new Client()
                {
                   ClientId = Guid.NewGuid().ToString(),
                   AbsoluteRefreshTokenLifetime = 5,
                   AccessTokenLifetime = 5,
                   AccessTokenType = AccessTokenType.Jwt,
                   AllowAccessToAllCustomGrantTypes = true,
                   AllowAccessToAllScopes = true,
                   AllowAccessTokensViaBrowser = true,
                   AllowClientCredentialsOnly = true,
                   AllowRememberConsent = true,
                   AllowedCorsOrigins = new List<string>() { "www.google,com", "www.microsoft.com" },
                   AllowedCustomGrantTypes = new List<string>() { "AllowedCustomGrantTypes" + i },
                   AllowedScopes = scopes.ToList(),
                   AlwaysSendClientClaims = true,
                   AuthorizationCodeLifetime = 5,
                   Claims = new List<Claim>() { new Claim("Type:"+i,"Value:"+i)},
                   ClientName = "CLIENTNAME:"+i,
                   ClientSecrets = new List<Secret>() { new Secret("Secret:"+i)},
                   ClientUri = "www.someuri.com/"+i,
                   Enabled = true,
                   EnableLocalLogin = true,
                   Flow = Flows.ClientCredentials,
                   IdentityProviderRestrictions = new List<string>() { "IPR:"+i},
                   IdentityTokenLifetime = 5,
                   IncludeJwtId = true,
                   LogoUri = "LogoUri.com/"+i,
                   LogoutSessionRequired = true,
                   LogoutUri = "LogoutUri.com/"+i,
                   PostLogoutRedirectUris = new List<string>() { "PLRUri.com/" + i },
                   PrefixClientClaims = true,
                   RedirectUris = new List<string>() { "RedirectUri.com/"+i},
                   RefreshTokenExpiration = TokenExpiration.Absolute,
                   RefreshTokenUsage = TokenUsage.ReUse,
                   RequireConsent = true,
                   RequireSignOutPrompt = true,
                   SlidingRefreshTokenLifetime = 5,
                   UpdateAccessTokenClaimsOnRefresh = true
                };
                var clientRecord = new ClientRecord(record.ToClientHandle());
                store.CreateAsync(clientRecord.Record);
                result.Add(clientRecord);
            }
            return result;
        }


        private ClientStore _clientStore;
        private ScopeStore _scopeStore;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
            _clientStore = new ClientStore(StoreSettings.UsingFolder(TargetFolder));
            _scopeStore = new ScopeStore(StoreSettings.UsingFolder(TargetFolder));
        }

        [TestMethod]
        public void TestCreateAsync()
        {
            var insert = ClientStoreTest.InsertTestData(_clientStore,_scopeStore, 1);

            Guid id = insert[0].Id;
            var result = _clientStore.RetrieveAsync(id);
            var clientRecord = new ClientRecord(result.Result);


            Assert.AreEqual(clientRecord.Id, id);

        }
        [TestMethod]
        public void TestUpdateAsync()
        {

            var insert = ClientStoreTest.InsertTestData(_clientStore, _scopeStore, 1);

            Guid id = insert[0].Id;

            var result = _clientStore.RetrieveAsync(id);
            var clientRecord = new ClientRecord(result.Result);


            Assert.AreEqual(clientRecord.Id, id);
            var testData = Guid.NewGuid().ToString();
            clientRecord.Record.ClientName = testData;
            _clientStore.UpdateAsync(clientRecord.Record);

            result = _clientStore.RetrieveAsync(id);
            clientRecord = new ClientRecord(result.Result);


            Assert.AreEqual(clientRecord.Id, id);
            Assert.AreEqual(clientRecord.Record.ClientName,testData);
        }
        [TestMethod]
        public void TestDeleteAsync()
        {
            var insert = ClientStoreTest.InsertTestData(_clientStore, _scopeStore, 1);

            Guid id = insert[0].Id;
            var result = _clientStore.RetrieveAsync(id);
            var clientRecord = new ClientRecord(result.Result);


            Assert.AreEqual(clientRecord.Id, id);

            _clientStore.DeleteAsync(id);

            result = _clientStore.RetrieveAsync(id);
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public void TestFindClientByIdAsync()
        {
            var insert = ClientStoreTest.InsertTestData(_clientStore, _scopeStore, 1);

            Guid id = insert[0].Id;
            var result = _clientStore.FindClientByIdAsync(insert[0].Record.ClientId);
            var clientRecord = new ClientRecord(result.Result.ToClientHandle());


            Assert.AreEqual(clientRecord.Id, id);
        }
    }
}
