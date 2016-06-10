namespace P5.IdentityServer3.Cassandra
{
    public interface IIdentityServer3AdminStore : 
        IIdentityServer3UserStore,
        IIdentityServer3AdminClientStore, 
        IIdentityServer3AdminScopeStore
    {

    }
}