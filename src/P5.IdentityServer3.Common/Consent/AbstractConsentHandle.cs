using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<global::IdentityServer3.Core.Models.Consent> MakeConsentAsync()
        {
            var result = new global::IdentityServer3.Core.Models.Consent
            {
                ClientId = ClientId,
                Scopes = DeserializeScopes(Scopes),
                Subject = Subject
            };
            return await Task.FromResult(result);
        }

        public abstract TScopes Serialize(IEnumerable<string> scopes);
        public abstract List<string> DeserializeScopes(TScopes obj);

        public string ClientId { get; set; }

        public TScopes Scopes { get; set; }

        public string Subject { get; set; }

    }
}