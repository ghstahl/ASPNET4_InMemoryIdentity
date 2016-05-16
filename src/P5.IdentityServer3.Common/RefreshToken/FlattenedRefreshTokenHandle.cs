using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Common.Models;

namespace P5.IdentityServer3.Common.RefreshToken
{
    public class FlattenedRefreshTokenHandle : AbstractRefreshTokenHandle<string>
    {
        public FlattenedRefreshTokenHandle()
        {
        }

        public FlattenedRefreshTokenHandle(string key, global::IdentityServer3.Core.Models.RefreshToken token)
            : base(key, token)
        {
        }

        protected override string Serialize(Token accessToken)
        {
            var tokenHandle = accessToken.ToTokenHandle();
            var document = new SimpleDocument<TokenHandle>(tokenHandle).DocumentJson;
            return document;
        }

        public override async Task<Token> DeserializeTokenHandleAsync(string obj, IClientStore clientStore)
        {
            var simpleDocument = new SimpleDocument<TokenHandle>(obj);
            var document = (TokenHandle) simpleDocument.Document;
            var result = await document.MakeIdentityServerTokenAsync(clientStore);
            return result;
        }
    }
}