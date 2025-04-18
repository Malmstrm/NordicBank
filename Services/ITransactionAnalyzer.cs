using DataAccessLayer.DTO;

namespace Services
{
    public interface ITransactionAnalyzer
    {
        Task<DateTime> GetEarliestTransactionDateAsync(string country);
        Task<List<SuspiciousTransactionDTO>> GetSuspiciousTransactionsAsync(string country, DateTime from, DateTime to);
    }

}
