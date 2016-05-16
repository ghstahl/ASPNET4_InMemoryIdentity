using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common
{
    public interface ITokenHandle
    {
        string Audience { get; set; }

        string ClientId { get; set; }

        DateTimeOffset CreationTime { get; set; }

        DateTimeOffset Expires { get; set; }

        string Issuer { get; set; }

        string Key { get; set; }

        int Lifetime { get; set; }

        string SubjectId { get; set; }

        string Type { get; set; }

        int Version { get; set; }
      
        Task<global::IdentityServer3.Core.Models.Token> MakeIdentityServerTokenAsync(IClientStore clientStore);

    }
}