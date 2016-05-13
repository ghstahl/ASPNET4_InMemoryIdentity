using System.Collections.Generic;
using System.Security.Claims;
using P5.IdentityServer3.Common.Models;

namespace P5.IdentityServer3.Common
{
    public class FlattenedAuthorizationCodeHandle : AbstractAuthorizationCodeHandle<string,string>
    
    {
        protected override string SerializeRequestScopes(List<string> scopeNames)
        {
            var simpleDocument = new SimpleDocument<List<string>>(scopeNames).DocumentJson;
            return simpleDocument;
        }

        protected override string SerializeClaimsIdentityRecords(List<ClaimIdentityRecord> claimIdentityRecords)
        {
            var simpleDocument = new SimpleDocument<List<ClaimIdentityRecord>>(claimIdentityRecords).DocumentJson;
            return simpleDocument;
        }

        protected override ClaimsPrincipal DeserializeSubject(string claimIdentityRecords)
        {
            var simpleDocument = new SimpleDocument<List<ClaimIdentityRecord>>(claimIdentityRecords);
            var document = (List<ClaimIdentityRecord>)simpleDocument.Document;
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentities(document.ToClaimsIdentitys());
            return claimsPrincipal;
        }

        protected override IEnumerable<string> DeserializeScopes(string requestedScopes)
        {
            var simpleDocument = new SimpleDocument<List<string>>(requestedScopes);
            var document = (List<string>)simpleDocument.Document;
            return document;
        }
    }
}