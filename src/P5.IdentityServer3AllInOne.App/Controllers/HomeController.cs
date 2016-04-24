using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace P5.IdentityServer3AllInOne.App.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            var caller = User as ClaimsPrincipal;

            var result = caller.Claims.Select(c =>
            {
                var r = new Claim(c.Type, c.Value);
                return r;
            });


            return View(result);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}