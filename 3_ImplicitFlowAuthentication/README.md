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

## MvcClient

`dotnet new mvc -n MvcClient`

- 修改`Startup.cs`

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    services.AddAuthentication(opt =>
    {
        opt.DefaultScheme = "Cookie";// 未登录的时候 执行oidc的Challenge方法
        opt.DefaultChallengeScheme = "oidc";
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