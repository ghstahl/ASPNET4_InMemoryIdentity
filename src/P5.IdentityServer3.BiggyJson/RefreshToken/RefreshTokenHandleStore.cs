using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.BiggyJson
{
    public class RefreshTokenHandleStore : BiggyStore<RefreshTokenHandleRecord, RefreshTokenHandle>, IRefreshTokenStore
    {
        public static RefreshTokenHandleStore NewFromDefaultSetting(string folderStorage)
        {
            var clientStore = ClientStore.NewFromDefaultSetting(folderStorage);
            var store = new RefreshTokenHandleStore(
                clientStore,
                folderStorage,
                StoreSettings.DefaultSettings.Database,
                StoreSettings.DefaultSettings.RefreshTokenCollection);
            return store;
        }

        public static RefreshTokenHandleStore NewFromSetting(StoreSettings settings)
        {
            var clientStore = ClientStore.NewFromSetting(settings);
            var store = new RefreshTokenHandleStore(
                clientStore,
                settings.Folder,
                settings.Database,
                settings.RefreshTokenCollection);
            return store;
        }

        private IClientStore _clientStore;

        public RefreshTokenHandleStore(
            IClientStore clientStore,
            string folderStorage,
            string groupName = "IdentityServer3",
            string databaseName = "TokenHandles")
            : base(folderStorage, groupName, databaseName)
        {
            _clientStore = clientStore;
        }


        protected override Guid GetId(RefreshTokenHandle record)
        {
            return record.CreateGuid(RefreshTokenHandleRecord.Namespace);
        }

        protected override RefreshTokenHandleRecord NewWrap(RefreshTokenHandle record)
        {
            var result = new RefreshTokenHandleRecord(record);
            return result;
        }

        public async Task StoreAsync(string key, RefreshToken value)
        {
            var record = new RefreshTokenHandle(key, value);
            await CreateAsync(record);
        }

        public async Task<RefreshToken> GetAsync(string key)
        {
            var tokenHandleRecord = new RefreshTokenHandleRecord(new RefreshTokenHandle(key, null));
            var result = RetrieveAsync(tokenHandleRecord.Id);
            if (result.Result == null)
                return await Task.FromResult<RefreshToken>(null);
            var token = result.Result.ToToken(_clientStore);
            return await Task.FromResult(token);
        }

        public async Task RemoveAsync(string key)
        {
            var record = new RefreshTokenHandleRecord(new RefreshTokenHandle(key, null));
            await DeleteAsync(record.Id);
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var collection = this.Store.TryLoadData();
            var query = from item in collection
                        where item.Record.SubjectId == subject
                        select item.Record.ToToken(_clientStore);

            var list = query.ToArray();
            return await Task.FromResult(list.Cast<ITokenMetadata>());
        }

        public async Task RevokeAsync(string subject, string client)
        {
            var collection = this.Store.TryLoadData();
            var query = from item in collection
                        where item.Record.SubjectId == subject && item.Record.ClientId == client
                        select item;
            foreach (var record in query)
            {
                await DeleteAsync(record.Id);
            }
        }
    }
}