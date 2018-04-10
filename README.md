# IdentitySample

IdentitySample

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

### Protecting an API using Client Credentials

In this scenario we will define an API and a client that wants to access it. The client will request an access token at IdentityServer and use it to gain access to the API.

dotnet new web -o Geek.Api && cd ./Geek.Api && dotnet add package IdentityServer4.AccessTokenValidation

dotnet new console -o Geek.Client && cd ./Geek.Client && dotnet add package IdentityModel

[discovery document](http://localhost:5000/.well-known/openid-configuration)

### Protecting an API using Passwords

The OAuth 2.0 resource owner password grant allows a client to send username and password to the token service and get an access token back that represents that user.

The presence (or absence) of the sub claim lets the API distinguish between calls on behalf of clients and calls on behalf of users.

### Adding User Authentication with OpenID Connect

All the protocol support needed for OpenID Connect is already built into IdentityServer. You need to provide the necessary UI parts for login, logout, consent and error.

`iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/release/get.ps1'))`