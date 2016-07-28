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
            if (scopeNames == null)
                return "[]";
            var simpleDocument = new SimpleDocument<List<string>>(scopeNames).DocumentJson;
            return simpleDocument;
        }

        public override string SerializeClaimsIdentityRecords(List<ClaimIdentityRecord> claimIdentityRecords)
        {
            if (claimIdentityRecords == null)
                return "[]";
            var simpleDocument = new SimpleDocument<List<ClaimIdentityRecord>>(claimIdentityRecords).DocumentJson;
            return simpleDocument;
        }

        public override ClaimsPrincipal DeserializeSubject(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleDocument<List<ClaimIdentityRecord>>(obj);
            var document = (List<ClaimIdentityRecord>) simpleDocument.Document;
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentities(document.ToClaimsIdentitys());
            return claimsPrincipal;
        }

        public override IEnumerable<string> DeserializeScopes(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleDocument<List<string>>(obj);
            var document = (List<string>) simpleDocument.Document;
            return document;
        }
    }
}