namespace Dinex.AppService
{
    public class FileAppService : IFileAppService
    {
        private readonly CultureInfo Culture = new("pt-BR");
        private readonly string[] excelMimeTypes =
        [
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xlsx
            "application/vnd.ms-excel", // .xls
            "application/vnd.ms-office", // .xls (antiga versão)
            "application/vnd.ms-excel.sheet.macroEnabled.12", // .xlsm (com macros)
            // Adicione outros tipos, se necessário...
        ];

        private readonly ILogger<FileAppService> _logger;

        private readonly IQueueInRepository _queueInRepository;
        private readonly IInvestmentHistoryRepository _investmentHistoryRepository;

        private readonly IProcessingService _processingService;

        public FileAppService(
            ILogger<FileAppService> logger,
            IQueueInRepository queueInRepository,
            IInvestmentHistoryRepository investmentHistoryRepository,
            IProcessingService processingService)
        {
            _logger = logger;
            _queueInRepository = queueInRepository;
            _investmentHistoryRepository = investmentHistoryRepository;
            _processingService = processingService;
        }

        public async Task<OperationResult> UploadInvestingStatement(HistoryFileRequest request, Guid userId)
        {
            try
            {
                _logger.LogInformation("starting UploadInvestingStatement");

                var result = new OperationResult();

                if (request.FileHistory == null || request.FileHistory.Length == 0)
                {
                    result.AddError("Arquivo é obrigatório");
                }
                else if (!excelMimeTypes.Contains(request.FileHistory.ContentType))
                {
                    result.AddError("Formato de arquivo inválido");
                }

                if (result.HasErrors())
                    return result;

                var investmentHistoryData = GetInvestingHistoryData(request.FileHistory);
                if (investmentHistoryData.Count <= 0)
                {
                    result.AddError("Não há dados de historico no arquivo enviado.");
                    return result;
                }

                var queueIn = QueueIn.Create(userId, request.QueueType, request.FileHistory.FileName);

                var investmentHistoryList = InvestmentHistory.CreateRange(investmentHistoryData, queueIn.Id, Culture);
                if (investmentHistoryList.Any(x => !x.IsValid))
                {
                    var errors = investmentHistoryList.Where(x => !x.IsValid)
                            .SelectMany(x => x.Notifications).ToList();
                    result.AddErrors(errors);
                    return result;
                }

                await _queueInRepository.AddAsync(queueIn);
                await _investmentHistoryRepository.AddRangeAsync(investmentHistoryList);

                _processingService.ProcessQueueIn(userId);

                _logger.LogInformation("finishing UploadInvestingStatement");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error ocurred to method: UploadInvestingStatement");
                return new OperationResult().SetAsInternalServerError()
                        .AddError($"An unexpected error ocurred to method: UploadInvestingStatement {ex}");
            }
        }

        #region private methods
        private static Dictionary<int, List<dynamic>> GetInvestingHistoryData(IFormFile fileHistory)
        {
            var dictionary = new Dictionary<int, List<dynamic>>();

            using (var stream = fileHistory?.OpenReadStream())
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var isFirstRow = true;
                    var row = 0;
                    while (reader.Read())
                    {
                        if (isFirstRow)
                        {
                            isFirstRow = false;
                            continue;
                        }

                        var listOfColumns = new List<dynamic>();
                        for (int columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
                        {
                            var columnType = reader.GetFieldType(columnIndex);
                            var columnValue = reader.GetValue(columnIndex);

                            if (columnType == typeof(string))
                            {
                                string stringValue = columnValue.ToString();
                                listOfColumns.Add(stringValue);
                            }
                            else if (columnType == typeof(int))
                            {
                                int intValue = Convert.ToInt32(columnValue);
                                listOfColumns.Add(intValue);
                            }
                            else if (columnType == typeof(double))
                            {
                                double doubleValue = Convert.ToDouble(columnValue);
                                listOfColumns.Add(doubleValue);
                            }
                            else if (columnType == typeof(float))
                            {
                                float floatValue = Convert.ToSingle(columnValue);
                                listOfColumns.Add(floatValue);
                            }
                            else if (columnType == typeof(DateTime))
                            {
                                var dateTime = Convert.ToDateTime(columnValue);
                                listOfColumns.Add(dateTime);
                            }
                        }
                        dictionary.Add(row, listOfColumns);
                        row++;
                    }
                    reader.Close();
                }
            }

            return dictionary;
        }
        #endregion
    }
}
