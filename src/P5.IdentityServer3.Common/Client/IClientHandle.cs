using System.Threading.Tasks;

namespace P5.IdentityServer3.Common
{
    public interface IClientHandle
    {
        Task<global::IdentityServer3.Core.Models.Client> MakeClientAsync();
        string ClientId { get; set; }
    }
}