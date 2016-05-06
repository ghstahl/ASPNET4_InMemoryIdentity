using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Cassandra.CommonStore;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Stores;
using System.Linq;

namespace P5.IdentityServer3.Cassandra
{
    public class ScopeStore : CassandraStore<ScopeRecord, global::IdentityServer3.Core.Models.Scope>, IScopeStore
    {
        public async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> FindScopesAsync(
            IEnumerable<string> scopeNames)
        {
            var scopeRecords = IdentityServer3CassandraDao.FindScopesByNamesAsync(scopeNames);
            return await Task.FromResult(scopeRecords.Result);
        }

        public Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> GetScopesAsync(bool publicOnly = true)
        {
            throw new NotImplementedException();
        }
    }
}