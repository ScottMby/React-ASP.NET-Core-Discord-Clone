using Carter;
using Discord_Clone.Server.Models.Data_Transfer_Objects;
using Discord_Clone.Server.Repositories.Interfaces;
using Discord_Clone.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Discord_Clone.Server.Endpoints
{
    public static class UserFriendsEndpoints
    {
        public static void MapUserFriendsEndpoints(this IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("api");

            var usersGroup = api.MapGroup("user")
                .WithOpenApi()
                .RequireAuthorization();

            usersGroup.MapGet("search", UserSearch);

            usersGroup.MapPost("sendfriendrequest", SendFriendRequest);

            usersGroup.MapPost("acceptfriendrequest", AcceptFriendRequest);

            usersGroup.MapDelete("declinefriendrequest", DeclineFriendRequest);
        }

        /// <summary>
        /// Searches for a specific user.
        /// </summary>
        /// <param name="userFriendsService">The user friends service.</param>
        /// <param name="searchTerm">The search term to use to find the user.</param>
        /// <returns>A list of users relevant to the search term.</returns>
        public static async Task<Results<Ok<List<UserSearchResult>>, NotFound>> UserSearch(UserFriendsService userFriendsService, ClaimsPrincipal user, [FromQuery] string searchTerm)
        {
            List<UserSearchResult> userSearchResults = await userFriendsService.UserSearch(user, searchTerm);
            if (userSearchResults.Count > 0)
                return TypedResults.Ok(userSearchResults);
            else
                return TypedResults.NotFound();
        }

        /// <summary>
        /// Send a user a friend request.
        /// </summary>
        /// <param name="userFriendsService">The user friends service.</param>
        /// <param name="sendingUser">The claims principle user who is sending the request.</param>
        /// <param name="receivingUserId">The id of the user to receive the request.</param>
        /// <returns></returns>
        public static async Task<Results<Ok, BadRequest>> SendFriendRequest(UserFriendsService userFriendsService, ClaimsPrincipal sendingUser, [FromBody] string receivingUserId)
        {
            await userFriendsService.UserFriendRequest(sendingUser, receivingUserId);
            return TypedResults.Ok();
        }

        /// <summary>
        /// Accepts a friend request.
        /// </summary>
        /// <param name="userFriendsService">The user friends service.</param>
        /// <param name="user">The claims principal of the user who is accepting the request.</param>
        /// <param name="friendRequestId">The id of the friend request.</param>
        /// <returns></returns>
        public static async Task<Results<Ok, NotFound>> AcceptFriendRequest(UserFriendsService userFriendsService, ClaimsPrincipal user, [FromBody] string friendRequestId)
        {
            await userFriendsService.AcceptFriendRequest(user, friendRequestId);
            return TypedResults.Ok();
        }

        /// <summary>
        /// Declines a friend request.
        /// </summary>
        /// <param name="userFriendsService">The user friends service.</param>
        /// <param name="user">The claims principal of the user who is accepting the request.</param>
        /// <param name="friendRequestId">The id of the friend request.</param>
        /// <returns></returns>
        public static async Task<Results<Ok, NotFound>> DeclineFriendRequest(UserFriendsService userFriendsService, ClaimsPrincipal user, [FromBody] string friendRequestId)
        {
            await userFriendsService.DeclineFriendRequest(user, friendRequestId);
            return TypedResults.Ok();
        }


    }
}
