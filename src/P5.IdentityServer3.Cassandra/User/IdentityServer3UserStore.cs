using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using P5.CassandraStore.DAO;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra
{
    public class IdentityServer3UserStore : IIdentityServer3UserStore
    {
        private ResilientSessionContainer _resilientSessionContainer;

        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer()); }
        }

        public async Task CreateIdentityServerUserAsync(IdentityServerUser user)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.UpsertIdentityServerUserAsync(user);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<IdentityServerUser> FindIdentityServerUserByUserIdAsync(string userId)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                 async () =>
                 {
                     await ResilientSessionContainer.EstablishSessionAsync();
                     return
                         await
                             ResilientSessionContainer.ResilientSession.FindIdentityServerUserByUserIdAsync(userId);
                 },
                 async (ex) => ResilientSessionContainer.HandleCassandraException<IdentityServerUser>(ex));
            return result;
        }
    }
}