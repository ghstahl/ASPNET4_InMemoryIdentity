using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common
{
    public interface IAuthorizationCodeHandle
    {
        string Key { get; set; }
        Task<global::IdentityServer3.Core.Models.AuthorizationCode> MakeAuthorizationCodeAsync(IClientStore clientStore, IScopeStore scopeStore);
    }
}