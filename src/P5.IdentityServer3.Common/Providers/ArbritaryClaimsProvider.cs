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
    public class ArbritaryClaimsProvider : DefaultClaimsProvider, IOptionalParams
    {
        private static List<string> _requiredArguments;

        private static List<string> RequiredArgument
        {
            get
            {
                return _requiredArguments ?? (_requiredArguments = new List<string>
                {
                    "arbritary-data"
                });
            }
        }
        private static List<string> _requiredDictionaryArguments;

        private static List<string> RequiredDictionaryArgument
        {
            get
            {
                return _requiredDictionaryArguments ?? (_requiredDictionaryArguments = new List<string>
                {
                    "dictionary-data"
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

        private static readonly ILog Logger = LogProvider.For<ArbritaryClaimsProvider>();

        public ArbritaryClaimsProvider(IUserService users)
            : base(users)
        {
        }

        public override Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Client client,
            IEnumerable<Scope> scopes, ValidatedRequest request)
        {
            var arbritraryCheck = request.Raw.Validate(RequiredArgument);
            var arbritraryDictionaryCheck = request.Raw.Validate(RequiredDictionaryArgument);
            if (!arbritraryCheck && !arbritraryDictionaryCheck)
            {
                var missing = string.Join(",", RequiredArgument.ToArray());
                missing += ",";
                missing += string.Join(",", RequiredDictionaryArgument.ToArray());
                throw new Exception(string.Format("RequiredArgument failed need the following [{0}]", missing));
            }



            var result = base.GetAccessTokenClaimsAsync(subject, client, scopes, request);
            var rr = request.Raw.AllKeys.ToDictionary(k => k, k => request.Raw[k]);
            List<Claim> finalClaims = new List<Claim>(result.Result);
            Dictionary<string, string> values;
            if (arbritraryDictionaryCheck)
            {
                values =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(rr["dictionary-data"]);
            }
            else
            {
                values = new Dictionary<string, string>
                {
                    {ClaimTypes.ArbritaryData, rr["arbritary-data"]}
                };
            }

            finalClaims.AddRange(values.Select(value => new Claim(value.Key, value.Value)));

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
