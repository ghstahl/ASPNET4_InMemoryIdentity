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
        IUserPasswordStore<CassandraUser, Guid>  
    {
        public async Task SetPasswordHashAsync(CassandraUser user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            // Password hash can be null when removing a password from a user
            user.PasswordHash = passwordHash;
        }

        public async Task<string> GetPasswordHashAsync(CassandraUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return user.PasswordHash;
        }

        public async Task<bool> HasPasswordAsync(CassandraUser user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return !string.IsNullOrEmpty(user.PasswordHash);
        }

    }
}