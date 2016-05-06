using System;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.BiggyJson
{
    public class AuthorizationCodeHandleRecord : WrappedRecord<AuthorizationCodeHandle>
    {
        public static readonly Guid Namespace = new Guid("32fce69d-c234-4b73-8484-7242cd4bc256");

        public AuthorizationCodeHandleRecord() { }
        public AuthorizationCodeHandleRecord(AuthorizationCodeHandle record)
            : base(record, f =>f.CreateGuid(Namespace))
        {
        }
    }
}