using System.Threading.Tasks;

namespace P5.IdentityServer3.Common
{
    public interface IConsentHandle
    {
        Task<global::IdentityServer3.Core.Models.Consent> MakeConsentAsync();
    }
}