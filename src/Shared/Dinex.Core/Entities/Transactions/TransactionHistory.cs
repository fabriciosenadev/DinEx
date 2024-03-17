namespace Dinex.Core
{
    public class TransactionHistory : Entity
    {
        public Guid UserId { get; private set; }
        public DateTime Date { get; private set; }
        public TransactionActivity Activity { get; private set; }

        public static TransactionHistory Create(Guid userId, DateTime date, TransactionActivity activity)
        {
            var newTransactionHistory = new TransactionHistory 
            { 
                UserId = userId,
                Date = date,
                Activity = activity,
                CreatedAt = DateTime.UtcNow,
            };
            return newTransactionHistory;
        }
    }
}
