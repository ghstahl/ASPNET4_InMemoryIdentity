using P5.Store.Core.Services;

namespace P5.Store.Core.Models
{
    public interface IFlattenedDocument : IDocumentMetadata
    {
        string DocumentJson { get; }
    }
}