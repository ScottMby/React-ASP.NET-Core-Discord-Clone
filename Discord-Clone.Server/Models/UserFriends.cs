namespace Discord_Clone.Server.Models
{
    public class UserFriends
    {
        /// <summary>
        /// Id of the record of which users are friends.
        /// </summary>
        public string UserFriendsId { get; set; }

        /// <summary>
        /// The if of the user who originally sent the friend request.
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// The id of user who originally received and accepted the friend request.
        /// </summary>
        public string ReceiverId { get; set; }

        /// <summary>
        /// The timestamp of when the friend request was accepted.
        /// </summary>
        public DateTime FriendsSince { get; set; } = DateTime.Now;

        /// <summary>
        /// The id of the chat attached to the users' friendship.
        /// </summary>
        public string? ChatID { get; set; }

        /// <summary>
        /// The chat object between each friend.
        /// </summary>
        public Chat Chat { get; set; }

        /// <summary>
        /// The user who originally sent the friend request.
        /// </summary>
        public User Sender { get; set; }

        /// <summary>
        /// The user who originally received and accepted the friend request.
        /// </summary>
        public User Receiver { get; set; }
    }
}
