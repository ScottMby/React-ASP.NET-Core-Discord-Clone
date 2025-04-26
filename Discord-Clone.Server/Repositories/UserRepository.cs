using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using RandomFriendlyNameGenerator;
using System.Security.Claims;
using System.Net;

namespace Discord_Clone.Server.Repositories
{
    public class UserRepository(DiscordCloneDbContext dbContext, UserManager<User> userManager, ILogger<Program> logger) : IUserRepository
    {
        private DiscordCloneDbContext DbContext { get; set; } = dbContext;
        private UserManager<User> UserManager { get; set; } = userManager;

        private ILogger<Program> Logger { get; set; } = logger;

        /// <summary>
        /// Checks that the display name of a user has been set. If not, sets the display name as a random name.
        /// </summary>
        /// <param name="User">The user whose display name you want to check.</param>
        public async Task CheckDisplayNameValid(ClaimsPrincipal User)
        {
            User? userEntity = GetUser(User);

            if (string.IsNullOrEmpty(userEntity.DisplayName))
            {
                // Generate Random Display Name
                userEntity.DisplayName = NameGenerator.Identifiers.Get(IdentifierTemplate.SilentBob, NameOrderingStyle.SilentBobStyle);
                await DbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Changes the display name of a user.
        /// </summary>
        /// <param name="User">The user whose display name to edit.</param>
        /// <param name="DisplayName">The display name of the user.</param>
        public async Task ChangeDisplayName(ClaimsPrincipal User, string DisplayName)
        {
            User? userEntity = GetUser(User);

            userEntity.DisplayName = DisplayName;
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Adds or changes the first name of a user.
        /// </summary>
        /// <param name="User">The user whose first name to edit.</param>
        /// <param name="FirstName">The first name of the user.</param>
        public async Task EditFirstName(ClaimsPrincipal User, string FirstName)
        {
            User? userEntity = GetUser(User);

            userEntity.FirstName = FirstName;
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Adds or changes the last name of a user.
        /// </summary>
        /// <param name="User">The user whose last name to edit.</param>
        /// <param name="LastName">The last name of the user.</param>
        public async Task EditLastName(ClaimsPrincipal User, string LastName)
        {
            User? userEntity = GetUser(User);

            userEntity.LastName = LastName;
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Adds or changes the about section of a user.
        /// </summary>
        /// <param name="User">The user whose about me section to edit.</param>
        /// <param name="AboutMe">The about me section text of a user.</param>
        public async Task EditAboutMe(ClaimsPrincipal User, string AboutMe)
        {
            User? userEntity = GetUser(User);

            userEntity.AboutMe = AboutMe;
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Stores uploaded user images.
        /// </summary>
        /// <param name="User">The user whose image to store.</param>
        /// <param name="File">The file to store.</param>
        public async Task StoreUserImage(ClaimsPrincipal User, IFormFile File)
        {
            User? userEntity = GetUser(User);

            string filePath = "";

            ///!!!Important: Do not use original file name without validation & sanitization... this could lead to security issues.
            if (File.Length > 0)
            {
                //In temporary folder. For production builds use blob storage.
                filePath = Path.Combine("/userImages", Path.GetTempFileName());

                using (var stream = System.IO.File.Create(filePath))
                {
                    File.CopyTo(stream);
                }

                string? oldURL = DbContext.Users.Where(i => i.Id == userEntity.Id).First().PhotoURL;

                DbContext.Users.Where(i => i.Id == userEntity.Id).First().PhotoURL = filePath;

                if(!string.IsNullOrEmpty(oldURL))
                {
                    System.IO.File.Delete(oldURL);
                }
            }
            else
            {
                throw new Exception("File length is smaller than a byte.");
            }
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
