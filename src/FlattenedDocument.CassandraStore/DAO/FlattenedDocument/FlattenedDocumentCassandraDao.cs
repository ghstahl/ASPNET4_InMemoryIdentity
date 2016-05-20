using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using P5.CassandraStore;
using P5.CassandraStore.Extensions;
using P5.Store.Core.Models;
using ProductStore.Cassandra.DAO;


namespace FlattenedDocument.CassandraStore.DAO
{
    public partial class FlattenedDocumentCassandraDao
	{
		//-----------------------------------------------
		// PREPARED STATEMENTS for FlattenedDocument
		//-----------------------------------------------

		#region PREPARED STATEMENTS for FlattenedDocument

		private static AsyncLazy<PreparedStatement> _UpsertFlattenedDocumentById { get; set; }
		private static AsyncLazy<PreparedStatement> _UpsertFlattenedDocumentByTypeAndVersion { get; set; }
		private static AsyncLazy<PreparedStatement> _DeleteFlattenedDocumentById { get; set; }
		private static AsyncLazy<PreparedStatement> _DeleteFlattenedDocumentByType { get; set; }
		private static AsyncLazy<PreparedStatement> _DeleteFlattenedDocumentByTypeAndVersion { get; set; }
		private static string FindFlattenedDocumentQuery { get; set; }
        private static string FindFlattenedDocumentByIdQuery { get; set; }
		private static string FindFlattenedDocumentByType { get; set; }
		private static string FindFlattenedDocumentByTypeAndVersion { get; set; }

		#endregion

		public static void PrepareFlattenedDocumentStatements(string seedTableName)
		{
			#region PREPARED STATEMENTS for FlattenedDocument

			string tableById = TableByIdName(seedTableName);
			string tableByTypeAndVersion = TableByTypeAndVersionName(seedTableName);
			/*
						 ************************************************
							Id uuid,
							DocumentType text,
							DocumentVersion text,
							Document text,
						 ************************************************
						 */

            FindFlattenedDocumentQuery = string.Format("SELECT * FROM {0}", tableById);
            FindFlattenedDocumentByIdQuery = string.Format("SELECT * FROM {0} WHERE id = ?", tableById);

            
			FindFlattenedDocumentByType  =
				string.Format("SELECT * FROM {0} WHERE DocumentType = ?",
					tableByTypeAndVersion);

			FindFlattenedDocumentByTypeAndVersion =
				string.Format("SELECT * FROM {0} WHERE DocumentType = ? AND DocumentVersion = ?",
					tableByTypeAndVersion);

			var queryUpsertFlattenedDocumentById = string.Format(@"INSERT INTO " +
                                                                 @"{0}(Id,DocumentType,DocumentVersion,DocumentJson) " +
																 @"VALUES(?,?,?,?)", tableById);

			_UpsertFlattenedDocumentById =
				new AsyncLazy<PreparedStatement>(
					() =>
					{
						var result = CassandraSession.PrepareAsync(queryUpsertFlattenedDocumentById);
						return result;
					});

			var queryUpsertFlattenedDocumentByTypeAndVersion = string.Format(@"INSERT INTO " +
                                                                             @"{0}(Id,DocumentType,DocumentVersion,DocumentJson) " +
																			 @"VALUES(?,?,?,?)", tableByTypeAndVersion);
			_UpsertFlattenedDocumentByTypeAndVersion =
				new AsyncLazy<PreparedStatement>(
					() =>
					{
						var result = CassandraSession.PrepareAsync(queryUpsertFlattenedDocumentByTypeAndVersion);
						return result;
					});

			var queryDeleteFlattenedDocumentById = string.Format(@"Delete FROM {0} " +
																 @"WHERE id = ?", tableById);
			_DeleteFlattenedDocumentById =
				new AsyncLazy<PreparedStatement>(
					() =>
					{
						var result = CassandraSession.PrepareAsync(queryDeleteFlattenedDocumentById);
						return result;
					});

			var queryDeleteFlattenedDocumentByType = string.Format(@"Delete FROM {0} " +
																   @"WHERE DocumentType = ?", tableByTypeAndVersion);
			_DeleteFlattenedDocumentByType =
				new AsyncLazy<PreparedStatement>(
					() =>
					{
						var result = CassandraSession.PrepareAsync(queryDeleteFlattenedDocumentByType);
						return result;
					});

			var queryDeleteFlattenedDocumentByTypeAndVersion = string.Format(@"Delete FROM {0} " +
																			 @"WHERE DocumentType = ? " +
																			 @"AND DocumentVersion = ?",
				tableByTypeAndVersion);

			_DeleteFlattenedDocumentByTypeAndVersion =
				new AsyncLazy<PreparedStatement>(
					() =>
					{
						var result = CassandraSession.PrepareAsync(queryDeleteFlattenedDocumentByTypeAndVersion);
						return result;
					});

			#endregion
		}

		public static async Task<List<BoundStatement>> BuildBoundStatements_ForUpsert(
			IEnumerable<IDocumentRecord> items)
		{

			var result = new List<BoundStatement>();
			foreach (var item in items)
			{
				PreparedStatement prepared = await _UpsertFlattenedDocumentById;
				BoundStatement bound = prepared.Bind(
					item.Id,
					item.DocumentType,
					item.DocumentVersion,
					item.DocumentJson
					);
				result.Add(bound);

				prepared = await _UpsertFlattenedDocumentByTypeAndVersion;
				bound = prepared.Bind(
					item.Id,
					item.DocumentType,
					item.DocumentVersion,
					item.DocumentJson
					);
				result.Add(bound);
			}
			return result;
		}

		public static async Task<List<BoundStatement>> BuildBoundStatements_ForFlattenedDocumentDelete(Guid id,
			string documentType,
			string documentVersion)
		{
			var result = new List<BoundStatement>();

			PreparedStatement prepared = await _DeleteFlattenedDocumentByTypeAndVersion;
			BoundStatement bound = prepared.Bind(
				documentType, documentVersion);
			result.Add(bound);

			prepared = await _DeleteFlattenedDocumentById;
			bound = prepared.Bind(id);
			result.Add(bound);

			return result;
		}
        public static async Task<bool> DeleteFlattenedDocumentByTypeAsync(string type,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                var findResult = await FindFlattenedDocumentsByTypeAsync(type, cancellationToken);
                if (findResult != null)
                {
                    var batch = new BatchStatement();

                    foreach (var item in findResult)
                    {
                        var boundStatements = await BuildBoundStatements_ForFlattenedDocumentDelete(
                            item.Id,
                            item.DocumentType,
                            item.DocumentVersion);
                        batch.AddRange(boundStatements);
                    }
                    if (!batch.IsEmpty)
                    {
                        await session.ExecuteAsync(batch).ConfigureAwait(false);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                // only here to catch during a debug unit test.
                throw;
            }
        }

        public static async Task<bool> DeleteFlattenedDocumentByTypeAndVersionAsync(string type,string version,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                var findResult = await FindFlattenedDocumentByTypeAndVersionAsync(type, version, cancellationToken);
                if (findResult != null)
                {
                    var batch = new BatchStatement();
                    var boundStatements = await BuildBoundStatements_ForFlattenedDocumentDelete(
                        findResult.Id,
                        findResult.DocumentType,
                        findResult.DocumentVersion);
                    batch.AddRange(boundStatements);
                    await session.ExecuteAsync(batch).ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception e)
            {
                // only here to catch during a debug unit test.
                throw;
            }
        }

        public static async Task<bool> DeleteFlattenedDocumentByIdAsync(Guid id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                var findResult = await FindFlattenedDocumentByIdAsync(id, cancellationToken);
                if (findResult != null)
                {
                    var batch = new BatchStatement();
                    var boundStatements = await BuildBoundStatements_ForFlattenedDocumentDelete(
                        findResult.Id,
                        findResult.DocumentType,
                        findResult.DocumentVersion);
                    batch.AddRange(boundStatements);
                    await session.ExecuteAsync(batch).ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception e)
            {
                // only here to catch during a debug unit test.
                throw;
            }
        }
        public static async Task<bool> UpsertFlattenedDocumentAsync(IDocumentRecord documentRecord,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var list = new List<IDocumentRecord> { documentRecord };
				return await UpsertManyFlattenedDocumentAsync(list, cancellationToken);
			}
			catch (Exception e)
			{
				// only here to catch during a debug unit test.
				throw;
			}
		}

		public static async Task<bool> UpsertManyFlattenedDocumentAsync(
			IEnumerable<IDocumentRecord> documentRecords,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				if (documentRecords == null)
                    throw new ArgumentNullException("documentRecords");

                var session = CassandraSession;
				cancellationToken.ThrowIfCancellationRequested();


				var batch = new BatchStatement();
				var boundStatements = await BuildBoundStatements_ForUpsert(documentRecords);
				batch.AddRange(boundStatements);

				await session.ExecuteAsync(batch).ConfigureAwait(false);
				return true;
			}
			catch (Exception e)
			{
				// only here to catch during a debug unit test.
				throw;
			}
		}

		public static async Task<IDocumentRecord> FindFlattenedDocumentByIdAsync(Guid id,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var session = CassandraSession;
				IMapper mapper = new Mapper(session);
				cancellationToken.ThrowIfCancellationRequested();
				var record =
					await
						mapper.SingleAsync<DocumentRecord>(FindFlattenedDocumentByIdQuery, id);
				IDocumentRecord ch = record;
				return ch;
			}
			catch (Exception e)
			{
                // throws an exception when nothing is found,
                // Indication of missing element is only in the message, which can't be trusted to be stable.
                // need to file a defect against datastax to get better exception information
			    return null;
			}
		}

        public static async Task<IDocumentRecord> FindFlattenedDocumentByTypeAndVersionAsync(string type, string version,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var session = CassandraSession;
				IMapper mapper = new Mapper(session);
				cancellationToken.ThrowIfCancellationRequested();
				var record =
					await
                        mapper.SingleAsync<DocumentRecord>(FindFlattenedDocumentByTypeAndVersion, type, version);
                IDocumentRecord ch = record;
				return ch;
			}
			catch (Exception e)
			{
                // throws an exception when nothing is found,
                // Indication of missing element is only in the message, which can't be trusted to be stable.
                // need to file a defect against datastax to get better exception information
                return null;
			}
		}

        public static async Task<IEnumerable<IDocumentRecord>> FindFlattenedDocumentsByTypeAsync(string type,
		  CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var session = CassandraSession;
				IMapper mapper = new Mapper(session);
				cancellationToken.ThrowIfCancellationRequested();
				var record =
					await
                        mapper.FetchAsync<DocumentRecord>(FindFlattenedDocumentByType, type);
                return record;
			}
			catch (Exception e)
			{
				// only here to catch during a debug unit test.
				throw;
			}
		}

        public static async Task<P5.Store.Core.Models.IPage<DocumentRecord>> PageFlattenedDocumentsAsync(
            int pageSize, byte[] pagingState,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                Cassandra.Mapping.IPage<DocumentRecord> page;
                if (pagingState == null)
                {
                    page = await mapper.FetchPageAsync<DocumentRecord>(
                        Cql.New(FindFlattenedDocumentQuery).WithOptions(opt =>
                            opt.SetPageSize(pageSize)));
                }
                else
                {
                    page = await mapper.FetchPageAsync<DocumentRecord>(
                        Cql.New(FindFlattenedDocumentQuery).WithOptions(opt =>
                            opt.SetPageSize(pageSize).SetPagingState(pagingState)));
                }

                // var result = CreatePageProxy(page);
                var result = new PageProxy<DocumentRecord>(page);

                return result;
            }
            catch (Exception e)
            {
                // only here to catch during a debug unit test.
                throw;
            }
        }
        public static async Task<P5.Store.Core.Models.IPage<DocumentRecord>> PageFlattenedDocumentsByTypeAsync(
            string type,
            int pageSize, byte[] pagingState,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                Cassandra.Mapping.IPage<DocumentRecord> page;
                if (pagingState == null)
                {
                    page = await mapper.FetchPageAsync<DocumentRecord>(
                        Cql.New(FindFlattenedDocumentByType, type).WithOptions(opt =>
                            opt.SetPageSize(pageSize)));
                }
                else
                {
                    page = await mapper.FetchPageAsync<DocumentRecord>(
                        Cql.New(FindFlattenedDocumentByType, type).WithOptions(opt =>
                            opt.SetPageSize(pageSize).SetPagingState(pagingState)));
                }

                // var result = CreatePageProxy(page);
                var result = new PageProxy<DocumentRecord>(page);

                return result;
            }
            catch (Exception e)
            {
                // only here to catch during a debug unit test.
                throw;
            }
        }
#if _USE_CASTLE
        private static P5.Store.Core.Models.IPage<DocumentRecord> CreatePageProxy(
           Cassandra.Mapping.IPage<DocumentRecord> cassandrPage)
        {
            ProxyGenerator generator = new ProxyGenerator();

            var service =
                generator.CreateInterfaceProxyWithoutTarget<P5.Store.Core.Models.IPage<DocumentRecord>>(
                    new CassandraPageInterceptor<DocumentRecord>(
                        new CassandraPageProxy<DocumentRecord>(cassandrPage)));
            return service;
        }
#endif

	}
}
