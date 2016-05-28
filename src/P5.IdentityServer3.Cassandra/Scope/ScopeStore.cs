using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;

using P5.IdentityServer3.Cassandra.DAO;
using System.Linq;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{
    public class ScopeStore : IIdentityServer3AdminScopeStore
    {
        public async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> FindScopesAsync(
            IEnumerable<string> scopeNames)
        {
            var scopeRecords = await IdentityServer3CassandraDao.FindScopesByNamesAsync(scopeNames);
            return scopeRecords;
        }

        public async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> GetScopesAsync(bool publicOnly = true)
        {
            var scopeRecords = await IdentityServer3CassandraDao.FindScopesAsync(publicOnly);
            return scopeRecords;
        }

        public async Task UpdateScopeByNameAsync(string name, IEnumerable<PropertyValue> properties)
        {
            await IdentityServer3CassandraDao.UpdateScopeByNameAsync(name, properties);
        }

        public async Task CreateScopeAsync(Scope scope)
        {
            await IdentityServer3CassandraDao.UpsertScopeAsync(scope);
        }

       
        public async Task AddScopeSecretsAsync(string name, IEnumerable<Secret> secrets)
        {
            await IdentityServer3CassandraDao.AddScopeSecretsByNameAsync(name, secrets);
        }

        public async Task DeleteScopeSecretsAsync(string name, IEnumerable<Secret> secrets)
        {
            await IdentityServer3CassandraDao.DeleteScopeSecretsFromScopeByNameAsync(name, secrets);
        }


        public async Task AddScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims)
        {
            await IdentityServer3CassandraDao.AddScopeClaimsToScopeByNameAsync(name, claims);
        }

        public async Task DeleteScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims)
        {
            await IdentityServer3CassandraDao.DeleteScopeClaimsFromScopeByNameAsync(name, claims);
        }

        public async Task UpdateScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims)
        {
            await IdentityServer3CassandraDao.UpdateScopeClaimsInScopeByNameAsync(name, claims);
        }
    }
}