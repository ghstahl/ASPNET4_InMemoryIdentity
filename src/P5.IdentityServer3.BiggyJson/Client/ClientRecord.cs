using System;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.BiggyJson
{
    public class ClientRecord : WrappedRecord<ClientHandle>
    {
        public static readonly Guid Namespace = new Guid("1f314c01-998b-45df-b121-d63d7183f50e");

        public ClientRecord() { }
        public ClientRecord(ClientHandle record)
            : base(record, f => f.CreateGuid(Namespace))
        {
        }
    }
}