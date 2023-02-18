using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BookShop.Classes;
using BookShop.Exceptions;
using BookShop.Areas.Api.Classes;
using System.Net;

namespace BookShop.Areas.Identity.Services
{
    public static class AddCustomAuthenticationExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services,SiteSettings siteSettings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    if(siteSettings.JwtSettings != null)
                    {
                        //secretKey and enryptionkey 
                        var secretkey = Encoding.UTF8.GetBytes(siteSettings.JwtSettings.SecretKey);
                        var encryptionkey = Encoding.UTF8.GetBytes(siteSettings.JwtSettings.EncryptKey);

                        var validationParameters = new TokenValidationParameters
                        {
                            RequireSignedTokens = true,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                            RequireExpirationTime = true,
                            ValidateLifetime = true,

                            ValidateAudience = true, //default : false
                            ValidAudience = siteSettings.JwtSettings.Audience,

                            ValidateIssuer = true, //default : false
                            ValidIssuer = siteSettings.JwtSettings.Issuer,

                            TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                        };

                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = validationParameters;
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = contex =>
                            {
                                if(contex.Exception != null)
                                    throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed!", HttpStatusCode.Unauthorized, contex.Exception, null);
                                return Task.CompletedTask;
                            },
                        };
                    }
                });
            //Google and Yahoo Authentications Comes Here
            //.AddGoogle()
            //.AddYahoo
            return services;
        }
    }
}
