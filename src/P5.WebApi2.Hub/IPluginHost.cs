using System.Reflection;
using System.Threading.Tasks;

namespace P5.WebApi2.Hub
{
    public interface IPluginHost
    {
        Assembly GetHostingAssembly();
    }
}