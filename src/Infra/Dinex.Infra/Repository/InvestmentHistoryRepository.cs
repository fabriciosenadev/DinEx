namespace Dinex.Infra
{

    public class InvestmentHistoryRepository : Repository<InvestmentHistory>, IInvestmentHistoryRepository
    {
        public InvestmentHistoryRepository(DinexApiContext context) : base(context)
        {
        }
    }
}
