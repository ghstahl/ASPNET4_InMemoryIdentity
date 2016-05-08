using System.Collections.Generic;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Common.Models;

namespace P5.IdentityServer3.Common
{
    public class FlattenedScopeHandle : AbstractScopeHandle<string, string>
    {
        public FlattenedScopeHandle(global::IdentityServer3.Core.Models.Scope scope = null)
            : base(scope)
        {
        }

        protected override string Serialize(List<Secret> scopeSecrets)
        {
            if (scopeSecrets == null)
                return null;
            var simpleDocument = new SimpleDocument<List<Secret>>(scopeSecrets).DocumentJson;
            return simpleDocument;
        }
        protected override List<Secret> DeserializeSecretes(string scopeSecrets)
        {
            var simpleDocument = new SimpleDocument<List<Secret>>(scopeSecrets);
            var document = (List<Secret>)simpleDocument.Document;
            return document;
        }
        protected override string Serialize(List<ScopeClaim> claims)
        {
            if (claims == null)
                return null;
            var simpleDocument = new SimpleDocument<List<ScopeClaim>>(claims).DocumentJson;
            return simpleDocument;
        }
        protected override List<ScopeClaim> DeserializeClaims(string claims)
        {
            var simpleDocument = new SimpleDocument<List<ScopeClaim>>(claims);
            var document = (List<ScopeClaim>)simpleDocument.Document;
            return document;
        }
    }
}