using System.Collections.Generic;
using P5.IdentityServer3.Common.Models;

namespace P5.IdentityServer3.Common
{
    public class FlattenedConsentHandle : AbstractConsentHandle<string>
    {
        public FlattenedConsentHandle() { }
        public FlattenedConsentHandle(global::IdentityServer3.Core.Models.Consent consent) : base(consent) { }


        public override string Serialize(IEnumerable<string> scopes)
        {
            if (scopes == null)
                return "[]";
            var simpleDocument = new SimpleDocument<IEnumerable<string>>(scopes).DocumentJson;
            return simpleDocument;
        }

        public override List<string> DeserializeScopes(string obj)
        {
            obj = string.IsNullOrEmpty(obj) ? "[]" : obj;
            var simpleDocument = new SimpleDocument<List<string>>(obj);
            var document = (List<string>)simpleDocument.Document;
            return document;
        }
    }
}