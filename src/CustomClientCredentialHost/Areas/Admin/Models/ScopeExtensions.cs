using IdentityServer3.Core.Models;

namespace CustomClientCredentialHost.Areas.Admin.Models
{
    static public class ScopeExtensions
    {
        public static ScopeNewModel ToScopeNewModel(this Scope scope)
        {
            ScopeNewModel other = new ScopeNewModel
            {
                Name = scope.Name,
                AllowUnrestrictedIntrospection = scope.AllowUnrestrictedIntrospection,
                Claims =  scope.Claims,
                ClaimsRule = scope.ClaimsRule,
                Description = scope.Description,
                DisplayName = scope.DisplayName,
                Emphasize = scope.Emphasize,
                Enabled = scope.Enabled,
                IncludeAllClaimsForUser = scope.IncludeAllClaimsForUser,
                Required = scope.Required,
                ScopeSecrets = scope.ScopeSecrets,
                ShowInDiscoveryDocument = scope.ShowInDiscoveryDocument,
                Type = scope.Type,
                SelectedScopeType = scope.Type
            };
            return other;
        }
    }
}