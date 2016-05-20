using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using P5.CassandraStore;
using P5.CassandraStore.DAO;
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

		private static string CreateFlattenedDocumentTableById { get; set; }
		private static string CreateFlattenedDocumentTableByTypeAndVersion { get; set; }
		private static List<string> FlattenedTables { get; set; }

		#endregion

		public static void PrepareFlattenedDocumentUtilityStatements(string seedTableName)
		{
			#region PREPARED STATEMENTS for FlattenedDocument
			string tableById = TableByIdName(seedTableName);
			string tableByTypeAndVersion = TableByTypeAndVersionName(seedTableName);
			FlattenedTables = new List<string> {tableById, tableByTypeAndVersion};

			/*
						 ************************************************
							Id uuid,
							DocumentType text,
							DocumentVersion text,
							Document text,
						 ************************************************
						 */

			CreateFlattenedDocumentTableById = string.Format(@"CREATE TABLE IF NOT EXISTS {0} (" +
															 @"Id uuid," +
															 @"DocumentType text," +
															 @"DocumentVersion text," +
                                                             @"DocumentJson text," +
															 @"PRIMARY KEY (Id))", tableById);

			CreateFlattenedDocumentTableByTypeAndVersion = string.Format(@"CREATE TABLE IF NOT EXISTS {0} (" +
																		 @"Id uuid," +
																		 @"DocumentType text," +
																		 @"DocumentVersion text," +
                                                                         @"DocumentJson text," +
																		 @"PRIMARY KEY (DocumentType,DocumentVersion))",
				tableByTypeAndVersion);

			#endregion

		}
		public static async Task<bool> TruncateTablesAsync(
		CancellationToken cancellationToken = default(CancellationToken))
		{
			var session = CassandraSession;
			return await CassandraDao.TruncateTablesAsync(session, FlattenedTables, cancellationToken);
		}

		public static async Task CreateFlattenedDocumentTablesAsync(
			CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var session = CassandraSession;
				IMapper mapper = new Mapper(session);
				cancellationToken.ThrowIfCancellationRequested();
				await mapper.ExecuteAsync(CreateFlattenedDocumentTableById);
				await mapper.ExecuteAsync(CreateFlattenedDocumentTableByTypeAndVersion);
			}
			catch (Exception e)
			{
				// only here to catch during a debug unit test.
				throw;
			}
		}
	}
}
