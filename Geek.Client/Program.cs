using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Geek.Client
{
    // http://localhost:5000/.well-known/openid-configuration
    class Program
    {
        static async Task Main(string[] args)
        {
            // var disco = await new DiscoveryClient("http://localhost:5000").GetAsync();
            // var token = await new TokenClient("http://localhost:5000/connect/token", "client1", "secret1").RequestClientCredentialsAsync("api1");
            var token = await new TokenClient("http://localhost:5000/connect/token", "re.client", "secret").RequestClientCredentialsAsync("api1");
            // token = await new TokenClient(disco.TokenEndpoint, "ro.client", "secret2").RequestResourceOwnerPasswordAsync("user1", "pwd1", "api1");
            var client = new HttpClient();
            client.SetBearerToken(token.AccessToken);
            var content = await client.GetStringAsync("http://localhost:5001/api/value");
            System.Console.WriteLine(token.AccessToken);
            System.Console.WriteLine(content);
        }
    }
}
