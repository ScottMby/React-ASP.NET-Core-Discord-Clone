using Discord_Clone.Server.Endpoints;
using Discord_Clone.Server.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using Microsoft.EntityFrameworkCore;

namespace Discord_Clone.Server.Tests.IntegrationTests
{
    public class UserTests : BaseIntegrationTest
    {
        public UserTests(IntegrationTestWebAppFactory factory) : base(factory)
        {

        }

        [Fact]
        public async Task CheckDisplayName_NullName_CreatesRandomName()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            // Arrange
            await RegisterUser("scott@test.com", "Test123!");
            string? token = await LoginUser("scott@test.com", "Test123!");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Patch, "/api/user/checkdisplayname");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("Cookie", $"{token}");
            }
            var response = await HttpClient.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            string? displayName = DbContext.Users.Where(u => u.Email == "scott@test.com").First().DisplayName;
            Assert.False(string.IsNullOrEmpty(displayName));

            await DbContext.Database.EnsureDeletedAsync();
        }
    }
}
