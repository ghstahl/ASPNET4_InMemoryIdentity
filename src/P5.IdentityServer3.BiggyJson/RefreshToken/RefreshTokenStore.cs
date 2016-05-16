using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.IdentityServer3.Common.RefreshToken;

namespace P5.IdentityServer3.BiggyJson
{
    public class RefreshTokenStore : BiggyStore<RefreshTokenHandleRecord, RefreshTokenHandle>, IRefreshTokenStore
    {
        private IClientStore _clientStore;
        public RefreshTokenStore(StoreSettings settings)
            : this(settings, new ClientStore(settings))
        {
            _clientStore = new ClientStore(settings);
        }

        public RefreshTokenStore(
            StoreSettings settings,
            IClientStore clientStore)
            : this(clientStore,settings.Folder, settings.Database, settings.RefreshTokenCollection)
        {
        }
        public RefreshTokenStore(
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
            var token = await result.Result.MakeRefreshTokenAsync(_clientStore);
            return token;
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
                        select item.Record;

            List<RefreshToken> finalRefreshTokens = new List<RefreshToken>();
            foreach (var item in query)
            {
                var refreshToken = await item.MakeRefreshTokenAsync(_clientStore);
                finalRefreshTokens.Add(refreshToken);
            }

            return finalRefreshTokens;
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