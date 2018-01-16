# IdentityServer示例（1）：ClientCredentials

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
`dotnet add package IdentityServer4.AccessTokenValidation --version 2.3.0`

- 修改`Statup.cs`

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddAuthentication("Bearer")
        .AddIdentityServerAuthentication(options=>{
            options.Authority = "http://localhost:5000";
            options.ApiName = "api1";
            options.RequireHttpsMetadata = false;   // develop
        });
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseAuthentication();

    app.UseMvc();
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

`dotnet new console -n Client`
`dotnet add package IdentityModel --version 3.0.0`

```c#
private static async Task MainAsync()
{
    // discover endpoints from metadata
    var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
    if (disco.IsError)
    {
        Console.WriteLine(disco.Error);
        return;
    }

    // request token
    var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
    var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

    if (tokenResponse.IsError)
    {
        Console.WriteLine(tokenResponse.Error);
        return;
    }

    Console.WriteLine(tokenResponse.Json);

    // call api
    var client = new HttpClient();
    client.SetBearerToken(tokenResponse.AccessToken);

    var response = await client.GetAsync("http://localhost:5001/identity");
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine(response.StatusCode);
    }
    else
    {
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JArray.Parse(content));
    }
}
```

```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImRiMzdhNzIzNGMxYzM0ZmZlOTM2ZDlmYmJkYTk2NDkyIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MTYwOTM0OTIsImV4cCI6MTUxNjA5NzA5MiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC9yZXNvdXJjZXMiLCJhcGkxIl0sImNsaWVudF9pZCI6ImNsaWVudCIsInNjb3BlIjpbImFwaTEiXX0.Mdd3Wc82fZcwIEePq1WnTb_Pkhr0AUzER_iyF7cK7kOs-S_TJQR_ylR2HfiJE1u8nD3VetfcTT5GBvK4h-jlcJFpL2_4913jrkeKZeVjce4ka0u6qGJOIjN-kplnAdyM8HgD1_GKgEq2DI3A5rsUsPRW7bdv415fCR2jtL6MQhKm6eG2utF4CrVDTFW3tDtx7j7Ey5RcdFmdjClgXqUdQbTt94YwBnUDrr9s-VWJg8R_nilL58eIuJo9u6nABinesmdb9V--Hzg1j3IlujaFP9EA2N_6xrDS-DadzxQ2Q1rucSYGZV43hcv_HYe3WINEJWKAVeZxaz3MmIgtbP8cog",
  "expires_in": 3600,
  "token_type": "Bearer"
}
[
  {
    "type": "nbf",
    "value": "1516093492"
  },
  {
    "type": "exp",
    "value": "1516097092"
  },
  {
    "type": "iss",
    "value": "http://localhost:5000"
  },
  {
    "type": "aud",
    "value": "http://localhost:5000/resources"
  },
  {
    "type": "aud",
    "value": "api1"
  },
  {
    "type": "client_id",
    "value": "client"
  },
  {
    "type": "scope",
    "value": "api1"
  }
]
```