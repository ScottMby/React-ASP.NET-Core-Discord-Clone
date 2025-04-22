using Discord_Clone.Server.Models;
using Discord_Clone.Server.Repositories.Interfaces;
using Discord_Clone.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Discord_Clone.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController(IUserRepository userRepository) : ControllerBase
    {
        private IUserRepository UserRepository { get; set; } = userRepository;

        /// <summary>
        /// Signs Out User.
        /// </summary>
        /// <param name="signInManager">The user sign in API</param>
        /// <param name="empty">The body of the request</param>
        /// <returns>HTTP Status Code</returns>
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromServices] SignInManager<User> signInManager)
        {
                await signInManager.SignOutAsync();
                return Ok();
        }

        /// <summary>
        /// Checks that the user's display name isn't empty, if it is it generates a random one.
        /// </summary>
        /// <returns>HTTP Status Code</returns>
        [HttpPatch("CheckDisplayName")]
        public IActionResult CheckDisplayName()
        {
            try
            {
                UserRepository.CheckDisplayNameValid(this.User);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
            
        }

        /// <summary>
        /// Edits a users display name.
        /// </summary>
        /// <param name="DisplayName">New display name of the user.</param>
        /// <returns>HTTP Status Code</returns>
        [HttpPatch("EditDisplayName")]
        public IActionResult ChangeDisplayName(string DisplayName)
        {
            try
            {
                UserRepository.ChangeDisplayName(this.User, DisplayName);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Edits a user's first name.
        /// </summary>
        /// <param name="FirstName">New first name of the user.</param>
        /// <returns>HTTP Status Code</returns>
        [HttpPatch("EditFirstName")]
        public IActionResult ChangeFirstName(string FirstName)
        {
            try
            {
                UserRepository.EditFirstName(this.User, FirstName);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Edits a user's last name.
        /// </summary>
        /// <param name="LastName">New last name of the user.</param>
        /// <returns>HTTP Status Code</returns>
        [HttpPatch("EditLastName")]
        public IActionResult ChangeLastName(string LastName)
        {
            try
            {
                UserRepository.EditLastName(this.User, LastName);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Edits a user's about me section.
        /// </summary>
        /// <param name="AboutMe">New content of user's about me section.</param>
        /// <returns>HTTP Status Code</returns>
        [HttpPatch("EditAboutMe")]
        public IActionResult ChangeAboutMe(string AboutMe)
        {
            try
            {
                UserRepository.EditAboutMe(this.User, AboutMe);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Changes user's profile photo.
        /// </summary>
        /// <param name="File">A file the user has uploaded to change to their new profile photo.</param>
        /// <returns>HTTP Status Code</returns>
        [HttpPost("ChangePhoto")]
        [ImageValidationFilter(5242880)]
        public IActionResult ChangePhoto(IFormFile File)
        {
            UserRepository.StoreUserImage(this.User, File);
            return Ok();
        }
    }
}
