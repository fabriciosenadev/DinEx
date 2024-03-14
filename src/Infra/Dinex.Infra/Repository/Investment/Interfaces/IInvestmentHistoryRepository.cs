namespace Dinex.Infra
{
    public interface IInvestmentHistoryRepository : IRepository<InvestmentHistory>
    {
        Task AddRangeAsync(IEnumerable<InvestmentHistory> investingHistory);
    }
}
