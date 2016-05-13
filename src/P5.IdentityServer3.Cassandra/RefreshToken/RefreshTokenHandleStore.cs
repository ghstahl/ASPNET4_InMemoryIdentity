using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Cassandra.Client;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.RefreshToken;

namespace P5.IdentityServer3.Cassandra
{
    public class RefreshTokenHandleStore : IRefreshTokenStore
    {
        public RefreshTokenHandleStore()
        {
        }

        public async Task StoreAsync(string key, RefreshToken value)
        {
            var flat = new FlattenedRefreshTokenHandle(key, value);
            var result = await IdentityServer3CassandraDao.CreateRefreshTokenHandleAsync(flat);
        }

        public async Task<RefreshToken> GetAsync(string key)
        {
            IClientStore cs = new ClientStore();
            var result_of_find = await IdentityServer3CassandraDao.FindRefreshTokenByKey(key, cs);
            return result_of_find;
        }

        public async Task RemoveAsync(string key)
        {
            var result_of_find = await IdentityServer3CassandraDao.DeleteRefreshTokenByKey(key);
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            IClientStore cs = new ClientStore();
            var result_of_find = await IdentityServer3CassandraDao.FindRefreshTokenMetadataBySubject(subject, cs);
            return result_of_find;
        }

        public async Task RevokeAsync(string subject, string client)
        {
            var result_of_find = await IdentityServer3CassandraDao.DeleteRefreshTokensByClientIdAndSubjectId(client, subject);
        }
    }
}