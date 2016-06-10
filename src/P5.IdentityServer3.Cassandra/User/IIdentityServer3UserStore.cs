using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{
    public interface IIdentityServer3UserStore
    {
        /// <summary>
        /// Upsert a brand new IdentityServer3 User or overwrite a matching one.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task CreateIdentityServerUserAsync(IdentityServerUser user);


        Task<IdentityServerUser> FindIdentityServerUserByUserIdAsync(string userId);

    }

}
