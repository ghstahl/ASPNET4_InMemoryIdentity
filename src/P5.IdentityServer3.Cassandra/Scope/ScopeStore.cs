using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;

using P5.IdentityServer3.Cassandra.DAO;
using System.Linq;
using IdentityServer3.Core.Models;
using P5.CassandraStore.DAO;
using P5.IdentityServer3.Common;
using P5.Store.Core.Models;

namespace P5.IdentityServer3.Cassandra
{
    public class ScopeStore : IIdentityServer3AdminScopeStore
    {
        private ResilientSessionContainer _resilientSessionContainer;
        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer()); }
        }
        public async Task<IEnumerable<Scope>> FindScopesAsync(
            IEnumerable<string> scopeNames)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                   async () =>
                   {
                       await ResilientSessionContainer.EstablishSessionAsync();
                       return await ResilientSessionContainer.ResilientSession.FindScopesByNamesAsync(scopeNames);
                   },
                   async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<Scope>>(ex));
            return result;
        }

        public async Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                   async () =>
                   {
                       await ResilientSessionContainer.EstablishSessionAsync();
                       return await ResilientSessionContainer.ResilientSession.FindScopesAsync(publicOnly);
                   },
                   async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<Scope>>(ex));
            return result;
        }

        public async Task UpdateScopeByNameAsync(string name, IEnumerable<PropertyValue> properties)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.UpdateScopeByNameAsync(name, properties);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task CreateScopeAsync(Scope scope)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                   async () =>
                   {
                       await ResilientSessionContainer.EstablishSessionAsync();
                       await ResilientSessionContainer.ResilientSession.UpsertScopeAsync(scope);
                   },
                   async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task DeleteScopeAsync(Scope scope)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                  async () =>
                  {
                      await ResilientSessionContainer.EstablishSessionAsync();
                      await ResilientSessionContainer.ResilientSession.DeleteScopeAsync(scope);
                  },
                  async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }


        public async Task AddScopeSecretsAsync(string name, IEnumerable<Secret> secrets)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                      async () =>
                      {
                          await ResilientSessionContainer.EstablishSessionAsync();
                          await ResilientSessionContainer.ResilientSession.AddScopeSecretsByNameAsync(name, secrets);
                      },
                      async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task DeleteScopeSecretsAsync(string name, IEnumerable<Secret> secrets)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.DeleteScopeSecretsFromScopeByNameAsync(name, secrets);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }


        public async Task AddScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.AddScopeClaimsToScopeByNameAsync(name, claims);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task DeleteScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                   async () =>
                   {
                       await ResilientSessionContainer.EstablishSessionAsync();
                       await ResilientSessionContainer.ResilientSession.DeleteScopeClaimsFromScopeByNameAsync(name, claims);
                   },
                   async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task UpdateScopeClaimsAsync(string name, IEnumerable<ScopeClaim> claims)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.UpdateScopeClaimsInScopeByNameAsync(name, claims);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task<IPage<Scope>> PageScopesAsync(int pageSize, byte[] pagingState)
        {
            IPage<Scope> result =
                 await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync<IPage<Scope>>(
                     async () =>
                     {
                         await ResilientSessionContainer.EstablishSessionAsync();
                         return await ResilientSessionContainer.ResilientSession.PageScopesAsync(pageSize, pagingState);
                     },
                     async (ex) => ResilientSessionContainer.HandleCassandraException<IPage<Scope>>(ex));
            return result;
        }

        public async Task<Scope> FindScopeByNameAsync(string name)
        {
           var result =
                  await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync< Scope >(
                      async () =>
                      {
                          await ResilientSessionContainer.EstablishSessionAsync();
                          return await ResilientSessionContainer.ResilientSession.FindScopeByNameAsync(name);
                      },
                      async (ex) => ResilientSessionContainer.HandleCassandraException< Scope>(ex));
            return result;
        }
    }
}