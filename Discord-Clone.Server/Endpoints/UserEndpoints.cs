using Discord_Clone.Server.Models;
using Discord_Clone.Server.Repositories;
using Discord_Clone.Server.Repositories.Interfaces;
using Discord_Clone.Server.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RandomFriendlyNameGenerator.Data;
using System.Security.Claims;

namespace Discord_Clone.Server.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var apiGroup = app.MapGroup("api");

            var authGroup = apiGroup.MapGroup("authorization")
                .WithOpenApi();

            authGroup.MapPost("logout", Logout)
                .RequireAuthorization();

            var userGroup = apiGroup.MapGroup("user")
                .WithOpenApi()
                .RequireAuthorization();

            userGroup.MapPatch("checkdisplayname", CheckDisplayName);

            userGroup.MapPatch("changedisplayname", ChangeDisplayName);

            userGroup.MapPatch("changefirstname", ChangeFirstName);

            userGroup.MapPatch("changelastname", ChangeLastName);

            userGroup.MapPatch("changeaboutme", ChangeAboutMe);

            userGroup.MapPatch("changephoto", ChangePhoto);
        }

        /// <summary>
        /// Signs Out User.
        /// </summary>
        /// <param name="signInManager">The user sign in API.</param>
        /// <param name="empty">The body of the request.</param>
        /// <returns>HTTP Status Code.</returns>
        public static async Task<IResult> Logout(SignInManager<User> signInManager,
                [FromBody] object empty)
        {
            if (empty != null)
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            }
            return Results.Unauthorized();
        }

        /// <summary>
        /// Checks that the user's display name isn't empty, if it is it generates a random one.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="user">The user to check.</param>
        /// <returns>HTTP Status Code.</returns>
        public static async Task<IResult> CheckDisplayName(IUserRepository userRepository, ClaimsPrincipal user)
        {
            try
            {
                await userRepository.CheckDisplayNameValid(user);
                return Results.Ok();
            }
            catch
            {
                return Results.BadRequest();
            }
        }

        /// <summary>
        /// Edits a users display name.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="user">The user.</param>
        /// <param name="displayName">New display name of the user.</param>
        /// <returns>HTTP Status Code.</returns>
        public static async Task<IResult> ChangeDisplayName(IUserRepository userRepository, ClaimsPrincipal user, [FromBody] string displayName)
        {
            try
            {
                await userRepository.ChangeDisplayName(user, displayName);
                return Results.Ok();
            }
            catch
            {
                return Results.BadRequest();
            }
        }

        /// <summary>
        /// Edits a user's first name.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="user">The user.</param>
        /// <param name="firstName">New first name of the user.</param>
        /// <returns>HTTP Status Code</returns>
        public static async Task<IResult> ChangeFirstName(IUserRepository userRepository, ClaimsPrincipal user, [FromBody] string firstName)
        {
            try
            {
                await userRepository.EditFirstName(user, firstName);
                return Results.Ok();
            }
            catch
            {
                return Results.BadRequest();
            }
        }

        /// <summary>
        /// Edits a user's last name.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="user">The user.</param>
        /// <param name="lastName">New last name of the user.</param>
        /// <returns>HTTP Status Code</returns>
        public static async Task<IResult> ChangeLastName(IUserRepository userRepository, ClaimsPrincipal user, [FromBody] string lastName)
        {
            try
            {
                await userRepository.EditLastName(user, lastName);
                return Results.Ok();
            }
            catch
            {
                return Results.BadRequest();
            }
        }

        /// <summary>
        /// Edits a user's about me section.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="user">The user.</param>
        /// <param name="aboutMe">New content of user's about me section.</param>
        /// <returns>HTTP Status Code</returns>
        public static async Task<IResult> ChangeAboutMe(IUserRepository userRepository, ClaimsPrincipal user, [FromBody] string aboutMe)
        {
            try
            {
                await userRepository.EditAboutMe(user, aboutMe);
                return Results.Ok();
            }
            catch
            {
                return Results.BadRequest();
            }
        }

        /// <summary>
        /// Changes user's profile photo.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="user">The user.</param>
        /// <param name="file">A file the user has uploaded to change to their new profile photo.</param>
        /// <returns>HTTP Status Code</returns>
        [ImageValidationFilter(5242880)]
        public static async Task<IResult> ChangePhoto(IUserRepository userRepository, ClaimsPrincipal user, IFormFile file)
        {
            await userRepository.StoreUserImage(user, file);
            return Results.Ok();
        }
    }
}
