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
        public void CheckDisplayNameValid(ClaimsPrincipal user)
        {
            string? userId = _userManager.GetUserId(user);
            if (userId == null)
                return;

            var userEntity = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (userEntity == null)
                return;

            if (string.IsNullOrEmpty(userEntity.DisplayName))
            {
                // Generate Random Display Name
                userEntity.DisplayName = NameGenerator.Identifiers.Get(IdentifierTemplate.SilentBob, NameOrderingStyle.SilentBobStyle);
                _dbContext.SaveChanges();
            }
        }

        public void EditFirstName(ClaimsPrincipal User, string FirstName)
        {
            throw new NotImplementedException();
        }

        public void EditLastName(ClaimsPrincipal User, string LastName)
        {
            throw new NotImplementedException();
        }

        public void EditAboutMe(ClaimsPrincipal User, string AboutMe)
        {
            throw new NotImplementedException();
        }
    }
}
