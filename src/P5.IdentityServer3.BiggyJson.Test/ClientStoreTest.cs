using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P5.IdentityServer3.BiggyJson.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class ClientStoreTest
    {
        public static void InsertTestData(ClientStore store,int count = 1)
        {

            for (int i = 0; i < count; ++i)
            {

                Client record = new Client()
                {
                   ClientId = "CLIENTID:"+i,
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
                   AllowedScopes = new List<string>() { "AllowedScopes" + i },
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
            }
        }

        private string _targetFolder;
        private ClientStore _clientStore;

        [TestInitialize]
        public void Setup()
        {
            _targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");
            _clientStore = new ClientStore(StoreSettings.UsingFolder(_targetFolder));
            ClientStoreTest.InsertTestData(_clientStore, 10);
        }

        [TestMethod]
        public void TestCreateAsync()
        {
            Client record = new Client()
            {
                ClientId = "CLIENTID:" + 0
            };
            ClientRecord clientRecord = new ClientRecord(record.ToClientHandle());
            Guid id = clientRecord.Id;
            var result = _clientStore.RetrieveAsync(id);
            clientRecord = new ClientRecord(result.Result);


            Assert.AreEqual(clientRecord.Id, id);

        }
        [TestMethod]
        public void TestUpdateAsync()
        {

            Client record = new Client()
            {
                ClientId = "CLIENTID:" + 0
            };
            ClientRecord clientRecord = new ClientRecord(record.ToClientHandle());
            Guid id = clientRecord.Id;
            var result = _clientStore.RetrieveAsync(id);
            clientRecord = new ClientRecord(result.Result);


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

            Client record = new Client()
            {
                ClientId = "CLIENTID:" + 0
            };
            ClientRecord clientRecord = new ClientRecord(record.ToClientHandle());
            Guid id = clientRecord.Id;
            var result = _clientStore.RetrieveAsync(id);
            clientRecord = new ClientRecord(result.Result);


            Assert.AreEqual(clientRecord.Id, id);
            var testData = Guid.NewGuid().ToString();
            clientRecord.Record.ClientName = testData;
            _clientStore.DeleteAsync(id);

            result = _clientStore.RetrieveAsync(id);
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public void TestFindClientByIdAsync()
        {

            Client record = new Client()
            {
                ClientId = "CLIENTID:" + 0
            };
            ClientRecord clientRecord = new ClientRecord(record.ToClientHandle());
            Guid id = clientRecord.Id;
            var result = _clientStore.FindClientByIdAsync("CLIENTID:" + 0);
            clientRecord = new ClientRecord(result.Result.ToClientHandle());


            Assert.AreEqual(clientRecord.Id, id);
        }
    }
}
