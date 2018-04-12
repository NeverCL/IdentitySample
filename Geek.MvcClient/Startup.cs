using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Geek.MvcClient
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // 关闭jwt到默认scheme的映射
            services.AddMvc();

            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = "Cookie";
                opt.DefaultChallengeScheme = "hybrid-oidc";
            }).AddCookie("Cookie")
             .AddOpenIdConnect("oidc", opt =>
             {
                 opt.ClientId = "mvc.client";
                 opt.Authority = "http://localhost.:5000";
                 opt.RequireHttpsMetadata = false;
                 opt.SignInScheme = "Cookie";
                //  opt.CallbackPath = "/signin-oidc"; // 默认该值
                //  opt.SignedOutCallbackPath = "/signout-callback-oidc";  // 默认该值

                 //  opt.GetClaimsFromUserInfoEndpoint = true; // 请求userinfo获取IdentityResource
                 //  opt.ResponseType 默认为idtoken
                 //  opt.Scope.Add(""); // 默认openid,profile
             })
            .AddOpenIdConnect("hybrid-oidc", opt =>
             {
                 opt.ClientId = "hybrid.client";
                 opt.ClientSecret = "secret";
                 opt.Authority = "http://localhost.:5000";
                 opt.RequireHttpsMetadata = false;
                 opt.SignInScheme = "Cookie";
                 opt.CallbackPath = "/signin-hybrid-oidc";
                 opt.SaveTokens = true; // 默认为false，将token存储到cookie。便于后面直接获取

                 opt.GetClaimsFromUserInfoEndpoint = true;
                 opt.ResponseType = "code id_token";
                 opt.Scope.Add("api1");
                 opt.Scope.Add("offline_access");   // 用于获取refresh token
             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World1!");
            });
        }
    }
}
