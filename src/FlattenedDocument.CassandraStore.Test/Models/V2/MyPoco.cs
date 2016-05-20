using System.Collections.Generic;

namespace FlattenedDocument.CassandraStore.Test.Models.V2
{
    public class MyPoco
    {
        public string Name { get; set; }
        public string City { get; set; }
        private Dictionary<string, string> _services;

        public Dictionary<string, string> Services
        {
            get { return _services ?? (_services = new Dictionary<string, string>()); }
        }
    }
}