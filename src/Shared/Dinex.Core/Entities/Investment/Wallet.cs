namespace Dinex.Core
{
    public class Wallet : Entity
    {
        public Guid UserId { get; private set; }
        public Guid AssetId { get; private set; }
        public int AssetQuantity { get; private set; }
        public decimal InvestedAmount { get; private set; }
        public decimal AveragePrice { get; private set; }

        public static Wallet CreateAsset(Guid userId, Guid assetId, int assetQuantity, decimal investedAmount)
        {
            var wallet = new Wallet
            {
                UserId = userId,
                AssetId = assetId,
                AssetQuantity = assetQuantity,
                InvestedAmount = investedAmount,
                AveragePrice = investedAmount / assetQuantity
            };
            return wallet;
        }

        public void UpdateAsset(int assetQuantity, decimal investedAmount)
        {
            AssetQuantity += assetQuantity;
            InvestedAmount += investedAmount;
            AveragePrice = InvestedAmount / AssetQuantity;
        }

    }
}
