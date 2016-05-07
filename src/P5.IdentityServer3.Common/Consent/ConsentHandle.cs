using System.Collections.Generic;
using System.Linq;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Common
{
    public class ConsentHandle : AbstractConsentHandle<List<string>>
    {
        public ConsentHandle(){}
        public ConsentHandle(global::IdentityServer3.Core.Models.Consent consent):base(consent){}

        public override List<string> Serialize(IEnumerable<string> scopes)
        {
            return scopes.ToList();
        }

        public override List<string> DeserializeScopes(List<string> obj)
        {
            return obj;
        }
    }
}

