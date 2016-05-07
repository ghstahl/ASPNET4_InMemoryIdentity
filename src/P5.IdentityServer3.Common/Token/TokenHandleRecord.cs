using System;

namespace P5.IdentityServer3.Common
{
    public class TokenHandleRecord : WrappedRecord<TokenHandle>
    {
        public static readonly Guid Namespace = new Guid("aae42b8f-d447-4dbf-a5e1-0ba03e14ca41");

        public TokenHandleRecord() { }
        public TokenHandleRecord(TokenHandle record)
            : base(record, f =>f.CreateGuid(Namespace))
        {
        }
    }
}