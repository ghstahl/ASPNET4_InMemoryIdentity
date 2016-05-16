using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public class ClientHandle :
        AbstractClientHandle<
            List<ClaimTypeRecord>,
            List<Secret>,
            List<string>
            >
    {
        public ClientHandle()
            : base()
        {

        }

        public ClientHandle(Client client):base(client)
        {

        }

        public override List<string> Serialize(List<string> stringList)
        {
            return stringList;
        }

        public override async Task<List<Claim>> DeserializeClaimsAsync(List<ClaimTypeRecord> obj)
        {
            return await Task.FromResult( obj == null ? null : obj.ToClaims());
        }


        public override List<ClaimTypeRecord> Serialize(List<Claim> claims)
        {
             return claims == null ? null : claims.ToClaimTypeRecords();
        }



        public override List<Secret> Serialize(List<Secret> secrets)
        {
            return secrets;
        }

        public override async Task<List<string>> DeserializeStringsAsync(List<string> obj)
        {
              return await Task.FromResult( obj);
        }

        public override async Task<List<Secret>> DeserializeSecretsAsync(List<Secret> obj)
        {
             return await Task.FromResult( obj);
        }

    }
}