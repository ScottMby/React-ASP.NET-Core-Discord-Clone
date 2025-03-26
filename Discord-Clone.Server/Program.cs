
using Discord_Clone.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Discord_Clone.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<DiscordCloneDbContext>(options =>
            {
                options.UseNpgsql("Server=172.19.0.2;Port=5432;Database=DiscordCloneDb;User Id=ApplicationUser;Password=ApplicationUserAdminPassword;");
            });

            builder.Services.AddCors(options => options.AddPolicy("default", policy =>
            {
                policy.WithOrigins("https://localhost:4892");
            }));

            builder.Services.AddAuthorization();

            builder.Services.AddIdentityApiEndpoints<IdentityUser>()
                .AddEntityFrameworkStores<DiscordCloneDbContext>();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer { Url = "https://localhost:32769" });
            });

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

            app.UseAuthorization();

            app.UseCors("default");


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.MapIdentityApi<IdentityUser>();

            app.Run();
        }
    }
}
