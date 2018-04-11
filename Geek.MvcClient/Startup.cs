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
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddMvc();

            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = "Cookie";
                opt.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookie")
            .AddOpenIdConnect("oidc", opt =>
             {
                 opt.ClientId = "mvc.client";
                 opt.Authority = "http://localhost.:5000";
                 opt.RequireHttpsMetadata = false;
                 opt.SignInScheme = "Cookie";

                //  opt.GetClaimsFromUserInfoEndpoint = true; // 获取更全的Claims

                //  opt.Scope.Add(""); // 默认openid,profile
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
