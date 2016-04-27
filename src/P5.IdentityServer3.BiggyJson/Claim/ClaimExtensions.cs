using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace P5.IdentityServer3.BiggyJson
{
    static class ClaimExtensions
    {
        public static Claim ToClaim(this ClaimTypeRecord record)
        {
            return new Claim(record.Type, record.Value, record.ValueType);
        }
        public static List<Claim> ToClaims(this List<ClaimTypeRecord> records)
        {
            var query = from item in records
                select item.ToClaim();
            return query.ToList();
        }

        public static ClaimTypeRecord ToClaimTypeRecord(this Claim claim)
        {
            return new ClaimTypeRecord()
            {
                Type = claim.Type,
                Value = claim.Value,
                ValueType = claim.ValueType

            };
        }
        public static List<ClaimTypeRecord> ToClaimTypeRecords(this List<Claim> claims)
        {
            var query = from item in claims
                        select item.ToClaimTypeRecord();
            return query.ToList();
        }

    }
}