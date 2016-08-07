using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;
using P5.IdentityServer3.Common.Settings;

namespace P5.IdentityServer3.Common
{
    public class ClientRequests
    {
        public static string TokenEndpoint { get; set; }
        public static async Task<TokenResponse> RequestResourceOwnerTokenAsync(string clientId, string clientSecret, string userName, string password, string scopes, Dictionary<string, string> customClaims)
        {
            var client = new TokenClient(IdentityServerSettings.TokenEndpoint,clientId,clientSecret);

            var dictionaryData = JsonConvert.SerializeObject(customClaims);
            // idsrv supports additional non-standard parameters
            // that get passed through to the user service
            var customParams = new Dictionary<string, string>
            {
                { "handler", "arbritary-provider" },
                { "dictionary-data", dictionaryData }
            };

            return await client.RequestResourceOwnerPasswordAsync(userName, password, scopes, customParams);
        }
    }
}
