using System.Collections.Generic;
using System.Security.Claims;
using P5.IdentityServer3.Common.Models;

namespace P5.IdentityServer3.Common
{
    public class FlattenedAuthorizationCodeHandle : AbstractAuthorizationCodeHandle<string, string>

    {
        public FlattenedAuthorizationCodeHandle() : base()
        {
        }

        public FlattenedAuthorizationCodeHandle(string key, global::IdentityServer3.Core.Models.AuthorizationCode code)
            : base(key, code)
        {

        }

        public override string SerializeRequestScopes(List<string> scopeNames)
        {
            var simpleDocument = new SimpleDocument<List<string>>(scopeNames).DocumentJson;
            return simpleDocument;
        }

        public override string SerializeClaimsIdentityRecords(List<ClaimIdentityRecord> claimIdentityRecords)
        {
            var simpleDocument = new SimpleDocument<List<ClaimIdentityRecord>>(claimIdentityRecords).DocumentJson;
            return simpleDocument;
        }

        public override ClaimsPrincipal DeserializeSubject(string claimIdentityRecords)
        {
            var simpleDocument = new SimpleDocument<List<ClaimIdentityRecord>>(claimIdentityRecords);
            var document = (List<ClaimIdentityRecord>) simpleDocument.Document;
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentities(document.ToClaimsIdentitys());
            return claimsPrincipal;
        }

        public override IEnumerable<string> DeserializeScopes(string requestedScopes)
        {
            var simpleDocument = new SimpleDocument<List<string>>(requestedScopes);
            var document = (List<string>) simpleDocument.Document;
            return document;
        }
    }
}