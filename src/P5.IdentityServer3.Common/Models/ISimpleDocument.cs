namespace P5.IdentityServer3.Common.Models
{
    public interface ISimpleDocument
    {
        object Document { get; }
        string DocumentJson { get; }
    }
}
