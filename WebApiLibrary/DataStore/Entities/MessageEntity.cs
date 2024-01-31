namespace WebApiLibrary.DataStore.Entities

{
    public class MessageEntity
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public virtual UserEntity Sender { get; set; }
        public Guid RecipientId { get; set; }
        public virtual UserEntity Recipient { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
    }
}
