using System;

namespace P5.IdentityServer3.Common
{
    public class AuthorizationCodeHandleRecord : WrappedRecord<AuthorizationCodeHandle>
    {
        public AuthorizationCodeHandleRecord()
        {
        }

        public AuthorizationCodeHandleRecord(AuthorizationCodeHandle record)
            : base(record, f => f.CreateGuid())
        {
        }
    }
}