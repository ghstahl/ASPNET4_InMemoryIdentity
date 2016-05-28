using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Cassandra
{
    public partial class IdentityServer3AdminStore : IIdentityServer3AdminStore
    {
        private ScopeStore _ScopeStore;

        private ScopeStore ScopeStore
        {
            get { return _ScopeStore ?? (_ScopeStore = new ScopeStore()); }
        }
        public async Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            return await ScopeStore.FindScopesAsync(scopeNames);
        }

        public async Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            return await ScopeStore.GetScopesAsync(publicOnly);
        }

        public async Task UpdateScopeByNameAsync(string name, IEnumerable<PropertyValue> properties)
        {
            await ScopeStore.UpdateScopeByNameAsync(name, properties);
        }

        public async Task CreateScopeAsync(Scope scope)
        {
            await ScopeStore.CreateScopeAsync(scope);
        }

       
        public async Task AddScopeSecretsAsync(string name, IEnumerable<Secret> secrets)
        {
            await ScopeStore.AddScopeSecretsAsync(name, secrets);
        }

        public async Task DeleteScopeSecretsAsync(string name, IEnumerable<Secret> secrets)
        {
            await ScopeStore.DeleteScopeSecretsAsync(name, secrets);
        }
        public async Task AddScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims)
        {
            await ScopeStore.AddScopeClaimsAsync(name, claims);
        }

        public async Task DeleteScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims)
        {
            await ScopeStore.DeleteScopeClaimsAsync(name, claims);
        }

        public async Task UpdateScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims)
        {
            await ScopeStore.UpdateScopeClaimsAsync(name, claims);
        }
    }
}