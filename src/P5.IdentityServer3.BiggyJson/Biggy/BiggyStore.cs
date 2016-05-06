using System;
using System.Linq;
using System.Threading.Tasks;
using Biggy.Data.Json;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.BiggyJson
{
    public abstract class BiggyStore<TWrappedRecord, T> where TWrappedRecord : WrappedRecord<T>, new() where T:class
    {
        protected string DatabaseName { get; set; }
        protected string CollectionName { get; set; }
        protected readonly string FolderStorage = string.Empty;
        protected JsonStore<TWrappedRecord> _theStore = null;
        protected JsonStore<TWrappedRecord> Store
        {
            get { return _theStore ?? (_theStore = new JsonStore<TWrappedRecord>(this.FolderStorage, DatabaseName, CollectionName)); }
        }
        public BiggyStore(string folderStorage, string database, string collection)
        {
            FolderStorage = folderStorage;
            DatabaseName = database;
            CollectionName = collection;
        }

        protected abstract Guid GetId(T record);
        protected abstract TWrappedRecord NewWrap(T record);
        public Task CreateAsync(T record)
        {
            var existing = RetrieveAsync(GetId(record));
            if (existing.Result == null)
            {
                Store.Add(NewWrap(record));
            }
            else
            {
                Store.Update(NewWrap(record));
            }
            return Task.FromResult(true);
        }
        public Task UpdateAsync(T record)
        {
            return CreateAsync(record);
        }
        public Task<T> RetrieveAsync(Guid id)
        {
            try
            {
                var collection = this.Store.TryLoadData();
                var query = from item in collection
                    where id == item.Id
                    select item;
                if(!query.Any())
                    return Task.FromResult<T>(null);
                var record = query.SingleOrDefault();

                T result = ((record == null) ? (T) null : (T) record.RecordObject);
                return Task.FromResult<T>(result);
            }
            catch (Exception e)
            {
//TODO: Log something
            }
            return Task.FromResult<T>(null);
        }
        public Task DeleteAsync(Guid id)
        {
            var collection = this.Store.TryLoadData();
            var query = from item in collection
                        where item.Id == id
                        select item;
            foreach (var item in query)
            {
                this.Store.Delete(item);
            }
            return Task.FromResult(true);
        }
    }
}