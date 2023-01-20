using BookShop.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Areas.Identity.Services
{
    public static class AddCustomPoliciesExtensions
    {
        public static IServiceCollection AddCustomPolicies(this IServiceCollection services)
        {
            //Add Policy based rules here
            services.AddSingleton<IAuthorizationHandler, HappyBirthDayHandler>();
            services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AccessToUsersManager", policy => policy.RequireRole("مدیر سایت", "کاربر"));
                //options.AddPolicy("HappyBirthDay", policy => policy.RequireClaim(ClaimTypes.DateOfBirth,DateTime.Now.ToString("MM/dd")));
                options.AddPolicy("HappyBirthDay", policy => policy.Requirements.Add(new HappyBirthDayRequirement()));
                options.AddPolicy("AtLeast18", policy => policy.Requirements.Add(new MinimumAgeRequirement(18)));
            });

            return services;
        }
    }
}
