using System.Text.RegularExpressions;

namespace Dinex.Core
{
    public class InvestmentHistory : Entity
    {
        public Guid QueueId { get; private set; }
        public Applicable Applicable { get; private set; }
        public DateTime Date { get; private set; }
        public InvestingTrnasactionType TrnasactionType { get; private set; }
        public string Product { get; private set; }
        public string Institution { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal OperationValue { get; private set; }

        public static InvestmentHistory Create(Dictionary<int, List<dynamic>> investingHistoryData, int selectedRow, Guid queueInId, CultureInfo culture)
        {
            var selectedApplicable = investingHistoryData.FirstOrDefault(x => x.Key == selectedRow).Value[0];
            var activityDate = investingHistoryData.FirstOrDefault(x => x.Key == selectedRow).Value[1];
            var selectedActivity = investingHistoryData.FirstOrDefault(x => x.Key == selectedRow).Value[2];
            var selectedProduct = investingHistoryData.FirstOrDefault(x => x.Key == selectedRow).Value[3];
            var selectedInstitution = investingHistoryData.FirstOrDefault(x => x.Key == selectedRow).Value[4];
            var selectedQuantity = investingHistoryData.FirstOrDefault(x => x.Key == selectedRow).Value[5];
            var selectedUnityPrice = investingHistoryData.FirstOrDefault(x => x.Key == selectedRow).Value[6];
            var selectedOperationValue = investingHistoryData.FirstOrDefault(x => x.Key == selectedRow).Value[7];

            var historyFile = new InvestmentHistory
            {
                QueueId = queueInId,
                Applicable = GetApplicable(selectedApplicable),
                Date = DateTime.Parse(activityDate, culture),
                TrnasactionType = GetInvestmentActivityTypeByDescription(selectedActivity),
                Product = selectedProduct,
                Institution = selectedInstitution,
                Quantity = ConvertToInt(selectedQuantity),
                UnitPrice = ConvertToDecimal(selectedUnityPrice),
                OperationValue = ConvertToDecimal(selectedOperationValue)
            };

            if (historyFile.TrnasactionType == InvestingTrnasactionType.Unknown)
            {
                var errorMsg = $"Erro ao identificar tipo de atividade {selectedActivity}, para o produto {selectedProduct} na data {historyFile.Date}";
                historyFile.AddNotification("InvestingHistory.ActivityType", errorMsg);
            }

            return historyFile;
        }

        public static IEnumerable<InvestmentHistory> CreateRange(Dictionary<int, List<dynamic>> investmentHistoryData, Guid queueInId, CultureInfo culture)
        {
            var investmentHistoryList = new List<InvestmentHistory>();
            for (int selectedRow = 0; selectedRow < investmentHistoryData.Count; selectedRow++)
            {
                var investingHistory = Create(investmentHistoryData, selectedRow, queueInId, culture);
                investmentHistoryList.Add(investingHistory);
            }
            return investmentHistoryList;
        }

        private static InvestingTrnasactionType GetInvestmentActivityTypeByDescription(string? description)
        {
            var enumValues = Enum.GetValues(typeof(InvestingTrnasactionType));

            foreach (var enumValue in enumValues)
            {
                if (enumValue is InvestingTrnasactionType activityType)
                {
                    var enumDescription = GetEnumDescription(activityType);

                    if (enumDescription == description)
                        return activityType;
                }
            }

            return InvestingTrnasactionType.Unknown;
        }

        private static string GetEnumDescription(Enum enumValue)
        {
            var descriptionAttribute = enumValue.GetType()
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

            return descriptionAttribute?.Description ?? enumValue.ToString();
        }

        private static Applicable GetApplicable(string value)
        {
            if (value == "Credito")
            {
                return Applicable.In;
            }
            else
            {
                return Applicable.Out;
            }
        }

        private static int ConvertToInt(dynamic value)
        {
            var stringValue = Convert.ToString(value);

            var isInt = int.TryParse(stringValue, out int result);
            if (!isInt)
                return 0;

            return result;
        }

        private static decimal ConvertToDecimal(dynamic value)
        {
            var stringValue = Convert.ToString(value);

            var isDecimal = decimal.TryParse(stringValue, out decimal result);
            if (!isDecimal)
                return decimal.Zero;

            return result;
        }
    }
}
