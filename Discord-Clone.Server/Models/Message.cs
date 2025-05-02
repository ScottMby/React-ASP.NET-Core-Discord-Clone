namespace Discord_Clone.Server.Models
{
    public class Message
    {
        /// <summary>
        /// Id of the message.
        /// </summary>
        public string MessageId { get; set; } = null!;

        /// <summary>
        /// Content of the message.
        /// </summary>
        public required string Content { get; set; }

        /// <summary>
        /// List of Uniform Resource Identifier for each attachment for the message.
        /// </summary>
        public List<string> AttachmentURI { get; set; } = [];

        /// <summary>
        /// The timestamp of the messages creation.
        /// </summary>
        public required DateTime Timestamp { get; set; } = DateTime.Now.ToUniversalTime();

        /// <summary>
        /// The timestamp of when the message was last edited.
        /// </summary>
        public DateTime LastEdit { get; set; }

        /// <summary>
        /// Whether this message is pinned in the chat or not.
        /// </summary>
        public required bool Pinned { get; set; } = false;

        /// <summary>
        /// Unicode string list for reaction emojis.
        /// </summary>
        public List<string> Reaction = [];

        /// <summary>
        /// The id of the chat that is 
        /// </summary>
        public string ChatId { get; set; } = null!;

        /// <summary>
        /// The chat that the message is attached to.
        /// </summary>
        public required Chat Chat { get; set; }
    }
}
