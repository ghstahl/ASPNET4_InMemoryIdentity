﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;


namespace P5.IdentityServer3.Common
{
    public static class ClaimExtensions
    {
        public static System.Security.Claims.Claim ToClaim(this ClaimTypeRecord record)
        {
            return new System.Security.Claims.Claim(record.Type, record.Value, record.ValueType);
        }
        public static List<System.Security.Claims.Claim> ToClaims(this List<ClaimTypeRecord> records)
        {
            var query = from item in records
                select item.ToClaim();
            return query.ToList();
        }
        public static ClaimIdentityRecord ToClaimIdentityRecord(this ClaimsIdentity claimsIdentity)
        {
            return new ClaimIdentityRecord()
            {
                AuthenticationType = claimsIdentity.AuthenticationType,
                ClaimTypeRecords = claimsIdentity.Claims.ToList().ToClaimTypeRecords()
            };
        }
        public static List<ClaimIdentityRecord> ToClaimIdentityRecords(this List<ClaimsIdentity> claimsIdentitys)
        {
            var query = from item in claimsIdentitys
                        select item.ToClaimIdentityRecord();
            return query.ToList();
        }

        public static ClaimsIdentity ToClaimsIdentity(this ClaimIdentityRecord claimIdentityRecord)
        {
            var claims = claimIdentityRecord.ClaimTypeRecords.ToClaims();
            var claimsIdentity = claimIdentityRecord.AuthenticationType == null
                ? new ClaimsIdentity(claims)
                : new ClaimsIdentity(claims, claimIdentityRecord.AuthenticationType);
            return claimsIdentity;
        }
        public static List<ClaimsIdentity> ToClaimsIdentitys(this List<ClaimIdentityRecord> claimIdentityRecords)
        {
            var query = from item in claimIdentityRecords
                        select item.ToClaimsIdentity();
            return query.ToList();
        }

        public static ClaimTypeRecord ToClaimTypeRecord(this System.Security.Claims.Claim claim)
        {
            return new ClaimTypeRecord(claim);
        }
        public static List<ClaimTypeRecord> ToClaimTypeRecords(this List<System.Security.Claims.Claim> claims)
        {
            var query = from item in claims
                        select item.ToClaimTypeRecord();
            return query.ToList();
        }

    }
}