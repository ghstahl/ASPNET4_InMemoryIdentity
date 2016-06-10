namespace P5.IdentityServer3.Common
{
    public class FlattenedIdentityServerUserRecord : WrappedRecord<FlattenedIdentityServerUserHandle>
    {
        public FlattenedIdentityServerUserRecord() { }
        public FlattenedIdentityServerUserRecord(FlattenedIdentityServerUserHandle record)
            : base(record, f => f.CreateGuid())
        {
        }
    }
}