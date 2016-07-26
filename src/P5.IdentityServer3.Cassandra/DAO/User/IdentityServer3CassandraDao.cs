using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer3.Core.Models;
using P5.CassandraStore;
using P5.CassandraStore.Extensions;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
    {

        //-----------------------------------------------
        // PREPARED STATEMENTS for User
        //-----------------------------------------------

        #region PREPARED STATEMENTS for User

        private AsyncLazy<PreparedStatement> _CreateClientByUserId { get; set; }
        private AsyncLazy<PreparedStatement> _CreateUserById { get; set; }
        private AsyncLazy<PreparedStatement> _FindUserById { get; set; }
        private AsyncLazy<PreparedStatement> _DeleteUserId { get; set; }
        private AsyncLazy<PreparedStatement> _DeleteUserIdFromClientIds;
        private AsyncLazy<PreparedStatement> _DeleteUserIdFromAllowedScopes;

        private const string SelectUserQuery = @"SELECT * FROM user_profile_by_id";

        #endregion
        public void PrepareUserStatements()
        {
            #region PREPARED STATEMENTS for User

            /*
            *************************************************
                    CREATE TABLE IF NOT EXISTS user_profile_by_id (
                        UserId text,
                        AllowedScopes text,
                        ClientIds text,
                        Enabled boolean,
                        UserName text,
                        PRIMARY KEY (id)
                    );
            ************************************************
            */
            _FindUserById =
                new AsyncLazy<PreparedStatement>(
                    () => _cassandraSession.PrepareAsync("SELECT * " +
                                                         "FROM user_profile_by_id " +
                                                         "WHERE userid = ?"));
            _CreateUserById =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"user_profile_by_id (UserId,Enabled,UserName) " +
                            @"VALUES(?,?,?)");
                        return result;
                    });
            _DeleteUserId =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"Delete FROM user_profile_by_id " +
                            @"WHERE userid = ?");
                        return result;
                    });



            _DeleteUserIdFromClientIds =
              new AsyncLazy<PreparedStatement>(
                  () =>
                  {
                      var result = _cassandraSession.PrepareAsync(
                          @"Delete FROM user_clientid " +
                          @"WHERE userid = ?");
                      return result;
                  });

            _DeleteUserIdFromAllowedScopes =
                          new AsyncLazy<PreparedStatement>(
                              () =>
                              {
                                  var result = _cassandraSession.PrepareAsync(
                                      @"Delete FROM user_scopename " +
                                      @"WHERE userid = ?");
                                  return result;
                              });
            #endregion
        }
        public async Task<List<BoundStatement>> BuildBoundStatements_ForUserDelete(string userId)
        {
            var result = new List<BoundStatement>();
            PreparedStatement prepared;
            BoundStatement bound;

            prepared = await _DeleteUserId;
            bound = prepared.Bind(userId);
            result.Add(bound);


            prepared = await _DeleteUserIdFromClientIds;
            bound = prepared.Bind(userId);
            result.Add(bound);

            prepared = await _DeleteUserIdFromAllowedScopes;
            bound = prepared.Bind(userId);
            result.Add(bound);

            return result;
        }

        public async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
            IdentityServerUserModel user)
        {

            var result = new List<BoundStatement>();
            // @"user_profile_by_id (UserId,Enabled,UserName) " +

            PreparedStatement prepared = await _CreateUserById;
            BoundStatement bound = prepared.Bind(
                user.UserId,
                user.Enabled,
                user.UserName
                );
            result.Add(bound);
            return result;
        }

        public async Task<bool> FindDoesUserExistByUserIdAsync(string userId,
         CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await mapper.SingleAsync<string>("SELECT userid FROM user_profile_by_id WHERE userid = ?", userId);

                return !string.IsNullOrEmpty(record);
            }
            catch (Exception e)
            {
                //TODO: need to get better info for records not there.
                return false;
            }
        }


        public async Task<IdentityServerUser> FindIdentityServerUserByUserIdAsync(string userId,
         CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await mapper.SingleAsync<IdentityServerUserModel>("SELECT * FROM user_profile_by_id WHERE userid = ?", userId);

                var isu = new IdentityServerUser()
                {
                    Enabled = record.Enabled,
                    UserId = record.UserId,
                    UserName = record.UserName
                };
                var allowedScopes = await FindAllowedScopesByUserIdAsync(userId, cancellationToken);
                isu.AllowedScopes = allowedScopes.ToList();

                var allowedClients = await FindClientIdsByUserIdAsync(userId, cancellationToken);
                isu.ClientIds = allowedClients.ToList();

                return isu;
            }
            catch (Exception e)
            {
                //TODO: need to get better info for records not there.
                return null;
            }
        }



        public async Task<IdentityServerStoreAppliedInfo> UpsertIdentityServerUserAsync(IdentityServerUserModel user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
                throw new ArgumentNullException("user");
            var session = CassandraSession;
            IMapper mapper = new Mapper(session);
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var record = await mapper.InsertIfNotExistsAsync(user);
                return new IdentityServerStoreAppliedInfo(record.Applied);
            }
            catch (Exception e)
            {
                throw;
            }
            /*
                        var batch = new BatchStatement();
                        var boundStatements = await BuildBoundStatements_ForCreate(user);
                        batch.AddRange(boundStatements);
                        await session.ExecuteAsync(batch).ConfigureAwait(false);
             *   return new IdentityServerStoreAppliedInfo(true);
             * */

        }

        public async Task<IdentityServerStoreAppliedInfo> UpsertIdentityServerUserAsync(IdentityServerUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            if (user == null)
                throw new ArgumentNullException("user");
            var model = new IdentityServerUserModel()
            {
                Enabled = user.Enabled,
                UserId = user.UserId,
                UserName = user.UserName
            };
            return await UpsertIdentityServerUserAsync(model,
                cancellationToken);
        }

        public async Task<IdentityServerStoreAppliedInfo> DeleteUserByUserIdAsync(string userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("userId");
            var session = CassandraSession;
            cancellationToken.ThrowIfCancellationRequested();


            var batch = new BatchStatement();
            var boundStatements = await BuildBoundStatements_ForUserDelete(userId);
            batch.AddRange(boundStatements);

            await session.ExecuteAsync(batch).ConfigureAwait(false);
            return new IdentityServerStoreAppliedInfo(true);
        }


    }
}
