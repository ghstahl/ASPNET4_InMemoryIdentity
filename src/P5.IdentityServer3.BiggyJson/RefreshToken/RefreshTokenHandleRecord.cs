using System;

namespace P5.IdentityServer3.BiggyJson
{
    public class RefreshTokenHandleRecord : WrappedRecord<RefreshTokenHandle>
    {
        public static readonly Guid Namespace = new Guid("24c7fe63-ed0f-4594-915a-ee717ac0cfa2");

        public RefreshTokenHandleRecord() { }
        public RefreshTokenHandleRecord(RefreshTokenHandle record)
            : base(record, f =>f.CreateGuid(Namespace))
        {
        }
    }
}