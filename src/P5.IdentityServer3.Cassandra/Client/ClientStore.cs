using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
 

namespace P5.IdentityServer3.Cassandra.Client
{
    public class ClientStore : IClientStore
    {
        public async Task<global::IdentityServer3.Core.Models.Client> FindClientByIdAsync(string clientId)
        {
            if (clientId == null)
            {
                throw new ArgumentNullException("clientId","Validation failed.");
            }
            var cr = new FlattenedClientRecord(record: new FlattenedClientHandle() {ClientId = clientId});
            var result =  await IdentityServer3CassandraDao.FindClientIdAsync(cr.Id);
            return result;
        }
    }
}
