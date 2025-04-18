using Microsoft.Extensions.DependencyInjection;
using Services;
using Spectre.Console;

namespace AntiMoneyLaundry;

public class ConsoleMenu
{
    private readonly IAntiMoneyLaunderingService _scanService;

    public ConsoleMenu(IServiceProvider provider)
    {
        _scanService = provider.GetRequiredService<IAntiMoneyLaunderingService>();
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            AnsiConsole.Write(new FigletText("Money Laundry").Centered().Color(Color.Green));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Select an option:[/]")
                    .AddChoices("Start scanning", "View scan history", "Exit"));

            switch (choice)
            {
                case "Start scanning":
                    await StartScanAsync();
                    break;
                case "View scan history":
                    ScanDisplay.ShowSavedScanDates();
                    break;
                case "Exit":
                    running = false;
                    break;
            }

            if (running)
            {
                AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
                Console.ReadKey();
            }
        }
    }

    private async Task StartScanAsync()
    {
        var country = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select country to scan:[/]")
                .AddChoices("Sweden", "Finland", "Denmark", "Norway"));

        // Hämta senaste scannade datum eller fall tillbaka till första transaktion
        var startDate = await _scanService.GetLastScanDateAsync(country);
        if (startDate == default)
            startDate = await _scanService.GetEarliestTransactionDateAsync(country);

        AnsiConsole.MarkupLine($"[blue]Scanning will start from:[/] [bold]{startDate:yyyy-MM-dd}[/]");

        var endDate = PromptForDate(startDate, DateTime.Today); // Din metod för att välja datum med piltangenter

        var result = await _scanService.RunScanAsync(country, startDate, endDate);

        AnsiConsole.MarkupLine($"\n[bold green]Scan completed![/]");
        AnsiConsole.MarkupLine($"Country: [blue]{result.Country}[/]");
        AnsiConsole.MarkupLine($"From: [blue]{result.StartDate:yyyy-MM-dd}[/] To: [blue]{result.EndDate:yyyy-MM-dd}[/]");
        AnsiConsole.MarkupLine($"Suspicious Transactions: [red]{result.SuspiciousTransactions.Count}[/]");

        ScanDisplay.PrintSuspicious(result.SuspiciousTransactions);
    }


    public static DateTime PromptForDate(DateTime earliest, DateTime latest)
    {
        var year = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("[yellow]Select year:[/]")
                .AddChoices(Enumerable.Range(earliest.Year, latest.Year - earliest.Year + 1).ToList()));

        var month = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("[yellow]Select month:[/]")
                .AddChoices(Enumerable.Range(1, 12).ToList()));

        var daysInMonth = DateTime.DaysInMonth(year, month);
        var day = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("[yellow]Select day:[/]")
                .AddChoices(Enumerable.Range(1, daysInMonth).ToList()));

        var selectedDate = new DateTime(year, month, day);

        if (selectedDate < earliest || selectedDate > latest)
        {
            AnsiConsole.MarkupLine("[red]Selected date is outside the allowed range.[/]");
            return PromptForDate(earliest, latest); // rekursivt om fel
        }

        return selectedDate;
    }

}
