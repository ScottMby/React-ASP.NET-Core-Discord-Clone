using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using RandomFriendlyNameGenerator;
using System.Security.Claims;
using System.Web.Http;
using System.Net;

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
        /// <summary>
        /// Checks that the display name of a user has been set. If not, sets the display name as a random name.
        /// </summary>
        /// <param name="User">The user whose display name you want to check.</param>
        public void CheckDisplayNameValid(ClaimsPrincipal User)
        {
            User? userEntity = GetUser(User);

            if (string.IsNullOrEmpty(userEntity.DisplayName))
            {
                // Generate Random Display Name
                userEntity.DisplayName = NameGenerator.Identifiers.Get(IdentifierTemplate.SilentBob, NameOrderingStyle.SilentBobStyle);
                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Changes the display name of a user.
        /// </summary>
        /// <param name="User">The user whose display name to edit.</param>
        /// <param name="DisplayName">The display name of the user.</param>
        public void ChangeDisplayName(ClaimsPrincipal User, string DisplayName)
        {
            User? userEntity = GetUser(User);

            userEntity.DisplayName = DisplayName;
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds or changes the first name of a user.
        /// </summary>
        /// <param name="User">The user whose first name to edit.</param>
        /// <param name="FirstName">The first name of the user.</param>
        public void EditFirstName(ClaimsPrincipal User, string FirstName)
        {
            User? userEntity = GetUser(User);

            userEntity.FirstName = FirstName;
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds or changes the last name of a user.
        /// </summary>
        /// <param name="User">The user whose last name to edit.</param>
        /// <param name="LastName">The last name of the user.</param>
        public void EditLastName(ClaimsPrincipal User, string LastName)
        {
            User? userEntity = GetUser(User);

            userEntity.LastName = LastName;
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds or changes the about section of a user.
        /// </summary>
        /// <param name="User">The user whose about me section to edit.</param>
        /// <param name="AboutMe">The about me section text of a user.</param>
        public void EditAboutMe(ClaimsPrincipal User, string AboutMe)
        {
            User? userEntity = GetUser(User);

            userEntity.AboutMe = AboutMe;
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Stores uploaded user images.
        /// </summary>
        /// <param name="User">The user whose image to store.</param>
        /// <param name="File">The file to store.</param>
        public void StoreUserImage(ClaimsPrincipal User, IFormFile File)
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

                string? oldURL = _dbContext.Users.Where(i => i.Id == userEntity.Id).First().PhotoURL;

                _dbContext.Users.Where(i => i.Id == userEntity.Id).First().PhotoURL = filePath;

                if(!string.IsNullOrEmpty(oldURL))
                {
                    System.IO.File.Delete(oldURL);
                }
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Gets the user from the database.
        /// </summary>
        /// <param name="User">The claims principle of the user.</param>
        /// <returns>The user.</returns>
        private User GetUser(ClaimsPrincipal User)
        {
            string? userId = _userManager.GetUserId(User);
            if (userId == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var userEntity = _dbContext.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (userEntity != null)
                return userEntity;

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }


    }
}
