# IdentityServer示例（2）：ResourceOwnerPasswords

## IdentityServer

`dotnet new web -n IdentityServer`
`dotnet add package IdentityServer4 --version 2.1.1`

- 修改 `Config.cs`

```c#
public static IEnumerable<Client> GetClients()
{
    return new List<Client>
    {
        ...
        new Client
        {
            ClientId = "ro.client",
            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            // secret for authentication
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            // scopes that client has access to
            AllowedScopes = { "api1" }
        }
    };
}

public static List<TestUser> GetUsers()
{
    return new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "1",
            Username = "alice",
            Password = "password"
        },
        new TestUser
        {
            SubjectId = "2",
            Username = "bob",
            Password = "password"
        }
    };
}
```

- 修改`Startup.cs`

```c#
services.AddIdentityServer()
        .AddDeveloperSigningCredential()
        .AddTestUsers(Config.GetUsers())
        .AddInMemoryApiResources(Config.GetApiResources())
        .AddInMemoryClients(Config.GetClients());
```

## Api

`dotnet new web -n Api`
`dotnet add package IdentityServer4.AccessTokenValidation --version 2.3.0`

同上节
