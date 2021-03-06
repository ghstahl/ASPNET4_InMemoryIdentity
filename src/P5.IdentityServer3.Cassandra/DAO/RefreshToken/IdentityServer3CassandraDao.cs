﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using P5.CassandraStore;
using P5.CassandraStore.Extensions;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.RefreshToken;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for RefreshToken
        //-----------------------------------------------

        #region PREPARED STATEMENTS for RefreshToken

        private  AsyncLazy<PreparedStatement> _CreateRefreshTokenByClientId { get; set; }
        private  AsyncLazy<PreparedStatement> _CreateRefreshTokenByKey { get; set; }

        private  AsyncLazy<PreparedStatement> _DeleteRefreshTokenByClientIdAndKey { get; set; }
        private  AsyncLazy<PreparedStatement> _DeleteRefreshTokenByKey { get; set; }

        #endregion

        public void PrepareRefreshTokenStatements()
        {
            #region PREPARED STATEMENTS for RefreshToken

            /*
                         ************************************************
                            AccessToken text,
                            ClientId text,
                            CreationTime timestamp,
                            Expires timestamp,
                            Key text,
                            Lifetime int,
                            SubjectId text,
                            Version int,
                         ************************************************
                         */

            _CreateRefreshTokenByClientId =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"RefreshTokenHandle_By_ClientId(AccessToken, ClientId,CreationTime,Expires,Key,Lifetime,SubjectId,Version) " +
                            @"VALUES(?,?,?,?,?,?,?,?)");
                        return result;
                    });

            _CreateRefreshTokenByKey =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"RefreshTokenHandle_By_Key(AccessToken, ClientId,CreationTime,Expires,Key,Lifetime,SubjectId,Version) " +
                            @"VALUES(?,?,?,?,?,?,?,?)");
                        return result;
                    });

            _DeleteRefreshTokenByClientIdAndKey =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"Delete FROM Refreshtokenhandle_by_clientid " +
                            @"WHERE clientid = ? " +
                            @"AND key = ?");
                        return result;
                    });
            _DeleteRefreshTokenByKey =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"Delete FROM Refreshtokenhandle_by_key " +
                            @"WHERE key = ?");
                        return result;
                    });

            #endregion            

        }
        public async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
            IEnumerable<FlattenedRefreshTokenHandle> flattenedTokenHandles)
        {

            var result = new List<BoundStatement>();
            foreach (var flattenedTokenHandle in flattenedTokenHandles)
            {
                PreparedStatement prepared = await _CreateRefreshTokenByClientId;
                BoundStatement bound = prepared.Bind(
                    flattenedTokenHandle.AccessToken,
                    flattenedTokenHandle.ClientId,
                    flattenedTokenHandle.CreationTime,
                    flattenedTokenHandle.Expires,
                    flattenedTokenHandle.Key,
                    flattenedTokenHandle.LifeTime,
                    flattenedTokenHandle.SubjectId,
                    flattenedTokenHandle.Version
                    );
                result.Add(bound);

                prepared = await _CreateRefreshTokenByKey;
                bound = prepared.Bind(
                    flattenedTokenHandle.AccessToken,
                    flattenedTokenHandle.ClientId,
                    flattenedTokenHandle.CreationTime,
                    flattenedTokenHandle.Expires,
                    flattenedTokenHandle.Key,
                    flattenedTokenHandle.LifeTime,
                    flattenedTokenHandle.SubjectId,
                    flattenedTokenHandle.Version
                    );
                result.Add(bound);
            }
            return result;
        }

        public async Task<List<BoundStatement>> BuildBoundStatements_ForRefreshTokenHandleDelete(string clientId,
            string key)
        {
            var result = new List<BoundStatement>();

            PreparedStatement prepared = await _DeleteRefreshTokenByClientIdAndKey;
            BoundStatement bound = prepared.Bind(
                clientId, key);
            result.Add(bound);

            prepared = await _DeleteRefreshTokenByKey;
            bound = prepared.Bind(key);
            result.Add(bound);

            return result;
        }

        public async Task<bool> CreateRefreshTokenHandleAsync(FlattenedRefreshTokenHandle tokenHandle,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                MyMappings.Init();
                var list = new List<FlattenedRefreshTokenHandle> {tokenHandle};
                return await CreateManyRefreshTokenHandleAsync(list, cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> CreateManyRefreshTokenHandleAsync(
            IList<FlattenedRefreshTokenHandle> flattenedTokenHandles,
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

        public async Task<global::IdentityServer3.Core.Models.RefreshToken> FindRefreshTokenByKey(string key,
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
                        mapper.SingleAsync<FlattenedRefreshTokenHandle>(
                            "SELECT * FROM refreshtokenhandle_by_key WHERE key = ?", key);
                IRefreshTokenHandle ch = record;
                var result = await ch.MakeRefreshTokenAsync(clientStore);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ITokenMetadata>> FindRefreshTokenMetadataBySubject(string subject,
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
                        mapper.FetchAsync<FlattenedRefreshTokenHandle>(
                            "SELECT * FROM refreshtokenhandle_by_clientid WHERE subjectid = ?", subject);
                
                List<ITokenMetadata> listTokenMetadatas = new List<ITokenMetadata>();
                foreach (var item in record)
                {
                    var tmd = await item.MakeRefreshTokenAsync(clientStore);
                    listTokenMetadatas.Add(tmd);
                }

                return listTokenMetadatas;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> DeleteRefreshTokensByClientId(string client,
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
                        mapper.FetchAsync<FlattenedRefreshTokenHandle>(
                            "SELECT * FROM refreshtokenhandle_by_clientid WHERE ClientId = ?", client);
                cancellationToken.ThrowIfCancellationRequested();

                // now that we gots ourselves the record, we have the primary key
                // we can now build a big batch delete
                var batch = new BatchStatement();
                foreach (var rFind in record_find)
                {

                    var boundStatements =
                        await BuildBoundStatements_ForRefreshTokenHandleDelete(rFind.ClientId, rFind.Key);
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

        public async Task<bool> DeleteRefreshTokensByClientIdAndSubjectId(string client, string subject,
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
                        mapper.FetchAsync<FlattenedRefreshTokenHandle>(
                            "SELECT * FROM refreshtokenhandle_by_clientid WHERE ClientId = ? AND subjectId = ?",
                            client, subject);
                cancellationToken.ThrowIfCancellationRequested();

                // now that we gots ourselves the record, we have the primary key
                // we can now build a big batch delete
                var batch = new BatchStatement();
                foreach (var rFind in record_find)
                {

                    var boundStatements =
                        await BuildBoundStatements_ForRefreshTokenHandleDelete(rFind.ClientId, rFind.Key);
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

        public async Task<bool> DeleteRefreshTokenByKey(string key,
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
                        mapper.SingleAsync<FlattenedRefreshTokenHandle>(
                            "SELECT * FROM refreshtokenhandle_by_key WHERE key = ?", key);

                // now that we gots ourselves the record, we have the primary key

                cancellationToken.ThrowIfCancellationRequested();

                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForRefreshTokenHandleDelete(record_find.ClientId, key);
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
