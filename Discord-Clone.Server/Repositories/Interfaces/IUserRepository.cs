using Discord_Clone.Server.Models;
using System.Security.Claims;

namespace Discord_Clone.Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The user object if found, otherwise null.</returns>
        public Task<User?> GetUserById(string userId);

        /// <summary>
        /// Retrieves the display name of a user.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>The display name of the user, or null if not set.</returns>
        public Task<string?> GetUserDisplayName(User user);

        /// <summary>
        /// Updates the display name of a user.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <param name="displayName">The new display name to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SetUserDisplayName(User user, string displayName);

        /// <summary>
        /// Retrieves the first name of a user.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>The first name of the user, or null if not set.</returns>
        public Task<string?> GetFirstName(User user);

        /// <summary>
        /// Updates the first name of a user.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <param name="firstName">The new first name to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SetFirstName(User user, string firstName);

        /// <summary>
        /// Retrieves the last name of a user.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>The last name of the user, or null if not set.</returns>
        public Task<string?> GetLastName(User user);

        /// <summary>
        /// Updates the last name of a user.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <param name="lastName">The new last name to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SetLastName(User user, string lastName);

        ///<summary>
        /// Retrieves the "About Me" section of a user.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>The "About Me" section of the user, or null if not set.</returns>
        public Task<string?> GetAboutMe(User user);

        /// <summary>
        /// Updates the "About Me" section of a user.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <param name="aboutMe">The new "About Me" content to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SetAboutMe(User user, string aboutMe);

        /// <summary>
        /// Retrieves the photo URL of a user.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>The photo URL of the user, or null if not set.</returns>
        public Task<string?> GetPhotoURL(User user);

        /// <summary>
        /// Updates the photo URL of a user.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <param name="photoURL">The new photo URL to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SetPhotoURL(User user, string photoURL);
    }
}
