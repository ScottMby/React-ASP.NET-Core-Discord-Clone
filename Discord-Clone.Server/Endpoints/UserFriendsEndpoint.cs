using Carter;
using Discord_Clone.Server.Models.Data_Transfer_Objects;
using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Discord_Clone.Server.Endpoints
{
    public static class UserFriendsEndpoints
    {
        public static void MapUserFriendsEndpoints(this IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("api");

            var usersGroup = api.MapGroup("users")
                .WithOpenApi()
                .RequireAuthorization();

            usersGroup.MapGet("search", UserSearch);
        }

        /// <summary>
        /// Searches for a specific user.
        /// </summary>
        /// <param name="userFriendsRepository">The user friends repository.</param>
        /// <param name="searchTerm">The search term to use to find the user.</param>
        /// <returns>A list of users relevant to the search term.</returns>
        public static async Task<Results<Ok<List<UserSearchResult>>, NotFound>> UserSearch(IUserFriendsRepository userFriendsRepository, [FromQuery] string searchTerm)
        {
            List<UserSearchResult> userSearchResults = await userFriendsRepository.UserSearch(searchTerm);
            if (userSearchResults.Count > 0)
                return TypedResults.Ok(userSearchResults);
            else
                return TypedResults.NotFound();
        }
    }
}
