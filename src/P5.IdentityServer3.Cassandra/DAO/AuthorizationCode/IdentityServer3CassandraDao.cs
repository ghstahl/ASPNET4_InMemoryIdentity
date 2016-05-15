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
        // PREPARED STATEMENTS for AuthorizationCode
        //-----------------------------------------------

        #region PREPARED STATEMENTS for AuthorizationCode

        private static AsyncLazy<PreparedStatement> _CreateAuthorizationCodeByClientId { get; set; }
        private static AsyncLazy<PreparedStatement> _CreateAuthorizationCodeByKey { get; set; }

        private static AsyncLazy<PreparedStatement> _DeleteAuthorizationCodeByClientIdAndKey { get; set; }
        private static AsyncLazy<PreparedStatement> _DeleteAuthorizationCodeByKey { get; set; }

        #endregion

        public static void PrepareAuthorizationCodeStatements()
        {
            #region PREPARED STATEMENTS for AuthorizationCode

            /*
                         ************************************************
                            ClaimIdentityRecords text,
                            ClientId text,
                            CreationTime timestamp,
                            Expires timestamp,
                            IsOpenId boolean,
                            Key text,
                            Nonce text,
                            RedirectUri text,
                            RequestedScopes text,
                            SubjectId text,
                            WasConsentShown boolean,
                         ************************************************
                         */

            _CreateAuthorizationCodeByClientId =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"AuthorizationCodeHandle_By_ClientId(ClaimIdentityRecords, ClientId,CreationTime,Expires,IsOpenId,Key,Nonce,RedirectUri,RequestedScopes,SubjectId,WasConsentShown) " +
                            @"VALUES(?,?,?,?,?,?,?,?,?,?,?)");
                        return result;
                    });

            _CreateAuthorizationCodeByKey =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"AuthorizationCodeHandle_By_Key(ClaimIdentityRecords, ClientId,CreationTime,Expires,IsOpenId,Key,Nonce,RedirectUri,RequestedScopes,SubjectId,WasConsentShown) " +
                            @"VALUES(?,?,?,?,?,?,?,?,?,?,?)");
                        return result;
                    });

            _DeleteAuthorizationCodeByClientIdAndKey =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"Delete FROM AuthorizationCodeHandle_By_ClientId " +
                            @"WHERE clientid = ? " +
                            @"AND key = ?");
                        return result;
                    });
            _DeleteAuthorizationCodeByKey =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"Delete FROM AuthorizationCodeHandle_By_Key " +
                            @"WHERE key = ?");
                        return result;
                    });

            #endregion

        }

        public static async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
            IEnumerable<FlattenedAuthorizationCodeHandle> flats)
        {

            var result = new List<BoundStatement>();
            foreach (var flat in flats)
            {
                PreparedStatement prepared = await _CreateAuthorizationCodeByClientId;
                BoundStatement bound = prepared.Bind(
                    flat.ClaimIdentityRecords,
                    flat.ClientId,
                    flat.CreationTime,
                    flat.Expires,
                    flat.IsOpenId,
                    flat.Key,
                    flat.Nonce,
                    flat.RedirectUri,
                    flat.RequestedScopes,
                    flat.SubjectId,
                    flat.WasConsentShown
                    );
                result.Add(bound);

                prepared = await _CreateAuthorizationCodeByKey;
                bound = prepared.Bind(
                    flat.ClaimIdentityRecords,
                    flat.ClientId,
                    flat.CreationTime,
                    flat.Expires,
                    flat.IsOpenId,
                    flat.Key,
                    flat.Nonce,
                    flat.RedirectUri,
                    flat.RequestedScopes,
                    flat.SubjectId,
                    flat.WasConsentShown
                    );
                result.Add(bound);
            }
            return result;
        }

        public static async Task<List<BoundStatement>> BuildBoundStatements_ForAuthorizationCodeHandleDelete(string clientId,
            string key)
        {
            var result = new List<BoundStatement>();

            PreparedStatement prepared = await _DeleteAuthorizationCodeByClientIdAndKey;
            BoundStatement bound = prepared.Bind(
                clientId, key);
            result.Add(bound);

            prepared = await _DeleteAuthorizationCodeByKey;
            bound = prepared.Bind(key);
            result.Add(bound);

            return result;
        }


        public static async Task<bool> CreateAuthorizationCodeHandleAsync(string key,global::IdentityServer3.Core.Models.AuthorizationCode value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var fRecord = new FlattenedAuthorizationCodeHandle(key, value);
            return await CreateAuthorizationCodeHandleAsync(fRecord);
        }

        public static async Task<bool> CreateAuthorizationCodeHandleAsync(FlattenedAuthorizationCodeHandle tokenHandle,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var list = new List<FlattenedAuthorizationCodeHandle> { tokenHandle };
                return await CreateManyAuthorizationCodeHandleAsync(list, cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<bool> CreateManyAuthorizationCodeHandleAsync(
            IList<FlattenedAuthorizationCodeHandle> flattenedAuthorizationCodeHandles,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                if (flattenedAuthorizationCodeHandles == null)
                    throw new ArgumentNullException("flattenedAuthorizationCodeHandles");
                if (flattenedAuthorizationCodeHandles.Count == 0)
                    throw new ArgumentException("flattenedAuthorizationCodeHandles is empty");

                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();


                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForCreate(flattenedAuthorizationCodeHandles);
                batch.AddRange(boundStatements);

                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<global::IdentityServer3.Core.Models.AuthorizationCode> FindAuthorizationCodeByKey(
            string key,
            IClientStore clientStore,
            IScopeStore scopeStore,
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
                        mapper.SingleAsync<FlattenedAuthorizationCodeHandle>(
                            "SELECT * FROM authorizationcodehandle_by_key WHERE key = ?",  key);
                IAuthorizationCodeHandle ch = record;
                var result = await ch.MakeAuthorizationCodeAsync(clientStore, scopeStore);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<IEnumerable<ITokenMetadata>> FindAuthorizationCodeMetadataBySubject(string subject,
            IClientStore clientStore,
            IScopeStore scopeStore,
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
                        mapper.FetchAsync<FlattenedAuthorizationCodeHandle>(
                            "SELECT * FROM AuthorizationCodehandle_by_clientid WHERE subjectid = ?", subject);
                List<global::IdentityServer3.Core.Models.AuthorizationCode> authCodes = new List<global::IdentityServer3.Core.Models.AuthorizationCode>();
                foreach (var rec in record)
                {
                    var authCode = await rec.MakeAuthorizationCodeAsync(clientStore, scopeStore);
                    authCodes.Add(authCode);
                }

                return authCodes;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<bool> DeleteAuthorizationCodesByClientId(string client,
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
                        mapper.FetchAsync<FlattenedAuthorizationCodeHandle>(
                            "SELECT * FROM AuthorizationCodehandle_by_clientid WHERE ClientId = ?", client);
                cancellationToken.ThrowIfCancellationRequested();

                // now that we gots ourselves the record, we have the primary key
                // we can now build a big batch delete
                var batch = new BatchStatement();
                foreach (var rFind in record_find)
                {

                    var boundStatements =
                        await BuildBoundStatements_ForAuthorizationCodeHandleDelete(rFind.ClientId, rFind.Key);
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

        public static async Task<bool> DeleteAuthorizationCodesByClientIdAndSubjectId(string client, string subject,
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
                        mapper.FetchAsync<FlattenedAuthorizationCodeHandle>(
                            "SELECT * FROM AuthorizationCodehandle_by_clientid WHERE ClientId = ? AND subjectId = ?",
                            client, subject);
                cancellationToken.ThrowIfCancellationRequested();

                // now that we gots ourselves the record, we have the primary key
                // we can now build a big batch delete
                var batch = new BatchStatement();
                foreach (var rFind in record_find)
                {

                    var boundStatements =
                        await BuildBoundStatements_ForAuthorizationCodeHandleDelete(rFind.ClientId, rFind.Key);
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

        public static async Task<bool> DeleteAuthorizationCodeByKey(string key,
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
                        mapper.SingleAsync<FlattenedAuthorizationCodeHandle>(
                            "SELECT * FROM authorizationcodehandle_by_key WHERE key = ?", key);

                // now that we gots ourselves the record, we have the primary key

                cancellationToken.ThrowIfCancellationRequested();

                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForAuthorizationCodeHandleDelete(record_find.ClientId, key);
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
