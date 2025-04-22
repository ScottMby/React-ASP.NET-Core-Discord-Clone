using Microsoft.AspNetCore.Identity;
using NpgsqlTypes;

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
        /// A list of the friends the user has (where the user was the sender of the friend request).
        /// </summary>
        public List<UserFriends> SentUserFriends = [];

        /// <summary>
        /// A list of the friends the user has (where the user was the receiver of the friend request).
        /// </summary>
        public List<UserFriends> ReceivedUserFriends = [];

        /// <summary>
        /// A list of the friend requests a user has sent.
        /// </summary>
        public List<UserFriendRequests> SentUserFriendRequests = [];

        /// <summary>
        /// A list of the friend requests a user has received.
        /// </summary>
        public List<UserFriendRequests> ReceivedUserFriendRequests = [];

        /// <summary>
        /// A TsVector used to search for Users.
        /// </summary>
        public NpgsqlTsVector? UserSearchVector { get; set; }

    }
}
