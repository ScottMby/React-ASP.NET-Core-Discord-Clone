using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using RandomFriendlyNameGenerator;
using System.Security.Claims;

namespace Discord_Clone.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private DiscordCloneDbContext _dbContext {get; set;}
        private UserManager<User> _userManager { get; set; }

        public UserRepository(DiscordCloneDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public void CheckDisplayNameValid(ClaimsPrincipal User)
        {
            User? userEntity = GetUser(User);

            if (userEntity == null)
                return;

            if (string.IsNullOrEmpty(userEntity.DisplayName))
            {
                // Generate Random Display Name
                userEntity.DisplayName = NameGenerator.Identifiers.Get(IdentifierTemplate.SilentBob, NameOrderingStyle.SilentBobStyle);
                _dbContext.SaveChanges();
            }
        }

        public void ChangeDisplayName(ClaimsPrincipal User, string DisplayName)
        {
            User? userEntity = GetUser(User);

            if (userEntity == null)
                return;

            userEntity.DisplayName = DisplayName;
            _dbContext.SaveChanges();
        }

        public void EditFirstName(ClaimsPrincipal User, string FirstName)
        {
            User? userEntity = GetUser(User);

            if (userEntity == null)
                return;

            userEntity.FirstName = FirstName;
            _dbContext.SaveChanges();
        }

        public void EditLastName(ClaimsPrincipal User, string LastName)
        {
            User? userEntity = GetUser(User);

            if (userEntity == null)
                return;

            userEntity.LastName = LastName;
            _dbContext.SaveChanges();
        }

        public void EditAboutMe(ClaimsPrincipal User, string AboutMe)
        {
            User? userEntity = GetUser(User);

            if (userEntity == null)
                return;

            userEntity.AboutMe = AboutMe;
            _dbContext.SaveChanges();
        }

        private User GetUser(ClaimsPrincipal User)
        {
            string? userId = _userManager.GetUserId(User);
            if (userId == null)
                return null;

            var userEntity = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (userEntity != null)
                return userEntity;

            return null;
        }
    }
}
