using System;
using P5.IdentityServer3.Common;

namespace P5.IdentityServer3.Cassandra.DAO
{
    public class IdentityServerUserRecordCassandra
    {
        public IdentityServerUserRecordCassandra() { }

        public IdentityServerUserRecordCassandra(IdentityServerUserRecord record)
        {
            Id = record.Id;
            Enabled = record.Record.Enabled;
            UserId = record.Record.UserId;
            UserName = record.Record.UserName;
        }
        public Guid Id { get; set; }
        public bool Enabled{get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}