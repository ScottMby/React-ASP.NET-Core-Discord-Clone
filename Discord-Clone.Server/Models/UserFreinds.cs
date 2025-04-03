namespace Discord_Clone.Server.Models
{
    public class UserFreinds
    {
        public string UserFriendsId { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public DateTime FriendsSince { get; set; }

        public string? ChatID { get; set; }

        public User Sender { get; set; }

        public User Receiver { get; set; }
    }
}
