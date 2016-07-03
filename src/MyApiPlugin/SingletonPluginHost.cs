using System.ComponentModel.Composition;
using System.Reflection;
using System.Threading.Tasks;

using P5.WebApi2.Hub;

namespace MyApiPlugin
{
    [Export(typeof(IPluginHost))]
    public class SingletonPluginHost : IPluginHost
    {
        public Assembly GetHostingAssembly()
        {
            return GetType().Assembly;
        }
    }

    
}