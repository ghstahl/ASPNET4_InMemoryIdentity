using System;

namespace P5.Store.Core.Models
{
    public interface IDocumentMetadata
    {
        // Summary:
        //     Gets the document identifier.
        string DocumentType { get; }
        // Summary:
        //     Gets the document version.
        string DocumentVersion { get; set; }

    }
}