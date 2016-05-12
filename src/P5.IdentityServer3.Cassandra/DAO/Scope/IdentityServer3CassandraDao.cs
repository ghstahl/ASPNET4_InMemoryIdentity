using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer3.Core.Models;
using Newtonsoft.Json;
using P5.CassandraStore.Extensions;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for Scope
        //-----------------------------------------------

        #region PREPARED STATEMENTS for Scope

        private static AsyncLazy<PreparedStatement> _CreateScopeById { get; set; }
        private static AsyncLazy<PreparedStatement> _CreateScopeByName { get; set; }
        private static AsyncLazy<PreparedStatement[]> _CreateScope { get; set; }
        private static AsyncLazy<PreparedStatement> _CreateScopeClaimByNameAndScopeId { get; set; }
        private static AsyncLazy<PreparedStatement> _CreateScopeClaimByNameAndScopeName { get; set; }
        private static AsyncLazy<PreparedStatement[]> _CreateScopeClaim { get; set; }
        private static AsyncLazy<PreparedStatement> _FindScopeById { get; set; }
        private static AsyncLazy<PreparedStatement> _FindScopeByNamee { get; set; }

        #endregion
        public static async Task<bool> CreateScopeAsync(FlattenedScopeRecord scopeRecord,
    CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var scopeRecords = new List<FlattenedScopeRecord>();
                scopeRecords.Add(scopeRecord);
                return await CreateManyScopeAsync(scopeRecords, cancellationToken);
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<bool> CreateManyScopeAsync(IList<FlattenedScopeRecord> scopeRecords,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {

                var session = CassandraSession;
                cancellationToken.ThrowIfCancellationRequested();

                if (scopeRecords == null)
                    throw new ArgumentNullException("scopeRecord");
                if (scopeRecords.Count == 0)
                    throw new ArgumentException("scopeRecords is empty");

                var batch = new BatchStatement();
                var boundStatements = await BuildBoundStatements_ForCreate(scopeRecords);
                batch.AddRange(boundStatements);
                await session.ExecuteAsync(batch).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<global::IdentityServer3.Core.Models.Scope> FindScopeByIdAsync(Guid id,
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

        public static async Task<global::IdentityServer3.Core.Models.Scope> FindScopeByName(string name,
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

        public static async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> FindScopesByNamesAsync(
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
                var scopeMappedRecords = (await mapper.FetchAsync<ScopeMappedRecord>(query)).ToList();


                foreach (var scopeMappedRecord in scopeMappedRecords)
                {
                    scopeMappedRecord.Claims =
                        JsonConvert.DeserializeObject<List<ScopeClaim>>(scopeMappedRecord.ClaimsDocument);
                }
                var queryFinal = from item in scopeMappedRecords
                                 select (Scope)item;
                return queryFinal;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static async Task<IEnumerable<global::IdentityServer3.Core.Models.Scope>> FindScopesAsync(
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
                                 select (Scope)item;
                return queryFinal;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
    IEnumerable<FlattenedScopeRecord> scopeRecords)
        {
            var result = new List<BoundStatement>();
            foreach (var scopeRecord in scopeRecords)
            {
                PreparedStatement[] prepared = await _CreateScope;
                var scope = scopeRecord.Record;
                //   var scopeSecretsDocument = new SimpleDocument<List<Secret>>(scope.ScopeSecrets);
                //   var claimsDocument = new SimpleDocument<List<ScopeClaim>>(scope.Claims);
                int scopeType = (int)scope.Type;
                var preparedById = prepared[0];
                var preparedByName = prepared[1];
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
                BoundStatement boundById = preparedById.Bind(
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

                //@"producttemplates_by_type(documenttype,documentversion,id,document) " +
                BoundStatement boundByName = preparedByName.Bind(
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


                result.Add(boundById);
                result.Add(boundByName);

                var claimsQuery = from scopeClaim in scopeRecord.Record.GetScope().Claims
                                  select new ScopeClaimRecord(scopeRecord.Id, scopeRecord.Record.Name, scopeClaim);

                var scopeClaimBoundStatements = await BuildBoundStatements_ForCreate(claimsQuery);
                result.AddRange(scopeClaimBoundStatements);
            }
            return result;
        }

        public static async Task<List<BoundStatement>> BuildBoundStatements_ForCreate(
            IEnumerable<ScopeClaimRecord> scopeClaimRecords)
        {
            var result = new List<BoundStatement>();
            foreach (var scopeClaimRecord in scopeClaimRecords)
            {
                PreparedStatement[] prepared = await _CreateScopeClaim;
                PreparedStatement preparedByNameAndScopeId = prepared[0];
                PreparedStatement preparedByNameAndScopeName = prepared[1];

                /*
                 * @"scopeclaims_by_name_and_scopeid(Name,ScopeId,ScopeName,AlwaysIncludeInIdToken,Description) " +
                */
                BoundStatement statementByNameAndScopeId = preparedByNameAndScopeId.Bind(
                    scopeClaimRecord.Name,
                    scopeClaimRecord.ScopeId,
                    scopeClaimRecord.ScopeName
                    );
                BoundStatement statementByNameAndScopeName = preparedByNameAndScopeName.Bind(
                    scopeClaimRecord.Name,
                    scopeClaimRecord.ScopeId,
                    scopeClaimRecord.ScopeName
                    );

                result.Add(statementByNameAndScopeId);
                result.Add(statementByNameAndScopeName);
            }
            return result;
        }



        public static async Task<bool> CreateManyScopeClaimAsync(List<ScopeClaimRecord> scopeClaimsRecords,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            cancellationToken.ThrowIfCancellationRequested();

            if (scopeClaimsRecords == null)
                throw new ArgumentNullException("scopeClaims");
            if (scopeClaimsRecords.Count == 0)
                throw new ArgumentException("scopeClaims is empty");



            var batch = new BatchStatement();
            var boundStatements = await BuildBoundStatements_ForCreate(scopeClaimsRecords);
            batch.AddRange(boundStatements);

            await session.ExecuteAsync(batch).ConfigureAwait(false);
            return true;
        }

    }
}
