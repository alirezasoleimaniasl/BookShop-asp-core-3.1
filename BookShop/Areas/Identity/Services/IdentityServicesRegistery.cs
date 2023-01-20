using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Areas.Identity.Services
{
    public static class IdentityServicesRegistery
    {
        public static void AddCustomIdentityServices(this IServiceCollection services)
        {
            services.AddIdentityOptions();
            services.AddDynamicPermission();
            services.AddCustomAuthentication();
            services.AddScoped<IApplicationRoleManager, ApplicationRoleManager>();
            services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
            services.AddScoped<ApplicationIdentityErrorDescriber>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmsSender, SmsSender>();
        }
        public static void UseCustomIdentityServices(this IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}
