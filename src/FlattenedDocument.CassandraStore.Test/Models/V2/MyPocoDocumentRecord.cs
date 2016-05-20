using System;
using P5.Store.Core.Models;

namespace FlattenedDocument.CassandraStore.Test.Models.V2
{
    public class MyPocoDocumentRecord : WrappedRecord<MyPocoDocument>
    {
        public static readonly Guid Namespace = new Guid("16c8df1e-2130-413f-93d0-781836623625");

        public MyPocoDocumentRecord() { }
        public MyPocoDocumentRecord(MyPocoDocument record)
            : base(record)
        {
        }

        protected override Guid GetIdOfRecord(MyPocoDocument record)
        {
             var id = record.CreateGuid(Namespace);
            return id;
        }
    }
}