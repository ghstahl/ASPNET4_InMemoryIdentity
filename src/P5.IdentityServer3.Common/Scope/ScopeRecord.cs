using System;

namespace P5.IdentityServer3.Common
{
    public class ScopeRecord : WrappedRecord<ScopeHandle>
    {
        public ScopeRecord()
        {
        }
        public ScopeRecord(ScopeHandle record)
            : base(record, f => f.CreateGuid( ))
        {
        }
    }
}