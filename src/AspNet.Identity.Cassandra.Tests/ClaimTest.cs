using System;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.Store.Core.Models;

namespace AspNet.Identity.Cassandra.Tests
{
    [TestClass]
    public class ClaimTest
    {
        [TestMethod]
        public async Task Test_Add_Claims_Pager_Delete_All_Async()
        {
            var dao = new AspNetIdentityDao();
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            int nCount = 1000;
            for (int i = 0; i < nCount; ++i)
            {
                var guidId = Guid.NewGuid().ToString();
                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = userId, Value = "Value:" + guidId };
                await dao.CreateClaimAsync(claimHandle);
            }
            byte[] pageState = null;
            int pageSize = 99;
            IPage<ClaimHandle> result = await dao.PageClaimsAsync(userId, pageSize, pageState);
            pageState = result.PagingState;
            int nCounter = result.Count;
            while (pageState != null)
            {
                result = await dao.PageClaimsAsync(userId, pageSize, pageState);
                pageState = result.PagingState;
                nCounter += result.Count;
            }
            Assert.AreEqual(nCounter , nCount);


            await dao.DeleteClaimHandleByUserIdAsync(userId);
            var resultFind = await dao.FindClaimHandleByUserIdAsync(userId);
            Assert.IsNotNull(resultFind);
            Assert.AreEqual(0, resultFind.Count());
        }
        [TestMethod]
        public async Task Test_Add_Claim_Delete_All_Async()
        {
            var dao = new AspNetIdentityDao();
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                var guidId = Guid.NewGuid().ToString();
                var claimHandle = new ClaimHandle() { Type = "Type:" + guidId, UserId = userId, Value = "Value:" + guidId };
                await dao.CreateClaimAsync(claimHandle);
            }

            var result = await dao.FindClaimHandleByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());

            await dao.DeleteClaimHandleByUserIdAsync(userId);
            result = await dao.FindClaimHandleByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task Test_Find_Claim_Does_Not_Exist_Async()
        {
            var dao = new AspNetIdentityDao();
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            var result = await dao.FindClaimHandleByUserIdAsync(userId);
            Assert.AreEqual(0,result.Count());
        }

        [TestMethod]
        public async Task Test_Add_Claim_Delete_One_Async()
        {
            var dao = new AspNetIdentityDao();
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();

            var guidId = Guid.NewGuid().ToString();
            var claimHandle = new ClaimHandle() {Type = "Type:" + guidId, UserId = userId, Value = "Value:" + guidId};
            await dao.CreateClaimAsync(claimHandle);


            var result = await dao.FindClaimHandleByUserIdAsync(userId);
            Assert.IsNotNull(result);
            var finalResult = result.ToList();
            Assert.AreEqual(1, finalResult.Count());

            var claimH = finalResult[0];

            Assert.IsTrue(ClaimHandleComparer.Comparer.Equals(claimHandle, claimH));

            await dao.DeleteClaimHandleByUserIdTypeAndValueAsync(userId,claimHandle.Type,claimHandle.Value);
            result = await dao.FindClaimHandleByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }
        [TestMethod]
        public async Task Test_Add_Claim_Delete_Type_Async()
        {
            var dao = new AspNetIdentityDao();
            await dao.EstablishConnectionAsync();

            Guid userId = Guid.NewGuid();
            var type = "Type:" + userId;
            int nCount = 10;
            for (int i = 0; i < nCount; ++i)
            {
                var guidId = Guid.NewGuid().ToString();
                var claimHandle = new ClaimHandle() { Type = type, UserId = userId, Value = "Value:" + guidId };
                await dao.CreateClaimAsync(claimHandle);
            }

            type = "Type:" + Guid.NewGuid().ToString();
            for (int i = 0; i < nCount; ++i)
            {
                var guidId = Guid.NewGuid().ToString();
                var claimHandle = new ClaimHandle() { Type = type, UserId = userId, Value = "Value:" + guidId };
                await dao.CreateClaimAsync(claimHandle);
            }
            var result = await dao.FindClaimHandleByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount*2, result.Count());

            await dao.DeleteClaimHandleByUserIdAndTypeAsync(userId, "Type:" + userId);
            result = await dao.FindClaimHandleByUserIdAsync(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(nCount, result.Count());
        }
    }
}

