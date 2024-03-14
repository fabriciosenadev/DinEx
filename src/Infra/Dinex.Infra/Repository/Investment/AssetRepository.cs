namespace Dinex.Infra
{

    public class AssetRepository : Repository<Asset>, IAssetRepository
    {
        public AssetRepository(DinexApiContext context) : base(context)
        {
        }
    }
}
