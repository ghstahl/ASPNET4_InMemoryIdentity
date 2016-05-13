using System.Collections.Generic;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common
{
    public interface IAuthorizationCodeHandle
    {
        string Key { get; set; }
        global::IdentityServer3.Core.Models.AuthorizationCode MakeAuthorizationCode(IClientStore clientStore,IScopeStore scopeStore);
    }
}