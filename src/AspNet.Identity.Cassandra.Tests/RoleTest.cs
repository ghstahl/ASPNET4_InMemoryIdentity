using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.AspNet.Identity.Cassandra.DAO;

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