using Newtonsoft.Json;

namespace P5.IdentityServer3.Common.Models
{
    public class DocumentRecord<T> : IDocumentRecord where T : class
    {
        private T _document;

        public DocumentRecord(T document)
        {
            _document = document;
        }
        public DocumentRecord(T document, DocumentMetaData metaData)
        {
            _document = document;
            DocumentMetaData = metaData;
        }


        public DocumentMetaData DocumentMetaData { get; set; }

        public IDocumentMetaData MetaData { get; set; }

        public object Document { get; set; }

        public string DocumentJson
        {
            get
            {
                string output = JsonConvert.SerializeObject(_document);
                return output;
            }
        }


        public string MetaDataJson
        {
            get
            {
                string output = JsonConvert.SerializeObject(DocumentMetaData);
                return output;
            }
        }
    }
}