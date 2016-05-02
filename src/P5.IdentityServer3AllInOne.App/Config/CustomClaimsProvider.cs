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
using Newtonsoft.Json;
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
            var rr = request.Raw.AllKeys.ToDictionary(k => k, k => request.Raw[k]);
            List<Claim> finalClaims = new List<Claim>(result.Result);
            string output = JsonConvert.SerializeObject(rr);
            finalClaims.Add(new Claim(P5.IdentityServerCore.Constants.ClaimTypes.ClientRequestNameValueCollection,output));

            if (subject != null)
            {
                // Extra claims that came in from an upstream ICustomGrantValidator, but only those that match the ones in our know
                // ClaimTypes
                // look for claims in subject.Claims that match those in P5ClaimTypes 
                /*
                var query = from item in subject.Claims
                            join name in P5ClaimTypes
                                on item.Type equals name
                            select item;
                if (!query.Any())
                {
                    return result;
                }
                finalClaims.AddRange(query);
                 */
                finalClaims.AddRange(subject.Claims.Where(p2 =>
                    finalClaims.All(p1 => p1.Type != p2.Type)));

            }

            // if we find any, than add them to the original and send that back.
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