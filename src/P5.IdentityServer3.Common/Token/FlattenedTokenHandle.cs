using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Common.Models;

namespace P5.IdentityServer3.Common
{
    public class FlattenedTokenHandle : AbstractTokenHandle<string>
    {
        public FlattenedTokenHandle(string key, Token token)
            : base(key, token)
        {
        }

        public FlattenedTokenHandle()
            : base()
        {
        }

        public override string Serialize(List<Claim> claims)
        {
            var normalized = claims == null ? null : claims.ToClaimTypeRecords();
            if (normalized == null)
                return "[]";
            var simpleDocument = new SimpleDocument<List<ClaimTypeRecord>>(normalized).DocumentJson;
            return simpleDocument;
        }

        public override async Task<List<Claim>> DeserializeClaimsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleDocument<List<ClaimTypeRecord>>(obj);
            var document = (List<ClaimTypeRecord>)simpleDocument.Document;
            var result = document.ToClaims();
            return await Task.FromResult(result);
        }
    }
}