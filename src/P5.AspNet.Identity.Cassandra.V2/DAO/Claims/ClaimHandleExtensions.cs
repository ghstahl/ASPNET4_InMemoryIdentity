using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace P5.AspNet.Identity.Cassandra.DAO
{
    public static class ClaimHandleExtensions
    {

        public static Claim ToClaim(this ClaimHandle claimHandle)
        {
            if (claimHandle == null)
                return null;
            var claim = new Claim(claimHandle.Type, claimHandle.Value);

            return claim;
        }
        public static ClaimHandle ToClaimHandle(this Claim claim,Guid userId)
        {
            if (claim == null)
                return null;
            var claimHandle = new ClaimHandle() { Type = claim.Type, Value = claim.Value, UserId = userId };
            return claimHandle;
        }
        public static IEnumerable<Claim> ToClaims(this IEnumerable<ClaimHandle> claimHandles)
        {
            var query = from item in claimHandles
                        select item.ToClaim();
            return query;
        }
    }
}