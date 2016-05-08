using System;

namespace P5.IdentityServer3.Common
{
    public class FlattenedScopeRecord : WrappedRecord<FlattenedScopeHandle>
    {
        public static readonly Guid Namespace = new Guid("ac6e677a-3070-46e5-a480-7593aaa26f37");
        public FlattenedScopeRecord()
        {
        }
        public FlattenedScopeRecord(FlattenedScopeHandle record)
            : base(record, f => f.CreateGuid(Namespace))
        {
        }
    }
}