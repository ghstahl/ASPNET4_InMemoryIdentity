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
        IUserPhoneNumberStore<CassandraUser, Guid>
    {
        public async Task SetPhoneNumberAsync(CassandraUser user, string phoneNumber)
        {
            if (user == null) 
                throw new ArgumentNullException("user");
            if (phoneNumber == null) 
                throw new ArgumentNullException("phoneNumber");

            user.PhoneNumber = phoneNumber;
        }

        public async Task<string> GetPhoneNumberAsync(CassandraUser user)
        {
            if (user == null) 
                throw new ArgumentNullException("user");
            return user.PhoneNumber;
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(CassandraUser user)
        {
            if (user == null) 
                throw new ArgumentNullException("user");
            return user.PhoneNumberConfirmed;
        }

        public async Task SetPhoneNumberConfirmedAsync(CassandraUser user, bool confirmed)
        {
            if (user == null) 
                throw new ArgumentNullException("user");

            user.PhoneNumberConfirmed = confirmed;
        }

    }
}