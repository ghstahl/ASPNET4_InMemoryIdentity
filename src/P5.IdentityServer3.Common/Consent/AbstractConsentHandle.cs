using System.Collections.Generic;

namespace P5.IdentityServer3.Common
{
    public abstract class AbstractConsentHandle<TScopes> : IConsentHandle where TScopes : class
    {
        public AbstractConsentHandle() { }
        public AbstractConsentHandle(global::IdentityServer3.Core.Models.Consent consent)
        {
            ClientId = consent.ClientId;
            Subject = consent.Subject;
            Scopes = Serialize(consent.Scopes);
        }
        public global::IdentityServer3.Core.Models.Consent MakeConsent()
        {
            return new global::IdentityServer3.Core.Models.Consent
            {
                ClientId = ClientId,
                Scopes = DeserializeScopes(Scopes),
                Subject = Subject
            };
        }

        public abstract TScopes Serialize(IEnumerable<string> scopes);
        public abstract List<string> DeserializeScopes(TScopes obj);

        public string ClientId { get; set; }

        public TScopes Scopes { get; set; }

        public string Subject { get; set; }

    }
}