using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace P5.AspNet.Identity.Biggy
{
    public class CustomUserManager : UserManager<CustomUser>
    {
        public static string FolderStorage { get; set; }
        public CustomUserManager(JsonUserStore store)
            : base(store)
        {
        }

        public static CustomUserManager Create(IdentityFactoryOptions<CustomUserManager> options,
            IOwinContext context)
        {
            if(FolderStorage == null)
                throw new NullReferenceException(string.Format("FolderStorage is null.  did you forget to set CustomUserManager.FolderStorage to a proper path"));
            var manager = new CustomUserManager(new JsonUserStore(FolderStorage));
            return manager;
        }
    }
}