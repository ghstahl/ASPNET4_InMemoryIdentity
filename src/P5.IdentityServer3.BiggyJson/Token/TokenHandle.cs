using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.BiggyJson
{

    public class TokenHandle
    {
        public TokenHandle()
        {
        }

        public TokenHandle(string key, Token token)
        {
            Key = key;
            if (token != null)
            {
                Audience = token.Audience;
                Claims = token.Claims.ToClaimTypeRecords();
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

        public string Audience { get; set; }

        public List<ClaimTypeRecord> Claims { get; set; }

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

