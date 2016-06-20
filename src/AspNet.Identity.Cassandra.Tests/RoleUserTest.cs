using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.Store.Core.Models;

namespace AspNet.Identity.Cassandra.Tests
{
    [TestClass]
    public class RoleTest
    {
        [TestMethod]
        public async Task Test_Add_Gobal_Role_Delete_All_Async()
        {
            var globalDao = Global.GlobalTenantDao;
            await globalDao.EstablishConnectionAsync();


            await globalDao.DeleteRolesByTenantIdAsync(); // get rid of all global records
            var result = await globalDao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());


            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                string roleName = Guid.NewGuid().ToString();
                var role = new CassandraRole()
                {
                    Name = roleName
                };

                await dao.CreateRoleAsync(role);
            }

            result = await globalDao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());


            await globalDao.DeleteRolesByTenantIdAsync();
            result = await globalDao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
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
    }

    [TestClass]
    public class RoleUserTest
    {
        [TestMethod]
        public async Task Test_Add_Role_Delete_All_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                string roleName = Guid.NewGuid().ToString();
                await dao.AddToRoleAsync(userId, roleName);
            }

            var result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());


            await dao.DeleteUserFromRolesAsync(userId);
            result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task Test_Add_Role_Delete_Role_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            string roleName = Guid.NewGuid().ToString();
            await dao.AddToRoleAsync(userId, roleName);

            var result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());


            await dao.RemoveFromRoleAsync(userId, roleName);
            result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            var isThere = await dao.IsUserInRoleAsync(userId, roleName);
            Assert.IsFalse(isThere);
        }
        [TestMethod]
        public async Task Test_Find_Roles_For_UserId_No_Exist_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            var result = await dao.FindRoleNamesByUserIdAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task Test_Find_Roles_For_Good_UserId_No_Role_Exist_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                string roleName = Guid.NewGuid().ToString();
                await dao.AddToRoleAsync(userId, roleName);
            }

            var result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());

            result = await dao.FindRoleAsync(userId,"i don not exist");
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());

            await dao.DeleteUserFromRolesAsync(userId);
            result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task Test_Is_In_Role_No_UserId_No_Exist_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            var result = await dao.IsUserInRoleAsync(userId,"admin");

            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }
        [TestMethod]
        public async Task Test_Is_In_Role_Good_UserId_No_Exist_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                string roleName = Guid.NewGuid().ToString();
                await dao.AddToRoleAsync(userId, roleName);
            }

            var result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());

            var boolResult = await dao.IsUserInRoleAsync(userId, "admin");

            Assert.IsNotNull(boolResult);
            Assert.IsFalse(boolResult);

            await dao.DeleteUserFromRolesAsync(userId);
            result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
        [TestMethod]
        public async Task Test_Is_In_Role_Good_UserId_Does_Exist_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            List<string> roleNames = new List<string>();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                string roleName = Guid.NewGuid().ToString();
                roleNames.Add(roleName);
                await dao.AddToRoleAsync(userId, roleName);
            }

            var result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());

            foreach (var roleName in roleNames)
            {
                var boolResult = await dao.IsUserInRoleAsync(userId, roleName);
                Assert.IsNotNull(boolResult);
                Assert.IsTrue(boolResult);
            }

            await dao.DeleteUserFromRolesAsync(userId);
            result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
        [TestMethod]
        public async Task Test_Find_Role_Good_UserId_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            List<string> roleNames = new List<string>();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                string roleName = Guid.NewGuid().ToString();
                roleNames.Add(roleName);
                await dao.AddToRoleAsync(userId, roleName);
            }

            var result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());

            foreach (var roleName in roleNames)
            {
                result = await dao.FindRoleAsync(userId, roleName);
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count());
            }

            await dao.DeleteUserFromRolesAsync(userId);
            result = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
        [TestMethod]
        public async Task Test_Add_Roles_Pager_Delete_All_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            string userIdString = userId.ToString();
            int nCount = 1000;
            for (int i = 0; i < nCount; ++i)
            {
                string roleName = "Role:[" + i + "]:" + userIdString;
                await dao.AddToRoleAsync(userId, roleName);
            }
            byte[] pageState = null;
            int pageSize = 99;
            IPage<UserRoleHandle> result = await dao.PageUserRolesAsync(userId, pageSize, pageState);
            pageState = result.PagingState;
            int nCounter = result.Count;
            while (pageState != null)
            {
                result = await dao.PageUserRolesAsync(userId, pageSize, pageState);
                pageState = result.PagingState;
                nCounter += result.Count;
            }
            Assert.AreEqual(nCounter, nCount);


            await dao.DeleteUserFromRolesAsync(userId);
            var findResult = await dao.FindRoleNamesByUserIdAsync(userId);
            Assert.IsNotNull(findResult);
            Assert.AreEqual(0, findResult.Count());
        }
    }
}