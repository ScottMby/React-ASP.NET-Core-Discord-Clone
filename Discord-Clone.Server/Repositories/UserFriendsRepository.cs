using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Models.Data_Transfer_Objects;
using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Discord_Clone.Server.Repositories
{
    public class UserFriendsRepository(DiscordCloneDbContext dbContext, ILogger<Program> logger) : IUserFriendsRepository
    {
        private readonly DiscordCloneDbContext DbContext = dbContext;

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
    }
}
