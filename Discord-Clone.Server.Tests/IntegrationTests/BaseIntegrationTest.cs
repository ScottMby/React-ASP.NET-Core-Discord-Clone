using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Clone.Server.Tests.IntegrationTests
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly IServiceScope _scope;
        protected readonly DiscordCloneDbContext DbContext;
        protected readonly UserManager<User> UserManager;
        protected readonly UserService UserService;
        protected readonly UserFriendsService UserFriendsService;
        protected readonly SignInManager<User> SignInManager;
        protected readonly HttpClient HttpClient;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();
            DbContext = _scope.ServiceProvider.GetRequiredService<DiscordCloneDbContext>();
            DbContext.Database.Migrate();
            UserService = _scope.ServiceProvider.GetRequiredService<UserService>();
            UserFriendsService = _scope.ServiceProvider.GetRequiredService<UserFriendsService>();
            UserManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            SignInManager = _scope.ServiceProvider.GetRequiredService<SignInManager<User>>();
            HttpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {

            });
            _ = new DefaultHttpContext();
        }

        internal async Task RegisterUser(string email, string password)
        {
            var registerRequest = new HttpRequestMessage(HttpMethod.Post, "/register")
            {
                Content = new StringContent(
                    $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"confirmPassword\":\"{password}\"}}",
                    Encoding.UTF8,
                    "application/json")
            };
            var registerResponse = await HttpClient.SendAsync(registerRequest);
            registerResponse.EnsureSuccessStatusCode();
        }

        internal async Task<string> LoginUser(string email, string password)
        {
            var signInRequest = new HttpRequestMessage(HttpMethod.Post, "/login?useCookies=true")
            {
                Content = new StringContent(
                    $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}",
                    Encoding.UTF8,
                    "application/json")
            };
            var signInResponse = await HttpClient.SendAsync(signInRequest);
            signInResponse.EnsureSuccessStatusCode();

            if (signInResponse.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    if (cookie.StartsWith(".AspNetCore.Identity.Application="))
                    {
                        return cookie;
                    }
                }
            }
            throw new Exception("Couldn't log in");
        }

        internal async Task<string> GetAntiforgeryToken(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/antiforgery/token");
            if (!String.IsNullOrEmpty(token))
            {
                request.Headers.Add("Cookie", token);
            }
            var response = await HttpClient.SendAsync(request);

            string result = response.Headers.Where(h => h.Key == "Set-Cookie").First().Value.Where(v => v.Contains("X-XSRF-TOKEN")).FirstOrDefault() ?? throw new Exception("Could not get antiforgery token");
            result = result.Split('=')[1].Split(';')[0];
            return result;
        }
    }
}
