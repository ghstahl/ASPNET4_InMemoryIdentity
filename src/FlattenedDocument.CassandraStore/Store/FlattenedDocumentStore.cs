using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlattenedDocument.CassandraStore.DAO;
using P5.Store.Core.Models;
using P5.Store.Core.Services;

namespace FlattenedDocument.CassandraStore.Store
{
    public class FlattenedDocumentStore : IFlattenedDocumentStore
    {
        public async Task<IEnumerable<IDocumentRecord>> GetAllByTypeAsync(string documentType)
        {
            return await FlattenedDocumentCassandraDao.FindFlattenedDocumentsByTypeAsync(documentType);
        }

        public async Task<IDocumentRecord> GetByTypeAndVersionAsync(string documentType, string documentVersion)
        {
            return await FlattenedDocumentCassandraDao.FindFlattenedDocumentByTypeAndVersionAsync(documentType, documentVersion);
        }

        public async Task<IDocumentRecord> GetByIdAsync(Guid id)
        {
            return await FlattenedDocumentCassandraDao.FindFlattenedDocumentByIdAsync(id);
        }

        public async Task RemoveByIdAsync(Guid id)
        {
            await FlattenedDocumentCassandraDao.DeleteFlattenedDocumentByIdAsync(id);
        }

        public async Task RemoveAllByTypeAsync(string documentType)
        {
            await FlattenedDocumentCassandraDao.DeleteFlattenedDocumentByTypeAsync(documentType);
        }

        public async Task RemoveByTypeAndVersionAsync(string documentType, string documentVersion)
        {
            await FlattenedDocumentCassandraDao.DeleteFlattenedDocumentByTypeAndVersionAsync(documentType, documentVersion);
        }

        public async Task StoreAsync(IDocumentRecord documentRecord)
        {
            await FlattenedDocumentCassandraDao.UpsertFlattenedDocumentAsync(documentRecord);
        }

        public async Task StoreManyAsync(IEnumerable<IDocumentRecord> documentRecords)
        {
            await FlattenedDocumentCassandraDao.UpsertManyFlattenedDocumentAsync(documentRecords);
        }

        public async Task<IPage<DocumentRecord>> PageGetByTypeAsync(string documentType, int pageSize, byte[] pagingState)
        {
            var result = await FlattenedDocumentCassandraDao.PageFlattenedDocumentsByTypeAsync(documentType, pageSize,
                pagingState);
            return result;
        }

        public async Task<IPage<DocumentRecord>> PageGetAsync(int pageSize, byte[] pagingState)
        {
            var result = await FlattenedDocumentCassandraDao.PageFlattenedDocumentsAsync( pageSize,
                pagingState);
            return result;
        }
    }
}
