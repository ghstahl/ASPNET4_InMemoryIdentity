using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.Common
{
    public abstract class AbstractAuthorizationCodeHandle<TClaimIdentityRecords, TRequestedScopes> :
        IAuthorizationCodeHandle
        where TClaimIdentityRecords : class
        where TRequestedScopes : class
    {
        public AbstractAuthorizationCodeHandle() { }
        public AbstractAuthorizationCodeHandle(string key, global::IdentityServer3.Core.Models.AuthorizationCode code)
        {
            Key = key;
            if (code != null)
            {
                ClientId = code.ClientId;
                SubjectId = code.SubjectId;
                Expires = code.CreationTime.AddSeconds(code.Client.AuthorizationCodeLifetime);
                CreationTime = code.CreationTime;
                IsOpenId = code.IsOpenId;
                RedirectUri = code.RedirectUri;
                WasConsentShown = code.WasConsentShown;
                Nonce = code.Nonce;
                ClaimIdentityRecords =
                    SerializeClaimsIdentityRecords(code.Subject.Identities.ToList().ToClaimIdentityRecords());
                var query = from item in code.RequestedScopes
                            select new ScopeHandle(item);
                RequestedScopes = SerializeRequestScopes(query.ToNames());
            }
        }

        protected abstract TRequestedScopes SerializeRequestScopes(List<string> scopeNames);

        protected abstract TClaimIdentityRecords SerializeClaimsIdentityRecords(List<ClaimIdentityRecord> claimIdentityRecords);

        public global::IdentityServer3.Core.Models.AuthorizationCode MakeAuthorizationCode(IClientStore clientStore,IScopeStore scopeStore)
        {
            var result = new AuthorizationCode()
            {
                Client = clientStore.FindClientByIdAsync(this.ClientId).Result,
                CreationTime = CreationTime,
                IsOpenId = IsOpenId,
                RedirectUri = RedirectUri,
                WasConsentShown = WasConsentShown,
                Nonce = Nonce,
                RequestedScopes = scopeStore.FindScopesAsync(DeserializeScopes(RequestedScopes)).Result,
                Subject = DeserializeSubject(ClaimIdentityRecords)
            };
            return result;
        }

        protected abstract ClaimsPrincipal DeserializeSubject(TClaimIdentityRecords claimIdentityRecords);

        protected abstract IEnumerable<string> DeserializeScopes(TRequestedScopes requestedScopes);
        public string Key { get; set; }
        //
        // Summary:
        //     Gets the client identifier.
        public string ClientId { get; set; }
        //
        // Summary:
        //     Gets the subject identifier.
        public string SubjectId { get; set; }
        public DateTimeOffset Expires { get; set; }
        //
        // Summary:
        //     Gets or sets the creation time.
        public DateTimeOffset CreationTime { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether this code is an OpenID Connect code.
        public bool IsOpenId { get; set; }
        //
        // Summary:
        //     Gets or sets the redirect URI.
        public string RedirectUri { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether consent was shown.
        public bool WasConsentShown { get; set; }
        //
        // Summary:
        //     Gets or sets the nonce.
        public string Nonce { get; set; }
        public TClaimIdentityRecords ClaimIdentityRecords { get; set; }
        public TRequestedScopes RequestedScopes { get; set; }

    }
}