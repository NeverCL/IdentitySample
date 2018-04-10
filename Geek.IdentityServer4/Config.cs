using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace Geek.IdentityServer4
{
    public class Config
    {
        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                    new ApiResource("api1")
            };
        }
        public static List<Client> GetClients()
        {
            return new List<Client>
            {
                new Client{
                    ClientId = "client1",
                    ClientSecrets= { new Secret("secret1".Sha512()) },
                    AllowedScopes =  {"api1"},
                    AllowedGrantTypes ={ GrantType.ClientCredentials }
                },
                new Client{
                    ClientId = "ro.client",
                    ClientSecrets= { new Secret("secret2".Sha512()) },
                    AllowedScopes =  {"api1"},
                    AllowedGrantTypes ={ GrantType.ResourceOwnerPassword }
                },
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser> {
                new TestUser{
                    Username = "user1",
                    Password = "pwd1",
                    SubjectId = "sub1"
                },
                new TestUser{
                    Username = "user2",
                    Password = "pwd2",
                    SubjectId = "sub2"
                },
            };
        }
    }
}