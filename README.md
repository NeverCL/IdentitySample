# IdentitySample

IdentitySample包含

## ClientCredentials

Client Credentials是IdentityServer中的最基本场景。
在这种情况下，我们将定义一个API和一个想要访问它的客户端。客户端将在IdentityServer上请求访问令牌，并使用它来访问API。

安全性：access_token是jwt格式的。通过验签部分来确保数据非法篡改。
注意：定义ClientClaims时，在API端根据Name获取Claims时，需要添加前缀。
问题：如何防止access_token避免频繁调用。

## ResourceOwnerPasswords



2_ResourceOwnerPasswords
3_ImplicitFlowAuthentication
4_ImplicitFlowAuthenticationWithExternal

## 问题

### ClientSecrets 为什么是数组