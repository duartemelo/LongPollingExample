namespace LongPollingExample.Entities
{
    public class BaseEntity
    {
        public long Id { get; private set; }
        public DateTime DateOfCreation { get; private set; }

        public BaseEntity()
        {
            DateOfCreation = DateTime.Now;
        }
    }
}
