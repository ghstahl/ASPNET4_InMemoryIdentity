using System;

namespace P5.IdentityServer3.Common
{
    public class FlattenedAuthorizationCodeHandleRecord : WrappedRecord<FlattenedAuthorizationCodeHandle>
    {
        public FlattenedAuthorizationCodeHandleRecord()
        {
        }

        public FlattenedAuthorizationCodeHandleRecord(FlattenedAuthorizationCodeHandle record)
            : base(record, f => f.CreateGuid())
        {
        }
    }
}