using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
 
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{
    public class AuthorizationCodeStore : IAuthorizationCodeStore
    {
        public async Task StoreAsync(string key, global::IdentityServer3.Core.Models.AuthorizationCode value)
        {
            var result = await IdentityServer3CassandraDao.CreateAuthorizationCodeHandleAsync(key, value);
        }

        public async Task<global::IdentityServer3.Core.Models.AuthorizationCode> GetAsync(string key)
        {
            var clientStore = new ClientStore();
            var scopeStore = new ScopeStore();
            var result = await IdentityServer3CassandraDao.FindAuthorizationCodeByKey(key, clientStore,scopeStore);
            return result;
        }

        public async Task RemoveAsync(string key)
        {
            await IdentityServer3CassandraDao.DeleteAuthorizationCodeByKey(key);
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {

            var clientStore = new ClientStore();
            var scopeStore  = new ScopeStore();
            var resultOfFind = await IdentityServer3CassandraDao.FindAuthorizationCodeMetadataBySubject(subject, clientStore, scopeStore);
            return resultOfFind;
        }

        public async Task RevokeAsync(string subject, string client)
        {
            await IdentityServer3CassandraDao.DeleteAuthorizationCodesByClientIdAndSubjectId(client,subject);
        }
    }
}
