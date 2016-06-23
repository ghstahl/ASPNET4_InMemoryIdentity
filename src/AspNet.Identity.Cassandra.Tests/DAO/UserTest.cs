using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.AspNet.Identity.Cassandra.DAO;

namespace AspNet.Identity.Cassandra.Tests
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public async Task Test_Add_Full_User_Find_by_email_rename_Delete_User_Async()
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
            await dao.CreateRoleAsync(role);

            var roleNameResult = await dao.FindRoleByNameAsync(role.Name);
            Assert.IsNotNull(roleNameResult);
            var foundRoleNameResult = roleNameResult.ToList();
            Assert.IsTrue(foundRoleNameResult.Any());
            Assert.AreEqual(role.Name, foundRoleNameResult[0].Name);

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
                await dao.UpsertUserAsync(user);


                var providerLoginHandle = new ProviderLoginHandle()
                {
                    LoginProvider = Guid.NewGuid().ToString(),
                    ProviderKey = Guid.NewGuid().ToString(),
                    UserId = user.Id
                };
                await dao.UpsertLoginsAsync(providerLoginHandle);
                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = user.Id, Value = "Value:" + guidId };
                await dao.CreateClaimAsync(claimHandle);

                await dao.AddToRoleAsync(user.Id, role.Name);
            }


            foreach (var testUser in insertedUsers)
            {
                testUser.TenantId = dao.TenantId;

                var foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsTrue(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 1);
                var foundUser = foundUserList[0];
                Assert.IsTrue(CassandraUserComparer.ShallowComparer.Equals(foundUser, testUser));

                  foundUserResult = await dao.FindUserByUserNameAsync(testUser.UserName);
                Assert.IsNotNull(foundUserResult);
                 foundUserList = foundUserResult.ToList();
                Assert.IsTrue(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 1);
                 foundUser = foundUserList[0];
                 Assert.IsTrue(CassandraUserComparer.ShallowComparer.Equals(foundUser, testUser));

                 var roleResult = await dao.FindRoleNamesByUserIdAsync(testUser.Id);
                Assert.IsNotNull(roleResult);
                var roleResultList = roleResult.ToList();
                Assert.AreEqual(1, roleResultList.Count());
                var foundRole = roleResultList[0];
                Assert.AreEqual(foundRole, role.Name);

                var claimResult = await dao.FindClaimHandleByUserIdAsync(testUser.Id);
                Assert.IsNotNull(claimResult);
                var claimResultList = claimResult.ToList();
                Assert.AreEqual(1, claimResultList.Count());
                var foundClaim = claimResultList[0];

                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = testUser.Id, Value = "Value:" + guidId };
                Assert.IsTrue(ClaimHandleComparer.Comparer.Equals(foundClaim, claimHandle));
            }
            List<CassandraUser> newUserList = new List<CassandraUser>();
            foreach (var testUser in insertedUsers)
            {
                var newUserName = "Herb_" + testUser.UserName;
                await dao.ChangeUserNameAsync(testUser.UserName, newUserName);
                var foundUserResult = await dao.FindUserByUserNameAsync(newUserName);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsTrue(foundUserList.Any());
                newUserList.Add(foundUserList[0]);
            }

            foreach (var testUser in insertedUsers)
            {
                testUser.TenantId = dao.TenantId;

                // this should now be gone
                var foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsFalse(foundUserList.Any());

                // this should now be gone
                foundUserResult = await dao.FindUserByUserNameAsync(testUser.UserName);
                Assert.IsNotNull(foundUserResult);
                foundUserList = foundUserResult.ToList();
                Assert.IsFalse(foundUserList.Any());

                // this userid should still be good
                var roleResult = await dao.FindRoleNamesByUserIdAsync(testUser.Id);
                Assert.IsNotNull(roleResult);
                var roleResultList = roleResult.ToList();
                Assert.AreEqual(1, roleResultList.Count());
                var foundRole = roleResultList[0];
                Assert.AreEqual(foundRole, role.Name);

                // this userid should still be good
                var claimResult = await dao.FindClaimHandleByUserIdAsync(testUser.Id);
                Assert.IsNotNull(claimResult);
                var claimResultList = claimResult.ToList();
                Assert.AreEqual(1, claimResultList.Count());
                var foundClaim = claimResultList[0];

                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = testUser.Id, Value = "Value:" + guidId };
                Assert.IsTrue(ClaimHandleComparer.Comparer.Equals(foundClaim, claimHandle));
            }

            foreach (var testUser in newUserList)
            {
                testUser.TenantId = dao.TenantId;

                var foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsTrue(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 1);
                var foundUser = foundUserList[0];
                Assert.IsTrue(CassandraUserComparer.ShallowComparer.Equals(foundUser, testUser));

                foundUserResult = await dao.FindUserByUserNameAsync(testUser.UserName);
                Assert.IsNotNull(foundUserResult);
                foundUserList = foundUserResult.ToList();
                Assert.IsTrue(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 1);
                foundUser = foundUserList[0];
                Assert.IsTrue(CassandraUserComparer.ShallowComparer.Equals(foundUser, testUser));

                var roleResult = await dao.FindRoleNamesByUserIdAsync(testUser.Id);
                Assert.IsNotNull(roleResult);
                var roleResultList = roleResult.ToList();
                Assert.AreEqual(1, roleResultList.Count());
                var foundRole = roleResultList[0];
                Assert.AreEqual(foundRole, role.Name);

                var claimResult = await dao.FindClaimHandleByUserIdAsync(testUser.Id);
                Assert.IsNotNull(claimResult);
                var claimResultList = claimResult.ToList();
                Assert.AreEqual(1, claimResultList.Count());
                var foundClaim = claimResultList[0];

                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = testUser.Id, Value = "Value:" + guidId };
                Assert.IsTrue(ClaimHandleComparer.Comparer.Equals(foundClaim, claimHandle));
            }
            foreach (var testUser in newUserList)
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
            await dao.DeleteRoleAsync(role);
            roleNameResult = await dao.FindRoleByNameAsync(role.Name);
            Assert.IsNotNull(roleNameResult);
            foundRoleNameResult = roleNameResult.ToList();
            Assert.IsFalse(foundRoleNameResult.Any());

        }
        [TestMethod]
        public async Task Test_Add_Full_User_Find_by_email_Delete_User_Async()
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
            await dao.CreateRoleAsync(role);

            var roleNameResult = await dao.FindRoleByNameAsync(role.Name);
            Assert.IsNotNull(roleNameResult);
            var foundRoleNameResult = roleNameResult.ToList();
            Assert.IsTrue(foundRoleNameResult.Any());
            Assert.AreEqual(role.Name, foundRoleNameResult[0].Name);

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
                await dao.UpsertUserAsync(user);
                
               
                var providerLoginHandle = new ProviderLoginHandle()
                {
                    LoginProvider = Guid.NewGuid().ToString(),
                    ProviderKey = Guid.NewGuid().ToString(),
                    UserId = user.Id
                };
                await dao.UpsertLoginsAsync(providerLoginHandle);
                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = user.Id, Value = "Value:" + guidId };
                await dao.CreateClaimAsync(claimHandle);

                await dao.AddToRoleAsync(user.Id, role.Name);
            }


            foreach (var testUser in insertedUsers)
            {
                testUser.TenantId = dao.TenantId;
                var foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsTrue(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 1);
                var foundUser = foundUserList[0];
                Assert.IsTrue(CassandraUserComparer.ShallowComparer.Equals(foundUser, testUser));

                var roleResult = await dao.FindRoleNamesByUserIdAsync(testUser.Id);
                Assert.IsNotNull(roleResult);
                var roleResultList = roleResult.ToList();
                Assert.AreEqual(1, roleResultList.Count());
                var foundRole = roleResultList[0];
                Assert.AreEqual(foundRole, role.Name);

                var claimResult = await dao.FindClaimHandleByUserIdAsync(testUser.Id);
                Assert.IsNotNull(claimResult);
                var claimResultList = claimResult.ToList();
                Assert.AreEqual(1, claimResultList.Count());
                var foundClaim = claimResultList[0];

                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = testUser.Id, Value = "Value:" + guidId };
                Assert.IsTrue(ClaimHandleComparer.Comparer.Equals(foundClaim, claimHandle));
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
            await dao.DeleteRoleAsync(role);
            roleNameResult = await dao.FindRoleByNameAsync(role.Name);
            Assert.IsNotNull(roleNameResult);
            foundRoleNameResult = roleNameResult.ToList();
            Assert.IsFalse(foundRoleNameResult.Any());
           
        }
        [TestMethod]
        public async Task Test_Add_User_Find_by_email_Delete_User_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            List<CassandraUser> insertedUsers = new List<CassandraUser>();
            int nCount = 1;
            for (int i = 0; i < nCount; ++i)
            {
                string userName = Guid.NewGuid().ToString();
                var user = new CassandraUser()
                {
                    Email = userName,
                    UserName = userName,
                    AccessFailedCount = 123,
                    EmailConfirmed = true,
                    Enabled = true,
                    LockoutEnabled = true,
                    PasswordHash = Guid.NewGuid().ToString(),
                    PhoneNumber = Guid.NewGuid().ToString(),
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Source = Guid.NewGuid().ToString(),
                    SourceId = Guid.NewGuid().ToString(),
                    TwoFactorEnabled = true
                };
                insertedUsers.Add(user);
                await dao.UpsertUserAsync(user);
            }

            foreach (var testUser in insertedUsers)
            {
                testUser.TenantId = dao.TenantId;
                var foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsTrue(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count,1);
                var foundUser = foundUserList[0];
                Assert.IsTrue(CassandraUserComparer.ShallowComparer.Equals(foundUser, testUser));
            }

            foreach (var testUser in insertedUsers)
            {
                await dao.DeleteUserAsync(testUser);
                var foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsFalse(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 0);
            }          
        }

        [TestMethod]
        public async Task Test_Add_User_Find_by_username_Delete_User_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            List<CassandraUser> insertedUsers = new List<CassandraUser>();
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
                await dao.UpsertUserAsync(user);
            }

            foreach (var testUser in insertedUsers)
            {
                testUser.TenantId = dao.TenantId;
                var foundUserResult = await dao.FindUserByUserNameAsync(testUser.UserName);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsTrue(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 1);
                var foundUser = foundUserList[0];
                Assert.IsTrue(CassandraUserComparer.ShallowComparer.Equals(foundUser, testUser));
            }

            foreach (var testUser in insertedUsers)
            {
                await dao.DeleteUserAsync(testUser);
                var foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsFalse(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 0);
            }
        }

        [TestMethod]
        public async Task Test_Add_User_Find_by_userid_Delete_User_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            List<CassandraUser> insertedUsers = new List<CassandraUser>();
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
                await dao.UpsertUserAsync(user);
            }

            foreach (var testUser in insertedUsers)
            {
                testUser.TenantId = dao.TenantId;
                var foundUserResult = await dao.FindUserByIdAsync(testUser.Id);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsTrue(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 1);
                var foundUser = foundUserList[0];
                Assert.IsTrue(CassandraUserComparer.ShallowComparer.Equals(foundUser, testUser));
            }

            foreach (var testUser in insertedUsers)
            {
                await dao.DeleteUserAsync(testUser);
                var foundUserResult = await dao.FindUserByEmailAsync(testUser.Email);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsFalse(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 0);
            }
        }
 
    }
}