using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.CassandraStore.Extensions;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for Token
        //-----------------------------------------------

        #region PREPARED STATEMENTS for Token

        private static AsyncLazy<PreparedStatement> _CreateTokenByClientId { get; set; }
        private static AsyncLazy<PreparedStatement> _CreateTokenByKey { get; set; }

        private static AsyncLazy<PreparedStatement> _DeleteTokenByClientIdAndKey { get; set; }
        private static AsyncLazy<PreparedStatement> _DeleteTokenByKey { get; set; }

        #endregion

        public static async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
            IEnumerable<FlattenedTokenHandle> flattenedTokenHandles)
        {

            var result = new List<BoundStatement>();
            foreach (var flattenedTokenHandle in flattenedTokenHandles)
            {
                PreparedStatement prepared_CreateTokenByClientId = await _CreateTokenByClientId;
                PreparedStatement prepared_CreateTokenByKey = await _CreateTokenByKey;
                BoundStatement bound_CreateTokenByClientId = prepared_CreateTokenByClientId.Bind(
                    flattenedTokenHandle.Audience,
                    flattenedTokenHandle.Claims,
                    flattenedTokenHandle.ClientId,
                    flattenedTokenHandle.CreationTime,
                    flattenedTokenHandle.Expires,
                    flattenedTokenHandle.Issuer,
                    flattenedTokenHandle.Key,
                    flattenedTokenHandle.Lifetime,
                    flattenedTokenHandle.SubjectId,
                    flattenedTokenHandle.Type,
                    flattenedTokenHandle.Version
                    );
                BoundStatement bound_CreateTokenByKey = prepared_CreateTokenByKey.Bind(
                    flattenedTokenHandle.Audience,
                    flattenedTokenHandle.Claims,
                    flattenedTokenHandle.ClientId,
                    flattenedTokenHandle.CreationTime,
                    flattenedTokenHandle.Expires,
                    flattenedTokenHandle.Issuer,
                    flattenedTokenHandle.Key,
                    flattenedTokenHandle.Lifetime,
                    flattenedTokenHandle.SubjectId,
                    flattenedTokenHandle.Type,
                    flattenedTokenHandle.Version
                    );
                result.Add(bound_CreateTokenByClientId);
                result.Add(bound_CreateTokenByKey);
            }
            return result;
        }

        public static async Task<List<BoundStatement>> BuildBoundStatements_ForTokenHandleDelete(string clientId,
            string key)
        {
            var result = new List<BoundStatement>();
            PreparedStatement prepared_DeleteTokenByClientIdAndKey = await _DeleteTokenByClientIdAndKey;
            PreparedStatement prepared_DeleteTokenByKey = await _DeleteTokenByKey;

            BoundStatement bound_DeleteTokenByClientIdAndKey = prepared_DeleteTokenByClientIdAndKey.Bind(
                clientId, key);

            BoundStatement bound_DeleteTokenByKey = prepared_DeleteTokenByKey.Bind(key);

            result.Add(bound_DeleteTokenByClientIdAndKey);
            result.Add(bound_DeleteTokenByKey);
            return result;
        }

        public static async Task<bool> CreateTokenHandleAsync(FlattenedTokenHandle tokenHandle,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var list = new List<FlattenedTokenHandle> { tokenHandle };
                return await CreateManyTokenHandleAsync(list, cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<bool> CreateManyTokenHandleAsync(IList<FlattenedTokenHandle> flattenedTokenHandles,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                if (flattenedTokenHandles == null)
                    throw new ArgumentNullException("flattenedTokenHandles");
                if (flattenedTokenHandles.Count == 0)
                    throw new ArgumentException("flattenedTokenHandles is empty");

                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForCreate(flattenedTokenHandles);
                batch.AddRange(boundStatements);

                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static async Task<global::IdentityServer3.Core.Models.Token> FindTokenByKey(string key,
            IClientStore clientStore,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await
                        mapper.SingleAsync<FlattenedTokenHandle>("SELECT * FROM tokenhandle_by_key WHERE key = ?", key);
                ITokenHandle ch = record;
                var result = ch.MakeIdentityServerToken(clientStore);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<IEnumerable<ITokenMetadata>> FindTokenMetadataBySubject(string subject,
            IClientStore clientStore,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await
                        mapper.FetchAsync<FlattenedTokenHandle>(
                            "SELECT * FROM tokenhandle_by_clientid WHERE subjectid = ?", subject);
                var query = from item in record
                    select item.MakeIdentityServerToken(clientStore);
                return query;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<bool> DeleteTokensByClientId(string client,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();

                // first find all the keys that are associated with this client
                var record_find =
                    await
                        mapper.FetchAsync<FlattenedTokenHandle>(
                            "SELECT * FROM tokenhandle_by_clientid WHERE ClientId = ?", client);
                cancellationToken.ThrowIfCancellationRequested();

                // now that we gots ourselves the record, we have the primary key
                // we can now build a big batch delete
                var batch = new BatchStatement();
                foreach (var rFind in record_find)
                {

                    var boundStatements = await BuildBoundStatements_ForTokenHandleDelete(rFind.ClientId, rFind.Key);
                    batch.AddRange(boundStatements);
                }
                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<bool> DeleteTokensByClientIdAndSubjectId(string client, string subject,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                // first find all the keys that are associated with this client and subject
                var record_find =
                    await
                        mapper.FetchAsync<FlattenedTokenHandle>(
                            "SELECT * FROM tokenhandle_by_clientid WHERE ClientId = ? AND subjectId = ?",
                            client, subject);
                cancellationToken.ThrowIfCancellationRequested();

                // now that we gots ourselves the record, we have the primary key
                // we can now build a big batch delete
                var batch = new BatchStatement();
                foreach (var rFind in record_find)
                {

                    var boundStatements = await BuildBoundStatements_ForTokenHandleDelete(rFind.ClientId, rFind.Key);
                    batch.AddRange(boundStatements);
                }
                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<bool> DeleteTokenByKey(string key,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record_find =
                    await
                        mapper.SingleAsync<FlattenedTokenHandle>("SELECT * FROM tokenhandle_by_key WHERE key = ?", key);

                // now that we gots ourselves the record, we have the primary key

                cancellationToken.ThrowIfCancellationRequested();

                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForTokenHandleDelete(record_find.ClientId, key);
                batch.AddRange(boundStatements);

                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
