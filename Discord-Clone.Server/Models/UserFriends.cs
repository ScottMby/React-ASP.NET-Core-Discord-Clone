namespace Discord_Clone.Server.Models
{
    public class UserFriends
    {
        /// <summary>
        /// Id of the record of which users are friends.
        /// </summary>
        public string UserFriendsId { get; set; } = null!;

        /// <summary>
        /// The if of the user who originally sent the friend request.
        /// </summary>
        public string SenderId { get; set; } = null!;

        /// <summary>
        /// The id of user who originally received and accepted the friend request.
        /// </summary>
        public string ReceiverId { get; set; } = null!;

        /// <summary>
        /// The timestamp of when the friend request was accepted.
        /// </summary>
        public DateTime FriendsSince { get; set; } = DateTime.Now.ToUniversalTime();

        /// <summary>
        /// The id of the chat attached to the users' friendship.
        /// </summary>
        public string? ChatID { get; set; }

        /// <summary>
        /// The chat object between each friend.
        /// </summary>
        public required Chat Chat { get; set; }

        /// <summary>
        /// The user who originally sent the friend request.
        /// </summary>
        public required User Sender { get; set; }

        /// <summary>
        /// The user who originally received and accepted the friend request.
        /// </summary>
        public required User Receiver { get; set; }
    }
}
