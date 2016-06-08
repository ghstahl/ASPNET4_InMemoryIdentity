using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.CassandraStore.DAO;
using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{
    public class TokenHandleStore : ITokenHandleStore
    {
        private ResilientSessionContainer _resilientSessionContainer;

        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer()); }
        }

        public TokenHandleStore()
        {
        }

        public async Task StoreAsync(string key, Token value)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    var flat = new FlattenedTokenHandle(key, value);
                    await ResilientSessionContainer.ResilientSession.CreateTokenHandleAsync(flat);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<Token> GetAsync(string key)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                   async () =>
                   {
                       await ResilientSessionContainer.EstablishSessionAsync();
                       IClientStore cs = new ClientStore();
                       return await ResilientSessionContainer.ResilientSession.FindTokenByKey(key, cs);
                   },
                   async (ex) => ResilientSessionContainer.HandleCassandraException<Token>(ex));
            return result;
        }

        public async Task RemoveAsync(string key)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                  async () =>
                  {
                      await ResilientSessionContainer.EstablishSessionAsync();
                      await ResilientSessionContainer.ResilientSession.DeleteTokenByKey(key);
                  },
                  async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                     async () =>
                     {
                         await ResilientSessionContainer.EstablishSessionAsync();
                         IClientStore cs = new ClientStore();
                         return await ResilientSessionContainer.ResilientSession.FindTokenMetadataBySubject(subject, cs);
                     },
                     async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<ITokenMetadata>>(ex));
            return result;

        }

        public async Task RevokeAsync(string subject, string client)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.DeleteTokensByClientIdAndSubjectId(client, subject);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }
    }
}