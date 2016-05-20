using System;

namespace P5.Store.Core.Models
{
    public class ComplexDocumentRecord<T> : WrappedRecord<ComplexDocument<T>> where T : class
    {
        public static readonly Guid Namespace = new Guid("291F7D01-57A0-45CB-B2D5-347B8160DBD8");

        public ComplexDocumentRecord()
        {
        }

        public ComplexDocumentRecord(ComplexDocument<T> record)
            : base(record)
        {
        }

        protected override Guid GetIdOfRecord(ComplexDocument<T> record)
        {
            return record.CreateGuid(Namespace);
        }
    }
}