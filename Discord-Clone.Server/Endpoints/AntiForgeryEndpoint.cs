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

        public static Results<Ok, BadRequest> Token(IAntiforgery antiForgeryService, HttpContext httpContext)
        {
            var token = antiForgeryService.GetAndStoreTokens(httpContext);
            httpContext.Response.Cookies.Append("X-XSRF-TOKEN", token.RequestToken!, new CookieOptions { HttpOnly = false });
            return TypedResults.Ok();
        }
    }
}
