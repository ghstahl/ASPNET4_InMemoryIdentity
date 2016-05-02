using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.Extensions;

namespace P5.IdentityServerConsole.Client
{
    class Program
    {

        public const string domain_root = "http://localhost:33854";
        public const string idsrv_root = domain_root+"/idsrv3core";
        public const string token_endpoint = idsrv_root + "/connect/token";
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
                TokenResponse response;

                response = GetClientToken();
                Console.WriteLine(response.Json);
                CallApi(response);

                response = GetTokenFor_CustomWebApi1();
                Console.WriteLine(response.Json);
                CallApi(response);

                response = GetCustomGrantToken();
                Console.WriteLine(response.Json);
                CallApi(response);

                response = GetResourceOwnerToken();
                Console.WriteLine(response.Json);
                CallApi(response);

                response = GetSecondLevelToken(response);
                Console.WriteLine(response.Json);
                CallApi(response);




                response = GetUserToken();
                Console.WriteLine(response.Json);
                CallApi(response);

                /*
*/


            }
            catch (Exception e)
            {

            }
        }
        static TokenResponse GetCustomGrantToken()
        {
            var client = new TokenClient(
               token_endpoint,
               "custom_grant_client",
               "secret");

            var customParams = new Dictionary<string, string>
            {
                { "some_custom_parameter", "some_value" }
            };

            var result = client.RequestCustomGrantAsync("custom", "read", customParams).Result;
            return result;
        }

        static TokenResponse GetTokenFor_CustomWebApi1()
        {
            var client = new TokenClient(
               token_endpoint,
               "CustomWebApi1",
               "secret");

            var customParams = new Dictionary<string, string>
            {
                { "some_custom_parameter", "some_value" }
            };

            var result = client.RequestCustomGrantAsync("custom", "CustomWebApi1", customParams).Result;
            return result;
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

            var result = client.GetStringAsync(domain_root + "/api/identity");

            try
            {
                result.Result.ConsoleGreen();
            }
            catch (Exception e)
            {
                "\nException Caught..............".ConsoleRed();
                "--------------------------------".ConsoleRed();
                e.Message.ConsoleRed();
                if(e.InnerException != null)
                    e.InnerException.Message.ConsoleRed();
                "--------------------------------\n".ConsoleRed();

            }

        }

        static TokenResponse GetClientToken()
        {
            var client = new TokenClient(
                token_endpoint,
                "silicon",
                "secret");
            var customParams = new Dictionary<string, string>
            {
                { "some_custom_parameter", "some_value" }
            };
            return client.RequestClientCredentialsAsync("api1", customParams).Result;
        }

        static TokenResponse GetUserToken()
        {
            var client = new TokenClient(
               token_endpoint,
                "carbon",
                "21B5F798-BE55-42BC-8AA8-0025B903DC3B");

            return client.RequestResourceOwnerPasswordAsync("bob", "secret", "api1").Result;
        }

    }
}
