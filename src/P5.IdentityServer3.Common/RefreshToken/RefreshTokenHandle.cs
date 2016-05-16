using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common.RefreshToken
{
    public class RefreshTokenHandle : AbstractRefreshTokenHandle<TokenHandle>
    {
        public RefreshTokenHandle()
        {
        }

        public RefreshTokenHandle(string key, global::IdentityServer3.Core.Models.RefreshToken token) : base(key, token)
        {
        }

        protected override TokenHandle Serialize(Token accessToken)
        {
            return accessToken.ToTokenHandle();
        }

        public override async Task<Token> DeserializeTokenHandleAsync(TokenHandle obj, IClientStore clientStore)
        {
            var result = await obj.MakeIdentityServerTokenAsync(clientStore);
            return result;
        }
    }
}

