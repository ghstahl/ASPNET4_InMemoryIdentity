using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Validation;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using P5.IdentityServerCore.Extensions;

namespace CustomClientCredentialHost.Config
{
    public interface IOptionalParams
    {
        void SetOptionalParams(IDictionary<string, string> optionalParams);
    }
    public static class CustomClaimsProviderHubExtension
    {
        public static bool Validate(this NameValueCollection nvc, IList<string> againstList)
        {
            if (againstList.Any(item => nvc[item] == null))
            {
                return false;
            }
            return true;
        }
    }

    public class CustomClaimsProviderHub : DefaultClaimsProvider
    {
        public const string WellKnown_DefaultProviderName = "default-provider";
        private IDictionary<string, IClaimsProvider> _mapClaimsProviders;
        public CustomClaimsProviderHub(IUserService users, IDictionary<string, IClaimsProvider> mapClaimsProviders)
            : base(users)
        {
            _mapClaimsProviders = mapClaimsProviders;
        }

        public override Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Client client, IEnumerable<Scope> scopes, ValidatedRequest request)
        {
            var handler = request.Raw["handler"] ?? WellKnown_DefaultProviderName;
            handler = handler.ToLower();
            var provider = _mapClaimsProviders[handler];
            return provider.GetAccessTokenClaimsAsync(subject, client, scopes, request);
        }
    }

    class ArbritaryClaimsProvider : DefaultClaimsProvider, IOptionalParams
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

        private static List<string> _p5ClaimTypes;

        private static List<string> P5ClaimTypes
        {
            get
            {
                if (_p5ClaimTypes == null)
                {
                    var myConstants = typeof (P5.IdentityServerCore.Constants.ClaimTypes).GetConstants<System.String>();
                    var values = myConstants.GetConstantsValues<System.String>();
                    _p5ClaimTypes = values.ToList();
                }
                return _p5ClaimTypes;
            }
        }

        private static readonly ILog Logger = LogProvider.For<CustomOpenIdClaimsProvider>();

        public ArbritaryClaimsProvider(IUserService users)
            : base(users)
        {
        }

        public override Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Client client,
            IEnumerable<Scope> scopes, ValidatedRequest request)
        {
            if (!request.Raw.Validate(RequiredArgument))
            {
                throw new Exception(string.Format("RequiredArgument failed need the following [{0}]",
                    string.Join(",", RequiredArgument.ToArray())));
            }

            var result = base.GetAccessTokenClaimsAsync(subject, client, scopes, request);
            var rr = request.Raw.AllKeys.ToDictionary(k => k, k => request.Raw[k]);
            List<Claim> finalClaims = new List<Claim>(result.Result);
            var arbData = rr["arbritary-data"];

            finalClaims.Add(new Claim(P5.IdentityServerCore.Constants.ClaimTypes.ArbritaryData, arbData));

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

    class CustomOpenIdClaimsProvider : DefaultClaimsProvider, IOptionalParams
    {
        private static List<string> _requiredArguments;

        private static List<string> RequiredArgument
        {
            get
            {
                return _requiredArguments ?? (_requiredArguments = new List<string>
                {
                    "openid-connect-token"
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
                    var myConstants = typeof(P5.IdentityServerCore.Constants.ClaimTypes).GetConstants<System.String>();
                    var values = myConstants.GetConstantsValues<System.String>();
                    _p5ClaimTypes = values.ToList();
                }
                return _p5ClaimTypes;
            }
        }
        private static readonly ILog Logger = LogProvider.For<CustomOpenIdClaimsProvider>();

        public CustomOpenIdClaimsProvider(IUserService users)
            : base(users)
        {
        }

        public override Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Client client,
            IEnumerable<Scope> scopes, ValidatedRequest request)
        {
            if (!request.Raw.Validate(RequiredArgument))
            {
                throw new Exception(string.Format("RequiredArgument failed need the following [{0}]", string.Join(",", RequiredArgument.ToArray())));
            }

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

        private IDictionary<string, string> _optionalParams;
        public void SetOptionalParams(IDictionary<string, string> optionalParams)
        {
            _optionalParams = optionalParams;
        }
    }
}