using Discord_Clone.Server.Data;
using Discord_Clone.Server.Models;
using Discord_Clone.Server.Repositories;
using Discord_Clone.Server.Repositories.Interfaces;
using Discord_Clone.Server.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RandomFriendlyNameGenerator;
using RandomFriendlyNameGenerator.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Discord_Clone.Server.Services
{
    public class UserService(IUserRepository userRepository, UserManager<User> userManager, ILogger<UserService> logger)
    {
        private IUserRepository UserRepository { get; set; } = userRepository;

        private UserManager<User> UserManager { get; set; } = userManager;

        private ILogger<UserService> Logger { get; set; } = logger;

        /// <summary>
        /// Checks that the display name of a user has been set. If not, sets the display name as a random name.
        /// </summary>
        /// <param name="user">The user whose display name you want to check.</param>
        public async Task CheckDisplayNameValid(ClaimsPrincipal user)
        {
            User userEntity = await GetUser(user);

            if (string.IsNullOrEmpty(userEntity.DisplayName))
            {
                // Generate Random Display Name
                await UserRepository.SetUserDisplayName(userEntity, NameGenerator.Identifiers.Get(IdentifierTemplate.SilentBob, NameOrderingStyle.SilentBobStyle)); //NameGenerator is quite slow. Maybe replace.
            }
        }

        /// <summary>
        /// Changes the display name of a user.
        /// </summary>
        /// <param name="user">The user whose display name to edit.</param>
        /// <param name="displayName">The display name of the user.</param>
        public async Task ChangeDisplayName(ClaimsPrincipal user, string displayName)
        {
            if (displayName.Length > 255)
            {
                throw new BadRequestException("Display name is too long. Please use a shorter display name.");
            }
            if (displayName.Length < 3)
            {
                throw new BadRequestException("Display name is too short. Please use a longer display name.");
            }
            if (String.IsNullOrWhiteSpace(displayName))
            {
                throw new BadRequestException("Display name cannot be just spaces");
            }
            User userEntity = await GetUser(user);

            await UserRepository.SetUserDisplayName(userEntity, displayName);
        }

        /// <summary>
        /// Adds or changes the first name of a user.
        /// </summary>
        /// <param name="user">The user whose first name to edit.</param>
        /// <param name="firstName">The first name of the user.</param>
        
        public async Task EditFirstName(ClaimsPrincipal user, string firstName)
        {
            if (firstName.Length > 50)
            {
                throw new BadRequestException("First name is too long, Please use a shorter name.");
            }
            if (firstName.Length < 2)
            {
                throw new BadRequestException("First name is too short, Please use a longer name.");
            }
            if (String.IsNullOrWhiteSpace(firstName))
            {
                throw new BadRequestException("First name cannot just be spaces.");
            }
            User userEntity = await GetUser(user);

            await UserRepository.SetFirstName(userEntity, firstName);
        }

        /// <summary>
        /// Adds or changes the last name of a user.
        /// </summary>
        /// <param name="user">The user whose last name to edit.</param>
        /// <param name="lastName">The last name of the user.</param>
        public async Task EditLastName(ClaimsPrincipal user, string lastName)
        {
            if (lastName.Length > 255)
            {
                throw new BadRequestException("Last name is too long, Please use a shorter name.");
            }
            if (lastName.Length < 2)
            {
                throw new BadRequestException("Last name is too short, Please use a longer name.");
            }
            if (String.IsNullOrWhiteSpace(lastName))
            {
                throw new BadRequestException("Last name cannot just be spaces.");
            }
            User userEntity = await GetUser(user);

            await UserRepository.SetLastName(userEntity, lastName);
        }

        /// <summary>
        /// Adds or changes the about section of a user.
        /// </summary>
        /// <param name="user">The user whose about me section to edit.</param>
        /// <param name="aboutMe">The about me section text of a user.</param>
        public async Task EditAboutMe(ClaimsPrincipal user, string aboutMe)
        {
            if (aboutMe.Length > 255)
            {
                throw new BadRequestException("About me section is too long.");
            }
            User userEntity = await GetUser(user);

            await UserRepository.SetAboutMe(userEntity, aboutMe);
        }

        /// <summary>
        /// Stores uploaded user images.
        /// </summary>
        /// <param name="user">The user whose image to store.</param>
        /// <param name="file">The file to store.</param>
        public async Task StoreUserImage(ClaimsPrincipal user, IFormFile file)
        {
            User userEntity = await GetUser(user);
            string extension = Path.GetExtension(file.FileName);

            ///!!!Important: Do not use original file name without validation & sanitization... this could lead to security issues.
            if (file.Length > 0)
            {
                //In temporary folder. For production builds use blob storage.
                string filePath = Path.Combine("/userImages", Path.GetTempFileName());
                filePath = Path.ChangeExtension(filePath, extension);

                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }

                string? oldURL = await UserRepository.GetPhotoURL(userEntity);

                await UserRepository.SetPhotoURL(userEntity, filePath);

                if (!string.IsNullOrEmpty(oldURL))
                {
                    System.IO.File.Delete(oldURL);
                }
            }
            else
            {
                throw new BadRequestException("File length is smaller than a byte.");
            }
        }

        /// <summary>
        /// Gets the user from the database.
        /// </summary>
        /// <param name="user">The claims principle of the user.</param>
        /// <returns>The user.</returns>
        private async Task<User> GetUser(ClaimsPrincipal user)
        {
            string? userId = UserManager.GetUserId(user) ?? throw new NotFoundException("User's id could not be found. User: " + user);
            User? userEntity = await UserRepository.GetUserById(userId);
            if (userEntity != null)
                return userEntity;

            throw new NotFoundException("Found user ID {userId} but could not find user." + userId);
        }
    }
}
