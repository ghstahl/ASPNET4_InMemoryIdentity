using System;

namespace P5.IdentityServer3.Common.RefreshToken
{
    public class RefreshTokenHandleRecord : WrappedRecord<RefreshTokenHandle>
    {
        public static readonly Guid Namespace = new Guid("291F7D01-57A0-45CB-B2D5-347B8160DBD8");

        public RefreshTokenHandleRecord() { }
        public RefreshTokenHandleRecord(RefreshTokenHandle record)
            : base(record, f => f.CreateGuid(Namespace))
        {
        }
    }
}