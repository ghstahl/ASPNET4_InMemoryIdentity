using System;
using System.Security.Claims;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common.RefreshToken
{
    public abstract class AbstractRefreshTokenHandle<TTokenHandle> : IRefreshTokenHandle where TTokenHandle : class
    {
        public AbstractRefreshTokenHandle()
        {
        }

        public AbstractRefreshTokenHandle(string key, global::IdentityServer3.Core.Models.RefreshToken token)
        {
            Key = key;
            if (token != null)
            {
                AccessToken = Serialize(token.AccessToken);
                ClientId = token.ClientId;
                CreationTime = token.CreationTime;
                Expires = token.CreationTime.AddSeconds(token.LifeTime);
                LifeTime = token.LifeTime;
                SubjectId = token.SubjectId;
                Version = token.Version;
            }
        }

        protected abstract TTokenHandle Serialize(Token accessToken);
        public abstract Token DeserializeTokenHandle(TTokenHandle obj, IClientStore clientStore);
        public TTokenHandle AccessToken { get; set; }
        public string ClientId { get; set; }
        public Token MakeAccessToken(IClientStore clientStore)
        {
            return DeserializeTokenHandle(AccessToken, clientStore);
        }

        public global::IdentityServer3.Core.Models.RefreshToken MakeRefreshToken(IClientStore clientStore)
        {
            var token = new global::IdentityServer3.Core.Models.RefreshToken();
            token.AccessToken = MakeAccessToken(clientStore);
            token.CreationTime = CreationTime;
            token.LifeTime = LifeTime;
            token.Version = Version;
            return token;
        }

        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset Expires { get; set; }
        public string Key { get; set; }
        public int LifeTime { get; set; }
        public string SubjectId { get; set; }
        public int Version { get; set; }
    }
}