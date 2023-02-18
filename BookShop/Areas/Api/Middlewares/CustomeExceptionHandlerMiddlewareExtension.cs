using BookShop.Areas.Api.Classes;
using BookShop.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BookShop.Areas.Admin.Middlewares
{
    public static class CustomeExceptionHandlerMiddlewareExtension
    {
        public static IApplicationBuilder UseCustomeExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomeExceptionHandlerMiddleware>();
        }
    }
    public class CustomeExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _env;
        public CustomeExceptionHandlerMiddleware(RequestDelegate next, IHostingEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            List<string> Message = new List<string>();
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            ApiResultStatusCode apiResultStatus = ApiResultStatusCode.ServerError;

            try
            {
                await _next(context);
            }
            //Two catch here-If the son(AppException) catch the error it execute the next, but if son cannot catch the error, error goes to the father(Exception)
            catch (AppException exception)
            {
                httpStatusCode = exception.HttpStatusCode;
                apiResultStatus = exception.ApiStatusCode;

                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace,
                    };
                    if (exception.InnerException != null)
                    {
                        dic.Add("InnerException.Exception", exception.InnerException.Message);
                        dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
                    }
                    if (exception.AdditionalData != null)
                        dic.Add("AdditionalData", JsonConvert.SerializeObject(exception.AdditionalData));

                    Message.Add(JsonConvert.SerializeObject(dic));
                }
                else
                {
                    Message.Add("خطایی رخ داده است.");
                }
                await WriteToResponseAsync();
            }
            catch(Exception exception)
            {
                if (_env.IsDevelopment())
                {
                    var error = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace,
                    };
                    Message.Add(JsonConvert.SerializeObject(error));
                }
                else
                {
                    Message.Add("خطایی رخ داده است");
                }
                await WriteToResponseAsync();
            }

            async Task WriteToResponseAsync()
            {
                var result = new ApiResult(false, apiResultStatus, Message);
                var jsonResult = JsonConvert.SerializeObject(result);

                context.Response.StatusCode = (int)httpStatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(jsonResult);
            }
        }
    }
}
