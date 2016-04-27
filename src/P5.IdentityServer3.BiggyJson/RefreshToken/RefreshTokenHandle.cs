using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.BiggyJson
{

    public class RefreshTokenHandle
    {
        public RefreshTokenHandle()
        {
        }

        public RefreshTokenHandle(string key, RefreshToken token)
        {
            if (token != null)
            {
                AccessToken = token.AccessToken.ToTokenHandle();
                ClientId = token.ClientId;
                CreationTime = token.CreationTime;
                Expires = token.CreationTime.AddSeconds(token.LifeTime);
                LifeTime = token.LifeTime;
                Key = key;
                SubjectId = token.SubjectId;
                Version = token.Version;
            }
        }


        public TokenHandle AccessToken { get; set; }
        public string ClientId { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset Expires { get; set; }
        public string Key { get; set; }
        public int LifeTime { get; set; }
        public string SubjectId { get; set; }
        public int Version { get; set; }

    }
}

