namespace P5.IdentityServer3.BiggyJson
{
    public interface IDeepCloneable
    {
        object DeepClone();
    }
    public interface IDeepCloneable<out T> : IDeepCloneable
    {
        T DeepClone();
    }
}