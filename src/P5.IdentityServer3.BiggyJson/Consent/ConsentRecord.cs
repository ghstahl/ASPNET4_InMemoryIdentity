using System;
using IdentityServer3.Core.Models;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.BiggyJson
{
    public class ConsentRecord : WrappedRecord<Consent>
    {
        public static readonly Guid Namespace = new Guid("ac3c7055-f0d9-4707-aa3e-c0bc6dc5a8a1");

        public ConsentRecord() { }
        public ConsentRecord(Consent record)
            : base(record, f => f.CreateGuid(Namespace))
        {
        }
    }
}