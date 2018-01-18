# IdentityServer示例（4）：ImplicitFlowAuthenticationWithExternal

## IdentityServer

```powershell
dotnet new web -n IdentityServer
dotnet add package IdentityServer4 --version 2.1.1
iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/release/get.ps1'))
```

- 修改`Startup.cs`

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication()
        .AddOpenIdConnect("oidc", "OpenID Connect", options =>
        {
            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            options.SignOutScheme = IdentityServerConstants.SignoutScheme;

            options.Authority = "https://demo.identityserver.io/";
            options.ClientId = "implicit";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role"
            };
        });
}
```

## MvcClient

`dotnet new mvc -n MvcClient`

与上节相同