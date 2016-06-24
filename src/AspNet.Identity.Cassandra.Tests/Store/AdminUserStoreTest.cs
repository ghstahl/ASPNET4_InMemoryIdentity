using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.AspNet.Identity.Cassandra;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.Store.Core.Models;

namespace AspNet.Identity.Cassandra.Tests
{
    [TestClass]
    public class AdminUserStoreTest
    {


        [TestMethod]
        public async Task Test_add_many_claims_get_delete()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            var guidId = Guid.NewGuid();
            var insertedUsers = new List<CassandraUser>();

            var role = new CassandraRole()
            {
                Name = Guid.NewGuid().ToString(),
                IsGlobal = false,
            };


            var roleStore = Global.TanantCassandraRoleStore;
            var userStore = Global.TanantCassandraUserStore;

            await roleStore.CreateAsync(role);

            List<ClaimHandle> insertedClaimsHandles = new List<ClaimHandle>();


            string userName = Guid.NewGuid().ToString();
            var user = new CassandraUser()
            {
                Email = userName,
                UserName = userName
            };
            insertedUsers.Add(user);
            await userStore.CreateAsync(user);

            var providerLoginHandle = new ProviderLoginHandle()
            {
                LoginProvider = Guid.NewGuid().ToString(),
                ProviderKey = Guid.NewGuid().ToString(),
                UserId = user.Id
            };
            await
                userStore.AddLoginAsync(user,
                    new UserLoginInfo(providerLoginHandle.LoginProvider, providerLoginHandle.ProviderKey));
            int nCount = 100;
            for (int i = 0; i < nCount; ++i)
            {
                var claimType = Guid.NewGuid().ToString();
                var claimHandle = new ClaimHandle() {Type = claimType, UserId = user.Id, Value = "Value:" + claimType};
                await userStore.AddClaimAsync(user, new Claim(claimHandle.Type, claimHandle.Value));
                insertedClaimsHandles.Add(claimHandle);
            }

            await userStore.AddToRoleAsync(user, role.Name);
            var foundUserResult = await dao.FindUserByEmailAsync(user.Email);
            Assert.IsNotNull(foundUserResult);
            var foundUserList = foundUserResult.ToList();
            Assert.IsTrue(foundUserList.Any());
            Assert.AreEqual(foundUserList.Count, 1);
            var foundUser = foundUserList[0];

            byte[] pageState = null;
            int pageSize = 9;
            var result = await userStore.PageClaimsAsync(foundUser.Id, pageSize, pageState);
            pageState = result.PagingState;
            int nCounter = result.Count;
            while (pageState != null)
            {
                result = await userStore.PageClaimsAsync(foundUser.Id, pageSize, pageState);
                pageState = result.PagingState;
                nCounter += result.Count;
            }
            Assert.AreEqual(nCounter, nCount);

            var fetchClaims = await userStore.GetClaimsAsync(user.Id);
            Assert.IsNotNull(fetchClaims);
            var fetchClaimsList = fetchClaims.ToList();
            Assert.IsTrue(fetchClaimsList.Any());
            Assert.AreEqual(fetchClaimsList.Count, nCount);



            foreach (var testUser in insertedUsers)
            {
                await dao.DeleteUserAsync(testUser);
                foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                foundUserList = foundUserResult.ToList();
                Assert.IsFalse(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 0);

                var roleResult = await dao.FindRoleNamesByUserIdAsync(testUser.Id);
                Assert.IsNotNull(roleResult);
                var roleResultList = roleResult.ToList();
                Assert.IsFalse(roleResultList.Any());

                var claimResult = await dao.FindClaimHandleByUserIdAsync(testUser.Id);
                Assert.IsNotNull(claimResult);
                var claimResultList = claimResult.ToList();
                Assert.IsFalse(claimResultList.Any());

            }
            await roleStore.DeleteAsync(role);
            var findDeletedRole = await roleStore.FindByNameAsync(role.Name);
            Assert.IsNull(findDeletedRole);

            // make sure all the claims are gone
            pageState = null;
            pageSize = 9;
            result = await userStore.PageClaimsAsync(foundUser.Id, pageSize, pageState);
            pageState = result.PagingState;
            nCounter = result.Count;
            while (pageState != null)
            {
                result = await userStore.PageClaimsAsync(foundUser.Id, pageSize, pageState);
                pageState = result.PagingState;
                nCounter += result.Count;
            }
            Assert.AreEqual(nCounter, 0);

        }

        [TestMethod]
        public async Task Test_add_many_claims_page_delete()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            var guidId = Guid.NewGuid();
            var insertedUsers = new List<CassandraUser>();

            var role = new CassandraRole()
            {
                Name = Guid.NewGuid().ToString(),
                IsGlobal = false,
            };


            var roleStore = Global.TanantCassandraRoleStore;
            var userStore = Global.TanantCassandraUserStore;

            await roleStore.CreateAsync(role);

            List<ClaimHandle> insertedClaimsHandles = new List<ClaimHandle>();


            string userName = Guid.NewGuid().ToString();
            var user = new CassandraUser()
            {
                Email = userName,
                UserName = userName
            };
            insertedUsers.Add(user);
            await userStore.CreateAsync(user);

            var providerLoginHandle = new ProviderLoginHandle()
            {
                LoginProvider = Guid.NewGuid().ToString(),
                ProviderKey = Guid.NewGuid().ToString(),
                UserId = user.Id
            };
            await
                userStore.AddLoginAsync(user,
                    new UserLoginInfo(providerLoginHandle.LoginProvider, providerLoginHandle.ProviderKey));
            int nCount = 100;
            for (int i = 0; i < nCount; ++i)
            {
                var claimType = Guid.NewGuid().ToString();
                var claimHandle = new ClaimHandle() {Type = claimType, UserId = user.Id, Value = "Value:" + claimType};
                await userStore.AddClaimAsync(user, new Claim(claimHandle.Type, claimHandle.Value));
                insertedClaimsHandles.Add(claimHandle);
            }

            await userStore.AddToRoleAsync(user, role.Name);
            var foundUserResult = await dao.FindUserByEmailAsync(user.Email);
            Assert.IsNotNull(foundUserResult);
            var foundUserList = foundUserResult.ToList();
            Assert.IsTrue(foundUserList.Any());
            Assert.AreEqual(foundUserList.Count, 1);
            var foundUser = foundUserList[0];

            byte[] pageState = null;
            int pageSize = 9;
            var result = await userStore.PageClaimsAsync(foundUser.Id, pageSize, pageState);
            pageState = result.PagingState;
            int nCounter = result.Count;
            while (pageState != null)
            {
                result = await userStore.PageClaimsAsync(foundUser.Id, pageSize, pageState);
                pageState = result.PagingState;
                nCounter += result.Count;
            }
            Assert.AreEqual(nCounter, nCount);


            foreach (var testUser in insertedUsers)
            {
                await dao.DeleteUserAsync(testUser);
                foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                foundUserList = foundUserResult.ToList();
                Assert.IsFalse(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 0);

                var roleResult = await dao.FindRoleNamesByUserIdAsync(testUser.Id);
                Assert.IsNotNull(roleResult);
                var roleResultList = roleResult.ToList();
                Assert.IsFalse(roleResultList.Any());

                var claimResult = await dao.FindClaimHandleByUserIdAsync(testUser.Id);
                Assert.IsNotNull(claimResult);
                var claimResultList = claimResult.ToList();
                Assert.IsFalse(claimResultList.Any());

            }
            await roleStore.DeleteAsync(role);
            var findDeletedRole = await roleStore.FindByNameAsync(role.Name);
            Assert.IsNull(findDeletedRole);

            // make sure all the claims are gone
            pageState = null;
            pageSize = 9;
            result = await userStore.PageClaimsAsync(foundUser.Id, pageSize, pageState);
            pageState = result.PagingState;
            nCounter = result.Count;
            while (pageState != null)
            {
                result = await userStore.PageClaimsAsync(foundUser.Id, pageSize, pageState);
                pageState = result.PagingState;
                nCounter += result.Count;
            }
            Assert.AreEqual(nCounter, 0);

        }

        [TestMethod]
        public async Task Test_add_many_users_page_delete()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            var guidId = Guid.NewGuid();
            var insertedUsers = new List<CassandraUser>();

            var role = new CassandraRole()
            {
                Name = Guid.NewGuid().ToString(),
                IsGlobal = false,
            };


            var roleStore = Global.TanantCassandraRoleStore;
            var userStore = Global.TanantCassandraUserStore;

            await roleStore.CreateAsync(role);

            int nCount = 100;
            for (int i = 0; i < nCount; ++i)
            {
                string userName = Guid.NewGuid().ToString();
                var user = new CassandraUser()
                {
                    Email = userName,
                    UserName = userName
                };
                insertedUsers.Add(user);
                await userStore.CreateAsync(user);

                var providerLoginHandle = new ProviderLoginHandle()
                {
                    LoginProvider = Guid.NewGuid().ToString(),
                    ProviderKey = Guid.NewGuid().ToString(),
                    UserId = user.Id
                };
                await
                    userStore.AddLoginAsync(user,
                        new UserLoginInfo(providerLoginHandle.LoginProvider, providerLoginHandle.ProviderKey));


                var claimHandle = new ClaimHandle()
                {
                    Type = "Type:" + guidId,
                    UserId = user.Id,
                    Value = "Value:" + guidId
                };
                await userStore.AddClaimAsync(user, new Claim(claimHandle.Type, claimHandle.Value));

                await userStore.AddToRoleAsync(user, role.Name);
            }

            byte[] pageState = null;
            int pageSize = 9;
            var result = await userStore.PageUsersAsync(pageSize, pageState);
            pageState = result.PagingState;
            int nCounter = result.Count;
            while (pageState != null)
            {
                result = await userStore.PageUsersAsync(pageSize, pageState);
                pageState = result.PagingState;
                nCounter += result.Count;
            }
            Assert.AreEqual(nCounter, nCount);


            foreach (var testUser in insertedUsers)
            {
                await dao.DeleteUserAsync(testUser);
                var foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsFalse(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 0);

                var roleResult = await dao.FindRoleNamesByUserIdAsync(testUser.Id);
                Assert.IsNotNull(roleResult);
                var roleResultList = roleResult.ToList();
                Assert.IsFalse(roleResultList.Any());

                var claimResult = await dao.FindClaimHandleByUserIdAsync(testUser.Id);
                Assert.IsNotNull(claimResult);
                var claimResultList = claimResult.ToList();
                Assert.IsFalse(claimResultList.Any());

            }
            await roleStore.DeleteAsync(role);
            var findDeletedRole = await roleStore.FindByNameAsync(role.Name);
            Assert.IsNull(findDeletedRole);

        }
    }
}