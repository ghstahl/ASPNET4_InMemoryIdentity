using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;


namespace P5.IdentityServer3.BiggyJson
{
    public class ClientHandle
    {
         public ClientHandle()
        {
        }

         public ClientHandle( Client client)
        {

            if (client != null)
            {
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime;
                AccessTokenLifetime = client.AccessTokenLifetime;
                AccessTokenType = client.AccessTokenType;
                AllowAccessToAllCustomGrantTypes = client.AllowAccessToAllCustomGrantTypes;
                AllowAccessToAllScopes = client.AllowAccessToAllScopes;
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser;
                AllowClientCredentialsOnly = client.AllowClientCredentialsOnly;
                AllowedCorsOrigins = client.AllowedCorsOrigins;
                AllowedCustomGrantTypes = client.AllowedCustomGrantTypes;
                AllowedScopes = client.AllowedScopes;
                AllowRememberConsent = client.AllowRememberConsent;
                AlwaysSendClientClaims = client.AlwaysSendClientClaims;
                AuthorizationCodeLifetime = client.AuthorizationCodeLifetime;
                Claims = client.Claims.ToClaimTypeRecords();
                ClientId = client.ClientId;
                ClientName = client.ClientName;
                ClientSecrets = client.ClientSecrets;
                ClientUri = client.ClientUri;
                Enabled = client.Enabled;
                EnableLocalLogin = client.EnableLocalLogin;
                Flow = client.Flow;
                IdentityProviderRestrictions = client.IdentityProviderRestrictions;
                IdentityTokenLifetime = client.IdentityTokenLifetime;
                IncludeJwtId = client.IncludeJwtId;
                LogoUri = client.LogoUri;
                LogoutSessionRequired = client.LogoutSessionRequired;
                LogoutUri = client.LogoutUri;
                PostLogoutRedirectUris = client.PostLogoutRedirectUris;
                PrefixClientClaims = client.PrefixClientClaims;
                RedirectUris = client.RedirectUris;
                RefreshTokenExpiration = client.RefreshTokenExpiration;
                RefreshTokenUsage = client.RefreshTokenUsage;
                RequireConsent = client.RequireConsent;
                RequireSignOutPrompt = client.RequireSignOutPrompt;
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime;
                UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh;
            }
        }
         public int AbsoluteRefreshTokenLifetime { get; set; }
         //
         // Summary:
         //     Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
         public int AccessTokenLifetime { get; set; }
         //
         // Summary:
         //     Specifies whether the access token is a reference token or a self contained
         //     JWT token (defaults to Jwt).
         public AccessTokenType AccessTokenType { get; set; }
         //
         // Summary:
         //     Gets or sets a value indicating whether the client has access to all custom
         //     grant types. Defaults to false.  You can set the allowed custom grant types
         //     via the AllowedCustomGrantTypes list.
         public bool AllowAccessToAllCustomGrantTypes { get; set; }
         //
         // Summary:
         //     Gets or sets a value indicating whether the client has access to all scopes.
         //     Defaults to false.  You can set the allowed scopes via the AllowedScopes
         //     list.
         public bool AllowAccessToAllScopes { get; set; }
         //
         // Summary:
         //     Controls whether access tokens are transmitted via the browser for this client
         //     (defaults to true).  This can prevent accidental leakage of access tokens
         //     when multiple response types are allowed.
         public bool AllowAccessTokensViaBrowser { get; set; }
         //
         // Summary:
         //     Gets or sets a value indicating whether this client is allowed to request
         //     token using client credentials only.  This is e.g. useful when you want a
         //     client to be able to use both a user-centric flow like implicit and additionally
         //     client credentials flow
         public bool AllowClientCredentialsOnly { get; set; }
         //
         // Summary:
         //     Gets or sets the allowed CORS origins for JavaScript clients.
         public List<string> AllowedCorsOrigins { get; set; }
         //
         // Summary:
         //     Gets or sets a list of allowed custom grant types when Flow is set to Custom.
         public List<string> AllowedCustomGrantTypes { get; set; }
         //
         // Summary:
         //     Specifies the scopes that the client is allowed to request. If empty, the
         //     client can't access any scope
         public List<string> AllowedScopes { get; set; }
         //
         // Summary:
         //     Specifies whether user can choose to store consent decisions (defaults to
         //     true)
         public bool AllowRememberConsent { get; set; }
         //
         // Summary:
         //     Gets or sets a value indicating whether client claims should be always included
         //     in the access tokens - or only for client credentials flow.
         public bool AlwaysSendClientClaims { get; set; }
         //
         // Summary:
         //     Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)
         public int AuthorizationCodeLifetime { get; set; }
         //
         // Summary:
         //     Allows settings claims for the client (will be included in the access token).
         public List<ClaimTypeRecord> Claims { get; set; }

         //
         // Summary:
         //     Unique ID of the client
         public string ClientId { get; set; }
         //
         // Summary:
         //     Client display name (used for logging and consent screen)
         public string ClientName { get; set; }
         //
         // Summary:
         //     Client secrets - only relevant for flows that require a secret
         public List<Secret> ClientSecrets { get; set; }
         //
         // Summary:
         //     URI to further information about client (used on consent screen)
         public string ClientUri { get; set; }
         //
         // Summary:
         //     Specifies if client is enabled (defaults to true)
         public bool Enabled { get; set; }
         //
         // Summary:
         //     Gets or sets a value indicating whether the local login is allowed for this
         //     client. Defaults to true.
         public bool EnableLocalLogin { get; set; }
         //
         // Summary:
         //     Specifies allowed flow for client (either AuthorizationCode, Implicit, Hybrid,
         //     ResourceOwner, ClientCredentials or Custom). Defaults to Implicit.
         public Flows Flow { get; set; }
         //
         // Summary:
         //     Specifies which external IdPs can be used with this client (if list is empty
         //     all IdPs are allowed). Defaults to empty.
         public List<string> IdentityProviderRestrictions { get; set; }
         //
         // Summary:
         //     Lifetime of identity token in seconds (defaults to 300 seconds / 5 minutes)
         public int IdentityTokenLifetime { get; set; }
         //
         // Summary:
         //     Gets or sets a value indicating whether JWT access tokens should include
         //     an identifier
         public bool IncludeJwtId { get; set; }
         //
         // Summary:
         //     URI to client logo (used on consent screen)
         public string LogoUri { get; set; }
         //
         // Summary:
         //     Specifies if the user's session id should be sent to the LogoutUri. Defaults
         //     to true.
         public bool LogoutSessionRequired { get; set; }
         //
         // Summary:
         //     Specifies logout URI at client for HTTP based logout.
         public string LogoutUri { get; set; }
         //
         // Summary:
         //     Specifies allowed URIs to redirect to after logout
         public List<string> PostLogoutRedirectUris { get; set; }
         //
         // Summary:
         //     Gets or sets a value indicating whether all client claims should be prefixed.
         public bool PrefixClientClaims { get; set; }
         //
         // Summary:
         //     Specifies allowed URIs to return tokens or authorization codes to
         public List<string> RedirectUris { get; set; }
         //
         // Summary:
         //     Absolute: the refresh token will expire on a fixed point in time (specified
         //     by the AbsoluteRefreshTokenLifetime) Sliding: when refreshing the token,
         //     the lifetime of the refresh token will be renewed (by the amount specified
         //     in SlidingRefreshTokenLifetime). The lifetime will not exceed AbsoluteRefreshTokenLifetime.
         public TokenExpiration RefreshTokenExpiration { get; set; }
         //
         // Summary:
         //     ReUse: the refresh token handle will stay the same when refreshing tokens
         //     OneTime: the refresh token handle will be updated when refreshing tokens
         public TokenUsage RefreshTokenUsage { get; set; }
         //
         // Summary:
         //     Specifies whether a consent screen is required (defaults to true)
         public bool RequireConsent { get; set; }
         //
         // Summary:
         //     Specifies if the client will always show a confirmation page for sign-out.
         //     Defaults to false.
         public bool RequireSignOutPrompt { get; set; }
         //
         // Summary:
         //     Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds
         //     / 15 days
         public int SlidingRefreshTokenLifetime { get; set; }
         //
         // Summary:
         //     Gets or sets a value indicating whether the access token (and its claims)
         //     should be updated on a refresh token request.
         public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
    }
}
