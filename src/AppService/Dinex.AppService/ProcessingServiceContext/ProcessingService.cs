namespace Dinex.AppService
{

    public class ProcessingService : IProcessingService
    {
        private readonly ILogger<ProcessingService> _logger;

        private readonly IQueueInRepository _queueInRepository;
        private readonly IInvestmentHistoryRepository _investmentHistoryRepository;
        private readonly IStockBrokerRepository _stockBrokerRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly ITransactionHistoryRepository _transactionHistoryRepository;
        private readonly IInvestmentTransactionRepository _investmentTransactionRepository;

        public ProcessingService(
            ILogger<ProcessingService> logger,
            IQueueInRepository queueInRepository,
            IInvestmentHistoryRepository investmentHistoryRepository,
            IStockBrokerRepository stockBrokerRepository,
            IAssetRepository assetRepository,
            ITransactionHistoryRepository transactionHistoryRepository,
            IInvestmentTransactionRepository investmentTransactionRepository)
        {
            _logger = logger;
            _queueInRepository = queueInRepository;
            _investmentHistoryRepository = investmentHistoryRepository;
            _stockBrokerRepository = stockBrokerRepository;
            _assetRepository = assetRepository;
            _transactionHistoryRepository = transactionHistoryRepository;
            _investmentTransactionRepository = investmentTransactionRepository;
        }

        public async Task ProcessQueueIn(Guid userId)
        {
            _logger.LogInformation("starting ProcessQueueIn");

            var queueIn = await _queueInRepository.FindAsync(x => x.UserId == userId);

            var investmentQueueIn = queueIn.Where(x => x.Type == TransactionActivity.Investment).ToList();
            if (investmentQueueIn.Count > 0)
            {
                var queueInIds = investmentQueueIn.Select(x => x.Id);
                await ProcessingInvestment(queueInIds, userId);

                investmentQueueIn.ForEach(x =>
                {
                    x.UpdatedAt = DateTime.UtcNow;
                });
                await _queueInRepository.UpdateRangeAsync(investmentQueueIn);
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
                    x.TransactionType == InvestmentTransactionType.Transfer
                    ||
                    x.TransactionType == InvestmentTransactionType.SettlementTransfer);
            if (boughtAssets.Any())
            {
                var stockBrokerNames = boughtAssets.Select(x => x.Institution).Distinct();
                var stockBrokers = await StockBrokerAddRangeAsync(stockBrokerNames);

                var assetNames = boughtAssets.Select(x => x.Product).Distinct();
                var assets = await AssetsAddRangeAsync(assetNames);

                await InvestmentTransactionsAddRangeAsync(boughtAssets, stockBrokers, assets, userId);
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
            if(!stockBrokerNamesToCreate.Any())
                return stockBrokersFound;

            var stockBrokers = StockBroker.CreateRange(stockBrokerNamesToCreate);

            await _stockBrokerRepository.AddRangeAsync(stockBrokers);

            return stockBrokers;
        }

        private async Task<IEnumerable<Asset>> AssetsAddRangeAsync(IEnumerable<string> assetNames)
        {
            try
            {
                var assetsFound = await _assetRepository.FindAsync(x => assetNames.Contains(x.Ticker.Trim()));

                var assetNamesToCreate = assetNames.Except(assetsFound.Select(x => x.Ticker.Trim()));
                if (!assetNamesToCreate.Any())
                    return assetsFound;

                var assets = Asset.CreateRange(assetNamesToCreate);

                await _assetRepository.AddRangeAsync(assets);

                return assets;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task InvestmentTransactionsAddRangeAsync(IEnumerable<InvestmentHistory> boughtAssets,
            IEnumerable<StockBroker> stockBrokers,
            IEnumerable<Asset> assets,
            Guid userId)
        {
            try
            {
                var transactionHistories = new List<TransactionHistory>();
                var investmentTransactions = new List<InvestmentTransaction>();
                foreach (var boughtAsset in boughtAssets)
                {
                    var assetId = assets.Where(x => boughtAsset.Product.Contains(x.Ticker.Trim()))
                                        .Select(x => x.Id).First();
                    var stockBrokerId = stockBrokers.Where(x => x.Name == boughtAsset.Institution)
                                                                .Select(x => x.Id).First();

                    var transactionHistory = TransactionHistory.Create(
                        userId,
                        GetOperationDate(boughtAsset.Date),
                        TransactionActivity.Investment);
                    transactionHistories.Add(transactionHistory);

                    var investmentTransaction = InvestmentTransaction.Create(
                        transactionHistoryId: transactionHistory.Id,
                        applicable: boughtAsset.Applicable,
                        transactionType: boughtAsset.TransactionType,
                        assetId,
                        unitPrice: boughtAsset.UnitPrice,
                        transactionAmount: boughtAsset.OperationValue,
                        assetQuantity: boughtAsset.Quantity,
                        stockBrokerId);
                    investmentTransactions.Add(investmentTransaction);
                }


                await _transactionHistoryRepository.AddRangeAsync(transactionHistories);

                await _investmentTransactionRepository.AddRangeAsync(investmentTransactions);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static DateTime GetOperationDate(DateTime dateFromFile)
        {
            DateTime operationDate = dateFromFile.AddDays(-2);

            switch (operationDate.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    operationDate = dateFromFile.AddDays(-4);
                    break;
                case DayOfWeek.Saturday:
                    operationDate = dateFromFile.AddDays(-3);
                    break;
            }

            return operationDate;
        }
        #endregion
    }
}
