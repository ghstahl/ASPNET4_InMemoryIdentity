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
        IUserLockoutStore<CassandraUser, Guid>
    {

        public async Task<DateTimeOffset> GetLockoutEndDateAsync(CassandraUser user)
        {
            if (user == null) 
                throw new ArgumentNullException("user");
            return user.LockoutEndDate;
        }

        public async Task SetLockoutEndDateAsync(CassandraUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.LockoutEndDate = lockoutEnd;
        }

        public async Task<int> IncrementAccessFailedCountAsync(CassandraUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            // NOTE:  Since we aren't using C* counters and an increment operation, the value for the counter we loaded could be stale when we
            // increment this way and so the count could be incorrect (i.e. this increment in not atomic)
            user.AccessFailedCount++;
            return user.AccessFailedCount;
        }

        public async Task ResetAccessFailedCountAsync(CassandraUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.AccessFailedCount = 0;
        }

        public async Task<int> GetAccessFailedCountAsync(CassandraUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return user.AccessFailedCount;
        }

        public async Task<bool> GetLockoutEnabledAsync(CassandraUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return user.LockoutEnabled;
        }

        public async Task SetLockoutEnabledAsync(CassandraUser user, bool enabled)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.LockoutEnabled = enabled;
        }

       
    }
}