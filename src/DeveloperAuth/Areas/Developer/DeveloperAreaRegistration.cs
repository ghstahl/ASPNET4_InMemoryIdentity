using System.Web.Mvc;

namespace DeveloperAuth.Areas.Developer
{
    public class DeveloperAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Developer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // more specific route first.

            context.MapRoute(
                "Developer_default",
                "Developer/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "DeveloperAuth.Areas.Developer.Controllers" }
            );
        }
    }
}
