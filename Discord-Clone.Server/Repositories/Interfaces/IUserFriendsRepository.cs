using Discord_Clone.Server.Models.Data_Transfer_Objects;
using System.Security.Claims;

namespace Discord_Clone.Server.Repositories.Interfaces
{
    public interface IUserFriendsRepository
    {
        /// <summary>
        /// Searches through all users for a user with a display or user name matching the search term.
        /// </summary>
        /// <param name="searchTerm">The search term to find the specified user.</param>
        /// <returns>A list of users that satisfy the search term.</returns>
        public Task<List<UserSearchResult>> UserSearch(string searchTerm);

        /// <summary>
        /// Sends a friend request to the receiver from the send.
        /// </summary>
        /// <param name="sender">The claims principal of the sending user.</param>
        /// <param name="receiverId">The Id of the receiving user.</param>
        /// <returns></returns>
        public Task UserFriendRequest(ClaimsPrincipal sender, string receiverId);
        
        /// <summary>
        /// Accepts a friend request. (Deletes request and creates the friend record).
        /// </summary>
        /// <param name="user">The claims principle of the user.</param>
        /// <param name="friendRequestId">The Id of the friend request.</param>
        /// <returns></returns>
        public Task AcceptFriendRequest(ClaimsPrincipal user, string friendRequestId);
    }
}
