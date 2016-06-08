using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using FlattenedDocument.CassandraStore.DAO;
using P5.CassandraStore.DAO;
using P5.Store.Core.Models;
using P5.Store.Core.Services;

namespace FlattenedDocument.CassandraStore.Store
{
    public class ResilientSessionContainer
    {
        public FlattenedDocumentCassandraDao ResilientSession { get; private set; }

        public TryWithAwaitInCatchExcpetionHandleResult<T> HandleCassandraException<T>(Exception ex)
        {
            var nhae = ex as NoHostAvailableException;
            if (nhae != null)
            {
                ResilientSession = null;
            }
            return new TryWithAwaitInCatchExcpetionHandleResult<T>
            {
                RethrowException = true
            };
        }

        public async Task EstablishSessionAsync()
        {
            if (ResilientSession == null)
            {
                var rs = new FlattenedDocumentCassandraDao();
                await rs.EstablishConnectionAsync();
                ResilientSession = rs;
            }
        }
    }

    public class FlattenedDocumentStore : IFlattenedDocumentStore
    {
        private ResilientSessionContainer _resilientSessionContainer;

        ResilientSessionContainer ResilientSessionContainer
        {
            get { return _resilientSessionContainer ?? (_resilientSessionContainer = new ResilientSessionContainer()); }
        }

        public FlattenedDocumentStore()
        {
        }

        public async Task<IEnumerable<IDocumentRecord>> GetAllByTypeAsync(string documentType)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return
                        await ResilientSessionContainer.ResilientSession.FindFlattenedDocumentsByTypeAsync(documentType);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IEnumerable<IDocumentRecord>>(ex));
            return result;
        }

        public async Task<IDocumentRecord> GetByTypeAndVersionAsync(string documentType, string documentVersion)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return
                        await
                            ResilientSessionContainer.ResilientSession.FindFlattenedDocumentByTypeAndVersionAsync(
                                documentType, documentVersion);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IDocumentRecord>(ex));
            return result;
        }

        public async Task<IDocumentRecord> GetByIdAsync(Guid id)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return await ResilientSessionContainer.ResilientSession.FindFlattenedDocumentByIdAsync(id);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IDocumentRecord>(ex));
            return result;
        }

        public async Task RemoveByIdAsync(Guid id)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.DeleteFlattenedDocumentByIdAsync(id);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task RemoveAllByTypeAsync(string documentType)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.DeleteFlattenedDocumentByTypeAsync(documentType);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task RemoveByTypeAndVersionAsync(string documentType, string documentVersion)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await
                        ResilientSessionContainer.ResilientSession.DeleteFlattenedDocumentByTypeAndVersionAsync(
                            documentType, documentVersion);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));

        }

        public async Task StoreAsync(IDocumentRecord documentRecord)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.UpsertFlattenedDocumentAsync(documentRecord);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task StoreManyAsync(IEnumerable<IDocumentRecord> documentRecords)
        {
            await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    await ResilientSessionContainer.ResilientSession.UpsertManyFlattenedDocumentAsync(documentRecords);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<Task>(ex));
        }

        public async Task<IPage<DocumentRecord>> PageGetByTypeAsync(string documentType, int pageSize,
            byte[] pagingState)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return await ResilientSessionContainer.ResilientSession
                        .PageFlattenedDocumentsByTypeAsync(documentType,
                            pageSize,
                            pagingState);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IPage<DocumentRecord>>(ex));
            return result;
        }

        public async Task<IPage<DocumentRecord>> PageGetAsync(int pageSize, byte[] pagingState)
        {
            var result = await TryWithAwaitInCatch.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    await ResilientSessionContainer.EstablishSessionAsync();
                    return await ResilientSessionContainer.ResilientSession
                        .PageFlattenedDocumentsAsync(pageSize,pagingState);
                },
                async (ex) => ResilientSessionContainer.HandleCassandraException<IPage<DocumentRecord>>(ex));
            return result;

        }
    }
}
