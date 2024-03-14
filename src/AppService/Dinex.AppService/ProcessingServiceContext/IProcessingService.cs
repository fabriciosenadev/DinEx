namespace Dinex.AppService
{
    public interface IProcessingService
    {
        Task ProcessQueueIn(Guid userId);
    }
}
