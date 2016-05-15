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
using P5.CassandraStore.DAO;
using P5.CassandraStore.Extensions;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public partial class IdentityServer3CassandraDao
    {
        //-----------------------------------------------
        // PREPARED STATEMENTS for Utility
        //-----------------------------------------------

        #region PREPARED STATEMENTS for Token


        private static List<string> _tables = new List<string>()
        {
            "scopeclaims_by_name_and_scopeid",
            "scopeclaims_by_name_and_scopename",
            "scopes_by_id",
            "scopes_by_name",
            "AuthorizationCodeHandle_By_Key",
            "AuthorizationCodeHandle_By_ClientId",
            "RefreshTokenHandle_By_Key",
            "RefreshTokenHandle_By_ClientId",
            "TokenHandle_By_Key",
            "TokenHandle_By_ClientId",
            "clients_by_id",
            "consent_by_id",
            "consent_by_clientid"
        };

        #endregion

        public static void PrepareUtilityStatements()
        {
            #region PREPARED STATEMENTS for Utility


            #endregion
        }


        public static async Task<bool> TruncateTablesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var session = CassandraSession;
            return await CassandraDao.TruncateTablesAsync(session, _tables, cancellationToken);
        }
    }
}
