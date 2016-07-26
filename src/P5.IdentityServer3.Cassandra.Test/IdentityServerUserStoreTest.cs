using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.MSTest.Common;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class IdentityServerUserStoreTest : TestBase
    {
        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }

        [TestInitialize]
        public async void Setup()
        {
            base.Setup();
            //       await dao.CreateTablesAsync();
            //       await dao.TruncateTablesAsync();
        }
        [TestMethod]
        public async Task Test_Find_No_IdentityServerUserAsync()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();
            var result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);
            Assert.IsNull(result);
        }
        [TestMethod]
        public async Task Test_Create_IdentityServerUserAsync()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();
            var user = new IdentityServerUser { Enabled = true, UserId = userId, UserName = "Herb-" + userId };
            var result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);
            Assert.IsNull(result);
            await adminStore.CreateIdentityServerUserAsync(user);
            result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);
            Assert.IsNotNull(result);



            Assert.AreEqual(user.UserId, result.UserId);
        }
        [TestMethod]
        public async Task Test_Create_IdentityServerUser_Find_No_AllowedScopes_Async()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();
            var user = new IdentityServerUser { Enabled = true, UserId = userId, UserName = "Herb-" + userId };

            await adminStore.CreateIdentityServerUserAsync(user);

            var result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);

            Assert.AreEqual(user.UserId, result.UserId);
            var scopes = await adminStore.FindScopesByUserAsync(userId);
            Assert.IsTrue(scopes.Count() == 0);
        }
        [TestMethod]
        public async Task Test_Add_AllowedScopes_To_NonExisting_User_Async()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();

            List<string> scopesToAdd = new List<string> { "scope1", "scope2" };
            var appliedInfo = await adminStore.AddScopesToIdentityServerUserAsync(userId, scopesToAdd);
            Assert.IsFalse(appliedInfo.Applied);
            Assert.IsNotNull(appliedInfo.Exception);
            Assert.IsNotNull(appliedInfo.Exception as UserDoesNotExitException);
        }
        [TestMethod]
        public async Task Test_Add_ClientIds_To_NonExisting_User_Async()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();

            List<string> clientIdsToAdd = new List<string> { "clientid1", "clientid2" };
            var appliedInfo = await adminStore.AddClientIdToIdentityServerUserAsync(userId, clientIdsToAdd);
            Assert.IsFalse(appliedInfo.Applied);
            Assert.IsNotNull(appliedInfo.Exception);
            Assert.IsNotNull(appliedInfo.Exception as UserDoesNotExitException);
        }
        [TestMethod]
        public async Task Test_Create_IdentityServerUser_Add_AllowedScopes_Async()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();
            var user = new IdentityServerUser { Enabled = true, UserId = userId, UserName = "Herb-" + userId };

            await adminStore.CreateIdentityServerUserAsync(user);

            var result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);

            Assert.AreEqual(user.UserId, result.UserId);
            List<string> scopesToAdd = new List<string> {"scope1","scope2"};
            await adminStore.AddScopesToIdentityServerUserAsync(userId, scopesToAdd);
            var scopes = await adminStore.FindScopesByUserAsync(userId);
            Assert.AreEqual(scopes.Count() , scopesToAdd.Count);
            var finalList = scopes.ToList().Except(scopesToAdd);
            Assert.IsFalse(finalList.Any());
        }
        [TestMethod]
        public async Task Test_Create_IdentityServerUser_Add_AND_Delete_AllowedScopes_Async()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();
            var user = new IdentityServerUser { Enabled = true, UserId = userId, UserName = "Herb-" + userId };

            await adminStore.CreateIdentityServerUserAsync(user);

            var result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);

            Assert.AreEqual(user.UserId, result.UserId);
            List<string> scopesToAdd = new List<string> { "scope1", "scope2", "scope3", "scope4" };
            await adminStore.AddScopesToIdentityServerUserAsync(userId, scopesToAdd);
            var scopes = await adminStore.FindScopesByUserAsync(userId);
            Assert.AreEqual(scopes.Count(), scopesToAdd.Count);
            var finalList = scopes.ToList().Except(scopesToAdd);
            Assert.IsFalse(finalList.Any());


            List<string> scopesToDelete = new List<string> { "scope1", "scope2" };
            await adminStore.DeleteScopesByUserIdAsync(userId, scopesToDelete);
            scopes = await adminStore.FindScopesByUserAsync(userId);
            var finalExpectedList = scopesToAdd.ToList().Except(scopesToDelete);
            Assert.AreEqual(scopes.Count(), finalExpectedList.Count());
            finalList = scopes.ToList().Except(finalExpectedList);
            Assert.IsFalse(finalList.Any());
        }


        [TestMethod]
        public async Task Test_Create_IdentityServerUser_Find_No_ClientIds_Async()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();
            var user = new IdentityServerUser { Enabled = true, UserId = userId, UserName = "Herb-" + userId };

            await adminStore.CreateIdentityServerUserAsync(user);

            var result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);

            Assert.AreEqual(user.UserId, result.UserId);
            var scopes = await adminStore.FindClientIdsByUserAsync(userId);
            Assert.IsTrue(scopes.Count() == 0);
        }
        [TestMethod]
        public async Task Test_Create_IdentityServerUser_Add_ClientIds_Async()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();
            var user = new IdentityServerUser { Enabled = true, UserId = userId, UserName = "Herb-" + userId };

            await adminStore.CreateIdentityServerUserAsync(user);

            var result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);
            Assert.AreEqual(user.UserId, result.UserId);

            List<string> clientIdsToAdd = new List<string> { "clientid1", "clientid2" };
            await adminStore.AddClientIdToIdentityServerUserAsync(userId, clientIdsToAdd);
            var clientIds = await adminStore.FindClientIdsByUserAsync(userId);
            Assert.AreEqual(clientIds.Count(), clientIdsToAdd.Count);
            var finalList = clientIds.ToList().Except(clientIdsToAdd);
            Assert.IsFalse(finalList.Any());
        }
        [TestMethod]
        public async Task Test_Create_IdentityServerUser_Add_AND_Delete_ClientIds_Async()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();
            var user = new IdentityServerUser { Enabled = true, UserId = userId, UserName = "Herb-" + userId };

            await adminStore.CreateIdentityServerUserAsync(user);

            var result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);
            Assert.AreEqual(user.UserId, result.UserId);

            List<string> clientIdsToAdd = new List<string> { "clientid1", "clientid2", "clientid3", "clientid4" };
            await adminStore.AddClientIdToIdentityServerUserAsync(userId, clientIdsToAdd);
            var clientIds = await adminStore.FindClientIdsByUserAsync(userId);
            Assert.AreEqual(clientIds.Count(), clientIdsToAdd.Count);
            var finalList = clientIds.ToList().Except(clientIdsToAdd);
            Assert.IsFalse(finalList.Any());


            List<string> clientsToDelete = new List<string> { "clientid1", "clientid2" };
            await adminStore.DeleteClientIdsByUserIdAsync(userId, clientsToDelete);
            clientIds = await adminStore.FindClientIdsByUserAsync(userId);
            var finalExpectedList = clientIdsToAdd.ToList().Except(clientsToDelete).ToList();
            Assert.AreEqual(clientIds.Count(), finalExpectedList.Count());
            finalList = clientIds.ToList().Except(finalExpectedList);
            Assert.IsFalse(finalList.Any());
        }
        [TestMethod]
        public async Task Test_Create_And_Delete_IdentityServerUserAsync()
        {
            var userId = Guid.NewGuid().ToString();
            var adminStore = new IdentityServer3AdminStore();
            var user = new IdentityServerUser { Enabled = true, UserId = userId, UserName = "Herb-" + userId };

            var appliedInfo = await adminStore.CreateIdentityServerUserAsync(user);
            Assert.IsTrue(appliedInfo.Applied);
            Assert.IsNull(appliedInfo.Exception);
            var result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);
            Assert.AreEqual(user.UserId, result.UserId);

            List<string> clientIdsToAdd = new List<string> { "clientid1", "clientid2", "clientid3", "clientid4" };
            await adminStore.AddClientIdToIdentityServerUserAsync(userId, clientIdsToAdd);
            var clientIds = await adminStore.FindClientIdsByUserAsync(userId);
            Assert.AreEqual(clientIds.Count(), clientIdsToAdd.Count);
            var finalList = clientIds.ToList().Except(clientIdsToAdd);
            Assert.IsFalse(finalList.Any());

            List<string> scopesToAdd = new List<string> { "scope1", "scope2", "scope3", "scope4" };
            await adminStore.AddScopesToIdentityServerUserAsync(userId, scopesToAdd);
            var scopes = await adminStore.FindScopesByUserAsync(userId);
            Assert.AreEqual(scopes.Count(), scopesToAdd.Count);
            finalList = scopes.ToList().Except(scopesToAdd);
            Assert.IsFalse(finalList.Any());

            appliedInfo = await adminStore.DeleteIdentityServerUserAsync(userId);
            Assert.IsTrue(appliedInfo.Applied);
            Assert.IsNull(appliedInfo.Exception);
            result = await adminStore.FindIdentityServerUserByUserIdAsync(userId);
            Assert.IsNull(result);
        }

    }
}