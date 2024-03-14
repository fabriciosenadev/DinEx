namespace Dinex.Core
{
    public abstract class Entity : Notifiable<Notification>
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}
