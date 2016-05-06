using System;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.BiggyJson
{
    public class RefreshTokenHandleRecord : WrappedRecord<RefreshTokenHandle>
    {
        public static readonly Guid Namespace = new Guid("20d6dd4c-5636-4caa-a26b-cbd216b93e72");

        public RefreshTokenHandleRecord() { }
        public RefreshTokenHandleRecord(RefreshTokenHandle record)
            : base(record, f =>f.CreateGuid(Namespace))
        {
        }
    }
}