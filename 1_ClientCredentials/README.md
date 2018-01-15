# IdentityServer示例

## ClientCredentials

### IdentityServer

`dotnet new web -n IdentityServer`
`dotnet add package IdentityServer4 --version 2.1.1`

- 添加 `Config.cs`

```c#
public static IEnumerable<ApiResource> GetApiResources()
{
    return new List<ApiResource>
    {
        new ApiResource("api1", "My API")
    };
}

public static IEnumerable<Client> GetClients()
{
    return new List<Client>
    {
        new Client
        {
            ClientId = "client",
            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = // secret for authentication
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "api1" } // scopes that client has access to
        }
    };
}
```

- 修改 `Startup.cs`

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddIdentityServer()
        .AddDeveloperSigningCredential()
        .AddInMemoryApiResources(Config.GetApiResources())
        .AddInMemoryClients(Config.GetClients());
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseIdentityServer();
}
```

访问`http://localhost:5000/.well-known/openid-configuration`确认

### Api

`dotnet new web -n Api`

- 修改`Statup.cs`

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
}
```

- 添加`Controller`

```c#
[Route("identity")]
[Authorize]
public class IdentityController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
    }
}
```

### Client

