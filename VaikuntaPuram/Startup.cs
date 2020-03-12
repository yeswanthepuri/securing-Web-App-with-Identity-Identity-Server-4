
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VaikuntaPuram.AuthorizationRequirement;
using VaikuntaPuram.Controllers;
using VaikuntaPuram.CustomPolicyResolver;

namespace VaikuntaPuram
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", config =>
            {
                config.Cookie.Name = "Identity.Cookie";
                config.LoginPath = "/Home/Authenticate";
            });
            // services.AddAuthorization(configure =>
            // {
            // configure.AddPolicy("Claims.DOB", policyBuilder =>
            //     {
            //         policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
            //     });
            // });
            services.AddAuthorization(configure =>
           {
               configure.AddPolicy("Claims.DOB", policyBuilder =>
                    {
                        policyBuilder.RequestCustomClaim(ClaimTypes.DateOfBirth);
                    });
           });
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
            services.AddScoped<IAuthorizationHandler, CustomRequiredClaimHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimTransformation>();

            //
            services.AddControllersWithViews(conf =>
            {
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder.RequireAuthenticatedUser().Build();
                //Global Authorization Filter
                conf.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            //This checks who you are
            app.UseAuthentication();
            //this askes are you allowed
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
