using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.BiggyJson
{
    public class TokenHandleStore : BiggyStore<TokenHandleRecord, TokenHandle>, ITokenHandleStore
    {
        private IClientStore _clientStore;

        public TokenHandleStore(
            StoreSettings settings)
            : this(settings, new ClientStore(settings))
        {       
        }
        public TokenHandleStore(
            StoreSettings settings,
            IClientStore clientStore)
            : this(clientStore, settings.Folder, settings.Database, settings.TokenHandleCollection)
        { 
        }

        public TokenHandleStore(
            IClientStore clientStore,
            string folderStorage,
            string databaseName = "IdentityServer3",
            string collectionName = "TokenHandles")
            : base(folderStorage, databaseName, collectionName)
        {
            _clientStore = clientStore;
        }

        protected override Guid GetId(TokenHandle record)
        {
            return record.CreateGuid(TokenHandleRecord.Namespace);
        }

        protected override TokenHandleRecord NewWrap(TokenHandle record)
        {
            return new TokenHandleRecord(record);
        }

        public async Task StoreAsync(string key, Token value)
        {
            var tokenHandle = new TokenHandle(key,value);
            await CreateAsync(tokenHandle);
        }

        public async Task<Token> GetAsync(string key)
        {
            var tokenHandleRecord = new TokenHandleRecord(new TokenHandle(key,null));
            var result = RetrieveAsync(tokenHandleRecord.Id);
            if (result.Result == null)
                return await Task.FromResult<Token>(null);
            var token = result.Result.ToToken(_clientStore);
            return await Task.FromResult(token);
        }

        public async Task RemoveAsync(string key)
        {
            var tokenHandleRecord = new TokenHandleRecord(new TokenHandle(key, null));
            await DeleteAsync(tokenHandleRecord.Id);
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