using System;


namespace P5.IdentityServer3.Common
{
    public class ClientRecord : WrappedRecord<ClientHandle>
    {
        public ClientRecord() { }
        public ClientRecord(ClientHandle record)
            : base(record, f => f.CreateGuid())
        {
        }
    }
}