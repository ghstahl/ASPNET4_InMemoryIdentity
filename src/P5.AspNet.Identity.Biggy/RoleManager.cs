using Microsoft.AspNet.Identity;

namespace P5.AspNet.Identity.Biggy
{
    public class RoleManager : RoleManager<Role, int>
    {
        public RoleManager(IRoleStore<Role, int> store)
            : base(store)
        {
            this.RoleValidator = new RoleValidator<Role, int>(this)
            {

            };
        }
    }
}