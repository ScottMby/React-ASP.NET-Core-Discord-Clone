using Discord_Clone.Server.Models.Data_Transfer_Objects;
using Discord_Clone.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Discord_Clone.Server.Data;
using Discord_Clone.Server.Repositories.Interfaces;

namespace Discord_Clone.Server.Services
{
    public class UserFriendsService(IUserRepository userRepository, IUserFriendsRepository userFriendsRepository, ILogger<Program> logger, UserManager<User> userManager)
    {
        private IUserRepository UserRepository { get; set; } = userRepository;
        private IUserFriendsRepository UserFriendsRepository { get; set; } = userFriendsRepository;

        private ILogger<Program> Logger { get; set; } = logger;

        private UserManager<User> UserManager { get; set; } = userManager;


        public async Task<List<UserSearchResult>> UserSearch(ClaimsPrincipal user, string searchTerm)
        {
            User userEntity = await GetUser(user);
            List<UserSearchResult> results = (await UserFriendsRepository.UserSearch(searchTerm));
            //Remove any matches containing the searching user.
            results.RemoveAll(u => u.Id == userEntity.Id);
            return results;
        }

        public async Task UserFriendRequest(ClaimsPrincipal sender, string receiverId)
        {
            User sendingUser = await GetUser(sender);

            if(sendingUser.Id == receiverId)
            {
                throw new Exception("Users can't send friend requests to themselves. Go make some friends.");
            }

            User receivingUser = await UserRepository.GetUserById(receiverId) ?? throw new Exception("Could Not Find Receiving User");

            if(await UserFriendsRepository.CheckUserHasPendingFriendRequest(sendingUser, receiverId))
            {
                throw new Exception("User already has a pending friend request.");
            }

            if(await UserFriendsRepository.CheckUserIsFriends(sendingUser, receiverId))
            {
                throw new Exception("User is already a friend.");
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

        public async Task AcceptFriendRequest(ClaimsPrincipal user, string FriendRequestId)
        {
            User userObject = await GetUser(user);
            //Check request exists
            UserFriendRequests request = (await UserFriendsRepository.GetFriendRequest(FriendRequestId)) ?? throw new Exception("Request does not exist.");
            //Check user is receiver of request
            if (request.ReceiverId != userObject.Id)
            {
                throw new Exception("User is not receiver of this request");
            }
            //Delete Request
            await UserFriendsRepository.DeleteFriendRequest(FriendRequestId);
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
        /// Gets the user from the database.
        /// </summary>
        /// <param name="user">The claims principle of the user.</param>
        /// <returns>The user.</returns>
        private async Task<User> GetUser(ClaimsPrincipal user)
        {
            string? userId = UserManager.GetUserId(user) ?? throw new Exception("User's id could not be found. User: " + user);
            User? userEntity = await UserRepository.GetUserById(userId);
            if (userEntity != null)
                return userEntity;

            throw new Exception("Found user ID {userId} but could not find user." + userId);
        }
    }
}
