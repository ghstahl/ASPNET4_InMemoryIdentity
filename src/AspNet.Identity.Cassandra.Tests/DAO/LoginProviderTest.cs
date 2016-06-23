using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.AspNet.Identity.Cassandra.DAO;

namespace AspNet.Identity.Cassandra.Tests
{
    [TestClass]
    public class LoginProviderTest
    {
        [TestMethod]
        public async Task Test_Add_LoginProviders_Find_by_userid_Remove_by_userId_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            List<ProviderLoginHandle> inserted = new List<ProviderLoginHandle>();
            var userId = Guid.NewGuid();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                var providerLoginHandle = new ProviderLoginHandle()
                {
                    LoginProvider = Guid.NewGuid().ToString(),
                    ProviderKey = Guid.NewGuid().ToString(),
                    UserId = userId
                };
                inserted.Add(providerLoginHandle);
                await dao.UpsertLoginsAsync(providerLoginHandle);
            }

            foreach (var item in inserted)
            {
                item.TenantId = dao.TenantId;
                var foundResult = await dao.FindLoginByProviderAsync(item.LoginProvider, item.ProviderKey);
                Assert.IsNotNull(foundResult);
                var foundList = foundResult.ToList();
                Assert.IsTrue(foundList.Any());
                Assert.AreEqual(foundList.Count, 1);
                var found = foundList[0];
                Assert.IsTrue(ProviderLoginHandleComparer.Comparer.Equals(found, item));
            }

            var foundByUserIdResult = await dao.FindLoginsByUserIdAsync(userId);
            Assert.IsNotNull(foundByUserIdResult);
            var foundByUserIdResultList = foundByUserIdResult.ToList();
            Assert.IsTrue(foundByUserIdResultList.Any());
            Assert.AreEqual(foundByUserIdResultList.Count, nCount);


            await dao.RemoveLoginsFromUserAsync(userId);
             foundByUserIdResult = await dao.FindLoginsByUserIdAsync(userId);
            Assert.IsNotNull(foundByUserIdResult);
             foundByUserIdResultList = foundByUserIdResult.ToList();
            Assert.IsFalse(foundByUserIdResultList.Any());
        }

        [TestMethod]
        public async Task Test_Add_LoginProviders_Find_by_userid_Delete_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            List<ProviderLoginHandle> inserted = new List<ProviderLoginHandle>();
            var userId = Guid.NewGuid();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                
                var providerLoginHandle = new ProviderLoginHandle()
                {
                    LoginProvider = Guid.NewGuid().ToString(),
                    ProviderKey = Guid.NewGuid().ToString(),
                    UserId = userId
                };
                inserted.Add(providerLoginHandle);
                await dao.UpsertLoginsAsync(providerLoginHandle);
            }

            foreach (var item in inserted)
            {
                item.TenantId = dao.TenantId;
                var foundResult = await dao.FindLoginByProviderAsync(item.LoginProvider, item.ProviderKey);
                Assert.IsNotNull(foundResult);
                var foundList = foundResult.ToList();
                Assert.IsTrue(foundList.Any());
                Assert.AreEqual(foundList.Count, 1);
                var found = foundList[0];
                Assert.IsTrue(ProviderLoginHandleComparer.Comparer.Equals(found, item));
            }

            var foundByUserIdResult = await dao.FindLoginsByUserIdAsync(userId);
            Assert.IsNotNull(foundByUserIdResult);
            var foundByUserIdResultList = foundByUserIdResult.ToList();
            Assert.IsTrue(foundByUserIdResultList.Any());
            Assert.AreEqual(foundByUserIdResultList.Count, nCount);
            var expectedCount = nCount;
            foreach (var item in inserted)
            {
                await dao.DeleteLoginsAsync(item);
                --expectedCount;
                var foundResult = await dao.FindLoginsByUserIdAsync(item.UserId);
                Assert.IsNotNull(foundResult);
                var foundList = foundResult.ToList();
                Assert.AreEqual(foundList.Count, expectedCount);
            }
        }
        [TestMethod]
        public async Task Test_Add_LoginProvider_Find_by_userid_Delete_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            List<ProviderLoginHandle> inserted = new List<ProviderLoginHandle>();
            int nCount = 1;
            for (int i = 0; i < nCount; ++i)
            {
                string userName = Guid.NewGuid().ToString();
                var providerLoginHandle = new ProviderLoginHandle()
                {
                   LoginProvider = Guid.NewGuid().ToString(),
                   ProviderKey = Guid.NewGuid().ToString(),
                   UserId = Guid.NewGuid()
                };
                inserted.Add(providerLoginHandle);
                await dao.UpsertLoginsAsync(providerLoginHandle);
            }

            foreach (var item in inserted)
            {
                item.TenantId = dao.TenantId;
                var foundResult = await dao.FindLoginsByUserIdAsync(item.UserId);
                Assert.IsNotNull(foundResult);
                var foundList = foundResult.ToList();
                Assert.IsTrue(foundList.Any());
                Assert.AreEqual(foundList.Count, 1);
                var found = foundList[0];
                Assert.IsTrue(ProviderLoginHandleComparer.Comparer.Equals(found, item));
            }

            foreach (var item in inserted)
            {
                await dao.DeleteLoginsAsync(item);
                var foundResult = await dao.FindLoginsByUserIdAsync(item.UserId);
                Assert.IsNotNull(foundResult);
                var foundList = foundResult.ToList();
                Assert.IsFalse(foundList.Any());
                Assert.AreEqual(foundList.Count, 0);
              
            }          
        }

        [TestMethod]
        public async Task Test_Add_LoginProvider_Find_by_LoginProvider_Delete_Async()
        {
            var dao = Global.TenantDao;
            await dao.EstablishConnectionAsync();

            List<ProviderLoginHandle> inserted = new List<ProviderLoginHandle>();
            int nCount = 1;
            for (int i = 0; i < nCount; ++i)
            {
                string userName = Guid.NewGuid().ToString();
                var providerLoginHandle = new ProviderLoginHandle()
                {
                    LoginProvider = Guid.NewGuid().ToString(),
                    ProviderKey = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid()
                };
                inserted.Add(providerLoginHandle);
                await dao.UpsertLoginsAsync(providerLoginHandle);
            }

            foreach (var item in inserted)
            {
                item.TenantId = dao.TenantId;
                var foundResult = await dao.FindLoginByProviderAsync(item.LoginProvider,item.ProviderKey);
                Assert.IsNotNull(foundResult);
                var foundList = foundResult.ToList();
                Assert.IsTrue(foundList.Any());
                Assert.AreEqual(foundList.Count, 1);
                var found = foundList[0];
                Assert.IsTrue(ProviderLoginHandleComparer.Comparer.Equals(found, item));
            }

            foreach (var item in inserted)
            {
                await dao.DeleteLoginsAsync(item);
                var foundResult = await dao.FindLoginsByUserIdAsync(item.UserId);
                Assert.IsNotNull(foundResult);
                var foundList = foundResult.ToList();
                Assert.IsFalse(foundList.Any());
                Assert.AreEqual(foundList.Count, 0);

            }
        }

    }
}