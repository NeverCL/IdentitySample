using System.Collections.Generic;
using System.Security.Claims;
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
                    new ApiResource("api1"){
                        ApiSecrets = {                  // for introspection
                            new Secret("sec1".Sha256())
                        }
                    }
            };
        }

        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>{
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
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
                    ClientId = "re.client",
                    ClientSecrets= { new Secret("secret".Sha512()) },
                    AllowedScopes =  {"api1"},
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedGrantTypes ={ GrantType.ClientCredentials }
                },
                new Client{
                    ClientId = "ro.client",
                    ClientSecrets= { new Secret("secret2".Sha512()) },
                    AllowedScopes =  {"api1"},
                    AllowedGrantTypes ={ GrantType.ResourceOwnerPassword }
                },
                new Client{
                    ClientId = "mvc.client",
                    AllowedScopes = {"openid","profile"},

                    AllowRememberConsent = false, // 默认为true，在Memory中记住

                    RedirectUris = {"http://localhost:5002/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5002/signout-callback-oidc"},

                    AllowedGrantTypes = {GrantType.Implicit}
                },
                new Client{
                    ClientId = "hybrid.client",
                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {"api1","openid","profile"},

                    RequireConsent = true,         // 默认为true，需要用户许可Client

                    AllowRememberConsent = false, // 默认为true，在Memory中记住
                    AllowOfflineAccess = true,   // 默认为false,支持返回refresh_token
                    // AllowAccessTokensViaBrowser = true, // 默认为false,允许access_token出现在浏览器中，而不是后端请求获取，需要搭配implicit

                    RedirectUris = {"http://localhost:5002/signin-hybrid-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5002/signout-callback-oidc2"},

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials
                },
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser> {
                new TestUser{
                    Username = "user1",
                    Password = "pwd1",
                    SubjectId = "sub1",
                    Claims = {
                        new Claim("name","小张"),
                        new Claim("website","Geek"),
                        new Claim("role","admin1"),
                        new Claim(ClaimTypes.Role,"admin"),
                    }
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