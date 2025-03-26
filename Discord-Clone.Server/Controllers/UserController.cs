using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Discord_Clone.Server.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Signs Out User.
        /// </summary>
        /// <param name="signInManager">The user sign in API</param>
        /// <param name="empty">The body of the request</param>
        /// <returns>200 - Ok if signed out, 401 - if not</returns>
        [HttpPost(Name = "Logout")]
        public async Task<IActionResult> Logout(SignInManager<IdentityUser> signInManager, [FromBody] object empty)
        {
            if(empty == null)
            {
                await signInManager.SignOutAsync();
                return Ok();
            }
            return Unauthorized();

        }
    }
}
