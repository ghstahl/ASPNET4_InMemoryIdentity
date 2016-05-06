namespace P5.IdentityServer3.Common.Models
{
    public interface IDocumentRecord
    {
        IDocumentMetaData MetaData { get; }
        object Document { get; }
        string DocumentJson { get; }

        string MetaDataJson { get; }
    }
}