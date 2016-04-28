using System;
using System.Security.Claims;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.BiggyJson
{
    static class AuthorizationCodeHandleExtensions
    {
        public static Guid CreateGuid(this AuthorizationCodeHandle tokenHandle, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, tokenHandle.Key);
        }

        public static AuthorizationCode ToAuthorizationCode(this AuthorizationCodeHandle handle, 
            IClientStore clientStore,
            IScopeStore scopeStore)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentities(handle.ClaimIdentityRecords.ToClaimsIdentitys());

            var result = new AuthorizationCode()
            { 
                Subject = claimsPrincipal,
                Client = clientStore.FindClientByIdAsync(handle.ClientId).Result,
                RequestedScopes = scopeStore.FindScopesAsync(handle.RequestedScopes).Result,
                CreationTime = handle.CreationTime,
                IsOpenId = handle.IsOpenId,
                RedirectUri = handle.RedirectUri,
                WasConsentShown = handle.WasConsentShown,
                Nonce = handle.Nonce
            };
            return result;
        }
        public static AuthorizationCodeHandle ToAuthorizationCodeHandle(this AuthorizationCode code, string key = "")
        {
            var result = new AuthorizationCodeHandle(key, code);
            return result;
        }
    }
}