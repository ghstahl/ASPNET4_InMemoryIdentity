using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer3.Core.Models;
using Newtonsoft.Json;
using P5.CassandraStore;
using P5.CassandraStore.Extensions;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.Extensions;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for Scope
        //-----------------------------------------------

        #region PREPARED STATEMENTS for Scope

        private AsyncLazy<PreparedStatement> _CreateScopeById { get; set; }
        private AsyncLazy<PreparedStatement> _CreateScopeByName { get; set; }

        private AsyncLazy<PreparedStatement> _CreateScopeClaimByNameAndScopeId { get; set; }
        private AsyncLazy<PreparedStatement> _CreateScopeClaimByNameAndScopeName { get; set; }

        private AsyncLazy<PreparedStatement> _FindScopeById { get; set; }
        private AsyncLazy<PreparedStatement> _FindScopeByName { get; set; }

        #endregion

        public  void PrepareScopeStatements()
        {
            #region PREPARED STATEMENTS for Scope

            _CreateScopeClaimByNameAndScopeId =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"scopeclaims_by_name_and_scopeid(Name,ScopeId,ScopeName) " +
                            @"VALUES(?,?,?)");
                        return result;
                    });
            _CreateScopeClaimByNameAndScopeName =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"scopeclaims_by_name_and_scopename(Name,ScopeId,ScopeName) " +
                            @"VALUES(?,?,?)");
                        return result;
                    });

            /*
            INSERT
            INTO scopes (Id,AllowUnrestrictedIntrospection,ClaimsRule,Description,DisplayName,
             *          Emphasize,Enabled,IncludeAllClaimsForUser,name,Required,ScopeSecrets,ShowInDiscoveryDocument,ScopeType)
            VALUES (1f65aebc-bf07-4afc-aa05-e9a1ed48e0b0,true,'1 ClaimsRule','1 Description','1 DisplayName',true,true,true,'1 name',true,[ 'rivendell', 'rohan' ],true,1 );
            */
            _CreateScopeById =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"scopes_by_id (Id,AllowUnrestrictedIntrospection,ClaimsDocument,ClaimsRule,Description,DisplayName,Emphasize,Enabled,IncludeAllClaimsForUser,name,Required,ScopeSecretsDocument,ShowInDiscoveryDocument,ScopeType) " +
                            @"VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)");
                        return result;
                    });

            _CreateScopeByName =
                new AsyncLazy<PreparedStatement>(
                    () =>
                    {
                        var result = _cassandraSession.PrepareAsync(
                            @"INSERT INTO " +
                            @"scopes_by_name (Id,AllowUnrestrictedIntrospection,ClaimsDocument,ClaimsRule,Description,DisplayName,Emphasize,Enabled,IncludeAllClaimsForUser,name,Required,ScopeSecretsDocument,ShowInDiscoveryDocument,ScopeType) " +
                            @"VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)");
                        return result;
                    });
            // All the statements needed by the CreateAsync method


            _FindScopeById =
                new AsyncLazy<PreparedStatement>(
                    () => _cassandraSession.PrepareAsync("SELECT * " +
                                                         "FROM scopes_by_id " +
                                                         "WHERE id = ?"));

            _FindScopeByName =
                new AsyncLazy<PreparedStatement>(
                    () => _cassandraSession.PrepareAsync("SELECT * " +
                                                         "FROM scopes_by_name " +
                                                         "WHERE name = ?"));

            #endregion
        }

        public async Task<bool> UpsertScopeAsync(Scope scope,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return
                await
                    UpsertScopeAsync(
                        new FlattenedScopeRecord(new FlattenedScopeHandle(scope)), cancellationToken);
        }

        public async Task<bool> UpsertScopeAsync(FlattenedScopeRecord scopeRecord,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                if (scopeRecord == null)
                    throw new ArgumentNullException("scopeRecord");

                // breaking batch apart.
                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForScopeClaimCreate(scopeRecord);
                batch.AddRange(boundStatements);
                await session.ExecuteAsync(batch).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                batch = new BatchStatement();
                boundStatements = await BuildBoundStatements_ForCreate(scopeRecord);
                batch.AddRange(boundStatements);

                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                throw;
            }

        }


        public async Task<global::IdentityServer3.Core.Models.Scope> FindScopeByIdAsync(Guid id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record = await mapper.SingleAsync<ScopeMappedRecord>("SELECT * FROM scopes_by_id WHERE id = ?", id);
                record.Claims = JsonConvert.DeserializeObject<List<ScopeClaim>>(record.ClaimsDocument);
                return record;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<global::IdentityServer3.Core.Models.Scope> FindScopeByName(string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();
                var record =
                    await mapper.SingleAsync<ScopeMappedRecord>("SELECT * FROM scopes_by_name WHERE name = ?", name);
                record.Claims = JsonConvert.DeserializeObject<List<ScopeClaim>>(record.ClaimsDocument);
                return record;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> FindScopesByNamesAsync(
            IEnumerable<string> scopeNames,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var queryInValues = from item in scopeNames
                    select string.Format("'{0}'", item);

                var inValues = string.Join(",", queryInValues);
                var query = string.Format("SELECT * FROM scopes_by_name WHERE name IN ({0})", inValues);


                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                var result = await mapper.FetchAsync<ScopeMappedRecord>(query);


                List<global::IdentityServer3.Core.Models.Scope> finalResult = new List<Scope>();

                foreach (var scopeMappedRecord in result)
                {
                    scopeMappedRecord.Claims =
                        JsonConvert.DeserializeObject<List<ScopeClaim>>(scopeMappedRecord.ClaimsDocument);
                    finalResult.Add(scopeMappedRecord);
                }

                return finalResult;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> FindScopesAsync(
            bool publicOnly = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var query = string.Format("SELECT * FROM scopes_by_id WHERE ShowInDiscoveryDocument = {0}",
                    publicOnly ? "true" : "false");
                var session = CassandraSession;
                IMapper mapper = new Mapper(session);
                var scopeMappedRecords = (await mapper.FetchAsync<ScopeMappedRecord>(query)).ToList();

                foreach (var scopeMappedRecord in scopeMappedRecords)
                {
                    scopeMappedRecord.Claims =
                        JsonConvert.DeserializeObject<List<ScopeClaim>>(scopeMappedRecord.ClaimsDocument);
                }
                var queryFinal = from item in scopeMappedRecords
                    select (Scope) item;
                return queryFinal;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<BoundStatement>> BuildBoundStatements_ForScopeClaimCreate(
            FlattenedScopeRecord scopeRecord)
        {
            var result = new List<BoundStatement>();

            var scope = scopeRecord.Record;
            //   var scopeSecretsDocument = new SimpleDocument<List<Secret>>(scope.ScopeSecrets);
            //   var claimsDocument = new SimpleDocument<List<ScopeClaim>>(scope.Claims);
            int scopeType = (int) scope.Type;


            var scopeInternal = await scopeRecord.Record.GetScopeAsync();

            var claimsQuery = from scopeClaim in scopeInternal.Claims
                select new ScopeClaimRecord(scopeRecord.Id, scopeRecord.Record.Name, scopeClaim);

            var scopeClaimBoundStatements = await BuildBoundStatements_ForCreate(claimsQuery);
            result.AddRange(scopeClaimBoundStatements);

            return result;
        }

        public async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
            FlattenedScopeRecord scopeRecord)
        {
            var result = new List<BoundStatement>();


            var scope = scopeRecord.Record;
            //   var scopeSecretsDocument = new SimpleDocument<List<Secret>>(scope.ScopeSecrets);
            //   var claimsDocument = new SimpleDocument<List<ScopeClaim>>(scope.Claims);
            int scopeType = (int) scope.Type;

            /*@"scopes_by_id (
                 * Id,
                 * AllowUnrestrictedIntrospection,
                 * ClaimsRule,
                 * Description,
                 * DisplayName,
                 * Emphasize,
                 * Enabled,
                 * IncludeAllClaimsForUser,
                 * Name,
                 * Required,
                 * ScopeSecrets,
                 * ShowInDiscoveryDocument,
                 * ScopeType) " +
                */
            PreparedStatement prepared = await _CreateScopeById;
            BoundStatement bound = prepared.Bind(
                scopeRecord.Id,
                scope.AllowUnrestrictedIntrospection,
                scope.Claims,
                scope.ClaimsRule,
                scope.Description,
                scope.DisplayName,
                scope.Emphasize,
                scope.Enabled,
                scope.IncludeAllClaimsForUser,
                scope.Name,
                scope.Required,
                scope.ScopeSecrets,
                scope.ShowInDiscoveryDocument,
                scopeType);
            result.Add(bound);

            //@"producttemplates_by_type(documenttype,documentversion,id,document) " +
            prepared = await _CreateScopeByName;
            bound = prepared.Bind(
                scopeRecord.Id,
                scope.AllowUnrestrictedIntrospection,
                scope.Claims,
                scope.ClaimsRule,
                scope.Description,
                scope.DisplayName,
                scope.Emphasize,
                scope.Enabled,
                scope.IncludeAllClaimsForUser,
                scope.Name,
                scope.Required,
                scope.ScopeSecrets,
                scope.ShowInDiscoveryDocument,
                scopeType);
            result.Add(bound);

            return result;
        }

        public async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
            IEnumerable<ScopeClaimRecord> scopeClaimRecords)
        {
            var result = new List<BoundStatement>();
            foreach (var scopeClaimRecord in scopeClaimRecords)
            {



                /*
                 * @"scopeclaims_by_name_and_scopeid(Name,ScopeId,ScopeName,AlwaysIncludeInIdToken,Description) " +
                */
                PreparedStatement prepared = await _CreateScopeClaimByNameAndScopeId;
                BoundStatement bound = prepared.Bind(
                    scopeClaimRecord.Name,
                    scopeClaimRecord.ScopeId,
                    scopeClaimRecord.ScopeName
                    );
                result.Add(bound);

                prepared = await _CreateScopeClaimByNameAndScopeName;
                bound = prepared.Bind(
                    scopeClaimRecord.Name,
                    scopeClaimRecord.ScopeId,
                    scopeClaimRecord.ScopeName
                    );
                result.Add(bound);

            }
            return result;
        }


        public async Task<bool> UpsertManyScopeClaimAsync(IEnumerable<ScopeClaimRecord> scopeClaimsRecords,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            cancellationToken.ThrowIfCancellationRequested();

            if (scopeClaimsRecords == null)
                throw new ArgumentNullException("scopeClaims");

            var scopes = scopeClaimsRecords.ToList();
            if (scopes.Count == 0)
                throw new ArgumentException("scopeClaims is empty");



            var batch = new BatchStatement();
            var boundStatements = await BuildBoundStatements_ForCreate(scopes);
            batch.AddRange(boundStatements);

            await session.ExecuteAsync(batch).ConfigureAwait(false);
            return true;
        }
        public async Task AddScopeClaimsToScopeByNameAsync(string name, IEnumerable<ScopeClaim> claims,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindScopeByName(name, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<ScopeClaim> ulist = stored.Claims.Union(claims, ScopeClaimComparer.MinimalScopeClaimComparer).ToList();
            stored.Claims = ulist;
            await UpsertScopeAsync(stored);
        }
        public async Task DeleteScopeClaimsFromScopeByNameAsync(string name, IEnumerable<ScopeClaim> claims,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindScopeByName(name, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var comparer = ScopeClaimComparer.MinimalScopeClaimComparer;
            var query = from item in stored.Claims
                        where !claims.Contains(item, comparer)
                        select item;
            stored.Claims = query.ToList();
            await UpsertScopeAsync(stored);

        }
        public async Task UpdateScopeClaimsInScopeByNameAsync(string name, IEnumerable<ScopeClaim> claims,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindScopeByName(name, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var comparer = ScopeClaimComparer.MinimalScopeClaimComparer;
            var claimsList = claims.ToList();
            var query = from item in stored.Claims
                        where !claimsList.Contains(item, comparer)
                        select item;
            var remainingClaims = query.ToList(); // these are the ones we keep.
            // create a merged version.
            List<ScopeClaim> ulist = remainingClaims.Union(claimsList, ScopeClaimComparer.MinimalScopeClaimComparer).ToList();

            // Upsert the new version
            stored.Claims = ulist;
            await UpsertScopeAsync(stored);
        }

        public async Task AddScopeSecretsByNameAsync(string name, IEnumerable<Secret> secrets,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindScopeByName(name, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            List<Secret> ulist = stored.ScopeSecrets.Union(secrets, SecretComparer.OrdinalIgnoreCase).ToList();
            stored.ScopeSecrets = ulist;

            await UpsertScopeAsync(stored);

        }
        public async Task DeleteScopeSecretsFromScopeByNameAsync(string name, IEnumerable<Secret> secrets,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindScopeByName(name, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var comparer = SecretComparer.OrdinalIgnoreCase;
            var query = from item in stored.ScopeSecrets
                        where !secrets.Contains(item,comparer)
                        select item;
            stored.ScopeSecrets = query.ToList();
            await UpsertScopeAsync(stored);

        }

        public async Task UpdateScopeByNameAsync(string name, IEnumerable<PropertyValue> properties,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stored = await FindScopeByName(name, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var value in properties)
            {
                stored.SetPropertyValue(value.Name, value.Value);
            }
            await UpsertScopeAsync(stored);

        }
    }
}