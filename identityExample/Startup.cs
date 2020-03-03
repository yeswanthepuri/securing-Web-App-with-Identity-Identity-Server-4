using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using identityExample.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

namespace identityExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(conf =>
            {
                conf.UseInMemoryDatabase("Memory");
            });
            //addIdentity Registers the services
            services.AddIdentity<IdentityUser, IdentityRole>(cnf =>
            {
                cnf.Password.RequireNonAlphanumeric = false;
                cnf.Password.RequireNonAlphanumeric = false;
                cnf.Password.RequireUppercase = false;
                cnf.Password.RequireDigit = false;
                cnf.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            services.AddAuthentication("CookieAuth").AddCookie(cnf =>
                        {
                            cnf.Cookie.Name = "identity.Cookie";
                            cnf.LoginPath = "/Home/LogIn";
                        });
            services.AddAuthorization(config=>{
                var defaultauthBuilder=new AuthorizationPolicyBuilder();
                var defaultAuthPolicy=defaultauthBuilder
                .RequireAuthenticatedUser()
                //.RequireClaim(ClaimTypes.DateOfBirth)
                .Build();

                config.DefaultPolicy = defaultAuthPolicy;
            });
            //This for configering mail into he project
            services.AddMailKit(config => config.UseMailKit(Configuration.GetSection("Email").Get<MailKitOptions>()));
            services.AddControllersWithViews();
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
