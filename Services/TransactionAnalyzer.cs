using DataAccessLayer.Data;
using DataAccessLayer.Enums;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class TransactionAnalyzer : ITransactionAnalyzer
    {
        private readonly NordicBankAppDataContext _dbContext;

        public TransactionAnalyzer(NordicBankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SuspiciousTransaction>> AnalyzeAsync(string country, DateTime start, DateTime end)
        {
            var result = new List<SuspiciousTransaction>();

            var customers = await _dbContext.Customers
                .Where(c => c.Country == country)
                .ToListAsync();

            foreach (var customer in customers)
            {
                var accounts = await _dbContext.Dispositions
                    .Where(d => d.CustomerId == customer.CustomerId && d.Type == "OWNER")
                    .Include(d => d.Account)
                    .ToListAsync();

                foreach (var disposition in accounts)
                {
                    var transactions = await _dbContext.Transactions
                        .Where(t => t.AccountId == disposition.AccountId)
                        .OrderBy(t => t.Date)
                        .ToListAsync();

                    foreach (var tx in transactions)
                    {
                        var txDate = tx.Date.ToDateTime(TimeOnly.MinValue);
                        if (txDate <= start || txDate > end) continue;

                        // Rule 1
                        if (tx.Amount > 15000)
                        {
                            result.Add(CreateSuspicious(customer, disposition.AccountId, tx, SuspicionReason.HighAmount));
                        }

                        // Rule 2
                        var windowSum = transactions
                            .Where(t =>
                                t.Date.ToDateTime(TimeOnly.MinValue) >= txDate.AddHours(-72) &&
                                t.Date.ToDateTime(TimeOnly.MinValue) <= txDate)
                            .Sum(t => t.Amount);

                        if (windowSum > 23000)
                        {
                            result.Add(CreateSuspicious(customer, disposition.AccountId, tx, SuspicionReason.WindowSum));
                        }
                    }
                }
            }

            return result;
        }

        public async Task<DateTime> GetEarliestTransactionDateAsync(string country)
        {
            var earliest = await _dbContext.Transactions
                .Where(t =>
                    _dbContext.Dispositions.Any(d => d.AccountId == t.AccountId && d.Customer.Country == country))
                .OrderBy(t => t.Date)
                .Select(t => t.Date)
                .FirstOrDefaultAsync();

            return earliest.ToDateTime(TimeOnly.MinValue);
        }

        private SuspiciousTransaction CreateSuspicious(Customer customer, int accountId, Transaction tx, SuspicionReason reason)
        {
            return new SuspiciousTransaction
            {
                CustomerId = customer.CustomerId,
                CustomerName = $"{customer.Givenname} {customer.Surname}",
                AccountId = accountId,
                TransactionId = tx.TransactionId,
                Amount = tx.Amount,
                Date = tx.Date.ToDateTime(TimeOnly.MinValue),
                Reason = reason
            };
        }
    }
}
