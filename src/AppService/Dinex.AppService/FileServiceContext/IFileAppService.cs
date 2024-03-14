namespace Dinex.AppService
{ 
    public interface IFileAppService
    {
        Task<OperationResult> UploadInvestingStatement(HistoryFileRequest request, Guid userId);
    }
}
