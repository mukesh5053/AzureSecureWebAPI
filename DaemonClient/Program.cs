using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace DaemonClient
{
    class Program
    {
        private static  AuthConfig config = null;
        static void Main(string[] args)
        {
             config = AuthConfig.ReadFromJsonFile("appsettings.json");
            ReadAsync().GetAwaiter().GetResult();
        }

        private static async Task ReadAsync()
        { 
            AuthConfig config = AuthConfig.ReadFromJsonFile("appsettings.json");
            IConfidentialClientApplication app;

            //Create client application instance
            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(new Uri(config.Authority))
                .Build();

            string[] ResourceIds = new string[] { config.ResourceId };
            AuthenticationResult result = null;

            try
            {
                //Call to azure resource to get token
                result = await app.AcquireTokenForClient(ResourceIds).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token Received...");
                Console.WriteLine(result.AccessToken);
                Console.ResetColor();
                await MakeCallToSecureWebAPI(result.AccessToken);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }          

        }

        private static async Task MakeCallToSecureWebAPI(string accessToken)
        {
            if (! string.IsNullOrEmpty(accessToken))
            {
                var httpClient = new HttpClient();

                var requestHeader = httpClient.DefaultRequestHeaders;

                if (requestHeader.Accept == null || !requestHeader.Accept.Any(hdr=> hdr.MediaType == "application/json" ) )
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new
                        MediaTypeWithQualityHeaderValue("application/json"));
                }

                requestHeader.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                HttpResponseMessage response = await httpClient.GetAsync(config.BaseAddress);

                if (response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to call API : {response.StatusCode }");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }

                Console.ResetColor();


            }
        
        }

    }
}
