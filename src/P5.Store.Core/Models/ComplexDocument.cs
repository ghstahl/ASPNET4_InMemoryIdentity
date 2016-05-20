using System;
using Newtonsoft.Json;

namespace P5.Store.Core.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ComplexDocument<T> : IComplexDocument<T> where T : class
    {
        public ComplexDocument(T document)
        {
            Document = document;
        }

        public ComplexDocument(string documentJson)
        {
            Document = JsonConvert.DeserializeObject<T>(documentJson);
        }

        [JsonProperty]
        public string DocumentType { get; set; }

        [JsonProperty]
        public string DocumentVersion { get; set; }

        [JsonProperty]
        public T Document { get; set; }

        public string DocumentJson
        {
            get
            {
                string output = JsonConvert.SerializeObject(Document);
                return output;
            }
        }
    }
}