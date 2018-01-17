# IdentityServer示例（3）：ImplicitFlowAuthentication

## IdentityServer

`dotnet new web -n IdentityServer`
`dotnet add package IdentityServer4 --version 2.1.1`
`iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/release/get.ps1'))`

- 修改`Config.cs`

```c#
new Client
{
    ClientId = "mvc",
    ClientName = "MVC Client",
    AllowedGrantTypes = GrantTypes.Implicit,

    RedirectUris = { "http://localhost:5002/signin-oidc" },
    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

    AllowedScopes =
    {
        IdentityServerConstants.StandardScopes.OpenId,
        IdentityServerConstants.StandardScopes.Profile
    }
}
```

- 修改`Startup.cs`

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddIdentityServer()
        .AddDeveloperSigningCredential()
        .AddInMemoryApiResources(Config.GetApiResources())
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