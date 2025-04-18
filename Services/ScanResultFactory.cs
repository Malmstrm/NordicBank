using DataAccessLayer.DTO;
using DataAccessLayer.Models;

namespace Services
{
    public class ScanResultFactory : IScanResultFactory
    {
        public ScanResultDTO Create(ScanLog log, List<SuspiciousTransaction> transactions)
        {
            return new ScanResultDTO
            {
                Country = log.Country,
                StartDate = log.StartDate,
                EndDate = log.EndDate,
                SuspiciousCount = transactions.Count,
                Transactions = transactions.Select(t => new SuspiciousTransactionDTO
                {
                    CustomerId = t.CustomerId,
                    CustomerName = t.CustomerName,
                    AccountId = t.AccountId,
                    TransactionId = t.TransactionId,
                    Amount = t.Amount,
                    Date = t.Date,
                    Reason = t.Reason.ToString()
                }).ToList()
            };
        }
    }
}
