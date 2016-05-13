using System;

namespace P5.IdentityServer3.Common
{
    public class FlattenedAuthorizationCodeHandleRecord : WrappedRecord<FlattenedAuthorizationCodeHandle>
    {
        public static readonly Guid Namespace = new Guid("32fce69d-c234-4b73-8484-7242cd4bc256");

        public FlattenedAuthorizationCodeHandleRecord() { }
        public FlattenedAuthorizationCodeHandleRecord(FlattenedAuthorizationCodeHandle record)
            : base(record, f => f.CreateGuid(Namespace))
        {
        }
    }
}