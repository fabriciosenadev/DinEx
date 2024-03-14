namespace Dinex.AppService
{

    public class ProcessingService : IProcessingService
    {
        private readonly ILogger<ProcessingService> _logger;

        private readonly IQueueInRepository _queueInRepository;
        private readonly IInvestmentHistoryRepository _investmentHistoryRepository;
        private readonly IStockBrokerRepository _stockBrokerRepository;
        private readonly IAssetRepository _assetRepository;

        public ProcessingService(
            ILogger<ProcessingService> logger,
            IQueueInRepository queueInRepository,
            IInvestmentHistoryRepository investmentHistoryRepository,
            IStockBrokerRepository stockBrokerRepository,
            IAssetRepository assetRepository)
        {
            _logger = logger;
            _queueInRepository = queueInRepository;
            _investmentHistoryRepository = investmentHistoryRepository;
            _stockBrokerRepository = stockBrokerRepository;
            _assetRepository = assetRepository;
        }

        public async Task ProcessQueueIn(Guid userId)
        {
            _logger.LogInformation("starting ProcessQueueIn");

            var queueIn = await _queueInRepository.FindAsync(x => x.UserId == userId);

            var investmentQueueIn = queueIn.Where(x => x.Type == TransactionActivity.Investment).ToList();
            if (investmentQueueIn.Count > 0)
            {
                var queueInIds = investmentQueueIn.Select(x => x.Id);
                //await ProcessingInvestment(queueInIds, userId);

                //investmentQueueIn.ForEach(x =>
                //{
                //    x.UpdatedAt = DateTime.UtcNow;
                //});
                //await _queueInRepository.UpdateRangeAsync(investmentQueueIn);
            }

            var financialPlanningQueueIn = queueIn.Where(x => x.Type == TransactionActivity.FinancialPlanning).ToList();
            if (financialPlanningQueueIn.Count > 0)
            {
                // se o arquivo for do tipo FinancialPlanning -> chama a regra de processamento de controle financeiro enviado o Id da fila
                var queueInIds = financialPlanningQueueIn.Select(x => x.Id);
                //await ProcessFinancialPlanning(queueInIds);

                //financialPlanningQueueIn.ForEach(x =>
                //{
                //    x.UpdatedAt = DateTime.UtcNow;
                //});
                //await _queueInRepository.UpdateRangeAsync(financialPlanningQueueIn);
            }

            _logger.LogInformation("finishing ProcessQueueIn");
        }

        #region
        private async Task ProcessingInvestment(IEnumerable<Guid> queueInIds, Guid userId)
        {
            var investmentData = await _investmentHistoryRepository.FindAsync(x => queueInIds.Contains(x.QueueId));

            var boughtAssets = investmentData.Where(x =>
                    x.TrnasactionType == InvestingTrnasactionType.Transfer
                    ||
                    x.TrnasactionType == InvestingTrnasactionType.SettlementTransfer);
            if (boughtAssets.Any())
            {
                var stockBrokerNames = boughtAssets.Select(x => x.Institution).Distinct();
                var assetNames = boughtAssets.Select(x => x.Product).Distinct();

                var stockBrokers = await StockBrokerAddRangeAsync(stockBrokerNames);
                var assets = await AssetsAddAsync(assetNames);
            }
        }

        private async Task ProcessFinancialPlanning(IEnumerable<Guid> queueInIds)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<StockBroker>> StockBrokerAddRangeAsync(IEnumerable<string> stockBrokerNames)
        {
            var stockBrokersFound = await _stockBrokerRepository.FindAsync(x => stockBrokerNames.Contains(x.Name));
            
            var stockBrokerNamesToCreate = stockBrokerNames.Except(stockBrokersFound.Select(x => x.Name));

            var stockBrokers = StockBroker.CreateRange(stockBrokerNamesToCreate);

            await _stockBrokerRepository.AddRangeAsync(stockBrokers);

            return stockBrokers;
        }

        private async Task<IEnumerable<Asset>> AssetsAddAsync(IEnumerable<string> AssetNames)
        {
            var assetsFound = await _assetRepository.FindAsync(x => AssetNames.Contains($"{x.Ticker} - {x.CompanyName}"));

            var assetNamesToCreate = AssetNames.Except(assetsFound.Select(x => $"{x.Ticker} - {x.CompanyName}"));

            var assets = Asset.CreateRange(assetNamesToCreate);

            await _assetRepository.AddRangeAsync(assets);

            return assets;
        }

        //private async Task InvestingLaunchAddAsync(InvestingHistoryFile investingHistoryFile,
        //    InvestingBrokerage investingBrokerage,
        //    InvestingProduct investingProduct,
        //    Guid userId)
        //{
        //    var launch = new Launch
        //    {
        //        Activity = TransactionActivity.Investing,
        //        Date = GetOperationDate(investingHistoryFile.Date),
        //        UserId = userId,
        //        CreatedAt = DateTime.UtcNow,
        //    };
        //    await _launchRepository.AddAsync(launch);

        //    var investingLaunch = new InvestingLaunch
        //    {
        //        LaunchId = launch.Id,
        //        Applicable = investingHistoryFile.Applicable,
        //        InvestingActivity = investingHistoryFile.ActivityType,
        //        ProductId = investingProduct.Id,
        //        UnitPrice = investingHistoryFile.UnitPrice,
        //        OperationPrice = investingHistoryFile.OperationValue,
        //        ProductOperationQuantity = investingHistoryFile.Quantity,
        //        InvestingBrokerageId = investingBrokerage.Id,
        //        CreatedAt = DateTime.UtcNow,
        //    };
        //    await _launchInvestingRepository.AddAsync(investingLaunch);
        //}

        #endregion
    }
}
