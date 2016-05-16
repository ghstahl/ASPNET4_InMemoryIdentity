using System;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common.RefreshToken
{
    public interface IRefreshTokenHandle
    {
        string Key { get; set; }
        Task<Token> MakeAccessTokenAsync(IClientStore clientStore);
        Task<global::IdentityServer3.Core.Models.RefreshToken> MakeRefreshTokenAsync(IClientStore clientStore);
        DateTimeOffset CreationTime { get; set; }
        int LifeTime { get; set; }
        int Version { get; set; }
    }
}