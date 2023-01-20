using BookShop.Areas.Identity.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Areas.Identity.Services
{
    public static class AddIdentityOptionsExtensions
    {
        public static IServiceCollection AddIdentityOptions(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(
                    option =>
                    {
                        //Configure Password
                        option.Password.RequireDigit = false;
                        option.Password.RequiredLength = 6;
                        option.Password.RequiredUniqueChars = 1;
                        option.Password.RequireLowercase = false;
                        option.Password.RequireNonAlphanumeric = false;
                        option.Password.RequireUppercase = false;

                        option.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstxyuwzABCDEFGHIJKLMNOPQRSTXYuWZ0123456789-_.@+";
                        option.User.RequireUniqueEmail = true;
                        //Does not allow user with Unconfirmed Email
                        option.SignIn.RequireConfirmedEmail = true;

                        option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                        option.Lockout.MaxFailedAccessAttempts = 3;
                    })
                .AddEntityFrameworkStores<BookShopContext>()
                //.AddDefaultUI()
                .AddErrorDescriber<ApplicationIdentityErrorDescriber>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
