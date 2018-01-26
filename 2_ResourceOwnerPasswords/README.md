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
        // other clients omitted...

        // resource owner password grant client
        new Client
        {
            ClientId = "ro.client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            
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

## Client

`dotnet new console -n Client`
`dotnet add package IdentityModel --version 3.0.0`

```c#
var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1");
```

access_token

```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImRiMzdhNzIzNGMxYzM0ZmZlOTM2ZDlmYmJkYTk2NDkyIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MTYwOTM0MzYsImV4cCI6MTUxNjA5NzAzNiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjpbImh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC9yZXNvdXJjZXMiLCJhcGkxIl0sImNsaWVudF9pZCI6InJvLmNsaWVudCIsInN1YiI6IjEiLCJhdXRoX3RpbWUiOjE1MTYwOTM0MzYsImlkcCI6ImxvY2FsIiwic2NvcGUiOlsiYXBpMSJdLCJhbXIiOlsicHdkIl19.EiOq3Oju29ARvt7IoetXSBLgudPoc-XO3Em29QHyKiDAHdIL-AYXawIQMwYC-at5vpawoKWe0cLSmHvFMPOv6bYuZArhzri4oEUfkcjfXvrNGqxmeRp7myMlqIcy4chKF5ezvNJPi2c5mo93ZUiw4hGxd7hiSbLOjKD5mH2LQvXCBcn6OQ3Fg4eG7Tgk6CvMyvser_UC3i9Otcrqeh2NaNcCykDYCY7bnmNuYuLUrv3fi2J2-3UhVHFBKoeaPnjJuNwVqgnLBgIdgiD4GT5AadWjCGZlWr9sO8rI3LP4I4uxr8rL22APd_72zn0vsOFPY0xnPDiNVhY5APqmCOfsIQ",
  "expires_in": 3600,
  "token_type": "Bearer"
}
```

claims

```json
[
  {
    "type": "nbf",
    "value": "1516093436"
  },
  {
    "type": "exp",
    "value": "1516097036"
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
    "value": "ro.client"
  },
  {
    "type": "sub",
    "value": "1"
  },
  {
    "type": "auth_time",
    "value": "1516093436"
  },
  {
    "type": "idp",
    "value": "local"
  },
  {
    "type": "scope",
    "value": "api1"
  },
  {
    "type": "amr",
    "value": "pwd"
  }
]
```

jwt

```json
{
  "alg": "RS256",
  "kid": "db37a7234c1c34ffe936d9fbbda96492",
  "typ": "JWT"
}
{
  "nbf": 1516093436,
  "exp": 1516097036,
  "iss": "http://localhost:5000",
  "aud": [
    "http://localhost:5000/resources",
    "api1"
  ],
  "client_id": "ro.client",
  "sub": "1",
  "auth_time": 1516093436,
  "idp": "local",
  "scope": [
    "api1"
  ],
  "amr": [
    "pwd"
  ]
}
```
