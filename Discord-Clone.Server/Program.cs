
using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Repositories;
using Discord_Clone.Server.Repositories.Interfaces;
using Discord_Clone.Server.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

            app.MapIdentityApi<User>();

            app.Run();
        }
    }
}
