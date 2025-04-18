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

        var earliest = await _scanService.GetEarliestTransactionDateAsync(country);

        AnsiConsole.MarkupLine($"[blue]Earliest transaction for {country} is:[/] [bold]{earliest:yyyy-MM-dd}[/]");

        var endDate = AnsiConsole.Prompt(
            new TextPrompt<DateTime>("[green]Enter end date (yyyy-MM-dd):[/]")
                .Validate(date => date >= earliest && date <= DateTime.Today
                    ? ValidationResult.Success()
                    : ValidationResult.Error("Date must be within the valid range."))
        );

        var result = await _scanService.RunScanAsync(country, endDate);

        AnsiConsole.MarkupLine($"\n[bold green]Scan completed![/]");
        AnsiConsole.MarkupLine($"Country: [blue]{result.Country}[/]");
        AnsiConsole.MarkupLine($"From: [blue]{result.StartDate:yyyy-MM-dd}[/] To: [blue]{result.EndDate:yyyy-MM-dd}[/]");
        AnsiConsole.MarkupLine($"Suspicious Transactions: [red]{result.SuspiciousTransactions.Count}[/]");

        ScanDisplay.PrintSuspicious(result.SuspiciousTransactions);
    }
}
