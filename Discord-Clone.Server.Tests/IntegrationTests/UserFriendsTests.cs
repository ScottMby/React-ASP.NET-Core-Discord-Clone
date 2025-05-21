using Discord_Clone.Server.Endpoints;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Models.Data_Transfer_Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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

        [Fact]
        public async Task GetUserFriends_ReturnsListOfRequests()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            //Arrange
            await RegisterUser("scott@test.com", "Test123!");
            await RegisterUser("abby@test.com", "Test123!");
            await RegisterUser("kylo@test.com", "Test123!");

            User userA = DbContext.Users.Where(u => u.Email == "scott@test.com").First();
            User userB = DbContext.Users.Where(u => u.Email == "abby@test.com").First();
            User userC = DbContext.Users.Where(u => u.Email == "kylo@test.com").First();

            string? tokenA = await LoginUser("scott@test.com", "Test123!");

            await SendFriendRequest(userB.Id, tokenA);
            await SendFriendRequest(userC.Id, tokenA);

            string? tokenB = await LoginUser("abby@test.com", "Test123!");

            await SendFriendRequest(userC.Id, tokenB);

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "api/user/getuserfriendrequests");
            if(!String.IsNullOrEmpty(tokenB))
            {
                request.Headers.Add("Cookie", tokenB);
            }
            var response = await HttpClient.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
           Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var friendRequests = JsonSerializer.Deserialize<List<UserFriendRequests>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(friendRequests);
            Assert.True(friendRequests.Count > 0);

            string userBId = DbContext.Users.Where(u => u.Email == "abby@test.com").First().Id;

            foreach(UserFriendRequests friendRequest in friendRequests)
            {
                Assert.True(friendRequest.SenderId == userBId || friendRequest.ReceiverId == userBId );
            }

            await DbContext.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task GetUserFriends_ReturnsListOfFriends()
        {
            await DbContext.Database.EnsureCreatedAsync();
            await DbContext.Database.MigrateAsync();

            // Arrange
            await RegisterUser("scott@test.com", "Test123!");
            await RegisterUser("abby@test.com", "Test123!");
            await RegisterUser("kylo@test.com", "Test123!");

            User userA = DbContext.Users.Where(u => u.Email == "scott@test.com").First();
            User userB = DbContext.Users.Where(u => u.Email == "abby@test.com").First();
            User userC = DbContext.Users.Where(u => u.Email == "kylo@test.com").First();

            DateTime friendsSince = DateTime.UtcNow;

            await DbContext.UserFriends.AddRangeAsync(new UserFriends[]
            {
                new UserFriends()
                {
                    Sender = userA,
                    SenderId = userA.Id,
                    Receiver = userC,
                    ReceiverId = userC.Id,
                    FriendsSince = friendsSince
                },
                new UserFriends()
                {
                    Sender = userB,
                    SenderId = userB.Id,
                    Receiver = userC,
                    ReceiverId = userC.Id,
                    FriendsSince = friendsSince
                }
            });

            await DbContext.SaveChangesAsync();

            string? token = await LoginUser("kylo@test.com", "Test123!");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "api/user/getuserfriends");
            if (!String.IsNullOrEmpty(token))
            {
                request.Headers.Add("Cookie", token);
            }
            var response = await HttpClient.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();

            List<UserFriendsResult>? results = JsonSerializer.Deserialize<List<UserFriendsResult>>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(results);

            var resultA = results.FirstOrDefault(r => r.Id == userA.Id);
            var resultB = results.FirstOrDefault(r => r.Id == userB.Id);

            Assert.NotNull(resultA);
            Assert.NotNull(resultB);

            Assert.Equal(userA.Id, resultA.Id);
            Assert.Equal(userA.DisplayName, resultA.DisplayName);
            Assert.Equal(userA.AboutMe, resultA.AboutMe);
            Assert.Equal(userA.PhotoURL, resultA.PhotoURL);
            Assert.True((resultA.FriendsSince - friendsSince).Duration() < TimeSpan.FromSeconds(1), "Friends since timestamp variance over 1 second.");

            Assert.Equal(userB.Id, resultB.Id);
            Assert.Equal(userB.DisplayName, resultB.DisplayName);
            Assert.Equal(userB.AboutMe, resultB.AboutMe);
            Assert.Equal(userB.PhotoURL, resultB.PhotoURL);
            Assert.True((resultB.FriendsSince - friendsSince).Duration() < TimeSpan.FromSeconds(1), "Friends since timestamp variance over 1 second.");

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
