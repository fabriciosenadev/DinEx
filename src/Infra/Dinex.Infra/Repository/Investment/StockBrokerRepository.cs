namespace Dinex.Infra
{

    public class StockBrokerRepository : Repository<StockBroker>, IStockBrokerRepository
    {
        public StockBrokerRepository(DinexApiContext context) : base(context)
        {
        }
    }
}
