using System;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common.RefreshToken
{
    public interface IRefreshTokenHandle
    {
        string Key { get; set; }
        Token MakeAccessToken(IClientStore clientStore);
        DateTimeOffset CreationTime { get; set; }
        int LifeTime { get; set; }
        int Version { get; set; }
    }
}