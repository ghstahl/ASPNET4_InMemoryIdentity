using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.AspNet.Identity.Cassandra;
using P5.AspNet.Identity.Cassandra.DAO;

namespace AspNet.Identity.Cassandra.Tests
{
    [TestClass]
    public class RoleStoreTest
    {
        [TestMethod]
        public async Task Test_Add_Tenant_Role_Find_by_Name_Delete_All_Async()
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

            var roleStore = Global.TanantCassandraRoleStore;
            var userStore = Global.TanantCassandraUserStore;

            List<CassandraRole> inserted = new List<CassandraRole>();
            Guid userId = Guid.NewGuid();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                string roleName = Guid.NewGuid().ToString();
                var role = new CassandraRole()
                {
                    Name = roleName,
                    IsGlobal = false
                };
                inserted.Add(role);
                await roleStore.CreateAsync(role);
            }

            result = roleStore.Roles;
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());

            foreach (var item in inserted)
            {
                var resultRole = await roleStore.FindByNameAsync(item.Name);
                Assert.IsTrue(CassandraRoleComparer.Comparer.Equals(item,resultRole));
            }
            foreach (var item in inserted)
            {
                var resultRole = await roleStore.FindByIdAsync(item.Id);
                Assert.IsTrue(CassandraRoleComparer.Comparer.Equals(item, resultRole));
            }
            foreach (var item in inserted)
            {
                await roleStore.DeleteAsync(item);
            }
            
            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
        [TestMethod]
        public async Task Test_Add_Tenant_Role_Find_by_Id_Delete_All_Async()
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

            var roleStore = Global.TanantCassandraRoleStore;
            var userStore = Global.TanantCassandraUserStore;

            List<CassandraRole> inserted = new List<CassandraRole>();
            Guid userId = Guid.NewGuid();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                string roleName = Guid.NewGuid().ToString();
                var role = new CassandraRole()
                {
                    Name = roleName,
                    IsGlobal = false
                };
                inserted.Add(role);
                await roleStore.CreateAsync(role);
            }

            result = roleStore.Roles;
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());

            
            foreach (var item in inserted)
            {
                var resultRole = await roleStore.FindByIdAsync(item.Id);
                Assert.IsTrue(CassandraRoleComparer.Comparer.Equals(item, resultRole));
            }

            foreach (var item in inserted)
            {
                await roleStore.DeleteAsync(item);
            }

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
            var roleStore = Global.TanantCassandraRoleStore;
            var userStore = Global.TanantCassandraUserStore;

            await roleStore.CreateAsync(role);


            result = roleStore.Roles;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());

            var resultRole = await roleStore.FindByNameAsync(roleName);
            Assert.IsTrue(CassandraRoleComparer.Comparer.Equals(role, resultRole));



            var roleNew = role;
            roleNew.DisplayName = "I Like Cheese";


            await roleStore.UpdateAsync(roleNew);

            resultRole = await roleStore.FindByNameAsync(roleName);
            Assert.IsTrue(CassandraRoleComparer.Comparer.Equals(roleNew, resultRole));
          
            await dao.DeleteRolesByTenantIdAsync();
            result = await dao.FindRolesByTenantIdAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
        
    }
}