﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Common;



namespace P5.IdentityServer3.BiggyJson
{
    public class ConsentStore : BiggyStore<ConsentRecord, ConsentHandle>, IConsentStore
    {
        public ConsentStore(StoreSettings settings)
            : base(settings.Folder, settings.Database, settings.ConsentCollection)
        {
        }
        public ConsentStore(string folderStorage, string groupName = "IdentityServer3", string databaseName = "Consents")
            : base(folderStorage, groupName, databaseName)
        {
        }

        protected override Guid GetId(ConsentHandle record)
        {
            return record.CreateGuid(ConsentRecord.Namespace);
        }

        protected override ConsentRecord NewWrap(ConsentHandle record)
        {
            return new ConsentRecord(record);
        }

        public async Task<IEnumerable<Consent>> LoadAllAsync(string subject)
        {
            var collection = this.Store.TryLoadData();
            var query = from item in collection
                where subject == item.Record.Subject
                select  item.Record.MakeConsent();
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
            var result = await RetrieveAsync(id);
            return result.MakeConsent();
        }

        public async Task UpdateAsync(Consent consent)
        {
            await base.UpdateAsync(new ConsentHandle(consent));
        }
    }
}