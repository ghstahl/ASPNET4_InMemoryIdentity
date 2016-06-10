using System.Collections.Generic;
using System.Threading.Tasks;
using P5.IdentityServer3.Common.Models;

namespace P5.IdentityServer3.Common
{
    public class FlattenedIdentityServerUserHandle :
        AbstractIdentityServerUserHandle<string>
    {
        public FlattenedIdentityServerUserHandle()
            : base()
        {

        }

        public FlattenedIdentityServerUserHandle(IdentityServerUser user):base(user)
        {
            
        }

        protected override string Serialize(List<string> stringList)
        {
            if (stringList == null)
                return null;
            var simpleDocument = new SimpleDocument<List<string>>(stringList).DocumentJson;
            return simpleDocument;
        }

        protected override async Task<List<string>> Deserialize(string obj)
        {
            var simpleDocument = new SimpleDocument<List<string>>(obj);
            var document = (List<string>)simpleDocument.Document;
            return await Task.FromResult(document);
        }
    }
}