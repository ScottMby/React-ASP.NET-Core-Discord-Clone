using System.Security.Claims;

namespace Discord_Clone.Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Checks that the display name of a user has been set. If not, sets the display name as a random name.
        /// </summary>
        /// <param name="User">The user whose display name you want to check.</param>
        public void CheckDisplayNameValid(ClaimsPrincipal User);

        /// <summary>
        /// Adds or changes the first name of a user.
        /// </summary>
        /// <param name="User">The user whose first name to edit.</param>
        /// <param name="FirstName">The first name of the user.</param>
        public void EditFirstName(ClaimsPrincipal User, string FirstName);

        /// <summary>
        /// Adds or changes the last name of a user.
        /// </summary>
        /// <param name="User">The user whose first name to edit.</param>
        /// <param name="LastName">The last name of the user.</param>
        public void EditLastName(ClaimsPrincipal User, string LastName);

        /// <summary>
        /// Adds or changes the about section of a user.
        /// </summary>
        /// <param name="User">The user whose about me section to edit.</param>
        /// <param name="AboutMe">The about me section text of a user.</param>
        public void EditAboutMe(ClaimsPrincipal User, string AboutMe);
    }
}
