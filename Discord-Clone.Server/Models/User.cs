using Microsoft.AspNetCore.Identity;

namespace Discord_Clone.Server.Models
{
    public class User : IdentityUser
    {  
        /// <summary>
        /// Display name of the user.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// First name of the user.
        /// </summary>
        [PersonalData]
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name of the user.
        /// </summary>
        [PersonalData]
        public string? LastName { get; set; }

        /// <summary>
        /// A URI pointing to the location of the users photo.
        /// </summary>
        public string? PhotoURL { get; set; }

        /// <summary>
        /// About me section of the user.
        /// </summary>
        public string? AboutMe { get; set; }

        /// <summary>
        /// A list of the friends the user has.
        /// </summary>
        public List<UserFriends> UserFriends = new();

        /// <summary>
        /// A list of the friend requests a user has.
        /// </summary>
        public List<UserFriendRequests> UserFriendRequests = new();

    }
}
