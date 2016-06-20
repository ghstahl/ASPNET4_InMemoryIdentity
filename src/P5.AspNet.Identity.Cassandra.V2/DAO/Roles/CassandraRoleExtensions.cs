using System;
using Cassandra;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public static class CassandraRoleExtensions
    {
        /// <summary>
        /// Creates a CassandraUser from a Row.  If the Row is null, returns null.
        /// </summary>
        public static CassandraRole FromRow(this Row row)
        {
            if (row == null) return null;

            var role = new CassandraRole()
            {
                Name = row.GetValue<string>("name"),
                DisplayName = row.GetValue<string>("displayname"),
                IsSystemRole = row.GetValue<bool>("is_systemrole"),
                IsGlobal = row.GetValue<bool>("is_global"),
                TenantId = row.GetValue<Guid>("tenantid"),
                Created = row.GetValue<DateTimeOffset>("created"),
                Modified = row.IsNull("modified") ? new DateTimeOffset?() : row.GetValue<DateTimeOffset>("modified"),
            };

            return role;
        }
    }
}