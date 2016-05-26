using System;


namespace P5.IdentityServer3.Common
{
    public static class ClientExtensions
    {
        public static Guid CreateGuid(this IClientHandle client, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, client.ClientId);
        }
        public static Guid CreateGuid(this IClientHandle client)
        {
            return GuidGenerator.CreateGuid(ClientConstants.Namespace, client.ClientId);
        }
        public static ClientHandle ToClientHandle(this global::IdentityServer3.Core.Models.Client client)
        {
            var handle = new ClientHandle(client);
            return handle;
        }

        public static Guid ClientIdToGuid(this string clientId, Guid @namespace)
        {
            return GuidGenerator.CreateGuid(@namespace, clientId);
        }
        public static Guid ClientIdToGuid(this string clientId)
        {
            return clientId.ClientIdToGuid(ClientConstants.Namespace);
        }
    }
}