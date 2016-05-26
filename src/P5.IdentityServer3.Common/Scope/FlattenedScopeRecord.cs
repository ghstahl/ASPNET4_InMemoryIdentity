using System;

namespace P5.IdentityServer3.Common
{
    public class FlattenedScopeRecord : WrappedRecord<FlattenedScopeHandle>
    {
        
        public FlattenedScopeRecord()
        {
        }
        public FlattenedScopeRecord(FlattenedScopeHandle record)
            : base(record, f => f.CreateGuid( ))
        {
        }
    }
}