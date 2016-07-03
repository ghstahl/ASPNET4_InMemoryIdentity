using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;

/*
 * http://www.strathweb.com/2013/08/customizing-controller-discovery-in-asp-net-web-api/
 * */
namespace P5.WebApi2.Hub
{
    public class WebApi2HubAssembliesResolver : DefaultAssembliesResolver
    {
        private static Object thisLock = new Object();
        private static bool Initialized;
        private static List<Assembly> CachedAssemblies { get; set; }

        private static void Initialize()
        {
            lock (thisLock)
            {
                if (Initialized)
                {
                    // incase of double entry.
                    return;
                }
                var pluginManager = new PluginLoadingManager(WebApi2Hub.WebApi2HubOptions);
                var plugins = pluginManager.GetPluginHosts();
                CachedAssemblies = new List<Assembly>();
                foreach (var plugin in plugins)
                {
                    CachedAssemblies.Add(plugin.GetHostingAssembly());
                }
                Initialized = true;
            }
        }
        public  override ICollection<Assembly> GetAssemblies()
        {
            Initialize();
            ICollection<Assembly> baseAssemblies = base.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(baseAssemblies);
            foreach (var assembly in CachedAssemblies)
            {
                baseAssemblies.Add(assembly);
            }
            return assemblies;
        }
    }
}
