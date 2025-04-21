using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discord_Clone.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserFriendsController : ControllerBase
    {
        public UserFriendsController()
        {
            
        }
    }
}
