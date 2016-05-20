

using P5.Store.Core.Models;

namespace FlattenedDocument.CassandraStore.Test.Models.V2
{
    public class MyPocoDocument : ComplexDocument<MyPoco>
    {
        public MyPocoDocument(MyPoco document)
            : base(document)
        {
            DocumentVersion = "2.0";
            DocumentType = this.GetType().ToString();
        }

        public MyPocoDocument(string documentJson)
            : base(documentJson)
        {
        }
    }
}
