namespace Dinex.Infra
{

    public class QueueInRepository : Repository<QueueIn>, IQueueInRepository
    {
        public QueueInRepository(DinexApiContext context) : base(context)
        {
        }
    }
}
