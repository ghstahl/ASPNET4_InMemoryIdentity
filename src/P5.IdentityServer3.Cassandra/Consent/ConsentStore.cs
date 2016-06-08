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
    public class ConsentStore : IConsentStore
    {
        private ResilientSessionContainer _resilientSessionContainer;

        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer()); }
        }

        public async Task<IEnumerable<Consent>> LoadAllAsync(string subject)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return await ResilientSessionContainer.ResilientSession.FindConsentsBySubjectAsync(subject);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<Consent>>(ex));
            return result;
        }

        public async Task RevokeAsync(string subject, string client)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.DeleteConsentBySubjectAndClientIdAsync(subject,
                            client);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task<Consent> LoadAsync(string subject, string client)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return
                        await
                            ResilientSessionContainer.ResilientSession.FindConsentBySubjectAndClientIdAsync(subject,
                                client);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Consent>(ex));
            return result;


        }

        public async Task UpdateAsync(Consent consent)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    List<FlattenedConsentHandle> flattenedConsentHandles = new List<FlattenedConsentHandle>
                    {
                        new FlattenedConsentHandle(consent)
                    };
                    await
                        ResilientSessionContainer.ResilientSession.CreateManyConsentHandleAsync(flattenedConsentHandles);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));


        }
    }
}