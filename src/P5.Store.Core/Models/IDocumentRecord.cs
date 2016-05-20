using System;

namespace P5.Store.Core.Models
{
    public interface IDocumentRecord : IFlattenedDocument
    {
        // Summary:
        //     Gets the document identifier.
        Guid Id { get; }
    }
}