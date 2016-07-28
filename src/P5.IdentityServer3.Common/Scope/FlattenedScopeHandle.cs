using System.Collections.Generic;
using System.Threading.Tasks;
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

        public override string Serialize(List<Secret> scopeSecrets)
        {
            if (scopeSecrets == null)
                return "[]";
            var simpleDocument = new SimpleDocument<List<Secret>>(scopeSecrets).DocumentJson;
            return simpleDocument;
        }

        public override async Task<List<Secret>> DeserializeSecretsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleDocument<List<Secret>>(obj);
            var document = (List<Secret>)simpleDocument.Document;
            return await Task.FromResult(document);
        }
        public override string Serialize(List<ScopeClaim> claims)
        {
            if (claims == null)
                return "[]";
            var simpleDocument = new SimpleDocument<List<ScopeClaim>>(claims).DocumentJson;
            return simpleDocument;
        }

        public override async Task<List<ScopeClaim>> DeserializeClaimsAsync(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleDocument<List<ScopeClaim>>(obj);
            var document = (List<ScopeClaim>)simpleDocument.Document;
            return await Task.FromResult(document);
        }

    }
}