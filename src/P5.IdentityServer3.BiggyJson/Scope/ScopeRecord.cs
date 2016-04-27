using System;
using IdentityServer3.Core.Models;

namespace P5.IdentityServer3.BiggyJson
{
    public class ScopeRecord : WrappedRecord<Scope>
    {
        public static readonly Guid Namespace = new Guid("ac6e677a-3070-46e5-a480-7593aaa26f37");

        public ScopeRecord() { }
        public ScopeRecord(Scope record)
            : base(record, f =>f.CreateGuid(Namespace))
        {
        }
    }
}