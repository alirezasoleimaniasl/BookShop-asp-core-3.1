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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BookShop.Areas.Identity.Data;

namespace BookShop.Areas.Identity.Services
{
    public static class AddCustomAuthenticationExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services,SiteSettings siteSettings)
        {
            services.AddAuthentication(options =>
            {
                //These thre lines enable sign in by JWT- To sign in webpage comment them
                //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
                            //TokenValidation on server side
                            OnTokenValidated = async context =>
                            {
                                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IApplicationUserManager>();

                                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                                if (claimsIdentity.Claims?.Any() != true)
                                    context.Fail("This token has no claims.");

                                var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                                if (!securityStamp.HasValue())
                                    context.Fail("This token has no secuirty stamp");

                                var userId = claimsIdentity.GetUserId<string>();
                                var user = await userRepository.GetUserAsync(context.Principal);

                                if (user.SecurityStamp != securityStamp)
                                    context.Fail("Token secuirty stamp is not valid.");

                                if (!user.IsActive)
                                    context.Fail("User is not active.");
                            },

                            OnChallenge = context =>
                            {
                                if (context.AuthenticateFailure != null)
                                    throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);
                                throw new AppException(ApiResultStatusCode.UnAuthorized, "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);
                            }
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
