using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Common;

 

namespace P5.IdentityServer3.BiggyJson
{
    public class ScopeStore : BiggyStore<ScopeRecord, ScopeHandle>, IScopeStore
    {
        public ScopeStore(StoreSettings settings)
            : base(settings.Folder, settings.Database, settings.ScopeCollection)
        {
        }
        public ScopeStore(string folderStorage, string database = "IdentityServer3", string databaseName = "Scopes")
            : base(folderStorage, database, databaseName)
        {
        }
        protected override Guid GetId(ScopeHandle record)
        {
            return record.CreateGuid(ScopeRecord.Namespace);
        }

        protected override ScopeRecord NewWrap(ScopeHandle record)
        {
            return new ScopeRecord(record);
        }

        public async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            var collection = this.Store.TryLoadData();
            var query = from item in collection
                join name in scopeNames
                    on item.Record.Name equals name
                select item.Record;

            List<global::IdentityServer3.Core.Models.Scope> finalScopes = new List<Scope>();
            foreach (var item in query)
            {
                var scope = await item.MakeIdentityServerScopeAsync();
                finalScopes.Add(scope);
            }
            return finalScopes;
        }

        public async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> GetScopesAsync(bool publicOnly = true)
        {
            var collection = this.Store.TryLoadData();
            var query = from item in collection
                where item.Record.ShowInDiscoveryDocument || item.Record.ShowInDiscoveryDocument == publicOnly
                select item.Record;

            List<global::IdentityServer3.Core.Models.Scope> finalScopes = new List<Scope>();
            foreach (var item in query)
            {
                var scope = await item.MakeIdentityServerScopeAsync();
                finalScopes.Add(scope);
            }
            return finalScopes;
        }


    }
}