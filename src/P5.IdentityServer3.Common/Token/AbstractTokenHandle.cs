using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common
{
    public abstract class AbstractTokenHandle<TClaim> : ITokenHandle where TClaim : class
    {
        public AbstractTokenHandle()
        {
        }
        public AbstractTokenHandle(string key, global::IdentityServer3.Core.Models.Token token)
        {
            Key = key;
            if (token != null)
            {
                Audience = token.Audience;
                Claims = Serialize(token.Claims);
                ClientId = token.ClientId;
                CreationTime = token.CreationTime;
                Expires = token.CreationTime.AddSeconds(token.Lifetime);
                Issuer = token.Issuer;
                Lifetime = token.Lifetime;
                SubjectId = token.SubjectId;
                Type = token.Type;
                Version = token.Version;
            }
        }

        public Token MakeIdentityServerToken(IClientStore clientStore)
        {
            var token = new Token(this.Type)
            {
                Audience = this.Audience,
                Claims = DeserializeClaims(Claims),
                Client = clientStore.FindClientByIdAsync(this.ClientId).Result,
                CreationTime = this.CreationTime,
                Issuer = this.Issuer,
                Lifetime = this.Lifetime,
                Type = this.Type,
                Version = this.Version
            };
            return token;
        }
        public abstract TClaim Serialize(List<Claim> claims);
        public abstract List<Claim> DeserializeClaims(TClaim obj);
        public string Audience { get; set; }
        public TClaim Claims { get; set; }

        public string ClientId { get; set; }

        public DateTimeOffset CreationTime { get; set; }

        public DateTimeOffset Expires { get; set; }

        public string Issuer { get; set; }

        public string Key { get; set; }

        public int Lifetime { get; set; }

        public string SubjectId { get; set; }

        public string Type { get; set; }

        public int Version { get; set; }

    }
}

