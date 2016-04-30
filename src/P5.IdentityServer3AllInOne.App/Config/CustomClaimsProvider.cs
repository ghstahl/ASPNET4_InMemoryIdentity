using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Validation;
using P5.IdentityServerCore.Extensions;

namespace P5.IdentityServer3AllInOne.App.Config
{
    class CustomClaimsProvider : DefaultClaimsProvider
    {
        private static List<string> _p5ClaimTypes;

        private static List<string> P5ClaimTypes
        {
            get
            {
                if (_p5ClaimTypes == null)
                {
                    var myConstants = typeof(P5.IdentityServerCore.Constants.ClaimTypes).GetConstants<System.String>();
                    var values = myConstants.GetConstantsValues<System.String>();
                    _p5ClaimTypes = values.ToList();
                }
                return _p5ClaimTypes;
            }
        }
        private static readonly ILog Logger = LogProvider.For<CustomClaimsProvider>();

        public CustomClaimsProvider(IUserService users)
            : base(users)
        { }

        public override Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Client client,
            IEnumerable<Scope> scopes, ValidatedRequest request)
        {
            var result = base.GetAccessTokenClaimsAsync(subject, client, scopes, request);
            if (subject == null)
            {
                return result;
            }

            // look for claims in subject.Claims that match those in P5ClaimTypes 
            var query = from item in subject.Claims
                join name in P5ClaimTypes
                    on item.Type equals name
                select item;
            if (!query.Any())
            {
                return result;
            }

            // if we find any, than add them to the original and send that back.
            List<Claim> finalClaims = new List<Claim>(result.Result);
            finalClaims.AddRange(query);
            IEnumerable<Claim> claimresults = finalClaims;
            var taskResult = Task.FromResult(claimresults);
            return taskResult;

        }

        public override Task<IEnumerable<Claim>> GetIdentityTokenClaimsAsync(System.Security.Claims.ClaimsPrincipal subject, Client client, IEnumerable<Scope> scopes, bool includeAllIdentityClaims, ValidatedRequest request)
        {
            Logger.Warn("--- Some custom warning !!!!!!!!!!!!!!!");

            return base.GetIdentityTokenClaimsAsync(subject, client, scopes, includeAllIdentityClaims, request);
        }
    }
}