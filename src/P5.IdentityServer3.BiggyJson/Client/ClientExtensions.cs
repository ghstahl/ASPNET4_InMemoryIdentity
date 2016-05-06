using System;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Stores;


namespace P5.IdentityServer3.BiggyJson
{
    public static class ClientExtensions
    {
        public static Guid CreateGuid(this ClientHandle client, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, client.ClientId);
        }
        public static Client ToClient(this ClientHandle handle)
        {
            Client client = new Client()
            {
                AbsoluteRefreshTokenLifetime = handle.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = handle.AccessTokenLifetime,
                AccessTokenType = handle.AccessTokenType,
                AllowAccessToAllCustomGrantTypes = handle.AllowAccessToAllCustomGrantTypes,
                AllowAccessToAllScopes = handle.AllowAccessToAllScopes,
                AllowAccessTokensViaBrowser = handle.AllowAccessTokensViaBrowser,
                AllowClientCredentialsOnly = handle.AllowClientCredentialsOnly,
                AllowedCorsOrigins = handle.AllowedCorsOrigins,
                AllowedCustomGrantTypes = handle.AllowedCustomGrantTypes,
                AllowedScopes = handle.AllowedScopes,
                AllowRememberConsent = handle.AllowRememberConsent,
                AlwaysSendClientClaims = handle.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = handle.AuthorizationCodeLifetime,
                Claims = handle.Claims.ToClaims(),
                ClientId = handle.ClientId,
                ClientName = handle.ClientName,
                ClientSecrets = handle.ClientSecrets,
                ClientUri = handle.ClientUri,
                Enabled = handle.Enabled,
                EnableLocalLogin = handle.EnableLocalLogin,
                Flow = handle.Flow,
                IdentityProviderRestrictions = handle.IdentityProviderRestrictions,
                IdentityTokenLifetime = handle.IdentityTokenLifetime,
                IncludeJwtId = handle.IncludeJwtId,
                LogoUri = handle.LogoUri,
                LogoutSessionRequired = handle.LogoutSessionRequired,
                LogoutUri = handle.LogoutUri,
                PostLogoutRedirectUris = handle.PostLogoutRedirectUris,
                PrefixClientClaims = handle.PrefixClientClaims,
                RedirectUris = handle.RedirectUris,
                RefreshTokenExpiration = handle.RefreshTokenExpiration,
                RefreshTokenUsage = handle.RefreshTokenUsage,
                RequireConsent = handle.RequireConsent,
                RequireSignOutPrompt = handle.RequireSignOutPrompt,
                SlidingRefreshTokenLifetime = handle.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = handle.UpdateAccessTokenClaimsOnRefresh,
            };
            return client;

        }
        public static ClientHandle ToClientHandle(this Client client)
        {
            var handle = new ClientHandle(client);
            return handle;
        }
    }
}