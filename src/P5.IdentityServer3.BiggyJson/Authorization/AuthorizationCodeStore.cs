using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.BiggyJson
{
    public class AuthorizationCodeStore : BiggyStore<AuthorizationCodeHandleRecord, AuthorizationCodeHandle>,
        IAuthorizationCodeStore
    {
        private IClientStore _clientStore;
        private IScopeStore _scopeStore;

        public AuthorizationCodeStore(
            StoreSettings settings)
            : base(settings.Folder, settings.Database, settings.AuthorizationCodeCollection)
        {
            _clientStore = new ClientStore(settings);
            _scopeStore = new ScopeStore(settings);
        }
        public AuthorizationCodeStore(
            StoreSettings settings,
            IClientStore clientStore,
            IScopeStore scopeStore)
            : base(settings.Folder, settings.Database, settings.AuthorizationCodeCollection)
        {
            _clientStore = clientStore;
            _scopeStore = scopeStore;
        }

        public AuthorizationCodeStore(
            IClientStore clientStore,
            IScopeStore scopeStore,
            string folderStorage,
            string groupName = "IdentityServer3",
            string databaseName = "AuthorizationCodes")
            : base(folderStorage, groupName, databaseName)
        {
            _clientStore = clientStore;
            _scopeStore = scopeStore;
        }

        protected override Guid GetId(AuthorizationCodeHandle record)
        {
            return record.CreateGuid(AuthorizationCodeHandleRecord.Namespace);
        }

        protected override AuthorizationCodeHandleRecord NewWrap(AuthorizationCodeHandle record)
        {
            return new AuthorizationCodeHandleRecord(record);
        }


        public async Task StoreAsync(string key, AuthorizationCode value)
        {
            var record = new AuthorizationCodeHandle(key, value);
            await CreateAsync(record);
        }

        public async Task<AuthorizationCode> GetAsync(string key)
        {
            var tokenHandleRecord = new AuthorizationCodeHandleRecord(new AuthorizationCodeHandle(key, null));
            var result = RetrieveAsync(tokenHandleRecord.Id);
            if (result.Result == null)
                return await Task.FromResult<AuthorizationCode>(null);
            var token = result.Result.ToAuthorizationCode(_clientStore, _scopeStore);
            return await Task.FromResult(token);
        }

        public async Task RemoveAsync(string key)
        {
            var record = new AuthorizationCodeHandleRecord(new AuthorizationCodeHandle(key, null));
            await DeleteAsync(record.Id);
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var collection = this.Store.TryLoadData();
            var query = from item in collection
                where item.Record.SubjectId == subject
                select item.Record.ToAuthorizationCode(_clientStore, _scopeStore);

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