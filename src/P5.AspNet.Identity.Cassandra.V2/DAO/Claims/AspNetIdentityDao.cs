using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using P5.CassandraStore;
using P5.CassandraStore.Extensions;

namespace P5.AspNet.Identity.Cassandra.DAO
{
   

    public partial class AspNetIdentityDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for Claim
        //-----------------------------------------------

        #region PREPARED STATEMENTS for Claim



        #endregion

        public void PrepareClaimsStatements()
        {

        }

        public async Task<IEnumerable<ClaimHandle>> FindClaimHandleByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            MyMappings.Init();
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            var records =
                await
                    mapper.FetchAsync<ClaimHandle>("WHERE userId = ?", userId);
            return records;

        }

        public async Task CreateClaimAsync(ClaimHandle claimHandle,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            await mapper.InsertAsync(claimHandle);
        }

        public async Task DeleteClaimHandleByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            await mapper.DeleteAsync<ClaimHandle>("WHERE userId = ?", userId);
        }

        public async Task DeleteClaimHandleByUserIdAndTypeAsync(Guid userId, string type,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            await mapper.DeleteAsync<ClaimHandle>("WHERE userId = ? and type = ?", userId, type);
        }

        public async Task DeleteClaimHandleByUserIdTypeAndValueAsync(Guid userId, string type, string value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();
            await mapper.DeleteAsync<ClaimHandle>("WHERE userId = ? and type = ? and value = ?", userId, type, value);
        }

        public async Task<Store.Core.Models.IPage<ClaimHandle>> PageClaimsAsync(Guid userId, int pageSize,
            byte[] pagingState,
            CancellationToken cancellationToken = default(CancellationToken))
        {


            cancellationToken.ThrowIfCancellationRequested();

            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            IPage<ClaimHandle> page;
            string cqlQuery = string.Format("Where userid={0}", userId.ToString());
            if (pagingState == null)
            {
                page = await mapper.FetchPageAsync<ClaimHandle>(
                    Cql.New(cqlQuery).WithOptions(opt =>
                        opt.SetPageSize(pageSize)));
            }
            else
            {
                page = await mapper.FetchPageAsync<ClaimHandle>(
                    Cql.New(cqlQuery).WithOptions(opt =>
                        opt.SetPageSize(pageSize).SetPagingState(pagingState)));
            }

            // var result = CreatePageProxy(page);
            var result = new PageProxy<ClaimHandle>(page);

            return result;

        }
    }
}
