using Discord_Clone.Server.Endpoints;
using Discord_Clone.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Clone.Server.Tests.IntegrationTests
{
    public class UserFriendsTests : BaseIntegrationTest
    {

        public UserFriendsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            var _context = new DefaultHttpContext();
        }

        [Fact]
        public async Task SendFriendRequest_Decline()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            //Arrange
            await RegisterUser("scott@test.com", "Test123!");
            await RegisterUser("abby@test.com", "Test123!");
            string? senderToken = await LoginUser("scott@test.com", "Test123!");

            User sender = DbContext.Users.Where(u => u.Email == "scott@test.com").First();
            User receiver = DbContext.Users.Where(u => u.Email == "abby@test.com").First();

            await SendFriendRequest(receiver.Id, senderToken);

            string friendRequestId = DbContext.UserFriendRequests.Where(ufr => ufr.SenderId == sender.Id && ufr.ReceiverId == receiver.Id).First().FriendRequestId;

            string? receiverToken = await LoginUser("abby@test.com", "Test123!");

            //Act
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/user/declinefriendrequest");
            if(!String.IsNullOrEmpty(receiverToken))
            {
                request.Headers.Add("Cookie", receiverToken);
            }
            request.Content = new StringContent($"\"{friendRequestId}\"", Encoding.UTF8, "application/json");
            var response = await HttpClient.SendAsync(request);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(DbContext.UserFriendRequests.Where(ufr => ufr.SenderId == sender.Id && ufr.ReceiverId == receiver.Id).FirstOrDefault());

            await DbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task SendFriendRequest_Decline_BadRequestIfUserIsSender()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            //Arrange
            await RegisterUser("scott@test.com", "Test123!");
            await RegisterUser("abby@test.com", "Test123!");
            string? token = await LoginUser("scott@test.com", "Test123!");

            User sender = DbContext.Users.Where(u => u.Email == "scott@test.com").First();
            User receiver = DbContext.Users.Where(u => u.Email == "abby@test.com").First();

            await SendFriendRequest(receiver.Id, token);

            string friendRequestId = DbContext.UserFriendRequests.Where(ufr => ufr.SenderId == sender.Id && ufr.ReceiverId == receiver.Id).First().FriendRequestId;

            //Act
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/user/declinefriendrequest");
            if (!String.IsNullOrEmpty(token))
            {
                request.Headers.Add("Cookie", token);
            }
            request.Content = new StringContent($"\"{friendRequestId}\"", Encoding.UTF8, "application/json");
            var response = await HttpClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

            await DbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task SendFriendRequest_Accept()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            //Arrange
            await RegisterUser("scott@test.com", "Test123!");
            await RegisterUser("abby@test.com", "Test123!");
            string? senderToken = await LoginUser("scott@test.com", "Test123!");

            User sender = DbContext.Users.Where(u => u.Email == "scott@test.com").First();
            User receiver = DbContext.Users.Where(u => u.Email == "abby@test.com").First();

            await SendFriendRequest(receiver.Id, senderToken);

            string friendRequestId = DbContext.UserFriendRequests.Where(ufr => ufr.SenderId == sender.Id && ufr.ReceiverId == receiver.Id).First().FriendRequestId;

            string? receiverToken = await LoginUser("abby@test.com", "Test123!");

            //Act
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/user/acceptfriendrequest");
            if (!String.IsNullOrEmpty(receiverToken))
            {
                request.Headers.Add("Cookie", receiverToken);
            }
            request.Content = new StringContent($"\"{friendRequestId}\"", Encoding.UTF8, "application/json");
            var response = await HttpClient.SendAsync(request);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(DbContext.UserFriendRequests.Where(ufr => ufr.SenderId == sender.Id && ufr.ReceiverId == receiver.Id).FirstOrDefault());
            Assert.NotNull(DbContext.UserFriends.Where(uf => uf.SenderId == sender.Id && uf.ReceiverId == receiver.Id).FirstOrDefault());

            await DbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task SendFriendRequest_Accept__BadRequestIfUserIsSender()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            //Arrange
            await RegisterUser("scott@test.com", "Test123!");
            await RegisterUser("abby@test.com", "Test123!");
            string? senderToken = await LoginUser("scott@test.com", "Test123!");

            User sender = DbContext.Users.Where(u => u.Email == "scott@test.com").First();
            User receiver = DbContext.Users.Where(u => u.Email == "abby@test.com").First();

            await SendFriendRequest(receiver.Id, senderToken);

            string friendRequestId = DbContext.UserFriendRequests.Where(ufr => ufr.SenderId == sender.Id && ufr.ReceiverId == receiver.Id).First().FriendRequestId;

            //Act
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/user/acceptfriendrequest");
            if (!String.IsNullOrEmpty(senderToken))
            {
                request.Headers.Add("Cookie", senderToken);
            }
            request.Content = new StringContent($"\"{friendRequestId}\"", Encoding.UTF8, "application/json");
            var response = await HttpClient.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.NotNull(DbContext.UserFriendRequests.Where(ufr => ufr.SenderId == sender.Id && ufr.ReceiverId == receiver.Id).FirstOrDefault());
            Assert.Null(DbContext.UserFriends.Where(uf => uf.SenderId == sender.Id && uf.ReceiverId == receiver.Id).FirstOrDefault());

            await DbContext.Database.EnsureDeletedAsync();
        }

        private async Task SendFriendRequest(string receiverId, string authorizationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/user/sendfriendrequest");
            // Add a string to the body of the request
            request.Content = new StringContent($"\"{receiverId}\"", Encoding.UTF8, "application/json");

            if (!String.IsNullOrEmpty(authorizationToken))
            {
                request.Headers.Add("Cookie", $"{authorizationToken}");
            }
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
