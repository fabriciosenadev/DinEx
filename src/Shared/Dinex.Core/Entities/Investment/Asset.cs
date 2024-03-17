namespace Dinex.Core
{
    public class Asset : Entity
    {
        public AssetType? Type { get; private set; }
        public string Ticker { get; private set; }
        public string CompanyName { get; private set; }
        public string? TaxId { get; private set; }

        public static Asset Create(string ticker, string companyName)
        {
            var newAsset = new Asset { 
                Ticker = ticker,
                CompanyName = companyName
            };
            return newAsset;
        }

        public static IEnumerable<Asset> CreateRange(IEnumerable<string> rawAssetNames) 
        { 
            var assetList = new List<Asset>();
            foreach (var rawAssetName in rawAssetNames)
            {
                var splitedName = rawAssetName.Split("-");
                var asset = Asset.Create(
                    ticker: splitedName[0].Trim(), 
                    companyName: splitedName[1].Trim());

                if(!assetList.Any(x => x.Ticker == asset.Ticker))                
                    assetList.Add(asset);
            }

            return assetList;
        }
    }
}
