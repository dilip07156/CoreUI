using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Voyager.App.Library;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Voyager
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _hostingEnvironment = env;
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
          .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add service and create Policy with options 
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                  builder => builder.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials());
            });

            //Added for 3rd party to create session and only be used for 3rd party
            services.AddDistributedMemoryCache();

            services.AddSession(options => {
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = ".ThirdParty.Session";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.None;
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });

            services.AddDataProtection()
                .SetApplicationName("voyager")
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"/var/dpkeys/"));
            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       options.AccessDeniedPath = new PathString("/Security/Access");
                       options.LoginPath = new PathString("/Account/Login");                       
                       // required or else it will result in an endless-login / redirect loop if it's called from an iframe in sharepoint
                       options.Cookie.SameSite = SameSiteMode.None;
                   });
                   //.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            //services.Configure<FormOptions>(options =>
            //{
            //    options.MultipartBodyLengthLimit = 60000000; // use to upload file of large size
            //});
            services.Configure<FormOptions>(x => x.ValueCountLimit = int.MaxValue);
            //services.AddMvc();

            services.AddMvc(options =>
            {
                options.Filters.Add(new CustomExceptionFilterAttribute(_hostingEnvironment, Configuration));
            }).AddSessionStateTempDataProvider();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IConfiguration>(Configuration);
            services.AddScoped<IViewRenderService, ViewRenderService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            _hostingEnvironment = env;

            // global policy, if assigned here (it could be defined indvidually for each controller) 
            app.UseCors("CorsPolicy");

            //Added for 3rd party to create session and only be used for 3rd party
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None
            });

            app.UseSession();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStatusCodePages(); 
            app.UseStatusCodePagesWithReExecute("/Error/ErrorCode", "?errCode={0}");

            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {

                context.Response.Headers.Remove("X-Frame-Options");
                //context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                //context.Response.Headers.Add("X-Frame-Options", "ALLOW-FROM https://voyager-uat.coxandkings.com/");
                context.Response.Headers.Add("X-Frame-Options", "ALLOW-ALL");
                await next();
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            }); 
        }
    }
}
