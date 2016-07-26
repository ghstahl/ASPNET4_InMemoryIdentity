using System.Collections.Generic;
using System.Threading.Tasks;

namespace P5.IdentityServer3.Common
{
    public class IdentityServerUserModel
    {
        //
        // Summary:
        //     Unique ID of the UserId
        public string UserId { get; set; }
        //
        // Summary:
        //     User display name (used for logging,etc)
        public string UserName { get; set; }
        //
        // Summary:
        //     Specifies if user is enabled (defaults to true)
        public bool Enabled { get; set; }
    }
    /*
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
    }*/
}