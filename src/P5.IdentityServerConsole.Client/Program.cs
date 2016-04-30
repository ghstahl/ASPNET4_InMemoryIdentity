using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace P5.IdentityServerConsole.Client
{
    class Program
    {
        public const string domain_root = "http://localhost:33854";
        public const string token_endpoint = domain_root + "/identity/connect/token";
        static void Main(string[] args)
        {
            string m1 = "Type a string of text then press Enter. " +
                        "Type 'x' anywhere in the text to quit:\n";
            Console.WriteLine(m1);
            int writes = 0;
            char ch;

            do
            {
                var x = Console.ReadLine();
                if (string.Compare(x, "x", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    break;

                }
                Task t = MainAsync(args);
                t.Wait();
                Console.WriteLine(++writes);


            } while (true);

            Console.WriteLine("Bye.....................");

        }

        static async Task MainAsync(string[] args)
        {
            try
            {

                var response = GetResourceOwnerToken();
                Console.WriteLine(response.Json);
                CallApi(response);

                response = GetSecondLevelToken(response);
                Console.WriteLine(response.Json);
                CallApi(response);


                response = GetClientToken();
                Console.WriteLine(response.Json);
                CallApi(response);

                response = GetUserToken();
                Console.WriteLine(response.Json);
                CallApi(response);

            }
            catch (Exception e)
            {

            }
        }

        static TokenResponse GetSecondLevelToken(TokenResponse response)
        {
            TokenClient tokenClient = new TokenClient(
               token_endpoint,
               "WebApi1",
               "4B79A70F-3919-435C-B46C-571068F7AF37"
           );

            var customParams = new Dictionary<string, string>
            {
                { "token", response.AccessToken }
            };

            var tokenResponse = tokenClient.RequestCustomGrantAsync("act-as", "WebApi2", customParams).Result;
            return tokenResponse;
        }
        static TokenResponse GetResourceOwnerToken()
        {
            var client = new TokenClient(
                token_endpoint,
                "ConsoleApplication",
                "F621F470-9731-4A25-80EF-67A6F7C5F4B8");

            return client.RequestResourceOwnerPasswordAsync("bob", "secret", "WebApi1").Result;
        }

        static void CallApi(TokenResponse response)
        {
            var client = new HttpClient();
            client.SetBearerToken(response.AccessToken);

            Console.WriteLine(client.GetStringAsync(domain_root+"/api/test").Result);
        }

        static TokenResponse GetClientToken()
        {
            var client = new TokenClient(
                token_endpoint,
                "silicon",
                "secret");

            return client.RequestClientCredentialsAsync("api1").Result;
        }

        static TokenResponse GetUserToken()
        {
            var client = new TokenClient(
               token_endpoint,
                "carbon",
                "21B5F798-BE55-42BC-8AA8-0025B903DC3B");

            return client.RequestResourceOwnerPasswordAsync("bob", "secret", "api1").Result;
        }
        static TokenResponse GetCustomGrantToken()
        {
            var client = new TokenClient(
               token_endpoint,
               "custom_grant_client",
               "cd19ac6f-3bfa-4577-9579-da32fd15788a");

            var customParams = new Dictionary<string, string>
            {
                { "some_custom_parameter", "some_value" }
            };

            var result = client.RequestCustomGrantAsync("custom", "read", customParams).Result;
            return result;
        }
    }
}
