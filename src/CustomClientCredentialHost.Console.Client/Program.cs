using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.Extensions;

namespace CustomClientCredentialHost.Console.Client
{
    class Program
    {
        const string domain_root = "http://localhost:55970";
        const string identity_server_route = "/idsrv3core";
        const string identity_server_authority = domain_root + identity_server_route;
        const string token_endpoint = identity_server_authority + "/connect/token";

        static void Main(string[] args)
        {
            string m1 = "Type a string of text then press Enter. " +
                       "Type 'x' anywhere in the text to quit:\n";
            System.Console.WriteLine(m1);
            int writes = 0;
            char ch;

            do
            {
                var x = System.Console.ReadLine();
                if (string.Compare(x, "x", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    break;

                }
                Task t = MainAsync(args);
                t.Wait();
                System.Console.WriteLine(++writes);


            } while (true);

            System.Console.WriteLine("Bye.....................");
        }
        static async Task MainAsync(string[] args)
        {
            try
            {
                CallTestApi();

                TokenResponse response;

                response = GetClientToken_error();
                if (response.IsError)
                {
                    response.HttpErrorReason.ConsoleRed();
                }

                response = GetClientToken();
                if (response.IsError)
                {
                    response.HttpErrorReason.ConsoleRed();
                }
                else
                {
                    System.Console.WriteLine(response.Json);
                    for (int i = 0; i < 5; ++i)
                    {
                        CallApi(response);
                    }

                }
            }
            catch (Exception e)
            {

            }
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
                if (e.InnerException != null)
                    e.InnerException.Message.ConsoleRed();
                "--------------------------------\n".ConsoleRed();

            }

        }
        static void CallTestApi( )
        {
            var client = new HttpClient();

            var result = client.GetStringAsync(domain_root + "/api/test");

            try
            {
                result.Result.ConsoleGreen();
            }
            catch (Exception e)
            {
                "\nException Caught..............".ConsoleRed();
                "--------------------------------".ConsoleRed();
                e.Message.ConsoleRed();
                if (e.InnerException != null)
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
                { "handler", "openid-provider" },
                { "openid-connect-token", "myOpenId" }
            };
            return client.RequestClientCredentialsAsync("api1", customParams).Result;
        }
        static TokenResponse GetClientToken_error()
        {
            var client = new TokenClient(
                token_endpoint,
                "sdfdsf",
                "secsdfdsfret");
            var customParams = new Dictionary<string, string>
            {
                { "handler", "openid-provider" },
                { "openid-connect-token", "myOpenId" }
            };
            return client.RequestClientCredentialsAsync("api1", customParams).Result;
        }
    }
}
