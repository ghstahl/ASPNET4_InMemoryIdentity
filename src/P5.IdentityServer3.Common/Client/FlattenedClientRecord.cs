using System;

namespace P5.IdentityServer3.Common
{
    public class FlattenedClientRecord : WrappedRecord<FlattenedClientHandle>
    {
        public FlattenedClientRecord() { }
        public FlattenedClientRecord(FlattenedClientHandle record)
            : base(record, f => f.CreateGuid())
        {
        }
    }
}