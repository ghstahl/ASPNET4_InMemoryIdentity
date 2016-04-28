using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.BiggyJson
{
    public class ScopeStore : BiggyStore<ScopeRecord, Scope>, IScopeStore
    {
        public static ScopeStore NewFromSetting(StoreSettings settings)
        {
            var store = new ScopeStore(
                settings.Folder,
                settings.Database,
                settings.ScopeCollection);
            return store;
        }

        public ScopeStore(string folderStorage, string groupName = "IdentityServer3", string databaseName = "Scopes")
            : base(folderStorage, groupName, databaseName)
        {
        }
        protected override Guid GetId(Scope record)
        {
            return record.CreateGuid(ScopeRecord.Namespace);
        }

        protected override ScopeRecord NewWrap(Scope record)
        {
            return new ScopeRecord(record);
        }

        public async Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            var collection = this.Store.TryLoadData();
            var query = from item in collection
                join name in scopeNames
                    on item.Record.Name equals name
                select item.Record;

            return await Task.FromResult(query);
        }

        public async Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            var collection = this.Store.TryLoadData();
            var query = from item in collection
                where item.Record.ShowInDiscoveryDocument || item.Record.ShowInDiscoveryDocument == publicOnly
                select item.Record;
            return await Task.FromResult(query);
        }


    }
}