namespace Dinex.Core
{
    public class HistoryFileRequest
    {
        public TransactionActivity QueueType { get; set; }

        public required IFormFile FileHistory { get; set; }
    }
}
