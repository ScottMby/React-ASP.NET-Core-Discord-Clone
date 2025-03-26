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
    }
}
