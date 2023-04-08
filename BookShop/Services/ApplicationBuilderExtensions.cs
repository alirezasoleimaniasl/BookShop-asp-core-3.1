using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace BookShop.Services
{
    public static class ApplicationBuilderExtensions
    {
        //Adding a middlewear that include all files in /node_modules
        public static IApplicationBuilder UseNodeModules(this IApplicationBuilder app, string rootPath)
        {
            //Combine the rootpath to the node_modules to get the absolute path
            var path = Path.Combine(rootPath, "node_modules");
            var fileProvider = new PhysicalFileProvider(path);
            var options = new StaticFileOptions();
            options.RequestPath = "/node_modules";
            options.FileProvider = fileProvider;
            app.UseStaticFiles(options);
            return app;
        }
    }
}
