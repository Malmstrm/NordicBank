using Spectre.Console;
using DataAccessLayer.DTO;

namespace AntiMoneyLaundry;

public static class ScanDisplay
{
    public static void ShowSavedScanDates()
    {
        var dir = "Progress";
        if (!Directory.Exists(dir))
        {
            AnsiConsole.MarkupLine("[red]No scan history found.[/]");
            return;
        }

        var files = Directory.GetFiles(dir, "*_LastChecked.txt");
        if (files.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]No scan timestamps found.[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("Country");
        table.AddColumn("Last Checked");

        foreach (var file in files)
        {
            var country = Path.GetFileNameWithoutExtension(file).Replace("_LastChecked", "");
            var content = File.ReadAllText(file);
            table.AddRow(country, content);
        }

        AnsiConsole.Write(table);
    }

    public static void PrintSuspicious(List<SuspiciousTransactionDTO> transactions)
    {
        if (!transactions.Any())
        {
            AnsiConsole.MarkupLine("[green]No suspicious transactions found.[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("CustomerId");
        table.AddColumn("AccountId");
        table.AddColumn("TransactionId");
        table.AddColumn("Amount");
        table.AddColumn("Date");
        table.AddColumn("Reason");

        foreach (var tx in transactions)
        {
            table.AddRow(
                tx.CustomerId.ToString(),
                tx.AccountId.ToString(),
                tx.TransactionId.ToString(),
                tx.Amount.ToString("C"),
                tx.Date.ToString("yyyy-MM-dd"),
                tx.Reason);
        }

        AnsiConsole.Write(table);
    }
}
