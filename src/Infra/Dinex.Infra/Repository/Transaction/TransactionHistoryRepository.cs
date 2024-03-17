namespace Dinex.Infra
{
    public class TransactionHistoryRepository : Repository<TransactionHistory>, ITransactionHistoryRepository
    {
        public TransactionHistoryRepository(DinexApiContext context) : base(context)
        {
        }
    }
}
