using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();
            DbContext = _scope.ServiceProvider.GetRequiredService<DiscordCloneDbContext>();
            DbContext.Database.Migrate();
            UserService = _scope.ServiceProvider.GetRequiredService<UserService>();
            UserFriendsService = _scope.ServiceProvider.GetRequiredService<UserFriendsService>();
            UserManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            SignInManager = _scope.ServiceProvider.GetRequiredService<SignInManager<User>>();
        }
    }
}
