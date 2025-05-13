using Discord_Clone.Server.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Discord_Clone.Server.Middleware
{
    public class HttpExceptionHandlingMiddleware(RequestDelegate next, ILogger<HttpExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate Next = next;

        private readonly ILogger<HttpExceptionHandlingMiddleware> Logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch (IHttpException exception)
            {
                Logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

                ProblemDetails problemDetails = new()
                {
                    Status = exception.Status,
                    Title = exception.Title,
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-" + exception.Type,
                    Detail = exception.Message
                };

                context.Response.StatusCode = exception.Status;

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
