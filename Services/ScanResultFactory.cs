using DataAccessLayer.DTO;

namespace Services;

public class ScanResultFactory : IScanResultFactory
{
    public ScanResultDTO Create(DateTime startDate, DateTime endDate, string country, List<SuspiciousTransactionDTO> transactions)
    {
        return new ScanResultDTO
        {
            Country = country,
            StartDate = startDate,
            EndDate = endDate,
            SuspiciousCount = transactions.Count,
            SuspiciousTransactions = transactions
        };
    }
}
