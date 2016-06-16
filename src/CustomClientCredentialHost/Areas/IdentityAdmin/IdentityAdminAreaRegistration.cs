using System.Web.Mvc;

namespace CustomClientCredentialHost.Areas.IdentityAdmin
{
    public class IdentityAdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "IdentityAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "IdentityAdmin_default",
                "IdentityAdmin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "CustomClientCredentialHost.Areas.IdentityAdmin.Controllers" }
            );
        }
    }
}