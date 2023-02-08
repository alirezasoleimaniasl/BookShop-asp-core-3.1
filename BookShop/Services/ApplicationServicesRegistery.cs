using BookShop.Areas.Api.Services;
using BookShop.Areas.Identity.Services;
using BookShop.Classes;
using BookShop.Models;
using BookShop.Models.Repository;
using BookShop.Models.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Services
{
    public static class ApplicationServicesRegistery
    {
        public static void AddCustomApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ConvertDate>();
            services.AddTransient<IConvertDate, ConvertDate>();
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<BookRepository>();
            services.AddTransient<BookShopContext>();
            services.AddTransient<IjwtService, jwtService>();
            services.AddHttpClient();
            //End
            //services.AddControllersWithViews();

            //services.AddDbContext<BookShopContext>(ServiceLifetime.Transient);
            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
            services.AddMvc(options =>
            {

                var F = services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
                var L = F.Create("ModelBindingMessages", "BookShop");
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                 (x) => L["انتخاب یکی از موارد لیست الزامی است."]);

                options.EnableEndpointRouting = false;

            }).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

        }
    }
}
