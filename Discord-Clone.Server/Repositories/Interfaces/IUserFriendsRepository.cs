using Discord_Clone.Server.Models;
using Discord_Clone.Server.Models.Data_Transfer_Objects;
using System.Security.Claims;

namespace Discord_Clone.Server.Repositories.Interfaces
{
    public interface IUserFriendsRepository
    {
        /// <summary>
        /// Searches through all users for a user with a display name matching the search term.
        /// </summary>
        /// <param name="searchTerm">The search term to find the specified user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of users that match the search term.</returns>
        public Task<List<UserSearchResult>> UserSearch(string searchTerm);

        /// <summary>
        /// Adds a friend request from one user to another in the database.
        /// </summary>
        /// <param name="userFriendRequests">The friend request details to be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task AddUserFriendRequest(UserFriendRequests userFriendRequests);

        /// <summary>
        /// Retrieves a specific friend request by its ID.
        /// </summary>
        /// <param name="friendRequestId">The ID of the friend request to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the friend request details.</returns>
        public Task<UserFriendRequests> GetFriendRequest(string friendRequestId);

        /// <summary>
        /// Retrieves all friend requests for a specific user.
        /// </summary>
        /// <param name="user">The user whose friend requests are being retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of friend requests for the user.</returns>
        public Task<List<UserFriendRequests>> GetUserFriendRequests(User user);

        /// <summary>
        /// Deletes a specific friend request by its ID.
        /// </summary>
        /// <param name="friendRequestId">The ID of the friend request to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task DeleteFriendRequest(string friendRequestId);

        /// <summary>
        /// Adds a user as a friend after a friend request is accepted.
        /// </summary>
        /// <param name="userFriends">The friendship details to be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task AddUserFriend(UserFriends userFriends);

        /// <summary>
        /// Checks if two users are friends.
        /// </summary>
        /// <param name="sender">The user initiating the check.</param>
        /// <param name="receiverId">The ID of the other user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the users are friends, otherwise false.</returns>
        public Task<bool> CheckUserIsFriends(User sender, string receiverId);

        /// <summary>
        /// Checks if a user has a pending friend request to another user.
        /// </summary>
        /// <param name="sender">The user initiating the check.</param>
        /// <param name="receiverId">The ID of the other user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if a pending friend request exists, otherwise false.</returns>
        public Task<bool> CheckUserHasPendingFriendRequest(User sender, string receiverId);
    }
}
