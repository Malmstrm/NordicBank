using DataAccessLayer.Models;

namespace Services
{
    public interface ITransactionAnalyzer
    {
        Task<List<SuspiciousTransaction>> GetSuspiciousTransactionsAsync(string country, DateTime startDate, DateTime endDate);
        Task<DateTime> GetEarliestTransactionDateAsync(string country);
    }
}
