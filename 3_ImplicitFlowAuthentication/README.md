# IdentityServer示例（3）：ImplicitFlowAuthentication

## IdentityServer

`dotnet new web -n IdentityServer`
`dotnet add package IdentityServer4 --version 2.1.1`
`iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/release/get.ps1'))`

- 修改`Config.cs`

```c#
// other clients omitted...

// OpenID Connect implicit flow client (MVC)
new Client
{
    ClientId = "mvc",
    ClientName = "MVC Client",
    AllowedGrantTypes = GrantTypes.Implicit,

    // where to redirect to after login
    RedirectUris = { "http://localhost:5002/signin-oidc" },

    // where to redirect to after logout
    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

    AllowedScopes = new List<string>
    {
        IdentityServerConstants.StandardScopes.OpenId,
        IdentityServerConstants.StandardScopes.Profile
    }
}
...
public static List<TestUser> GetUsers()
{
    return new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "1",
            Username = "alice",
            Password = "password",
            Claims = new List<Claim>
            {
                new Claim("name", "Alice"),
                new Claim("website", "https://alice.com")
            }
        }
    };
}
...
public static IEnumerable<IdentityResource> GetIdentityResources()
{
    return new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    };
}
```

- 修改`Startup.cs`

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddIdentityServer()
        .AddDeveloperSigningCredential()
        .AddInMemoryIdentityResources(Config.GetIdentityResources())    // 添加身份资源
        .AddInMemoryApiResources(Config.GetApiResources())              // 添加Api
        .AddInMemoryClients(Config.GetClients())
        .AddTestUsers(Config.GetUsers());
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseIdentityServer();

    app.UseStaticFiles();
    app.UseMvcWithDefaultRoute();
}
```

访问`http://localhost:5000`确认(Identity页面)

## MvcClient

`dotnet new mvc -n MvcClient`

- 修改`Startup.cs`

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    services.AddAuthentication(opt =>
    {
        opt.DefaultScheme = "Cookie";
        opt.DefaultChallengeScheme = "oidc";    // when we need the user to login, we will be using the OpenID Connect scheme
    })
    .AddCookie("Cookie")
    .AddOpenIdConnect("oidc",opt=>
    {
        opt.SignInScheme = "Cookie";    // 所有远程登录都需要指定本地登录的方法

        opt.Authority = "http://localhost:5000";
        opt.RequireHttpsMetadata = false;

        opt.ClientId = "mvc";
    });
}

...
app.UseAuthentication();
```

## 报文分析

### Form

```request
POST http://localhost:5002/signin-oidc HTTP/1.1

id_token=eyJhbGciOiJSUzI1NiIsImtpZCI6IjEzYmE3ZWQwZTU0ODdmZjU3NTJkMTZiODFhMWZmNWY3IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MTY5Njg2NDUsImV4cCI6MTUxNjk2ODk0NSwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjoibXZjIiwibm9uY2UiOiI2MzY1MjU2NTQxMzYxNzI4NzEuTVdSak1HUTJNVGt0Tm1Fd055MDBORFJtTFdJd01Ua3RaVGMxT1ROak1EQmtOekppWVRrNE9UVTRPVGN0WTJaak9DMDBOelpqTFdGa1kyTXRPVGt4TnpBMk16WTRZV1k0IiwiaWF0IjoxNTE2OTY4NjQ1LCJzaWQiOiJlNjVhOGVjNTJlMDY0MzU1ZjgyNmNiNjg0MTAxOGM0MSIsInN1YiI6IjEiLCJhdXRoX3RpbWUiOjE1MTY5Njg2MzUsImlkcCI6ImxvY2FsIiwibmFtZSI6IkFsaWNlIiwid2Vic2l0ZSI6Imh0dHBzOi8vYWxpY2UuY29tIiwiYW1yIjpbInB3ZCJdfQ.oZYT1_WGs00Uc-vgLW7F9ZW4xLl5AyHmFf9NlK1w568iF4Ksp3tp3SkO_PR7sIPZC10wDoZAViLqX-BCoRYWb7k84UGVyCXOcdgaPINMXuNjlYcAsaKMZlO0j4TZ-ikR5T187Qddi6GAJQ2ngUciCph6lQDQSidjOEgEvT-VquIp6M7l7NK-_szlpM0XfsOBMnJqyc07E-Lfi9JgYo4qcTayM06ibg6NBR3lQ3fJ0ZPT9igzKdpvzttVic1ibXK8z7E8A7x3XE4qRUG7gOsobEujxQRTQGuBM01yY-WEq4RrjkEI7k3goJs1LIFrjLnRGnyktZATarIh-86S7J_b7g&scope=openid+profile&state=CfDJ8JnhfyvJWlhFrDWInn0ebv-jWklrE_9sZfI6PSXbvUbEazXpBiXKS7AFHTqeIcOZozBbxG7gqqphiyRfohTXVKeiOg3k_nsvj3l43TF041cxf8h0GDRA68j8DqCbzYdDuZKUNnWC6-iZ606oapRr7vBpRGUSeMlh1nT8mXyVOQ6K2IB-BHWoxYNQtHTEouCLn3P9R4JHKHCaWvUjMWGKJ-Ux5kTNS4O6TNSLzZMtyOCzHV94I3UIHkkXCHSnz0kP2UmNYYT_SbpxpqcMsU9YSVl1HoSzRd7hOuXTrFa6OYUGBhQFwFGCZgg_VSt4kXDCLgN2bSO3_dHMe6xFdM2-deI&session_state=N3UzAjKNn82NeKkl6d0xYJnBC-TZts1g758sMTFLipo.f05b8d91ded3d4c5b44afdeae9153095
```

jwt

```json
{
  "alg": "RS256",
  "kid": "13ba7ed0e5487ff5752d16b81a1ff5f7",
  "typ": "JWT"
}
{
  "nbf": 1516968645,
  "exp": 1516968945,
  "iss": "http://localhost:5000",
  "aud": "mvc",
  "nonce": "636525654136172871.MWRjMGQ2MTktNmEwNy00NDRmLWIwMTktZTc1OTNjMDBkNzJiYTk4OTU4OTctY2ZjOC00NzZjLWFkY2MtOTkxNzA2MzY4YWY4",
  "iat": 1516968645,
  "sid": "e65a8ec52e064355f826cb6841018c41",
  "sub": "1",
  "auth_time": 1516968635,
  "idp": "local",
  "name": "Alice",
  "website": "https://alice.com",
  "amr": [
    "pwd"
  ]
}
```

```response
HTTP/1.1 302 Found

Location: http://localhost:5002/Home/Contact
Set-Cookie: .AspNetCore.Correlation.oidc.
Set-Cookie: .AspNetCore.OpenIdConnect.Nonce.
Set-Cookie: .AspNetCore.Cookie=
```
