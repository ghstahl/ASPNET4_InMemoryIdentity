using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
 
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{
    public class TokenHandleStore : ITokenHandleStore
    {
        public TokenHandleStore() { }
        public async Task StoreAsync(string key, Token value)
        {
            var flat = new FlattenedTokenHandle(key, value);
            var result = await IdentityServer3CassandraDao.CreateTokenHandleAsync(flat);
        }

        public async Task<Token> GetAsync(string key)
        {
            IClientStore cs = new ClientStore();
            var result_of_find = await IdentityServer3CassandraDao.FindTokenByKey(key, cs);
            return result_of_find;
        }

        public async Task RemoveAsync(string key)
        {
            var result_of_find = await IdentityServer3CassandraDao.DeleteTokenByKey(key);
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
             IClientStore cs = new ClientStore();
              var result_of_find = await IdentityServer3CassandraDao.FindTokenMetadataBySubject(subject, cs);
            return result_of_find;
        }

        public async Task RevokeAsync(string subject, string client)
        {
            var result_of_find = await IdentityServer3CassandraDao.DeleteTokensByClientIdAndSubjectId(client,subject);
        }
    }
}