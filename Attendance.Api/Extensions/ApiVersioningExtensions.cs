using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;


namespace Attendance.Api.Extensions
{
    public static class ApiVersioningExtensions
    {
        public static IServiceCollection AddApiVersionedSwagger(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // Metadata is extracted from the compiled assembly.
                // Change product name and description in .csproj
                var assemblyProduct = typeof(Startup).Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
                var assemblyDescription = typeof(Startup).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName,
                    new OpenApiInfo
                    {
                        Title = $"{assemblyProduct} {description.ApiVersion.ToString()}",
                        Version = description.ApiVersion.ToString(),
                        Description = assemblyDescription
                    });
                }

                //TODO: ADD Authentication

                // Add generated xmldoc
                // path to this file is configured in .csproj
                var documentationPath = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{typeof(Startup).GetTypeInfo().Assembly.GetName().Name}.xml");

                options.IncludeXmlComments(documentationPath);
            });

            return services;


        }

    }
}