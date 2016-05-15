using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using P5.CassandraStore.Extensions;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for Consent
        //-----------------------------------------------

        #region PREPARED STATEMENTS for Consent

        private static AsyncLazy<PreparedStatement> _CreateConsentById { get; set; }
        private static AsyncLazy<PreparedStatement> _CreateConsentByClientId { get; set; }

        #endregion
        public static void PrepareConsentStatements()
        {
            #region PREPARED STATEMENTS for Consent

            /*
                         ************************************************
                            id uuid,
                            ClientId text,
                            Scopes text,
                            Subject text,
                         ************************************************
                         */
            _CreateConsentById =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"consent_by_id(id,ClientId,Scopes,Subject) " +
                            @"VALUES(?,?,?,?)");
                        return result;
                    });
            _CreateConsentByClientId =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"consent_by_clientid(id,ClientId,Scopes,Subject) " +
                            @"VALUES(?,?,?,?)");
                        return result;
                    });

            #endregion

        }
        private static async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
          IList<FlattenedConsentRecord> flattenedConsentRecords)
        {
            try
            {
                var result = new List<BoundStatement>();
                foreach (var record in flattenedConsentRecords)
                {
                    var consent = record.Record;
                    PreparedStatement prepared = await _CreateConsentById;
                    BoundStatement bound = prepared.Bind(
                        record.Id,
                        consent.ClientId,
                        consent.Scopes,
                        consent.Subject);
                    result.Add(bound);

                    prepared = await _CreateConsentByClientId;
                    bound = prepared.Bind(
                        record.Id,
                        consent.ClientId,
                        consent.Scopes,
                        consent.Subject);
                    result.Add(bound);

                }
                return result;

            }
            catch (Exception e)
            {
                throw;
            }
        }
        public static async Task<bool> CreateConsentHandleAsync(FlattenedConsentHandle flat,
     CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var list = new List<FlattenedConsentHandle> { flat };
                return await CreateManyConsentHandleAsync(list, cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<bool> CreateManyConsentHandleAsync(IList<FlattenedConsentHandle> flatteneds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                if (flatteneds == null)
                    throw new ArgumentNullException("flattened");
                if (flatteneds.Count == 0)
                    throw new ArgumentException("flattened is empty");

                var session = DAO.IdentityServer3CassandraDao.CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                var batch = new BatchStatement();
                var query = from item in flatteneds
                            select new FlattenedConsentRecord(item);
                var boundStatements = await BuildBoundStatements_ForCreate(query.ToList());
                batch.AddRange(boundStatements);

                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<global::IdentityServer3.Core.Models.Consent> FindConsentByIdAsync(Guid id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = DAO.IdentityServer3CassandraDao.CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await mapper.SingleAsync<FlattenedConsentHandle>("SELECT * FROM consent_by_id WHERE id = ?", id);
                var result = record.MakeConsent();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<global::IdentityServer3.Core.Models.Consent> FindConsentBySubjectAndClientIdAsync(
            string subject, string clientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = DAO.IdentityServer3CassandraDao.CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await
                        mapper.SingleAsync<FlattenedConsentHandle>(
                            "SELECT * FROM consent_by_clientid WHERE clientid = ? AND subject = ?", clientId, subject);
                var result = record.MakeConsent();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<bool> DeleteConsentBySubjectAndClientIdAsync(
            string subject, string clientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = DAO.IdentityServer3CassandraDao.CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                await
                    mapper.DeleteAsync<FlattenedConsentHandle>("WHERE clientid = ? AND subject = ?", clientId, subject);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<IEnumerable<global::IdentityServer3.Core.Models.Consent>> FindConsentsBySubjectAsync(
            string subject,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = DAO.IdentityServer3CassandraDao.CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await
                        mapper.FetchAsync<FlattenedConsentHandle>(
                            "SELECT * FROM consent_by_clientid WHERE subject = ?", subject);
                var query = from item in record
                            select item.MakeConsent();
                return query;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<IEnumerable<global::IdentityServer3.Core.Models.Consent>> FindConsentsByClientIdAsync(
            string clientId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = DAO.IdentityServer3CassandraDao.CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await
                        mapper.FetchAsync<FlattenedConsentHandle>(
                            "SELECT * FROM consent_by_clientid WHERE clientid = ?", clientId);
                var query = from item in record
                            select item.MakeConsent();
                return query;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
