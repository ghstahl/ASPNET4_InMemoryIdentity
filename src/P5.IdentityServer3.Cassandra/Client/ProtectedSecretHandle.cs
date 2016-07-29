namespace P5.IdentityServer3.Cassandra
{
    public class ProtectedSecretQueryValues
    {
        public string ClientId { get; set; }
        public string Value { get; set; }
    }

    public class ProtectedSecretHandle : ProtectedSecretQueryValues
    {
        public string ProtectedValue { get; set; }
    }
}