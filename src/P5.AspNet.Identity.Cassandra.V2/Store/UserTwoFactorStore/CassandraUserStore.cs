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
        IUserTwoFactorStore<CassandraUser, Guid>
    {
        public async Task SetTwoFactorEnabledAsync(CassandraUser user, bool enabled)
        {
            if (user == null) 
                throw new ArgumentNullException("user");

            user.TwoFactorEnabled = enabled;
        }

        public async Task<bool> GetTwoFactorEnabledAsync(CassandraUser user)
        {
            if (user == null) 
                throw new ArgumentNullException("user");
            return user.TwoFactorEnabled;
        }
    }
}