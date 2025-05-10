using Discord_Clone.Server.Endpoints;
using Discord_Clone.Server.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Clone.Server.Tests.IntegrationTests
{
    public class UserTests : BaseIntegrationTest
    {
        private ClaimsPrincipal UserClaimsPrincipal = null!;

        public UserTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _ = new DefaultHttpContext();
        }

        [Fact]
        public async Task CheckDisplayName_NullName_CreatesRandomName()
        {
            //Arrange
            await CreateAuthenticatedUser();
            ClaimsPrincipal userClaimsPrincipal = UserClaimsPrincipal;

            //Act
            await UserEndpoints.CheckDisplayName(UserService, userClaimsPrincipal);
            //Asset
            string? displayName = DbContext.Users.Where(u => u.Email == "scott@test.com").First().DisplayName;
            Assert.False(String.IsNullOrEmpty(displayName));
        }

        private async Task CreateAuthenticatedUser()
        {
            User user = new() { UserName = "test", Email = "scott@test.com" };
            await UserManager.CreateAsync(user);
            ClaimsPrincipal userClaimsPrincipal = await SignInManager.CreateUserPrincipalAsync(user);
            UserClaimsPrincipal = userClaimsPrincipal;
        }
    }
}
