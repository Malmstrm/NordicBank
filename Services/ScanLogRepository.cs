using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Services;
public class ScanLogRepository : IScanLogRepository
{
    private readonly NordicBankAppDataContext _db;
    private readonly IMapper _mapper;

    public ScanLogRepository(NordicBankAppDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
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
    public async Task<List<ScanHistoryDTO>> GetScanHistoryAsync(string country)
    {
        return await _db.ScanLogs
            .Where(x => x.Country == country)
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<ScanHistoryDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
    public async Task<List<ScanLog>> GetScanWithTransactionsAsync(string country, DateTime from, DateTime to)
    {
        return await _db.ScanLogs
            .Include(s => s.SuspiciousTransactions)
            .Where(s => s.Country == country && s.StartDate >= from && s.EndDate <= to)
            .ToListAsync();
    }

}
