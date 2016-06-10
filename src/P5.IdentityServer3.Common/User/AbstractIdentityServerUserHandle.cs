using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Threading.Tasks;

namespace P5.IdentityServer3.Common
{
    public abstract class AbstractIdentityServerUserHandle< TStringList> : IIdentityServerUserHandle
        where TStringList : class
        
    {
        public AbstractIdentityServerUserHandle()
        {
        }
        public AbstractIdentityServerUserHandle(IdentityServerUser user)
        {

            if (user != null)
            {
                AllowedScopes = Serialize(user.AllowedScopes);
                ClientIds = Serialize(user.ClientIds);
                Enabled = user.Enabled;
                UserId = user.UserId;
                UserName = user.UserName;
            }
        }

        protected abstract TStringList Serialize(List<string> allowedScopes);

        public async Task<IdentityServerUser> MakeIdentityServerUserAsync()
        {
            var result = new IdentityServerUser()
            {
                UserName = this.UserName,
                AllowedScopes = await Deserialize(this.AllowedScopes),
                ClientIds = await Deserialize(this.ClientIds),
                Enabled = this.Enabled,
                UserId = this.UserId
            };
            return result;
        }

        protected abstract Task<List<string>> Deserialize(TStringList allowedScopes);
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
        //
        // Summary:
        //     Specifies the scopes that the user is allowed to request. If empty, the
        //     user can't access any scope
        public TStringList AllowedScopes { get; set; }
        //
        // Summary:
        //     Specifies the clients that the user owns. If empty, the
        //     user isn't yet setup to do any IdentityServer stuff.
        public TStringList ClientIds { get; set; }
    }
}