using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discord_Clone.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserFriendsController(IUserFriendsRepository userFriendsRepository) : ControllerBase
    {
        private readonly IUserFriendsRepository UserFriendsRepository = userFriendsRepository;

        /// <summary>
        /// Searches for a specific user.
        /// </summary>
        /// <param name="searchTerm">The search term to use to find the user.</param>
        /// <returns>A list of users relevant to the search term.</returns>
        [HttpGet("UserSearch")]
        public IActionResult UserSearch(string searchTerm)
        {
            return Ok(UserFriendsRepository.UserSearch(searchTerm));
        }

    }
}
