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
        IUserSecurityStampStore<CassandraUser, Guid>
    {

        public async Task SetSecurityStampAsync(CassandraUser user, string stamp)
        {
            if (user == null) 
                throw new ArgumentNullException("user");
            if (stamp == null) 
                throw new ArgumentNullException("stamp");

            user.SecurityStamp = stamp;
        }

        public async Task<string> GetSecurityStampAsync(CassandraUser user)
        {
            if (user == null) 
                throw new ArgumentNullException("user");
            return user.SecurityStamp;
        }

    }
}