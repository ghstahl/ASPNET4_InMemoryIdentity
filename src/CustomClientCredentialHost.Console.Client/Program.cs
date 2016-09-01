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

        /*
clientId: 2e6a2bb2-f8d7-4d72-b4c1-41b3015654f2
client.pfx ThumbPrint: 61B754C541BBCFC6A45A9E9EC5E47D8702B78C29
ids3client.se thumbprint: 584A8E2BCAE8F10C115628E2B8A6FD1D21288AB6
secret: 1f718d3c-a77b-4ee0-91e9-aa66cc7a0588
         * */

        static async Task<TokenResponse> GetClientTokenAsync()
        {
            var client = new TokenClient(
                IdentityServerSettings.TokenEndpoint,
                "2e6a2bb2-f8d7-4d72-b4c1-41b3015654f2",
                "1f718d3c-a77b-4ee0-91e9-aa66cc7a0588");
            var customParams = new Dictionary<string, string>
            {
                { "handler", "openid-provider" },
                { "openid-connect-token", "myOpenId" }
            };
            return await client.RequestClientCredentialsAsync("api1", customParams);
        }

        /*
         * clientId: 75b4508c-01d4-4a8a-914b-d745ba80a092
secrete: 3ff88385-4a4f-412f-a5ba-9d00f7424f39
username: service@internal.io
pass: anything

         */
        static async Task<TokenResponse> RequestResourceOwnerTokenAsync()
        {
            var customClaims = new Dictionary<string, string>
            {
                {"naGuid", "1234abcd"},
                {"In", "Flames"}
            };

            return await ClientRequests.RequestResourceOwnerTokenAsync(
                "75b4508c-01d4-4a8a-914b-d745ba80a092",
                "3ff88385-4a4f-412f-a5ba-9d00f7424f39",
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
