using System.Collections.Generic;
using System.Security.Claims;
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

        public override List<string> DeserializeStrings(List<string> obj)
        {
            return obj;
        }

        public override List<ClaimTypeRecord> Serialize(List<Claim> claims)
        {
             return claims == null ? null : claims.ToClaimTypeRecords();
        }

        public override List<Claim> DeserializeClaims(List<ClaimTypeRecord> obj)
        {
            return obj == null ? null : obj.ToClaims();
        }

        public override List<Secret> Serialize(List<Secret> secrets)
        {
            return secrets;
        }

        public override List<Secret> DeserializeSecrets(List<Secret> obj)
        {
            return obj;
        }
    }
}