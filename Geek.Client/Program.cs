using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Geek.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var disco = await new DiscoveryClient("http://localhost:5000").GetAsync();
            if (disco.IsError)
            {
                System.Console.WriteLine($"IdentityServer: {disco.Error}");
                return;
            }
            var token = await new TokenClient(disco.TokenEndpoint, "client1", "secret1").RequestClientCredentialsAsync("api1");
            token = await new TokenClient(disco.TokenEndpoint, "ro.client", "secret2").RequestResourceOwnerPasswordAsync("user1", "pwd1", "api1");
            var client = new HttpClient();
            client.SetBearerToken(token.AccessToken);
            var content = await client.GetStringAsync("http://localhost:5001/api/value");
            System.Console.WriteLine(token.AccessToken);
            System.Console.WriteLine(content);
        }
    }
}
