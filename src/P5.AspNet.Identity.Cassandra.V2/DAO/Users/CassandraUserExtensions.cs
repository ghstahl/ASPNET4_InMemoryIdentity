using System;
using Cassandra;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public static class CassandraUserExtensions
    {
        /// <summary>
        /// Creates a CassandraUser from a Row.  If the Row is null, returns null.
        /// </summary>
        static CassandraUser ToCassandraUser(this Row row)
        {
            if (row == null) return null;

            var user = new CassandraUser(row.GetValue<Guid>("userid"), row.GetValue<Guid>("tenantid"), row.GetValue<string>("username"), row.GetValue<string>("email"))
            {
                PasswordHash = row.GetValue<string>("password_hash"),
                SecurityStamp = row.GetValue<string>("security_stamp"),
                TwoFactorEnabled = row.GetValue<bool>("two_factor_enabled"),
                AccessFailedCount = row.GetValue<int>("access_failed_count"),
                LockoutEnabled = row.GetValue<bool>("lockout_enabled"),
                LockoutEndDate = row.GetValue<DateTimeOffset>("lockout_end_date"),
                PhoneNumber = row.GetValue<string>("phone_number"),
                PhoneNumberConfirmed = row.GetValue<bool>("phone_number_confirmed"),
                EmailConfirmed = row.GetValue<bool>("email_confirmed"),
                Created = row.GetValue<DateTimeOffset>("created"),
                Modified = row.IsNull("modified") ? new DateTimeOffset?() : row.GetValue<DateTimeOffset>("modified"),
                Enabled = row.GetValue<bool>("enabled"),
                Source = row.GetValue<string>("source"),
                SourceId = row.GetValue<string>("source_id")
            };

            return user;
        }

    }
}