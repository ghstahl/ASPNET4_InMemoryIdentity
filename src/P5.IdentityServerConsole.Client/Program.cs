using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace P5.IdentityServerConsole.Client
{
    class Program
    {
        public const string token_endpoint = "http://localhost:41031/identity/connect/token";
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
                var response = GetClientToken();
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

        static void CallApi(TokenResponse response)
        {
            var client = new HttpClient();
            client.SetBearerToken(response.AccessToken);

            Console.WriteLine(client.GetStringAsync("http://localhost:41031/api/test").Result);
        }

        static TokenResponse GetClientToken()
        {
            var client = new TokenClient(
                token_endpoint,
                "silicon",
                "F621F470-9731-4A25-80EF-67A6F7C5F4B8");

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
    }
}
