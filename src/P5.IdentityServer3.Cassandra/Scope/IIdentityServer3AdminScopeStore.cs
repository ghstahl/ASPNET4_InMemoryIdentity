using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Cassandra
{
    public interface IIdentityServer3AdminScopeStore : IScopeStore
    {
        Task UpdateScopeByNameAsync(string name, IEnumerable<PropertyValue> properties);
        Task CreateScopeAsync(global::IdentityServer3.Core.Models.Scope scope);
        Task AddScopeSecretsAsync(string name, IEnumerable<Secret> secrets);
        Task DeleteScopeSecretsAsync(string name, IEnumerable<Secret> secrets);
        Task AddScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims);
        Task DeleteScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims);
        Task UpdateScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims);
    }
}