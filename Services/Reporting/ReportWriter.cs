using DataAccessLayer.DTO;
using System.Text;

namespace Services.Reporting
{
    public static class ReportWriter
    {
        public static async Task WriteReportAsync(string country, DateTime from, DateTime to, List<SuspiciousTransactionDTO> transactions)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var fileName = $"{country}_{from:yyyyMMdd}_{to:yyyyMMdd}.txt";
            var path = Path.Combine(dir, fileName);

            using var writer = new StreamWriter(path, false);

            await writer.WriteLineAsync($"Suspicious Transactions Report - {country}");
            await writer.WriteLineAsync($"Period: {from:yyyy-MM-dd} to {to:yyyy-MM-dd}");
            await writer.WriteLineAsync($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}");
            await writer.WriteLineAsync(new string('-', 50));

            foreach (var tx in transactions)
            {
                await writer.WriteLineAsync($"Customer: {tx.CustomerName} (ID: {tx.CustomerId})");
                await writer.WriteLineAsync($"Account: {tx.AccountId} | Transaction: {tx.TransactionId}");
                await writer.WriteLineAsync($"Amount: {tx.Amount:C} | Date: {tx.Date:yyyy-MM-dd} | Reason: {tx.Reason}");
                await writer.WriteLineAsync(new string('-', 50));
            }
        }
    }
}
