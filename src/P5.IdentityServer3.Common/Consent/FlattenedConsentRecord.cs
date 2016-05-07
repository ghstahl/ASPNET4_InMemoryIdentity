using System;

namespace P5.IdentityServer3.Common
{
    public class FlattenedConsentRecord : WrappedRecord<FlattenedConsentHandle>
    {
        public static readonly Guid Namespace = new Guid("ac3c7055-f0d9-4707-aa3e-c0bc6dc5a8a1");

        public FlattenedConsentRecord() { }
        public FlattenedConsentRecord(FlattenedConsentHandle record)
            : base(record, f => f.CreateGuid(Namespace))
        {
        }
    }
}