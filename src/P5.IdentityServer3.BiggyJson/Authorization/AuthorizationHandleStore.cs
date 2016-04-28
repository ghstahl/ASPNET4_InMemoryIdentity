using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace P5.IdentityServer3.BiggyJson
{
    public class AuthorizationHandleStore : BiggyStore<AuthorizationCodeHandleRecord, AuthorizationCodeHandle>, IAuthorizationCodeStore
    {
        private IClientStore _clientStore;

        public AuthorizationHandleStore(
            IClientStore clientStore,
            string folderStorage,
            string groupName = "IdentityServer3",
            string databaseName = "AuthorizationCodes")
            : base(folderStorage, groupName, databaseName)
        {
            _clientStore = clientStore;
        }

        protected override Guid GetId(AuthorizationCodeHandle record)
        {
            return record.CreateGuid(AuthorizationCodeHandleRecord.Namespace);
        }

        protected override AuthorizationCodeHandleRecord NewWrap(AuthorizationCodeHandle record)
        {
            return new AuthorizationCodeHandleRecord(record);
        }


        public Task StoreAsync(string key, AuthorizationCode value)
        {
            throw new NotImplementedException();
        }

        public Task<AuthorizationCode> GetAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            throw new NotImplementedException();
        }

        public Task RevokeAsync(string subject, string client)
        {
            throw new NotImplementedException();
        }
    }
}