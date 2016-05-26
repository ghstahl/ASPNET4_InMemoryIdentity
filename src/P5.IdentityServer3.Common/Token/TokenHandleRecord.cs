using System;

namespace P5.IdentityServer3.Common
{
    public class TokenHandleRecord : WrappedRecord<TokenHandle>
    {


        public TokenHandleRecord()
        {
        }

        public TokenHandleRecord(TokenHandle record)
            : base(record, f => f.CreateGuid())
        {
        }
    }
}