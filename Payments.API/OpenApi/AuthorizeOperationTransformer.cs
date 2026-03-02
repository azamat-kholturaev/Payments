using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Payments.API.OpenApi
{
    internal sealed class AuthorizeOperationTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var hasAuthorize =
                context.Description.ActionDescriptor.EndpointMetadata
                    .OfType<AuthorizeAttribute>()
                    .Any();

            if (!hasAuthorize)
                return Task.CompletedTask;

            operation.Security ??= new List<OpenApiSecurityRequirement>();

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
            });

            return Task.CompletedTask;
        }
    }
}
