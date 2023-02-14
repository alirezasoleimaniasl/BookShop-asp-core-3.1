using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BookShop.Areas.Identity.Services
{
    public static class AddCustomAuthenticationExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    //secretKey and enryptionkey 
                    var secretkey = Encoding.UTF8.GetBytes("1234567890asdfgh");
                    var encryptionkey = Encoding.UTF8.GetBytes("1234567890asdfgh");

                    var validationParameters = new TokenValidationParameters
                    {
                        RequireSignedTokens = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        ValidateAudience = true, //default : false
                        ValidAudience = "Mizfa.com",

                        ValidateIssuer = true, //default : false
                        ValidIssuer = "Mizfa.com",

                        TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                    };

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = validationParameters;
                });
            //Google and Yahoo Authentications Comes Here
            //.AddGoogle()
            //.AddYahoo
            return services;
        }
    }
}
