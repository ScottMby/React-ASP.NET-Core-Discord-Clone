using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Models.Data_Transfer_Objects;
using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System;
using System.Reflection;
using System.Security.Claims;

namespace Discord_Clone.Server.Repositories
{
    public class UserFriendsRepository(DiscordCloneDbContext dbContext) : IUserFriendsRepository
    {
        private readonly DiscordCloneDbContext DbContext = dbContext;

        public async Task AddUserFriend(UserFriends userFriends)
        {
            await DbContext.UserFriends.AddAsync(userFriends);
            await DbContext.SaveChangesAsync();
        }

        public async Task AddUserFriendRequest(UserFriendRequests userFriendRequests)
        {
            await DbContext.UserFriendRequests.AddAsync(userFriendRequests);
            await DbContext.SaveChangesAsync();
        }

        public async Task<bool> CheckUserHasPendingFriendRequest(User sender, string receiverId)
        {
            return (await DbContext.UserFriendRequests.AnyAsync(ufr => ufr.SenderId == sender.Id && ufr.ReceiverId == receiverId
            || ufr.SenderId == receiverId && ufr.ReceiverId == sender.Id));
        }

        public async Task<bool> CheckUserIsFriends(User sender, string receiverId)
        {
            return (await DbContext.UserFriends.AnyAsync(uf => uf.SenderId == sender.Id && uf.ReceiverId == receiverId
            || uf.SenderId == receiverId && uf.ReceiverId == sender.Id));
        }

        public async Task DeleteFriendRequest(string friendRequestId)
        {
            DbContext.UserFriendRequests.Remove(await GetFriendRequest(friendRequestId));
            await DbContext.SaveChangesAsync();
        }

        public async Task<UserFriendRequests> GetFriendRequest(string friendRequestId)
        {
            return (await DbContext.UserFriendRequests.Where(ufr => ufr.FriendRequestId == friendRequestId).FirstAsync());
        }

        public async Task<List<UserFriendRequests>> GetUserFriendRequests(User user)
        {
            return (await DbContext.UserFriendRequests.Where(ufr => ufr.SenderId == user.Id || ufr.ReceiverId == user.Id).ToListAsync());
        }

        public async Task<List<UserSearchResult>> UserSearch(string searchTerm)
        {
            List<UserSearchResult> Users = await DbContext.Users
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
    }
}
