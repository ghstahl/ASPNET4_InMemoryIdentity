namespace P5.IdentityServer3.Common
{
    public interface IClientHandle
    {
        global::IdentityServer3.Core.Models.Client MakeClient();
        string ClientId { get; set; }
    }
}