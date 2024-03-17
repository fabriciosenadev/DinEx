namespace Dinex.Core
{
    public class InvestmentTransaction : Entity
    {
        public Guid TransactionHistoryId { get; private set; }
        public Applicable Applicable { get; private set; }
        public InvestmentTransactionType TransactionType { get; private set; }
        public Guid AssetId { get; private set; }
        public decimal AssetUnitPrice { get; private set; }
        public decimal AssetTransactionAmount { get; private set; }
        public int AssetQuantity { get; private set; }
        public Guid StockBrokerId { get; private set; }

        public static InvestmentTransaction Create(
            Guid transactionHistoryId, 
            Applicable applicable,
            InvestmentTransactionType transactionType,
            Guid assetId,
            decimal unitPrice,
            decimal transactionAmount,
            int assetQuantity,
            Guid stockBrokerId)
        {
            var investment = new InvestmentTransaction
            {
                TransactionHistoryId = transactionHistoryId,
                Applicable = applicable,
                TransactionType = transactionType,
                AssetId = assetId,
                AssetUnitPrice = unitPrice,
                AssetTransactionAmount = transactionAmount,
                AssetQuantity = assetQuantity,
                StockBrokerId = stockBrokerId,
                CreatedAt = DateTime.UtcNow,
            };
            return investment;
        }
    }
}
