using System;
using System.Collections.Generic;
using System.Linq;
using P5.IdentityServer3.Stores;

namespace P5.IdentityServer3.BiggyJson
{
    public class AuthorizationCodeHandle
    {
        public AuthorizationCodeHandle()
        {
        }
        /*
         *   doc["_id"] = key;
            doc["_version"] = 1;
            doc["_clientId"] = code.ClientId;
            doc["_subjectId"] = code.SubjectId;
            doc["_expires"] = code.CreationTime.AddSeconds(code.Client.AuthorizationCodeLifetime).ToBsonDateTime();
            doc["creationTime"] = code.CreationTime.ToBsonDateTime();
            doc["isOpenId"] = code.IsOpenId;
            doc["redirectUri"] = code.RedirectUri;
            doc["wasConsentShown"] = code.WasConsentShown;
            doc.SetIfNotNull("nonce", code.Nonce);
            doc["subject"] = SerializeIdentities(code);
         */
        public AuthorizationCodeHandle(string key, global::IdentityServer3.Core.Models.AuthorizationCode code)
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
                ClaimIdentityRecords = code.Subject.Identities.ToList().ToClaimIdentityRecords();
                RequestedScopes = code.RequestedScopes.ToNames();
            }
        }
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
        public List<ClaimIdentityRecord> ClaimIdentityRecords { get; set; }
        public List<string> RequestedScopes { get; set; }
    }
}

