using DataAccessLayer.DTO;

namespace Services
{
    public interface IScanResultFactory
    {
        ScanResultDTO Create(DateTime startDate, DateTime endDate, string country, List<SuspiciousTransactionDTO> transactions);
    }
}
