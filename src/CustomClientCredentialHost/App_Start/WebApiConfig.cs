using System.Web.Http;
using System.Web.Http.Cors;
using Swashbuckle.Application;

namespace CustomClientCredentialHost
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // Web API configuration and services
            // Web API routes
            config.MapHttpAttributeRoutes();

            config
                .EnableSwagger(c => c.SingleApiVersion("v1", "A title for your API"))
                .EnableSwaggerUi(c =>
                {
                    c.InjectJavaScript(typeof (Startup).Assembly,
                        "CustomClientCredentialHost.SwaggerExtensions.onComplete.js");
                });
        }
    }
}