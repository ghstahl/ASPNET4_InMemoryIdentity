using System;

namespace P5.IdentityServer3.Common
{
    public class FlattenedClientRecord : WrappedRecord<FlattenedClientHandle>
    {
        public static readonly Guid Namespace = new Guid("1f314c01-998b-45df-b121-d63d7183f50e");

        public FlattenedClientRecord() { }
        public FlattenedClientRecord(FlattenedClientHandle record)
            : base(record, f => f.CreateGuid(Namespace))
        {
        }
    }
}