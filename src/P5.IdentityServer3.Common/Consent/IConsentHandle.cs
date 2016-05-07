namespace P5.IdentityServer3.Common
{
    public interface IConsentHandle
    {
        global::IdentityServer3.Core.Models.Consent MakeConsent();
    }
}