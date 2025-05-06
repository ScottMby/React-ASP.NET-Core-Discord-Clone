using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using RandomFriendlyNameGenerator;
using System.Security.Claims;
using System.Net;
using Microsoft.EntityFrameworkCore;
using RandomFriendlyNameGenerator.Data;

namespace Discord_Clone.Server.Repositories
{
    public class UserRepository(DiscordCloneDbContext dbContext) : IUserRepository
    {

        private DiscordCloneDbContext DbContext { get; set; } = dbContext;

        public async Task<string?> GetAboutMe(User user)
        {
            return (await DbContext.Users.Where(u => u.Id == user.Id).Select(u => u.AboutMe).FirstOrDefaultAsync());
        }

        public async Task<string?> GetFirstName(User user)
        {
            return (await DbContext.Users.Where(u => u.Id == user.Id).Select(u => u.FirstName).FirstOrDefaultAsync());
        }

        public async Task<string?> GetLastName(User user)
        {
            return (await DbContext.Users.Where(u => u.Id == user.Id).Select(u => u.LastName).FirstOrDefaultAsync());
        }

        public async Task<string?> GetPhotoURL(User user)
        {
            return (await DbContext.Users.Where(u => u.Id == user.Id).Select(u => u.PhotoURL).FirstOrDefaultAsync());
        }

        public async Task<User?> GetUserById(string userId)
        {
            return (await DbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync())!;
        }

        public async Task<string?> GetUserDisplayName(User user)
        {
            return (await DbContext.Users.Where(u => u.Id == user.Id).Select(u => u.DisplayName).FirstOrDefaultAsync());
        }

        public async Task SetAboutMe(User user, string aboutMe)
        {
           await DbContext.Users.Where(u => u.Id == user.Id).ExecuteUpdateAsync(u => u.SetProperty(u => u.AboutMe, aboutMe));
        }

        public async Task SetFirstName(User user, string firstName)
        {
            await DbContext.Users.Where(u => u.Id == user.Id).ExecuteUpdateAsync(u => u.SetProperty(u => u.FirstName, firstName));
        }

        public async Task SetLastName(User user, string lastName)
        {
            await DbContext.Users.Where(u => u.Id == user.Id).ExecuteUpdateAsync(u => u.SetProperty(u => u.LastName, lastName));
        }

        public async Task SetPhotoURL(User user, string photoURL)
        {
            await DbContext.Users.Where(u => u.Id == user.Id).ExecuteUpdateAsync(u => u.SetProperty(u => u.PhotoURL, photoURL));
        }

        public async Task SetUserDisplayName(User user, string displayName)
        {
            await DbContext.Users.Where(u => u.Id == user.Id).ExecuteUpdateAsync(u => u.SetProperty(u => u.DisplayName, displayName));
        }
    }
}
