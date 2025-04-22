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
            catch (Exception exception)
            {
                Logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

                ProblemDetails problemDetails = new()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server Error",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
                };

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
