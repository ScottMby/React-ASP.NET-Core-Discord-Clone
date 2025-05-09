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
        private ClaimsPrincipal UserClaimsPrincipal;

        public UserTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            var _context = new DefaultHttpContext();
        }

        [Fact]
        public async Task CheckDisplayName_NullName_CreatesRandomName()
        {
            //Arrange
            ClaimsPrincipal userClaimsPrincipal = UserClaimsPrincipal;
            
            //Act
            await UserEndpoints.CheckDisplayName(UserService, userClaimsPrincipal);
            //Asset
            string displayName = DbContext.Users.Where(u => u.Email == "scott@test.com").First().DisplayName;
            Assert.True(String.IsNullOrEmpty(displayName));
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
