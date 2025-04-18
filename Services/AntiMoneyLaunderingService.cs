using DataAccessLayer.DTO;
using DataAccessLayer.Enums;
using DataAccessLayer.Models;

namespace Services
{
    public class AntiMoneyLaunderingService : IAntiMoneyLaunderingService
    {
        private readonly ITransactionAnalyzer _transactionAnalyzer;
        private readonly IScanLogRepository _scanLogRepo;
        private readonly IScanResultFactory _resultFactory;

        public AntiMoneyLaunderingService(
            ITransactionAnalyzer transactionAnalyzer,
            IScanLogRepository scanLogRepo,
            IScanResultFactory resultFactory)
        {
            _transactionAnalyzer = transactionAnalyzer;
            _scanLogRepo = scanLogRepo;
            _resultFactory = resultFactory;
        }

        public async Task<ScanResultDTO> RunScanAsync(string country, DateTime startDate, DateTime endDate)
        {
            var suspiciousDtos = await _transactionAnalyzer.GetSuspiciousTransactionsAsync(country, startDate, endDate);

            var suspiciousModels = suspiciousDtos.Select(dto => new SuspiciousTransaction
            {
                CustomerId = dto.CustomerId,
                CustomerName = dto.CustomerName ?? "",
                AccountId = dto.AccountId,
                TransactionId = dto.TransactionId,
                Amount = dto.Amount,
                Date = dto.Date,
                Reason = dto.Reason == "HighAmount"
                    ? SuspicionReason.HighAmount
                    : SuspicionReason.WindowSum
            }).ToList();

            await _scanLogRepo.SaveScanLogAsync(country, startDate, endDate, suspiciousModels);

            return _resultFactory.Create(startDate, endDate, country, suspiciousDtos);
        }

        public async Task<DateTime> GetEarliestTransactionDateAsync(string country)
        {
            return await _transactionAnalyzer.GetEarliestTransactionDateAsync(country);
        }
        public async Task<DateTime> GetLastScanDateAsync(string country)
        {
            return await _scanLogRepo.GetLastScanDateAsync(country);
        }
        public async Task<List<ScanHistoryDTO>> GetScanHistoryAsync(string country)
        {
            return await _scanLogRepo.GetScanHistoryAsync(country);
        }
    }
}
