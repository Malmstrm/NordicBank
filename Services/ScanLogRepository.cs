using DataAccessLayer.Data;
using DataAccessLayer.Models;

namespace Services
{
    public class ScanLogRepository : IScanLogRepository
    {
        private readonly NordicBankAppDataContext _dbContext;

        public ScanLogRepository(NordicBankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DateTime> LoadLastScanDateAsync(string country)
        {
            var path = $"Progress/{country}_LastChecked.txt";

            if (!File.Exists(path))
                return DateTime.MinValue;

            var content = await File.ReadAllTextAsync(path);
            return DateTime.TryParse(content, out var parsed) ? parsed : DateTime.MinValue;
        }

        public async Task<ScanLog> SaveScanLogAsync(string country, DateTime start, DateTime end, List<SuspiciousTransaction> transactions)
        {
            var log = new ScanLog
            {
                Country = country,
                StartDate = start,
                EndDate = end,
                SuspiciousCount = transactions.Count,
                CreatedAt = DateTime.UtcNow,
                SuspiciousTransactions = transactions
            };

            _dbContext.ScanLogs.Add(log);
            await _dbContext.SaveChangesAsync();

            SaveLastScanDate(country, end);

            return log;
        }

        private void SaveLastScanDate(string country, DateTime endDate)
        {
            Directory.CreateDirectory("Progress");
            File.WriteAllText($"Progress/{country}_LastChecked.txt", endDate.ToString("O"));
        }
    }
}
