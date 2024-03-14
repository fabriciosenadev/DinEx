namespace Dinex.Core
{
    public class StockBroker : Entity
    {
        public string Name { get; private set; }

        public static StockBroker Create(string stockBrokerName)
        {
            var newStockBroker = new StockBroker
            {
                Name = stockBrokerName,
                CreatedAt = DateTime.UtcNow
            };
            return newStockBroker;
        }

        public static IEnumerable<StockBroker> CreateRange(IEnumerable<string> stockBrokerNames)
        {
            var stockBrokers = new List<StockBroker>();
            foreach (var stockBrokerName in stockBrokerNames)
            {
                var stockBroker = Create(stockBrokerName.Trim());
                stockBrokers.Add(stockBroker);
            }
            return stockBrokers;
        }
    }
}
