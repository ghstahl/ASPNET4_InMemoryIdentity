namespace P5.Store.Core.Models
{
    public interface IComplexDocument<T> : IFlattenedDocument where T : class
    {
        T Document { get; set; }
    }
}