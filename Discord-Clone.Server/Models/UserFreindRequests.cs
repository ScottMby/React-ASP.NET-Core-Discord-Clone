namespace Discord_Clone.Server.Models
{
    /// <summary>
    /// A record of an outstanding friend request. This should be deleted when the request is accepted or rejected.
    /// </summary>
    public class UserFreindRequests
    {
        /// <summary>
        /// The id of the friend request.
        /// </summary>
        public string FriendRequestId { get; set; }

        /// <summary>
        /// The id of the sending user.
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// The id of the recieving user.
        /// </summary>
        public string RecieverId { get; set; }

        /// <summary>
        /// The timestamp of when the request was sent.
        /// </summary>
        public DateTime SentTimestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// The user who sent the friend request.
        /// </summary>
        public User Sender { get; set; }

        /// <summary>
        /// The user who recieved the friend request.
        /// </summary>
        public User Reciever { get; set; }
    }
}
