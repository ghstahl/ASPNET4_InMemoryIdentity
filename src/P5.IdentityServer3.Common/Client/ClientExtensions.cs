using System;


namespace P5.IdentityServer3.Common
{
    public static class ClientExtensions
    {
        public static Guid CreateGuid(this IClientHandle client, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, client.ClientId);
        }

        public static ClientHandle ToClientHandle(this global::IdentityServer3.Core.Models.Client client)
        {
            var handle = new ClientHandle(client);
            return handle;
        }
    }
}