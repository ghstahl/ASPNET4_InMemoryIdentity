using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.Extensions;
using P5.IdentityServer3.Common;
using P5.IdentityServer3.Common.Settings;

namespace CustomClientCredentialHost.Console.Client
{
    class Program
    {
        const string domain_root = "http://localhost:55970";
        const string identity_server_route = "idsrv3core";

        static void Main(string[] args)
        {
            IdentityServerSettings.DomainRoot = domain_root;
            IdentityServerSettings.IdentityServerBaseRoute = identity_server_route;

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

                response = await GetClientTokenAsync();
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
                response = await RequestResourceOwnerTokenAsync();
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

            var result = client.GetStringAsync(domain_root + "/api/v1/IDSAdmin/who");

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

            var result = client.GetStringAsync(domain_root + "/api/v1/IDSAdmin/who");

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
        static async Task<TokenResponse> GetClientTokenAsync()
        {
            var client = new TokenClient(
                IdentityServerSettings.TokenEndpoint,
                "9bdd52a6-9762-4493-b3d2-0e17d7603a4a",
                "8fbf9557-d43b-47d5-b5a7-f423f174c3cd");
            var customParams = new Dictionary<string, string>
            {
                { "handler", "openid-provider" },
                { "openid-connect-token", "myOpenId" }
            };
            return await client.RequestClientCredentialsAsync("api1", customParams);
        }

        static async Task<TokenResponse> RequestResourceOwnerTokenAsync()
        {
            var customClaims = new Dictionary<string, string>
            {
                {"naGuid", "1234abcd"},
                {"In", "Flames"}
            };

            return await ClientRequests.RequestResourceOwnerTokenAsync(
                "1369e607-9c2e-44c5-986a-b94a36c2ef3f",
                "4ba17f8c-0aed-4f33-9537-bfa2452b1f64",
                "service@internal.io",
                "Password1234@",
                "api1 offline_access", customClaims
                );
        }


        static TokenResponse GetClientToken_error()
        {
            var client = new TokenClient(
                IdentityServerSettings.TokenEndpoint,
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
