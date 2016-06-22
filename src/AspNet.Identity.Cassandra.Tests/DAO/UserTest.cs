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
        public async Task Test_Add_Full_User_Find_by_email_Delete_User_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            var guidId = Guid.NewGuid();
            var insertedUsers = new List<CassandraUserHandle>();
            
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
                var user = new CassandraUserHandle()
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
                    UserId = user.UserId
                };
                await dao.UpsertLoginsAsync(providerLoginHandle);
                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = user.UserId, Value = "Value:" + guidId };
                await dao.CreateClaimAsync(claimHandle);

                await dao.AddToRoleAsync(user.UserId, role.Name);
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
                Assert.IsTrue(CassandraUserHandleComparer.ShallowComparer.Equals(foundUser, testUser));

                var roleResult = await dao.FindRoleNamesByUserIdAsync(testUser.UserId);
                Assert.IsNotNull(roleResult);
                var roleResultList = roleResult.ToList();
                Assert.AreEqual(1, roleResultList.Count());
                var foundRole = roleResultList[0];
                Assert.AreEqual(foundRole, role.Name);

                var claimResult = await dao.FindClaimHandleByUserIdAsync(testUser.UserId);
                Assert.IsNotNull(claimResult);
                var claimResultList = claimResult.ToList();
                Assert.AreEqual(1, claimResultList.Count());
                var foundClaim = claimResultList[0];
                
                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = testUser.UserId, Value = "Value:" + guidId };
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

                var roleResult = await dao.FindRoleNamesByUserIdAsync(testUser.UserId);
                Assert.IsNotNull(roleResult);
                var roleResultList = roleResult.ToList();
                Assert.IsFalse(roleResultList.Any());

                var claimResult = await dao.FindClaimHandleByUserIdAsync(testUser.UserId);
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
            
            List<CassandraUserHandle> insertedUsers = new List<CassandraUserHandle>();
            int nCount = 1;
            for (int i = 0; i < nCount; ++i)
            {
                string userName = Guid.NewGuid().ToString();
                var user = new CassandraUserHandle()
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
                Assert.IsTrue(CassandraUserHandleComparer.ShallowComparer.Equals(foundUser, testUser));
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

            List<CassandraUserHandle> insertedUsers = new List<CassandraUserHandle>();
            int nCount = 1;
            for (int i = 0; i < nCount; ++i)
            {
                string userName = Guid.NewGuid().ToString();
                var user = new CassandraUserHandle()
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
                Assert.IsTrue(CassandraUserHandleComparer.ShallowComparer.Equals(foundUser, testUser));
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

            List<CassandraUserHandle> insertedUsers = new List<CassandraUserHandle>();
            int nCount = 1;
            for (int i = 0; i < nCount; ++i)
            {
                string userName = Guid.NewGuid().ToString();
                var user = new CassandraUserHandle()
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
                var foundUserResult = await dao.FindUserByIdAsync(testUser.UserId);
                Assert.IsNotNull(foundUserResult);
                var foundUserList = foundUserResult.ToList();
                Assert.IsTrue(foundUserList.Any());
                Assert.AreEqual(foundUserList.Count, 1);
                var foundUser = foundUserList[0];
                Assert.IsTrue(CassandraUserHandleComparer.ShallowComparer.Equals(foundUser, testUser));
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
        public async Task Test_Add_Tenant_Role_Delete_All_Async()
        {
            var globalDao = Global.GlobalTenantDao;
            await globalDao.EstablishConnectionAsync();

            await globalDao.DeleteRolesByTenantIdAsync(); // get rid of all global records
            var result = await globalDao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());


            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();
            await dao.DeleteRolesByTenantIdAsync(); // get rid of all global records
            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            Guid userId = Guid.NewGuid();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                string roleName = Guid.NewGuid().ToString();
                var role = new CassandraRole()
                {
                    Name = roleName,IsGlobal = false
                };

                await dao.CreateRoleAsync(role);
            }
            result = await globalDao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());


            await dao.DeleteRolesByTenantIdAsync();
            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task Test_Add_Tenant_Role_Update_Delete_All_Async()
        {
            var globalDao = Global.GlobalTenantDao;
            await globalDao.EstablishConnectionAsync();

            await globalDao.DeleteRolesByTenantIdAsync(); // get rid of all global records
            var result = await globalDao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());


            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();
            await dao.DeleteRolesByTenantIdAsync(); // get rid of all global records
            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            Guid userId = Guid.NewGuid();
            string roleName = Guid.NewGuid().ToString();
            var role = new CassandraRole()
            {
                Name = roleName,
                IsGlobal = false
            };

            await dao.CreateRoleAsync(role);


            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());

            result = await dao.FindRoleByNameAsync(roleName);
            Assert.IsNotNull(result);
            var roleResult = result.ToList();
            Assert.AreEqual(1, roleResult.Count());
            Assert.AreEqual(roleName, roleResult[0].Name);

            var roleNew = roleResult[0];
            roleNew.DisplayName = "I Like Cheese";

             
            await dao.UpdateRoleAsync(roleNew);

            result = await dao.FindRoleByNameAsync(roleNew.Name);
            Assert.IsNotNull(result);
            roleResult = result.ToList();
            Assert.AreEqual(1, roleResult.Count());
            Assert.IsTrue(CassandraRoleComparer.Comparer.Equals(roleNew, roleResult[0]));
          
            await dao.DeleteRolesByTenantIdAsync();
            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
        
        [TestMethod]
        public async Task Test_Add_Tenant_Role_Update_Change_Name_Delete_All_Async()
        {
            var globalDao = Global.GlobalTenantDao;
            await globalDao.EstablishConnectionAsync();

            await globalDao.DeleteRolesByTenantIdAsync(); // get rid of all global records
            var result = await globalDao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());


            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();
            await dao.DeleteRolesByTenantIdAsync(); // get rid of all global records
            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            Guid userId = Guid.NewGuid();
            string roleName = Guid.NewGuid().ToString();
            var role = new CassandraRole()
            {
                Name = roleName,
                IsGlobal = false
            };

            await dao.CreateRoleAsync(role);


            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());

            result = await dao.FindRoleByNameAsync(roleName);
            Assert.IsNotNull(result);
            var roleResult = result.ToList();
            Assert.AreEqual(1, roleResult.Count());
            Assert.AreEqual(roleName, roleResult[0].Name);

            var roleNew = roleResult[0];
            roleNew.DisplayName = "I Like Cheese";
            roleNew.Name = Guid.NewGuid().ToString();

            await dao.UpdateRoleAsync(roleNew);

            result = await dao.FindRoleByNameAsync(roleNew.Name);
            Assert.IsNotNull(result);
            roleResult = result.ToList();
            Assert.AreEqual(1, roleResult.Count());
            Assert.IsTrue(CassandraRoleComparer.Comparer.Equals(roleNew, roleResult[0]));

            await dao.DeleteRolesByTenantIdAsync();
            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
       
    }
}