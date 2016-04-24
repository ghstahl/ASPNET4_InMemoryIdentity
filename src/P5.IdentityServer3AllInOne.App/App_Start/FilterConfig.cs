using System.Web;
using System.Web.Mvc;

namespace P5.IdentityServer3AllInOne.App
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
