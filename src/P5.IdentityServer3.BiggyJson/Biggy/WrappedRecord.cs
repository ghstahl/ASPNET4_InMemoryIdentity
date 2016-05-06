using System;
using Biggy.Core;
using Newtonsoft.Json;

namespace P5.IdentityServer3.BiggyJson
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class WrappedRecord2<T>  where T : class
    {
        private Guid _id = Guid.Empty;
        [PrimaryKey(false)]
        [JsonProperty]
        public Guid Id
        {
            get
            {
                if (_id == Guid.Empty)
                {
                    _id = GetIdFunc(Record);
                }
                return _id;
            }
            set { _id = value; }
        }

        public object RecordObject
        {
            get { return Record; }
            set { Record = (T)value; }
        }



        [JsonProperty]
        public T Record { get; set; }
        public WrappedRecord2()
        {
        }

        private Func<T, Guid> GetIdFunc;
        public WrappedRecord2(T value, Func<T, Guid> getId)
        {
            Record = value;
            GetIdFunc = getId;
        }
    }
}