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
        IdentityServerConstants.StandardScopes.Profile  // 将IdentityServer配置的用户Claim传递到Client中
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
        opt.DefaultScheme = "Cookie";   // 未登录的时候 执行oidc的Challenge方法
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

## 报文分析

### Form

```request
POST http://localhost:5002/signin-oidc HTTP/1.1

id_token=eyJhbGciOiJSUzI1NiIsImtpZCI6IjEzYmE3ZWQwZTU0ODdmZjU3NTJkMTZiODFhMWZmNWY3IiwidHlwIjoiSldUIn0.eyJuYmYiOjE1MTYyNzkxMjcsImV4cCI6MTUxNjI3OTQyNywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjoibXZjIiwibm9uY2UiOiI2MzY1MTg3NTkxMjQ5MTE1MzguTlRVMk1UVTFZMkV0WW1VNE5DMDBZV1U1TFdJNU5XVXROelF6WlRabE5qUTVOamszWmpFd01XWmtaV010WkRRNFl5MDBZalUzTFdJM1ltTXRZak0xTnpnMk9UQmlNemd6IiwiaWF0IjoxNTE2Mjc5MTI3LCJzaWQiOiIzYjU5Y2JlOGQwMGRlZmYzOTAwMGNjNTRkNjA2ZGNkZSIsInN1YiI6IjEiLCJhdXRoX3RpbWUiOjE1MTYyNzkxMjQsImlkcCI6ImxvY2FsIiwiYW1yIjpbInB3ZCJdfQ.Yr-FrTdMQZo3GTvh8h14gw7SZnKQ5ziUi4wf9mi9657FFqH-RyuFeVvRelku6C9poLzpeaFDClTMCBxxTwmuDwBmERFYjg3EAvWkjSA-5Ba0MEzFMHbk88esW1INnf64_ts44YG8dOHI88jTdek7QtIcO3K6qaLkE2iHgVgNYAbt5EmamY6QlyG-Z9p8YHhdQIFDV0LQ9kS30iKwFjuKy8W3IfCi5eV6ZqnNQkrxQicCsus539fSiS66RznT3aRj4frtTzsUNJLVzdT1XYvnHmoM_XOjnDUCpPV3dHlXAxinu6FTGwLy9lP4tFAEJ5gW2pLDOTy55A6SVcKmv1AkQA&scope=openid+profile&state=CfDJ8JnhfyvJWlhFrDWInn0ebv8jyPyTAlyOBUzGWe0xy1T4SNIns7rz_KqfAGllS7qEaXcDUDj0ShKrzzDkd4Q_9h7P-2cMTSH-W3gvLRsXctohv-ELWenjZqQ45AYaA4hX0r4jmold3Nc6lAv3i00yhmsPyLXez8Gj577ajhhPD882wzVzQpPzRE8iEkjOqInIoJG4U3Lzz9FpD24psZcYM-NlJgcIPq7KEE6AcCczS1njeER90q17XHqV58KECDeNvFVA6qqcBBzp_K6ci6tIAsnz_EleOmXX_uFhA5QsTlK1UPHx5P6jFIbbnT8wlWn02CztcRU3q-QN3IX5MOVmBns&session_state=-slrJ-_EV00fUZIQEaOboI2ZBD-PFfYw54dQQjQOmyk.66566fb404c81514394f0109c5ce018c
```

```response
HTTP/1.1 302 Found

Location: http://localhost:5002/Home/Contact
Set-Cookie: .AspNetCore.Correlation.oidc.bShJvLbxRF1PxU0SWUljP2ug6Nq2T2g9eV7Mi2syeOw=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/signin-oidc; samesite=lax
Set-Cookie: .AspNetCore.OpenIdConnect.Nonce.CfDJ8JnhfyvJWlhFrDWInn0ebv-mpltj35x3-I6WKLRJF_SkcrS6S8OgTfuwAFim-mCQDxnTK3yk-qdr13fdkFoNf5elRSmNWCTjTSbNu9JhfXw1Ao8fvidXi1YH0ujWzL9NE3paZxYhmJ-istmGszS1ADOv6NaQlWlLB52nyZqduSNm09gwGPvdXSG7C40qnUlcMyLM1Yr_PmoH8-kVidzPWMjp_mAWRC1MFXLHkhXIBzXqpsujRRCzof45DsAfHNMpIHqsj5Mz3HITKBXCCLL0J1g=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/signin-oidc; samesite=lax
Set-Cookie: .AspNetCore.Cookie=CfDJ8JnhfyvJWlhFrDWInn0ebv_Pgl-wuOI5JsaL1jMf9DXI6Eg-kVxvVgVm94ubpvO2HzMYuigEF5DlWBOAkGMX-mV0hYg1rIZoynDec4ZTRkrJeo0L71Aq2Q23GizF43BgdNSUCPnunh00kjAYDNQQigYJa_CK7Fo6Ch1m6JawqJqqvzVJ8RPMLr8idBb9lRadhk9VLrGemadxiEOBfuOhmE_JIdyMLbhfBiat3btONuCf4se6Lukk-iFuZ7tbVwMEnYisyV4GhnYqJVw2Zqj7pVCQxU2u5dgmwgJlW-dl8eZe2NneAcowdwrizsodFVIgUBkbHNQ_l9PxI3AvKQ4mW34-Ppo2bTEXONEMj8D_Zql98oxhssaGKrWkg6KIjJmVyehPgt_12-JMXaXlqkxDPWAmZRBKV3xqx93NWi58LNShP_xpSYnYycKdJhpWiMXenY47dllPEB_7magCXn1n_Ujkq3qkjuY3zID57swaENK1mhG5GFFQ_gAW3P16qXYb4xz7SsnIynxsTRa7_kv5aaOiG6pkc2_cPVGbkvGyzukO9wHmTCWDhPBtToy-jlY4YjOfyvDd1fZNK9fz04fl19K6u3HKanCDOIX9ldYH80jCm-pjTqTrys71pYb2vCu6PY3hma9fSxK3TrSFDH6xMY0RPIR2zp_C2g96EhWapmtGJiFOJ7cTsPt11RC3E66L111TCffi-vD9YaQmLEAaSUtE6XsbPDorxYIJMEgVDUN4phsF-zbvhl2T55zy1M03mk8l-j3CEhZtc7cy8kZNqcfzI7suJjzaw2j0Whksr_TQiRdgilSG4wY053jK09AfP5DjuqNsGH1ZmTqDnRdDIoQ_1qKspBLOZdecBadRi3YeYZieFFmuLP1Vvuy8mSM2HCfA_LvO975OECsPM2i25Jrks7NzyrQIvNVUPrpsFrQFpgxKLGHUvp18-y4ZRH3f_78ok07wYPvQZ0EkmTYBNR_FspQFJ6Uh_zFxMolcE2-U92DQ3dEXEyTly56ZRlFkuK0Sp95BaXX5EoOXO-OATPc8CA56l65J7Ks_lHRw5UYkCFU7Lmi5pI5Io5tqFCpf5WbNaXyA0FB2iOmjF8TlIUq_RXA1anGwefmal0hHBEv2h_aP0-DvSxVbmMZD3w1FyeZ-Wmbnci0nfIyncTtDoK6vme_D2TycAtiwdl34lU4qKcfuhiqgqbYYj9vhu1Cc5UVGnJMnMdYeL2VV8qaiNyZaNwygVQpIo3ipfnwa0Yrd8TLeF5oqGQUaRe1yq8AmWyMtnZUaHm2hTzVHl-He_ajSYIZ5ruSh1x14b_x3N_oxWjTIocRj5Yxy8ggCq2k9CXC4Cw58R6GUQDrCEYsFz5cxXR7OIj2PuFPVcPW6BGlObpiWI5Rcsx5KyyOAuGj2-FjlV9xtf1V4ifiVuXfOJ3g; path=/; samesite=lax; httponly
```