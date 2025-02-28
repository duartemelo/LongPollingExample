namespace LongPollingExample.Entities
{
    public class Message : BaseEntity
    {
        public Guid ChatId { get; private set; }

        public string Description { get; private set; }

        public Message(Guid chatId, string description) {
            ChatId = chatId;
            Description = description;
        }
    }
}
