using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Validation;
using Newtonsoft.Json;
using P5.IdentityServer3.Common.Extensions;
using ClaimTypes = P5.IdentityServer3.Common.Constants.ClaimTypes;

namespace P5.IdentityServer3.Common.Providers
{
    public class ArbitraryClaimsProvider : DefaultClaimsProvider, IOptionalParams
    {
        private static List<string> _requiredArbitraryClaimsArguments;

        private static List<string> RequiredArbitraryClaimsArgument
        {
            get
            {
                return _requiredArbitraryClaimsArguments ?? (_requiredArbitraryClaimsArguments = new List<string>
                {
                    "arbitrary-claims"
                });
            }
        }

        private static List<string> _requiredArbitraryScopesArguments;

        private static List<string> RequiredArbitraryScopes
        {
            get
            {
                return _requiredArbitraryScopesArguments ?? (_requiredArbitraryScopesArguments = new List<string>
                {
                    "arbitrary-scopes"
                });
            }
        }

        private static List<string> _p5ClaimTypes;

        private static List<string> P5ClaimTypes
        {
            get
            {
                if (_p5ClaimTypes == null)
                {
                    var myConstants = typeof(ClaimTypes).GetConstants<System.String>();
                    var values = myConstants.GetConstantsValues<System.String>();
                    _p5ClaimTypes = values.ToList();
                }
                return _p5ClaimTypes;
            }
        }

        private static readonly ILog Logger = LogProvider.For<ArbitraryClaimsProvider>();

        public ArbitraryClaimsProvider(IUserService users)
            : base(users)
        {
        }

        public override Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Client client,
            IEnumerable<Scope> scopes, ValidatedRequest request)
        {

            var arbitraryClaimsCheck = request.Raw.Validate(RequiredArbitraryClaimsArgument);
            var arbitraryScopesCheck = request.Raw.Validate(RequiredArbitraryScopes);
            if (  !arbitraryClaimsCheck && !arbitraryScopesCheck)
            {
                var missing = string.Join(",", RequiredArbitraryClaimsArgument.ToArray());
                missing += ",";
                missing += string.Join(",", RequiredArbitraryScopes.ToArray());
                throw new Exception(string.Format("RequiredArgument failed need the following [{0}]", missing));
            }



            var result = base.GetAccessTokenClaimsAsync(subject, client, scopes, request);
            var rr = request.Raw.AllKeys.ToDictionary(k => k, k => request.Raw[k]);
            List<Claim> finalClaims = new List<Claim>(result.Result);


            if (arbitraryScopesCheck)
            {
                var newScopes = rr["arbitrary-scopes"].Split(new char[] {' ', '\t'},
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var scope in newScopes)
                {
                    finalClaims.Add(new Claim("scope", scope));
                }
            }

            Dictionary<string, string> values;
            if (arbitraryClaimsCheck)
            {
                values =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(rr["arbitrary-claims"]);
                finalClaims.AddRange(values.Select(value => new Claim(value.Key, value.Value)));
            }



            if (subject != null)
            {
                finalClaims.AddRange(subject.Claims.Where(p2 =>
                    finalClaims.All(p1 => p1.Type != p2.Type)));
            }

            // if we find any, than add them to the original and send that back.
            IEnumerable<Claim> claimresults = finalClaims;
            var taskResult = Task.FromResult(claimresults);
            return taskResult;

        }

        private IDictionary<string, string> _optionalParams;

        public void SetOptionalParams(IDictionary<string, string> optionalParams)
        {
            _optionalParams = optionalParams;
        }

    }

}
