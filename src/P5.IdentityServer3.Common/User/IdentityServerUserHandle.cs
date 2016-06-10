using System.Collections.Generic;
using System.Threading.Tasks;

namespace P5.IdentityServer3.Common
{
    public class IdentityServerUserHandle :
        AbstractIdentityServerUserHandle<List<string>>
    {
        public IdentityServerUserHandle()
            : base()
        {

        }

        public IdentityServerUserHandle(IdentityServerUser user)
            : base(user)
        {

        }

        protected override List<string> Serialize(List<string> stringList)
        {
            return stringList;
        }

        protected override async Task<List<string>> Deserialize(List<string> obj)
        {
            return await Task.FromResult(obj);
        }
    }
}