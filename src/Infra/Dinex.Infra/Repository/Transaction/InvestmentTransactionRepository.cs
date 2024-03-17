namespace Dinex.Infra
{

    public class InvestmentTransactionRepository : Repository<InvestmentTransaction>, IInvestmentTransactionRepository
    {
        public InvestmentTransactionRepository(DinexApiContext context) : base(context)
        {
        }
    }
}
