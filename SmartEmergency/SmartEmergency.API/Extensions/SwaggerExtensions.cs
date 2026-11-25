
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;

namespace SmartEmergency.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddOpenApiWithJwt(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                // Register Bearer Authentication scheme
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new OpenApiComponents();

                    document.Components.SecuritySchemes ??=
                        new Dictionary<string, IOpenApiSecurityScheme>();

                    document.Components.SecuritySchemes["Bearer"] =
                        new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            Description = "Paste the token you got from login"
                        };

                    return Task.CompletedTask;
                });

                // Apply security only to authorized endpoints
                options.AddOperationTransformer((operation, context, cancellationToken) =>
                {
                    var metadata = context.Description.ActionDescriptor.EndpointMetadata;

                    var hasAllowAnonymous = metadata
                        .OfType<IAllowAnonymous>()
                        .Any();

                    var hasAuthorize = metadata
                        .OfType<IAuthorizeData>()
                        .Any();

                    if (hasAllowAnonymous || !hasAuthorize)
                    {
                        return Task.CompletedTask;
                    }

                    operation.Security ??= new List<OpenApiSecurityRequirement>();

                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", context.Document)] =
                            new List<string>()
                    });

                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}
