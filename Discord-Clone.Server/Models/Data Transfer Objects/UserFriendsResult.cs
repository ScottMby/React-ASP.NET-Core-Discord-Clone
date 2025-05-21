namespace Discord_Clone.Server.Models.Data_Transfer_Objects
{
    public class UserFriendsResult
    {
        /// <summary>
        /// The friend's user ID.
        /// </summary>
        public required string Id { get; set; }
        
        /// <summary>
        /// The friend's display name
        /// </summary>
        public required string DisplayName { get; set; }

        /// <summary>
        /// The friend's photo URL.
        /// </summary>
        public required string PhotoURL { get; set; }

        /// <summary>
        /// The friend's about me section.
        /// </summary>
        public required string AboutMe { get; set; }

        /// <summary>
        /// The time the two users have been friends for.
        /// </summary>
        public required DateTime FriendsSince { get; set; }
    }
}
