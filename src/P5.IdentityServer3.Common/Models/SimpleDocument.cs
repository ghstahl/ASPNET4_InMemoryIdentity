using Newtonsoft.Json;

namespace P5.IdentityServer3.Common.Models
{
    public class SimpleDocument<T> : ISimpleDocument where T : class
    {
        private T _document;

        public SimpleDocument(T document)
        {
            _document = document;
        }


        public object Document { get; set; }

        public string DocumentJson
        {
            get
            {
                string output = JsonConvert.SerializeObject(_document);
                return output;
            }
        }
    }
}