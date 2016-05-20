using System;
using Newtonsoft.Json;

namespace P5.Store.Core.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class WrappedRecord<T> : IDocumentRecord where T : class,IFlattenedDocument
    {
        protected WrappedRecord()
        {
        }

        protected WrappedRecord(T value)
        {
            Record = value;
        }

        private Guid _id;

        [JsonProperty]
        public Guid Id
        {
            get
            {
                if (_id == Guid.Empty)
                {
                    _id = GetIdOfRecord(Record);
                }
                return _id;
            }
            set { _id = value; }
        }

        protected abstract Guid GetIdOfRecord(T record);

        [JsonProperty]
        protected T Record { get; set; }

        public string DocumentType
        {
            get { return Record.DocumentType; }
        }

        public string DocumentVersion { get { return Record.DocumentVersion; } set { Record.DocumentVersion = value; }}
        public string DocumentJson { get { return Record.DocumentJson; } }
    }
}