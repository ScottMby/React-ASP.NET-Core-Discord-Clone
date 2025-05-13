using Discord_Clone.Server.Models.Data_Transfer_Objects;
using Discord_Clone.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Discord_Clone.Server.Data;
using Discord_Clone.Server.Repositories.Interfaces;
using Discord_Clone.Server.Utilities;

namespace Discord_Clone.Server.Services
{
    public class UserFriendsService(IUserRepository userRepository, IUserFriendsRepository userFriendsRepository, ILogger<UserFriendsService> logger, UserManager<User> userManager)
    {
        private IUserRepository UserRepository { get; set; } = userRepository;
        private IUserFriendsRepository UserFriendsRepository { get; set; } = userFriendsRepository;

        private ILogger<UserFriendsService> Logger { get; set; } = logger;

        private UserManager<User> UserManager { get; set; } = userManager;

        /// <summary>
        /// Searches for users based on a search term, excluding the current user.
        /// </summary>
        /// <param name="user">The claims principal of the user performing the search.</param>
        /// <param name="searchTerm">The term to search for matching users.</param>
        /// <returns>A list of users matching the search term, excluding the current user.</returns>
        public async Task<List<UserSearchResult>> UserSearch(ClaimsPrincipal user, string searchTerm)
        {
            User userEntity = await GetUser(user);
            List<UserSearchResult> results = (await UserFriendsRepository.UserSearch(searchTerm));
            //Remove any matches containing the searching user.
            results.RemoveAll(u => u.Id == userEntity.Id);
            if(results.Count > 0)
            {
                return results;
            }
            throw new NotFoundException("No results found!");
        }

        /// <summary>
        /// Sends a friend request from the sender to the specified receiver.
        /// </summary>
        /// <param name="sender">The claims principal of the user sending the friend request.</param>
        /// <param name="receiverId">The ID of the user to receive the friend request.</param>
        /// <exception cref="Exception">Thrown if the sender tries to send a request to themselves, the receiver does not exist, a pending request already exists, or the users are already friends.</exception>
        public async Task UserFriendRequest(ClaimsPrincipal sender, string receiverId)
        {
            User sendingUser = await GetUser(sender);

            if (sendingUser.Id == receiverId)
            {
                throw new BadRequestException("Users can't send friend requests to themselves. Go make some friends.");
            }

            User receivingUser = await UserRepository.GetUserById(receiverId) ?? throw new NotFoundException("Could Not Find Receiving User");

            if (await UserFriendsRepository.CheckUserHasPendingFriendRequest(sendingUser, receiverId))
            {
                throw new BadRequestException("User already has a pending friend request.");
            }

            if (await UserFriendsRepository.CheckUserIsFriends(sendingUser, receiverId))
            {
                throw new BadRequestException("User is already a friend.");
            }

            UserFriendRequests userFriendRequest = new()
            {
                SenderId = sendingUser.Id,
                ReceiverId = receivingUser.Id,
                Sender = sendingUser,
                Receiver = receivingUser
            };

            await UserFriendsRepository.AddUserFriendRequest(userFriendRequest);
        }

        /// <summary>
        /// Accepts a friend request and establishes a friendship between the users.
        /// </summary>
        /// <param name="user">The claims principal of the user accepting the friend request.</param>
        /// <param name="friendRequestId">The ID of the friend request to accept.</param>
        /// <exception cref="Exception">Thrown if the request does not exist or the user is not the receiver of the request.</exception>
        public async Task AcceptFriendRequest(ClaimsPrincipal user, string friendRequestId)
        {
            User userObject = await GetUser(user);
            //Check request exists
            UserFriendRequests request = (await UserFriendsRepository.GetFriendRequest(friendRequestId)) ?? throw new NotFoundException("Request does not exist.");
            //Check user is receiver of request
            if (request.ReceiverId != userObject.Id)
            {
                throw new ForbiddenException("User is not receiver of this request");
            }
            //Delete Request
            await UserFriendsRepository.DeleteFriendRequest(friendRequestId);
            //Add Friend
            UserFriends userFriends = new()
            {
                Receiver = request.Receiver,
                ReceiverId = request.ReceiverId,
                Sender = request.Sender,
                SenderId = request.SenderId,
            };
            Chat chat = new()
            {
                UserFriends = userFriends
            };
            userFriends.Chat = chat;
            await UserFriendsRepository.AddUserFriend(userFriends);
        }

        /// <summary>
        /// Declines a friend request, removing it from the system.
        /// </summary>
        /// <param name="user">The claims principal of the user declining the friend request.</param>
        /// <param name="friendRequestId">The ID of the friend request to decline.</param>
        /// <exception cref="Exception">Thrown if the request does not exist or the user is not the receiver of the request.</exception>
        public async Task DeclineFriendRequest(ClaimsPrincipal user, string friendRequestId)
        {
            User userObject = await GetUser(user);

            UserFriendRequests request = (await UserFriendsRepository.GetFriendRequest(friendRequestId) ?? throw new NotFoundException("Request does not exist"));

            if (request.ReceiverId != userObject.Id)
            {
                throw new ForbiddenException("User is not receiver of this request");
            }

            await UserFriendsRepository.DeleteFriendRequest(friendRequestId);
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
