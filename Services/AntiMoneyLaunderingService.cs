using DataAccessLayer.DTO;

namespace Services.MoneyLaundryService
{
    public class AntiMoneyLaunderingService : IAntiMoneyLaunderingService
    {
        private readonly ITransactionAnalyzer _transactionAnalyzer;
        private readonly IScanLogRepository _scanLogRepository;
        private readonly IScanResultFactory _scanResultFactory;

        public AntiMoneyLaunderingService(
            ITransactionAnalyzer transactionAnalyzer,
            IScanLogRepository scanLogRepository,
            IScanResultFactory scanResultFactory)
        {
            _transactionAnalyzer = transactionAnalyzer;
            _scanLogRepository = scanLogRepository;
            _scanResultFactory = scanResultFactory;
        }

        public async Task<ScanResultDTO> RunScanAsync(string country, DateTime endDate)
        {
            var startDate = await _scanLogRepository.LoadLastScanDateAsync(country);
            if (startDate == DateTime.MinValue)
                startDate = await GetEarliestTransactionDateAsync(country);

            var suspiciousTransactions = await _transactionAnalyzer.GetSuspiciousTransactionsAsync(country, startDate, endDate);

            var scanLog = await _scanLogRepository.SaveScanLogAsync(country, startDate, endDate, suspiciousTransactions);

            return _scanResultFactory.Create(scanLog, suspiciousTransactions);
        }

        public async Task<DateTime> GetEarliestTransactionDateAsync(string country)
        {
            return await _transactionAnalyzer.GetEarliestTransactionDateAsync(country);
        }
    }
}
