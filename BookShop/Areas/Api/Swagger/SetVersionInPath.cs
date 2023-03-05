using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace BookShop.Areas.Api.Swagger
{
    public class SetVersionInPaths : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            //var updatedPaths = new Dictionary<string, OpenApiPathItem>();
            var updatedPaths = new OpenApiPaths();

            foreach (var entry in swaggerDoc.Paths)
            {
                updatedPaths.Add(
                    entry.Key.Replace("v{version}", swaggerDoc.Info.Version),
                    entry.Value);
            }

            swaggerDoc.Paths = (OpenApiPaths)updatedPaths;
        }
    }
}
