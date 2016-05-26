using System;

namespace P5.IdentityServer3.Common
{
    public class FlattenedConsentRecord : WrappedRecord<FlattenedConsentHandle>
    {
        public FlattenedConsentRecord()
        {
        }

        public FlattenedConsentRecord(FlattenedConsentHandle record)
            : base(record, f => f.CreateGuid())
        {
        }
    }
}