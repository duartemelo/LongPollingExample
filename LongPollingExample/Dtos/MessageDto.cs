namespace LongPollingExample.Dtos
{
    public class MessageDto
    {
        public Guid ChatId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
