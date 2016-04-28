using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.BiggyJson
{
    public class ConsentStore : BiggyStore<ConsentRecord,Consent>, IConsentStore
    {
        public static ConsentStore NewFromSetting(StoreSettings settings)
        {
            var store = new ConsentStore(
                settings.Folder,
                settings.Database,
                settings.ConsentCollection);
            return store;
        }
        public ConsentStore(string folderStorage, string groupName = "IdentityServer3", string databaseName = "Consents")
            : base(folderStorage, groupName, databaseName)
        {
        }

        protected override Guid GetId(Consent record)
        {
            return record.CreateGuid(ConsentRecord.Namespace);
        }

        protected override ConsentRecord NewWrap(Consent record)
        {
            return new ConsentRecord(record);
        }

        public async Task<IEnumerable<Consent>> LoadAllAsync(string subject)
        {
            var collection = this.Store.TryLoadData();
            var query = from item in collection
                where subject == item.Record.Subject
                select (Consent)item.Record;
            return await Task.FromResult(query);
        }

        public async Task RevokeAsync(string subject, string client)
        {
            var id = GuidGenerator.CreateGuid(ConsentRecord.Namespace, client, subject);
            await DeleteAsync(id);
        }

        public async Task<Consent> LoadAsync(string subject, string client)
        {
            var id = GuidGenerator.CreateGuid(ConsentRecord.Namespace, client, subject);
            return await RetrieveAsync(id);
        }


    }
}