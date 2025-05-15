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
using System.Net;

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
            if (!String.IsNullOrEmpty(token))
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

        [Fact]
        public async Task CheckDisplayName_NotNullName_DoesntChangeName()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            // Arrange
            await RegisterUser("scott@test.com", "Test123!");
            string? token = await LoginUser("scott@test.com", "Test123!");

            DbContext.Users.Where(u => u.Email == "scott@test.com").First().DisplayName = "test";

            // Act
            var request = new HttpRequestMessage(HttpMethod.Patch, "/api/user/checkdisplayname");
            if (!String.IsNullOrEmpty(token))
            {
                request.Headers.Add("Cookie", $"{token}");
            }
            var response = await HttpClient.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            string? displayName = DbContext.Users.Where(u => u.Email == "scott@test.com").First().DisplayName;
            Assert.Equal("test", displayName);
            await DbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task ChangeDisplayName_SetNewName()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            // Arrange
            await RegisterUser("scott@test.com", "Test123!");
            string? token = await LoginUser("scott@test.com", "Test123!");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Patch, "/api/user/changedisplayname");
            if(!String.IsNullOrEmpty(token))
            {
                request.Headers.Add("Cookie", $"{token}");
            }
            request.Content = new StringContent($"\"testName\"", Encoding.UTF8, "application/json");

            var response = await HttpClient.SendAsync(request);

            string? displayName = DbContext.Users.Where(u => u.Email == "scott@test.com").First().DisplayName;

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("testName", displayName);

            await DbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task ChangeFirstName_SetNewName()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            // Arrange
            await RegisterUser("scott@test.com", "Test123!");
            string? token = await LoginUser("scott@test.com", "Test123!");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Patch, "api/user/changefirstname");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("Cookie", $"{token}");
            }
            request.Content = new StringContent($"\"testName\"", Encoding.UTF8, "application/json");

            var response = await HttpClient.SendAsync(request);

            string? firstName = DbContext.Users.Where(u => u.Email == "scott@test.com").First().FirstName;

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("testName", firstName);

            await DbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task ChangeLastName_SetNewName()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            // Arrange
            await RegisterUser("scott@test.com", "Test123!");
            string? token = await LoginUser("scott@test.com", "Test123!");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Patch, "api/user/changelastname");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("Cookie", $"{token}");
            }
            request.Content = new StringContent($"\"testName\"", Encoding.UTF8, "application/json");

            var response = await HttpClient.SendAsync(request);

            string? lastName = DbContext.Users.Where(u => u.Email == "scott@test.com").First().LastName;

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("testName", lastName);

            await DbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task ChangeAboutMe_SetNewAboutMeSection()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            // Arrange
            await RegisterUser("scott@test.com", "Test123!");
            string? token = await LoginUser("scott@test.com", "Test123!");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Patch, "api/user/changeaboutme");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("Cookie", $"{token}");
            }
            request.Content = new StringContent($"\"This is a long about me section to test whether this works.\"", Encoding.UTF8, "application/json");

            var response = await HttpClient.SendAsync(request);

            string? aboutMe = DbContext.Users.Where(u => u.Email == "scott@test.com").First().AboutMe;

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("This is a long about me section to test whether this works.", aboutMe);

            await DbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task ChangePhoto_UploadsPhoto()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            // Arrange
            await RegisterUser("scott@test.com", "Test123!");
            string? token = await LoginUser("scott@test.com", "Test123!");

            //Add an old url to be replaced
            DbContext.Users.Where(u => u.Email == "scott@test.com").First().PhotoURL = "test.png";

            //Get an actual image
            var imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Images", "TestImage.png");
            using var fileStream = File.OpenRead(imagePath);
            var imageContent = new StreamContent(fileStream);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");

            var form = new MultipartFormDataContent();
            

            //Get antiforgery token
            string antiforgeryToken = await GetAntiforgeryToken(token);

            //Add the image to the mutipart/form-data body
            form.Add(imageContent, "file", "TestImage.png");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Patch, "api/user/changephoto");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add("Cookie", $"{token}");
                request.Headers.Add("X-XSRF-TOKEN", $"{antiforgeryToken}");
            }
            request.Content = form;

            var response = await HttpClient.SendAsync(request);

            // Reload the user from the database to get the latest value
            await DbContext.Entry(DbContext.Users.Where(u => u.Email == "scott@test.com").First()).ReloadAsync();
            string? photoUrl = DbContext.Users.Where(u => u.Email == "scott@test.com").First().PhotoURL;

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.False(string.IsNullOrEmpty(photoUrl));
            Assert.NotEqual("test.png", photoUrl);

            await DbContext.Database.EnsureDeletedAsync();
        }
    }
}
