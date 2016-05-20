using System;

namespace P5.Store.Core.Models
{
    public class DocumentRecord : IDocumentRecord
    {
        public string DocumentType { get;  set; }
        public string DocumentVersion { get; set; }
        public string DocumentJson { get;   set; }
        public Guid Id { get;   set; }
    }
}