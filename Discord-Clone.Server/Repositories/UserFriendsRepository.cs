using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Models.Data_Transfer_Objects;
using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Discord_Clone.Server.Repositories
{
    public class UserFriendsRepository(DiscordCloneDbContext dbContext, ILogger<Program> logger, UserManager<User> userManager) : IUserFriendsRepository
    {
        private readonly DiscordCloneDbContext DbContext = dbContext;

        private UserManager<User> UserManager { get; set; } = userManager;

        private readonly ILogger<Program> Logger = logger;

        public async Task<List<UserSearchResult>> UserSearch(string searchTerm)
        {
            var Users = await DbContext.Users
                .Where(u =>
                    u.UserSearchVector != null && u.UserSearchVector.Matches(EF.Functions.PlainToTsQuery("english", searchTerm)))
                .Select(u => new UserSearchResult
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName!,
                    PhotoURL = u.PhotoURL!,
                    Rank = u.UserSearchVector!.Rank(EF.Functions.PlainToTsQuery(searchTerm))
                })
                .OrderByDescending(u => u.Rank)
                .ToListAsync();

            return Users;
        }

        public async Task UserFriendRequest(ClaimsPrincipal sender, string receiverId)
        {
            User sendingUser = GetUser(sender);

            var receivingUser = DbContext.Users.Where(u => u.Id == receiverId).FirstOrDefault() ?? throw new Exception("Could Not Find Receiving User");

            if (DbContext.UserFriendRequests.Any(ufr => ufr.SenderId == sendingUser.Id && ufr.ReceiverId == receivingUser.Id
                    || ufr.SenderId == receivingUser.Id && ufr.ReceiverId == sendingUser.Id)
                || DbContext.UserFriends.Any(uf => uf.SenderId == sendingUser.Id && uf.ReceiverId == receivingUser.Id
                    || uf.SenderId == receivingUser.Id && uf.ReceiverId == sendingUser.Id))
            {
                throw new Exception("Friend Request Already Exists or The User's are already friends.");
            }

            DbContext.UserFriendRequests.Add(new UserFriendRequests
            {
                SenderId = sendingUser.Id,
                ReceiverId = receivingUser.Id,
                Sender = sendingUser,
                Receiver = receivingUser
            });

            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the user from the database.
        /// </summary>
        /// <param name="User">The claims principle of the user.</param>
        /// <returns>The user.</returns>
        private User GetUser(ClaimsPrincipal User)
        {
            string? userId = UserManager.GetUserId(User) ?? throw new Exception("User's id could not be found. User: " + User);
            var userEntity = DbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (userEntity != null)
                return userEntity;

            throw new Exception("Found user ID {userId} but could not find user." + userId);
        }
    }
}
