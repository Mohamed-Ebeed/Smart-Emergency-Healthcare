using Microsoft.OpenApi;

namespace SmartEmergency.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddOpenApiWithJwt(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                // Adds the "Authorize" button in Swagger so we can paste the token
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Paste the token you got from login"
                    };

                    document.Security ??= new List<OpenApiSecurityRequirement>();
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
                    });

                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}