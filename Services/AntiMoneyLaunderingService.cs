using DataAccessLayer.DTO;
using DataAccessLayer.Enums;
using DataAccessLayer.Models;


namespace Services
{
    public class AntiMoneyLaunderingService : IAntiMoneyLaunderingService
    {
        private readonly ITransactionAnalyzer _analyzer;
        private readonly IScanLogRepository _scanLogRepo;
        private readonly IScanResultFactory _resultFactory;
        private readonly ITransactionAnalyzer _transactionAnalyzer;

        public AntiMoneyLaunderingService(ITransactionAnalyzer analyzer, IScanLogRepository scanLogRepo, IScanResultFactory resultFactory, ITransactionAnalyzer transactionAnalyzer)
        {
            _analyzer = analyzer;
            _scanLogRepo = scanLogRepo;
            _resultFactory = resultFactory;
            _transactionAnalyzer = transactionAnalyzer;
        }

        public async Task<ScanResultDTO> RunScanAsync(string country, DateTime endDate)
        {
            var startDate = await _scanLogRepo.GetLastScanDateAsync(country);
            if (startDate == DateTime.MinValue)
                startDate = await _analyzer.GetEarliestTransactionDateAsync(country);

            var suspiciousDtos = await _analyzer.GetSuspiciousTransactionsAsync(country, startDate, endDate);

            var suspiciousModels = suspiciousDtos.Select(dto => new SuspiciousTransaction
            {
                CustomerId = dto.CustomerId,
                CustomerName = dto.CustomerName ?? "",
                AccountId = dto.AccountId,
                TransactionId = dto.TransactionId,
                Amount = dto.Amount,
                Date = dto.Date,
                Reason = dto.Reason == "HighAmount" ? SuspicionReason.HighAmount : SuspicionReason.WindowSum
            }).ToList();

            await _scanLogRepo.SaveScanLogAsync(country, startDate, endDate, suspiciousModels);

            return _resultFactory.Create(startDate, endDate, country, suspiciousDtos);
        }
        public async Task<DateTime> GetEarliestTransactionDateAsync(string country)
        {
            return await _transactionAnalyzer.GetEarliestTransactionDateAsync(country);
        }
    }
}