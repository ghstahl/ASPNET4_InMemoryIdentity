using System;

namespace P5.IdentityServer3.Common.RefreshToken
{
    public class RefreshTokenHandleRecord : WrappedRecord<RefreshTokenHandle>
    {


        public RefreshTokenHandleRecord()
        {
        }

        public RefreshTokenHandleRecord(RefreshTokenHandle record)
            : base(record, f => f.CreateGuid())
        {
        }
    }
}