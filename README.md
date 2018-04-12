# IdentitySample

IdentitySample

参考[官方Doc](https://identityserver4.readthedocs.io/en/release/index.html)

## Basic

dotnet new web -o Geek.IdentityServer4
dotnet add package IdentityServer4

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddIdentityServer()
        .AddDeveloperSigningCredential();
}
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseIdentityServer();
}
```

debug:

`cd ./Geek.IdentityServer4 && dotnet restore && dotnet watch run`

`cd ./Geek.Api && dotnet restore && dotnet watch run`

`cd ./Geek.Client && dotnet restore && dotnet watch run`

`cd ./Geek.MvcClient && dotnet restore && dotnet watch run`

### Protecting an API using Client Credentials

In this scenario we will define an API and a client that wants to access it. The client will request an access token at IdentityServer and use it to gain access to the API.

dotnet new web -o Geek.Api && cd ./Geek.Api && dotnet add package IdentityServer4.AccessTokenValidation

dotnet new console -o Geek.Client && cd ./Geek.Client && dotnet add package IdentityModel

[discovery document](http://localhost:5000/.well-known/openid-configuration)

### Protecting an API using Passwords

The OAuth 2.0 resource owner password grant allows a client to send username and password to the token service and get an access token back that represents that user.

The presence (or absence) of the sub claim lets the API distinguish between calls on behalf of clients and calls on behalf of users.

### Adding User Authentication with OpenID Connect

dotnet new web -o Geek.MvcClient && cd Geek.MvcClient && dotnet new page -n Index -o Pages -na Geek.MvcClient.Pages

All the protocol support needed for OpenID Connect is already built into IdentityServer. You need to provide the necessary UI parts for login, logout, consent and error.

`CallbackPath` = `RedirectUris`

`iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/release/get.ps1'))`

id_token固定为Jwt，所以无法revoke 或 实时更新
AddOpenIdConnect默认的scope为openid,profile

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
As well, we’ve turned off the JWT claim type mapping to allow well-known claims (e.g. ‘sub’ and ‘idp’) to flow through unmolested:

ResponseType
    idtoken

Claims
    Profile:name + website

Remember My Decision
    Store In Memory

User.Identity.Name or Other Claims
    [Identity Resource](http://openid.net/specs/openid-connect-basic-1_0-28.html#scopes)
    在Implicit模式下，不可以获取oidc以外的claim

Logout?
PostLogoutRedirectUris?

### Switching to Hybrid Flow and adding API Access back

In the previous quickstarts we explored both API access and user authentication. Now we want to bring the two parts together.

Access tokens are a bit more sensitive than identity tokens

OpenID Connect includes a flow called “Hybrid Flow” which gives us the best of both worlds：
    the identity token is transmitted via the browser channel
    the client opens a back-channel to the token service to retrieve the access token

HybridAndClientCredentials => ResponseType = "code id_token"