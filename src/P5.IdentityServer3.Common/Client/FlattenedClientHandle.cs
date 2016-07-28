using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Common.Models;

namespace P5.IdentityServer3.Common
{
    public class FlattenedClientHandle :
        AbstractClientHandle<
            string,
            string,
            string
            >
    {
        public FlattenedClientHandle()
            : base()
        {

        }

        public FlattenedClientHandle(Client client):base(client)
        {
            
        }
        public override string Serialize(List<string> stringList)
        {
            if (stringList == null)
                return "[]";
            var simpleDocument = new SimpleDocument<List<string>>(stringList).DocumentJson;
            return simpleDocument;
        }

        public override async  Task<List<string>> DeserializeStringsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleDocument<List<string>>(obj);
            var document = (List<string>)simpleDocument.Document;
            return await Task.FromResult(document);
        }

        public override string Serialize(List<Claim> claims)
        {
            var normalized = claims == null ? null : claims.ToClaimTypeRecords();
            if (normalized == null)
                return "[]";
            var simpleDocument = new SimpleDocument<List<ClaimTypeRecord>>(normalized).DocumentJson;
            return simpleDocument;
        }

       public override async  Task<List<Claim>> DeserializeClaimsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleDocument<List<ClaimTypeRecord>>(obj);
            var document = (List<ClaimTypeRecord>) simpleDocument.Document;
            var result = document.ToClaims();
            return await Task.FromResult(result);
        }

        public override string Serialize(List<Secret> secrets)
        {
            if (secrets == null)
                return "[]";
            var simpleDocument = new SimpleDocument<List<Secret>>(secrets).DocumentJson;
            return simpleDocument;
        }

        public override async Task<List<Secret>> DeserializeSecretsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleDocument<List<Secret>>(obj);
            var document = (List<Secret>)simpleDocument.Document;
            return await Task.FromResult(document);
        }

        
    }
}
