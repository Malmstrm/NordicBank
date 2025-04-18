using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class TransactionAnalyzer : ITransactionAnalyzer
{
    private readonly NordicBankAppDataContext _db;

    public TransactionAnalyzer(NordicBankAppDataContext db)
    {
        _db = db;
    }

    public async Task<DateTime> GetEarliestTransactionDateAsync(string country)
    {
        return await _db.Transactions
            .Where(t => _db.Dispositions
                .Any(d => d.AccountId == t.AccountId && d.Customer.Country == country))
            .OrderBy(t => t.Date)
            .Select(t => t.Date)
            .FirstOrDefaultAsync()
            .ContinueWith(t => t.Result.ToDateTime(TimeOnly.MinValue));
    }

    public async Task<List<SuspiciousTransactionDTO>> GetSuspiciousTransactionsAsync(string country, DateTime from, DateTime to)
    {
        var transactions = await _db.Transactions
            .Where(t => t.Date >= DateOnly.FromDateTime(from) && t.Date <= DateOnly.FromDateTime(to))
            .Include(t => t.AccountNavigation)
            .ThenInclude(a => a.Dispositions)
            .ThenInclude(d => d.Customer)
            .ToListAsync();

        var suspicious = new List<SuspiciousTransactionDTO>();

        foreach (var tx in transactions)
        {
            var customer = tx.AccountNavigation.Dispositions.FirstOrDefault(d => d.Type == "OWNER")?.Customer;
            if (customer == null || customer.Country != country)
                continue;

            var txDate = tx.Date.ToDateTime(TimeOnly.MinValue);

            if (tx.Amount > 15000)
            {
                suspicious.Add(new SuspiciousTransactionDTO
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.Givenname + " " + customer.Surname,
                    AccountId = tx.AccountId,
                    TransactionId = tx.TransactionId,
                    Amount = tx.Amount,
                    Date = txDate,
                    Reason = "HighAmount"
                });
            }

            var recentWindowSum = transactions
                .Where(t => t.AccountId == tx.AccountId)
                .Where(t => t.Date.ToDateTime(TimeOnly.MinValue) >= txDate.AddHours(-72) && t.Date.ToDateTime(TimeOnly.MinValue) <= txDate)
                .Sum(t => t.Amount);

            if (recentWindowSum > 23000)
            {
                suspicious.Add(new SuspiciousTransactionDTO
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.Givenname + " " + customer.Surname,
                    AccountId = tx.AccountId,
                    TransactionId = tx.TransactionId,
                    Amount = recentWindowSum,
                    Date = txDate,
                    Reason = "WindowSum"
                });
            }
        }

        return suspicious;
    }
}
