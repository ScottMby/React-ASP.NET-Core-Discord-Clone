using Discord_Clone.Server.Endpoints;
using Discord_Clone.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Clone.Server.Tests.IntegrationTests
{
    public class UserFriendsTests : BaseIntegrationTest
    {
        private ClaimsPrincipal SenderClaimsPrincipal;
        private ClaimsPrincipal ReceiverClaimsPrincipal;

        public UserFriendsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            var _context = new DefaultHttpContext();

        }

        [Fact]
        public async Task SendFriendRequest_Decline()
        {
            await CreateAuthenticatedUsers();
            
            await UserFriendsEndpoints.SendFriendRequest(UserFriendsService, SenderClaimsPrincipal, "receiverId");

            UserFriendRequests? request = await DbContext.UserFriendRequests.Where(ufr => ufr.SenderId == "senderId" && ufr.ReceiverId == "receiverId").FirstOrDefaultAsync();

            Assert.NotNull(request);

            await UserFriendsEndpoints.DeclineFriendRequest(UserFriendsService, ReceiverClaimsPrincipal, request.FriendRequestId);

            Assert.False(await DbContext.UserFriendRequests.AnyAsync(ufr => ufr.SenderId == "senderId" && ufr.ReceiverId == "receiverId"));

            UserFriends? userFriends = await DbContext.UserFriends.Where(uf => uf.SenderId == "senderId" && uf.ReceiverId == "receiverId").FirstOrDefaultAsync();

            Assert.Null(userFriends);
        }

        private async Task CreateAuthenticatedUsers()
        {
            User sender = new() { UserName = "sender", Email = "scott@test.com", Id="senderId" };
            User receiver = new() { UserName = "receiver", Email = "abby@test.com", Id="receiverId" };
            await UserManager.CreateAsync(sender);
            await UserManager.CreateAsync(receiver);
            ClaimsPrincipal senderClaimsPrincipal = await SignInManager.CreateUserPrincipalAsync(sender);
            ClaimsPrincipal receiverClaimsPrincipal = await SignInManager.CreateUserPrincipalAsync(receiver);
            SenderClaimsPrincipal = senderClaimsPrincipal;
            ReceiverClaimsPrincipal = receiverClaimsPrincipal;
        }
    }
}
