namespace P5.IdentityServer3.Cassandra
{
    public class ScopeMappedRecord : global::IdentityServer3.Core.Models.Scope
    {
        public string ClaimsDocument { get; set; }
        public string ScopeSecretsDocument { get; set; }
    }
}