using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Discord_Clone.Server.Endpoints
{
    public static class AntiForgeryEndpoint
    {

        public static void MapAntiForgeryEndpoints(this IEndpointRouteBuilder app)
        {
            var apiGroup = app.MapGroup("api");

            var antiForgeryGroup = apiGroup.MapGroup("antiforgery")
                .RequireAuthorization();

            antiForgeryGroup.MapGet("token", Token);
        }

        public static Results<Ok<string>, BadRequest> Token(IAntiforgery antiForgeryService, HttpContext httpContext)
        {
            var tokens = antiForgeryService.GetAndStoreTokens(httpContext);
            var xsrfToken = tokens.RequestToken!;
            return TypedResults.Ok(xsrfToken);
        }
    }
}
