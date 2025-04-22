
using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Repositories;
using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Discord_Clone.Server.Middleware;

namespace Discord_Clone.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("DiscordClone"))
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation();
                    metrics.AddHttpClientInstrumentation();
                    metrics.AddOtlpExporter(a =>
                    {
                        a.Endpoint = new Uri("http://host.docker.internal:5341/ingest/otlp/v1/metrics");
                        a.Protocol = OtlpExportProtocol.HttpProtobuf;
                        a.Headers = "X-Seq-ApiKey=AmodI5OpiGtkUTTgR5kG";
                    });
                })
                .WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation();
                    tracing.AddHttpClientInstrumentation();
                    tracing.AddEntityFrameworkCoreInstrumentation();

                    tracing.AddOtlpExporter(a =>
                    {
                        a.Endpoint = new Uri("http://host.docker.internal:5341/ingest/otlp/v1/traces");
                        a.Protocol = OtlpExportProtocol.HttpProtobuf;
                        a.Headers = "X-Seq-ApiKey=AmodI5OpiGtkUTTgR5kG";
                    });
                });

            builder.Logging.AddOpenTelemetry(logging =>
            {

                logging.AddOtlpExporter(a =>
                {
                    a.Endpoint = new Uri("http://host.docker.internal:5341/ingest/otlp/v1/logs");
                    a.Protocol = OtlpExportProtocol.HttpProtobuf;
                    a.Headers = "X-Seq-ApiKey=AmodI5OpiGtkUTTgR5kG";
                });
            });

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<DiscordCloneDbContext>(options =>
            {
                options.UseNpgsql("Server=host.docker.internal;Port=5432;Database=DiscordCloneDb;User Id=ApplicationUser;Password=ApplicationUserAdminPassword;");
            });

            builder.Services.AddCors(options => options.AddPolicy("default", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }));

            builder.Services.AddAuthorization();

            builder.Services.AddIdentityApiEndpoints<User>()
                .AddEntityFrameworkStores<DiscordCloneDbContext>();

            builder.Services.AddSwaggerGen(options =>
            {

            });

            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IUserFriendsRepository, UserFriendsRepository>();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.MapStaticAssets();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<HttpExceptionHandlingMiddleware>();

            app.UseAuthorization();

            app.UseCors("default");


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.MapIdentityApi<User>();

            app.Run();
        }
    }
}
