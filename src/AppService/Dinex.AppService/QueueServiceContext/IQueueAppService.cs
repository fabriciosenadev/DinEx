namespace Dinex.AppService;

public interface IQueueAppService
{
    Task<OperationResult> RequestToProcessQueue(Guid userId);
}
