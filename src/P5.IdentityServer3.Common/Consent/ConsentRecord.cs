using System;

namespace P5.IdentityServer3.Common
{
    public class ConsentRecord : WrappedRecord<ConsentHandle>
    {
        public ConsentRecord()
        {
        }

        public ConsentRecord(ConsentHandle record)
            : base(record, f => f.CreateGuid())
        {
        }
    }
}