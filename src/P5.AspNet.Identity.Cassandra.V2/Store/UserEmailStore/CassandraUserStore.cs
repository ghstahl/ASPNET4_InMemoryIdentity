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
        IUserEmailStore<CassandraUser, Guid>
    {

        public async Task SetEmailAsync(CassandraUser user, string email)
        {
            if (user == null) 
                throw new ArgumentNullException("user");
            if (email == null) 
                throw new ArgumentNullException("email");

            user.Email = email;
        }

        public async Task<string> GetEmailAsync(CassandraUser user)
        {
            if (user == null) 
                throw new ArgumentNullException("user");
            return user.Email;
        }

        public async Task<bool> GetEmailConfirmedAsync(CassandraUser user)
        {
            if (user == null) 
                throw new ArgumentNullException("user");
            return user.EmailConfirmed;
        }

        public async Task SetEmailConfirmedAsync(CassandraUser user, bool confirmed)
        {
            if (user == null) 
                throw new ArgumentNullException("user");

            user.EmailConfirmed = confirmed;
        }

        public async Task<CassandraUser> FindByEmailAsync(string email)
        {
            var resultList = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                   async () =>
                   {
                       await ResilientSessionContainer.EstablishSessionAsync();
                       return await ResilientSessionContainer.ResilientSession.FindUserByEmailAsync(email);
                   },
                   async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<CassandraUser>>(ex));
            return resultList.FirstOrDefault();
        }

      
    }
}