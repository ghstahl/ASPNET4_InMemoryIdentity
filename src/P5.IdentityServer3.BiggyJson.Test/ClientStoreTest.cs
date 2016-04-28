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

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestCreateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore store = ClientStore.NewFromDefaultSetting(targetFolder);
            InsertTestData(store);

            Client record = new Client()
            {
                ClientId = "CLIENTID:" + 0
            };
            ClientRecord clientRecord = new ClientRecord(record.ToClientHandle());
            Guid id = clientRecord.Id;
            var result = store.RetrieveAsync(id);
            clientRecord = new ClientRecord(result.Result);


            Assert.AreEqual(clientRecord.Id, id);

        }
        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestUpdateAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore store = ClientStore.NewFromDefaultSetting(targetFolder);

            InsertTestData(store);

            Client record = new Client()
            {
                ClientId = "CLIENTID:" + 0
            };
            ClientRecord clientRecord = new ClientRecord(record.ToClientHandle());
            Guid id = clientRecord.Id;
            var result = store.RetrieveAsync(id);
            clientRecord = new ClientRecord(result.Result);


            Assert.AreEqual(clientRecord.Id, id);
            var testData = Guid.NewGuid().ToString();
            clientRecord.Record.ClientName = testData;
            store.UpdateAsync(clientRecord.Record);

            result = store.RetrieveAsync(id);
            clientRecord = new ClientRecord(result.Result);


            Assert.AreEqual(clientRecord.Id, id);
            Assert.AreEqual(clientRecord.Record.ClientName,testData);
        }
        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestDeleteAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore store = ClientStore.NewFromDefaultSetting(targetFolder);

            InsertTestData(store);

            Client record = new Client()
            {
                ClientId = "CLIENTID:" + 0
            };
            ClientRecord clientRecord = new ClientRecord(record.ToClientHandle());
            Guid id = clientRecord.Id;
            var result = store.RetrieveAsync(id);
            clientRecord = new ClientRecord(result.Result);


            Assert.AreEqual(clientRecord.Id, id);
            var testData = Guid.NewGuid().ToString();
            clientRecord.Record.ClientName = testData;
            store.DeleteAsync(id);

            result = store.RetrieveAsync(id);
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        [DeploymentItem("source", "source")]
        public void TestFindClientByIdAsync()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

            ClientStore store = ClientStore.NewFromDefaultSetting(targetFolder);

            InsertTestData(store);

            Client record = new Client()
            {
                ClientId = "CLIENTID:" + 0
            };
            ClientRecord clientRecord = new ClientRecord(record.ToClientHandle());
            Guid id = clientRecord.Id;
            var result = store.FindClientByIdAsync("CLIENTID:" + 0);
            clientRecord = new ClientRecord(result.Result.ToClientHandle());


            Assert.AreEqual(clientRecord.Id, id);
        }
        /*
        [TestMethod]
        [DeploymentItem("source", "source")]

        public void TestGetScopesAsync_publicOnly()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

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
        [DeploymentItem("source", "source")]
        public void TestGetScopesAsync_all()
        {
            var targetFolder = Path.Combine(UnitTestHelpers.BaseDir, @"source");

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
         * */
    }
}
