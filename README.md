# IdentitySample

IdentitySample包含

## ClientCredentials

Client Credentials是IdentityServer中的最基本场景。
在这种情况下，我们将定义一个API和一个想要访问它的客户端。客户端将在IdentityServer上请求访问令牌，并使用它来访问API。

安全性：access_token是jwt格式的。通过验签部分来确保数据非法篡改。
注意：定义ClientClaims时，在API端根据Name获取Claims时，需要添加前缀。
问题：如何防止access_token避免频繁调用。

## ResourceOwnerPasswords

ResourceOwnerPasswords授权client发送 username and password 到 token service 来获取代表用户的access token。
该规范用于非常可信的 或旧的应用。

注意：
1. access_token的信息相比clientcredentials方式，多返回name为sub的claim标识用户的Id
1. 声明的存在（或不存在）sub使API能够区分代表Client 或 User的请求

问题：如果用户的Claims，默认不会传输到API端。


2_ResourceOwnerPasswords
3_ImplicitFlowAuthentication
4_ImplicitFlowAuthenticationWithExternal

## 问题

### ClientSecrets 为什么是数组