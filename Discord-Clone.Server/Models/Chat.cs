namespace Discord_Clone.Server.Models
{
    public class Chat
    {
        /// <summary>
        /// Id of the chat.
        /// </summary>
        public string ChatId { get; set; } = null!;

        /// <summary>
        /// The record of friendship between the chatting users.
        /// </summary>
        public required UserFriends UserFriends { get; set; }

        /// <summary>
        /// List of messages within the chat.
        /// </summary>
        public List<Message> Messages = [];
    }
}
