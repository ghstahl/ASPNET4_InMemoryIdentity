using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.AspNet.Identity.Cassandra;
using P5.AspNet.Identity.Cassandra.DAO;

namespace AspNet.Identity.Cassandra.Tests
{
    [TestClass]
    public class UserStoreTest
    {
        [TestMethod]
        public async Task Test_Add_Full_User_Find_by_all_Delete_User_Async()
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


            var roleStore = new CassandraRoleStore();
            var userStore = new CassandraUserStore();

            await roleStore.CreateAsync(role);
 
            int nCount = 1;
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
               

                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = user.Id, Value = "Value:" + guidId };
                await userStore.AddClaimAsync(user, new Claim(claimHandle.Type,claimHandle.Value));

                await userStore.AddToRoleAsync(user, role.Name);
            }


            foreach (var testUser in insertedUsers)
            {
                testUser.TenantId = dao.TenantId;

                var foundUser = await userStore.FindByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUser);
                Assert.IsTrue(CassandraUserComparer.ShallowComparer.Equals(foundUser, testUser));

                foundUser = await userStore.FindByNameAsync(testUser.UserName);
                Assert.IsNotNull(foundUser);
                Assert.IsTrue(CassandraUserComparer.ShallowComparer.Equals(foundUser, testUser));

                var roleResult = await userStore.GetRolesAsync(foundUser);
                Assert.IsNotNull(roleResult);

                Assert.AreEqual(1, roleResult.Count());
                var foundRole = roleResult[0];
                Assert.AreEqual(foundRole, role.Name);

                var claimResult = await userStore.GetClaimsAsync(foundUser);
                Assert.IsNotNull(claimResult);
                Assert.AreEqual(1, claimResult.Count());
                var foundClaim = claimResult[0];

                var foundClaimHandle = foundClaim.ToClaimHandle(foundUser.Id);
                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = testUser.Id, Value = "Value:" + guidId };
                Assert.IsTrue(ClaimHandleComparer.Comparer.Equals(foundClaimHandle, claimHandle));
            }



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