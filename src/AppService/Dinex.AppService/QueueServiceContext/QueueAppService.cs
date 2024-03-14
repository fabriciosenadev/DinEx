namespace Dinex.AppService;

public class QueueAppService : IQueueAppService
{
    private readonly ILogger<QueueAppService> _logger;
    
    private readonly IProcessingService _processingService;
    public QueueAppService(ILogger<QueueAppService> logger, IProcessingService processingService)
    {
        _logger = logger;
        _processingService = processingService;
    }

    public async Task<OperationResult> RequestToProcessQueue(Guid userId)
    {
        try
        {
            _logger.LogInformation("starting RequestToProcessQueue");

            var result = new OperationResult();

            _processingService.ProcessQueueIn(userId);

            _logger.LogInformation("finishing RequestToProcessQueue");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error ocurred to method: RequestToProcessQueue");
            return new OperationResult().SetAsInternalServerError()
                    .AddError($"An unexpected error ocurred to method: RequestToProcessQueue {ex}");
        }
    }
}
