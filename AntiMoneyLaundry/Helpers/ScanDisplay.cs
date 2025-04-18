using Spectre.Console;
using DataAccessLayer.DTO;
using Services;

namespace AntiMoneyLaundry;

public static class ScanDisplay
{
    public static async Task ShowSavedScanDatesAsync(IAntiMoneyLaunderingService scanService)
    {
        var country = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Select country to view scan history:[/]")
                .AddChoices("Sweden", "Finland", "Denmark", "Norway"));

        var history = await scanService.GetScanHistoryAsync(country);

        if (history.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No scan history found for this country.[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Start Date")
            .AddColumn("End Date")
            .AddColumn("Suspicious Count")
            .AddColumn("Logged At");

        foreach (var item in history)
        {
            table.AddRow(
                item.StartDate.ToString("yyyy-MM-dd"),
                item.EndDate.ToString("yyyy-MM-dd"),
                item.SuspiciousCount.ToString(),
                item.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
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
                tx.Reason == "HighAmount" ? "[red]HighAmount[/]" : "[orange1]WindowSum[/]");
        }

        AnsiConsole.Write(table);
    }
}
