using Discord_Clone.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Discord_Clone.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository { get; set; }
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Signs Out User.
        /// </summary>
        /// <param name="signInManager">The user sign in API</param>
        /// <param name="empty">The body of the request</param>
        /// <returns>200 - Ok if signed out, 401 - if not</returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromServices] SignInManager<IdentityUser> signInManager, [FromBody] object empty)
        {
            if(empty == null)
            {
                await signInManager.SignOutAsync();
                return Ok();
            }
            return Unauthorized();

        }

        /// <summary>
        /// Checks that the user's display name isn't empty, if it is it generates a random one.
        /// </summary>
        /// <returns></returns>
        [HttpPost("DisplayName")]
        public IActionResult CheckDisplayName()
        {
            _userRepository.CheckDisplayNameValid(this.User);
            return Ok();
        }
    }
}
