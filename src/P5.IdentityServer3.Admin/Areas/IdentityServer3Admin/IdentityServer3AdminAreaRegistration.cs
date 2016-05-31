using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace P5.IdentityServer3.Admin.Areas.IdentityServer3Admin
{
    public class IdentityServer3AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "IdentityServer3Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // more specific route first. 

            context.MapRoute(
                "IdentityServer3Admin_default",
                "IdentityServer3Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "P5.IdentityServer3.Admin.Areas.IdentityServer3Admin.Controllers" }
            );
        }
    }
}
