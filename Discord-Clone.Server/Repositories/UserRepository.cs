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
            DbContext.Users.Where(u => u.Id == user.Id).First().AboutMe = aboutMe;
            await DbContext.SaveChangesAsync();
        }

        public async Task SetFirstName(User user, string firstName)
        {
            DbContext.Users.Where(u => u.Id == user.Id).First().FirstName = firstName;
            await DbContext.SaveChangesAsync();
        }

        public async Task SetLastName(User user, string lastName)
        {
            DbContext.Users.Where(u => u.Id == user.Id).First().LastName = lastName;
            await DbContext.SaveChangesAsync();
        }

        public async Task SetPhotoURL(User user, string photoURL)
        {
            DbContext.Users.Where(u => u.Id == user.Id).First().PhotoURL = photoURL;
            await DbContext.SaveChangesAsync();
        }

        public async Task SetUserDisplayName(User user, string displayName)
        {
            DbContext.Users.Where(u => u.Id == user.Id).First().DisplayName = displayName;
            await DbContext.SaveChangesAsync();
        }
    }
}
