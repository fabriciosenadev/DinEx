namespace Dinex.Infra
{
    public interface IQueueInRepository : IRepository<QueueIn>
    {
        Task UpdateRangeAsync(IEnumerable<QueueIn> queueIn);
    }
}
