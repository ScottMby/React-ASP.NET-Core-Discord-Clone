using Discord_Clone.Server.Models.Data_Transfer_Objects;

namespace Discord_Clone.Server.Repositories.Interfaces
{
    public interface IUserFriendsRepository
    {
        /// <summary>
        /// Searches through all users for a user with a display or user name matching the search term.
        /// </summary>
        /// <param name="searchTerm">The search term to find the specified user.</param>
        /// <returns>A list of users that satisfy the search term.</returns>
        public List<UserSearchResult> UserSearch(string searchTerm);
    }
}
