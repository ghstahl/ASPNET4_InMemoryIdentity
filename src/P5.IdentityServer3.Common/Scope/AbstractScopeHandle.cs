using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.Common
{
    public abstract class AbstractScopeHandle<TScopeClaims, TScecrets> : IScopeHandle where TScopeClaims : class
        where TScecrets : class
    {
        private global::IdentityServer3.Core.Models.Scope _original;

        public global::IdentityServer3.Core.Models.Scope GetScope()
        {
            return _original ?? (_original = MakeIdentityServerScope());
        }

        public AbstractScopeHandle(global::IdentityServer3.Core.Models.Scope scope = null)
        {
            _original = scope;
            if (scope != null)
            {
                AllowUnrestrictedIntrospection = scope.AllowUnrestrictedIntrospection;
                Claims = Serialize(scope.Claims);
                ClaimsRule = scope.ClaimsRule;
                Description = scope.Description;
                DisplayName = scope.DisplayName;
                Emphasize = scope.Emphasize;
                Enabled = scope.Enabled;
                IncludeAllClaimsForUser = scope.IncludeAllClaimsForUser;
                Name = scope.Name;
                Required = scope.Required;
                ScopeSecrets = Serialize(scope.ScopeSecrets);
                ShowInDiscoveryDocument = scope.ShowInDiscoveryDocument;
                Type = scope.Type;
            }
        }
        public Scope MakeIdentityServerScope()
        {
            var scope = new Scope( )
            {
                Claims = DeserializeClaims(Claims),
                Name = Name,
                Type = Type,
                Description = Description,
                Enabled = Enabled,
                ClaimsRule = ClaimsRule,
                ScopeSecrets = DeserializeSecretes(ScopeSecrets),
                Required = Required,
                Emphasize = Emphasize,
                IncludeAllClaimsForUser = IncludeAllClaimsForUser,
                ShowInDiscoveryDocument = ShowInDiscoveryDocument,
                AllowUnrestrictedIntrospection = AllowUnrestrictedIntrospection,
                DisplayName = DisplayName
            };
            return scope;
        }

        protected abstract List<Secret> DeserializeSecretes(TScecrets scopeSecrets);
        protected abstract TScecrets Serialize(List<Secret> scopeSecrets);
        protected abstract List<ScopeClaim> DeserializeClaims(TScopeClaims claims);
        protected abstract TScopeClaims Serialize(List<ScopeClaim> claims);



        // Summary:
        //     Specifies whether this scope is allowed to see other scopes when using the
        //     introspection endpoint
        public bool AllowUnrestrictedIntrospection { get; set; }
        //
        // Summary:
        //     List of user claims that should be included in the identity (identity scope)
        //     or access token (resource scope).
        public TScopeClaims Claims { get; set; }
        //
        // Summary:
        //     Rule for determining which claims should be included in the token (this is
        //     implementation specific)
        public string ClaimsRule { get; set; }
        //
        // Summary:
        //     Description. This value will be used e.g. on the consent screen.
        public string Description { get; set; }
        //
        // Summary:
        //     Display name. This value will be used e.g. on the consent screen.
        public string DisplayName { get; set; }
        //
        // Summary:
        //     Specifies whether the consent screen will emphasize this scope. Use this
        //     setting for sensitive or important scopes. Defaults to false.
        public bool Emphasize { get; set; }
        //
        // Summary:
        //     Indicates if scope is enabled and can be requested. Defaults to true.
        public bool Enabled { get; set; }
        //
        // Summary:
        //     If enabled, all claims for the user will be included in the token. Defaults
        //     to false.
        public bool IncludeAllClaimsForUser { get; set; }
        //
        // Summary:
        //     Name of the scope. This is the value a client will use to request the scope.
        public string Name { get; set; }
        //
        // Summary:
        //     Specifies whether the user can de-select the scope on the consent screen.
        //     Defaults to false.
        public bool Required { get; set; }
        //
        // Summary:
        //     Gets or sets the scope secrets.
        public TScecrets ScopeSecrets { get; set; }
        //
        // Summary:
        //     Specifies whether this scope is shown in the discovery document. Defaults
        //     to true.
        public bool ShowInDiscoveryDocument { get; set; }
        //
        // Summary:
        //     Specifies whether this scope is about identity information from the userinfo
        //     endpoint, or a resource (e.g. a Web API). Defaults to Resource.
        public ScopeType Type { get; set; }
    }
}