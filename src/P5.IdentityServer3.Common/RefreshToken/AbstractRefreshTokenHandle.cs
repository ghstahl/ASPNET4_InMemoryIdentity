using System;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public abstract Task<Token> DeserializeTokenHandleAsync(TTokenHandle obj, IClientStore clientStore);
        public TTokenHandle AccessToken { get; set; }
        public string ClientId { get; set; }
        public async Task<Token> MakeAccessTokenAsync(IClientStore clientStore)
        {
            var result = await DeserializeTokenHandleAsync(AccessToken, clientStore);
            return result;
        }


        public async Task<global::IdentityServer3.Core.Models.RefreshToken> MakeRefreshTokenAsync(IClientStore clientStore)
        {
            var accessToken = await MakeAccessTokenAsync(clientStore);
            var token = new global::IdentityServer3.Core.Models.RefreshToken()
            {
                AccessToken = accessToken,
                CreationTime = CreationTime,
                LifeTime = LifeTime,
                Version = Version
            };
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