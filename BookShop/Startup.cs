using BookShop.Areas.Admin.Middlewares;
using BookShop.Areas.Api.Swagger;
using BookShop.Areas.Identity.Services;
using BookShop.Classes;
using BookShop.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ReflectionIT.Mvc.Paging;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace BookShop
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly SiteSettings _siteSettings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Get the information from appsetting-similar name
            _siteSettings = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));
            services.AddCustomPolicies();
            services.AddCustomIdentityServices(_siteSettings);
            services.AddCustomApplicationServices();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            

            services.AddRazorPages();
            //Show enums numbers as string(show string instead of return code)
            services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            //Add Api Errors as list of messages to output
            //services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    options.InvalidModelStateResponseFactory = actionContext =>
            //    {
            //        var errors = actionContext.ModelState
            //        .Where(e => e.Value.Errors.Count() != 0)
            //        .Select(e => e.Value.Errors.First().ErrorMessage).ToList();

            //        return new BadRequestObjectResult(errors);
            //    };
            //});

            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;//Adding Api version to Rquest Header
                option.AssumeDefaultVersionWhenUnspecified = true;//Add when api has unknown versioning
                option.DefaultApiVersion = new ApiVersion(1,0);//Add default version when you have not specified the version
                //option.ApiVersionReader = new HeaderApiVersionReader("x-api-key");//Add api version by header-Query by url will be disbled
                option.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader(),
                    new HeaderApiVersionReader("api-key"));

                //option.Conventions.Controller<SampleV1Controller>().HasApiVersion(new ApiVersion(1, 0));//Define version of Api for specific controller
            });
            services.Configure<FormOptions> (options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/SignIn";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });
            services.AddSwagger();
            //services.AddSwaggerGenNewtonsoftSupport();
            services.AddPaging(options =>
            {
                options.ViewName = "Bootstrap4";
                options.HtmlIndicatorDown ="<i class='fa fa-sort-amount-up'></i>";
                options.HtmlIndicatorUp = "<i class='fa fa-sort-amount-down'></i>";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomeExceptionHandler();
            //If using api use AddCu
            //app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
            //{
            //    appBuilder.UseCustomeExceptionHandler();
            //});

            //app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
            //{
            //    if (env.IsDevelopment())
            //    {
            //        appBuilder.UseDeveloperExceptionPage();
            //    }
            //    else
            //    {
            //        appBuilder.UseExceptionHandler("/Home/Error");
            //        app.UseHsts();
            //    }
            //});
            //Using node_modules files by asp.net core
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),
                RequestPath = "/" + "node_modules",
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthentication();
            app.UseCustomIdentityServices();
            app.UseAuthorization();
            app.UseSession();
            app.UseSwaggerAndUI();


            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "areas",
            //        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            //        );
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //    endpoints.MapRazorPages();
            //});
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
