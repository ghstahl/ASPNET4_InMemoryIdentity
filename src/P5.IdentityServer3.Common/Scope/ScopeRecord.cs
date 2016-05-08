using System;

namespace P5.IdentityServer3.Common
{
    public class ScopeRecord : WrappedRecord<ScopeHandle>
    {
        public static readonly Guid Namespace = new Guid("ac6e677a-3070-46e5-a480-7593aaa26f37");
        public ScopeRecord()
        {
        }
        public ScopeRecord(ScopeHandle record)
            : base(record, f => f.CreateGuid(Namespace))
        {
        }
    }
}