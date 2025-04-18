using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Services;
public class ScanLogRepository : IScanLogRepository
{
    private readonly NordicBankAppDataContext _db;

    public ScanLogRepository(NordicBankAppDataContext db)
    {
        _db = db;
    }

    public async Task<DateTime> GetLastScanDateAsync(string country)
    {
        return await _db.ScanLogs
            .Where(x => x.Country == country)
            .OrderByDescending(x => x.EndDate)
            .Select(x => x.EndDate)
            .FirstOrDefaultAsync();
    }

    public async Task SaveScanLogAsync(string country, DateTime from, DateTime to, List<SuspiciousTransaction> transactions)
    {
        var log = new ScanLog
        {
            Country = country,
            StartDate = from,
            EndDate = to,
            SuspiciousCount = transactions.Count,
            CreatedAt = DateTime.UtcNow,
            SuspiciousTransactions = transactions
        };

        _db.ScanLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}
