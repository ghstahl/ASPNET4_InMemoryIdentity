using System;

namespace P5.IdentityServer3.Common.Models
{
    public class DocumentMetaData : IDocumentMetaData
    {
        public DocumentMetaData()
        {
            Type = String.Empty;
            Version = String.Empty;
        }
        public DocumentMetaData(string type,string version)
        {
            Type = type;
            Version = version;
        }

        public string Type { get; set; }
        public string Version { get; set; }
    }
}