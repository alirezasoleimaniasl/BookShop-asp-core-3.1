using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Api.Swagger
{
    public static class SwaggerConfigurationExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo()
                    {
                        Title = "Library Api",
                        Version = "v1",
                        Description = "Through this Api you can access BookInfo",
                        Contact = new OpenApiContact
                        {
                            Email = "alimamali1387@gmail.com",
                            Name = "Alireza Soleimani",
                            Url = new Uri("http://www.google.com"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "License",
                            Url = new Uri("http://www.google.com")
                        }
                    });
                c.SwaggerDoc(
                    "v2",
                    new OpenApiInfo()
                    {
                        Title = "Library Api",
                        Version = "v2",
                        Description = "Through this Api you can access BookInfo",
                        Contact = new OpenApiContact
                        {
                            Email = "alimamali1387@gmail.com",
                            Name = "Alireza Soleimani",
                            Url = new Uri("http://www.google.com"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "License",
                            Url = new Uri("http://www.google.com")
                        }
                    });
                c.UseInlineDefinitionsForEnums();
                c.DescribeAllParametersInCamelCase();
                c.OperationFilter<RemoveVersionParameters>();
                c.DocumentFilter<SetVersionInPaths>();

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes<ApiVersionAttribute>(true)
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });
                c.OperationFilter<UnauthorizedResponsesOperationFilter>(true,"Bearer");
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header
                });

                //Apply Authorize for all actions
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            },
                //            //Scheme = "oauth2",
                //            Name = "Bearer",
                //            In = ParameterLocation.Header
                //        },
                //     new List<string>()
                //    }
                //});

                //var xmlFiles = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFiles);
                //c.IncludeXmlComments(xmlPath);

                //c.SwaggerDoc(
                //       "v2",
                //       new OpenApiInfo()
                //       {
                //           Title = "Library Api",
                //           Version = "v2",
                //           Description = "Through this Api you can access BookInfo",
                //           Contact = new OpenApiContact
                //           {
                //               Email = "arezoo.ebrahimi@gmail.com",
                //               Name = "arezoo ebrahimi",
                //               Url = new Uri("http://www.mizfa.com"),
                //           },
                //           License = new OpenApiLicense
                //           {
                //               Name = "License",
                //               Url = "http://www.mizfa.com",
                //           },
                //       });
            });
        }

        public static void UseSwaggerAndUI(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Api v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "My Api v2");
            });
        }
    }
}
