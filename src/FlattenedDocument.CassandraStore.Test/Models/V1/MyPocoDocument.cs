

using P5.Store.Core.Models;

namespace FlattenedDocument.CassandraStore.Test.Models.V1
{
    public class MyPocoDocument : ComplexDocument<MyPoco>
    {
        public MyPocoDocument(MyPoco document)
            : base(document)
        {
            DocumentVersion = "1.0";
            DocumentType = this.GetType().ToString();
        }

        public MyPocoDocument(string documentJson)
            : base(documentJson)
        {
        }
    }
}
