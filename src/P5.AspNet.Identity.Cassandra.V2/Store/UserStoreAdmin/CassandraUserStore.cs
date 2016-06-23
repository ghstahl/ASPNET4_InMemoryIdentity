using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using P5.AspNet.Identity.Cassandra.DAO;
using P5.CassandraStore.DAO;
using P5.Store.Core.Models;

namespace P5.AspNet.Identity.Cassandra
{
    public partial class CassandraUserStore :
        IUserStoreAdmin<CassandraUser, Guid>
    {

        public Task<IPage<CassandraUser>> PageUsersAsync(int pageSize, byte[] pagingState)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IPage<ClaimHandle>> PageClaimsAsync(Guid userId, int pageSize, byte[] pagingState)
        {
            throw new NotImplementedException();
        }
    }
}