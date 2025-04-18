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
                    ScanDisplay.ShowSavedScanDatesAsync(_scanService);
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
                .Title("[yellow]Select country to scan (or Cancel):[/]")
                .AddChoices("Sweden", "Finland", "Denmark", "Norway", "Cancel"));

        if (country == "Cancel")
        {
            AnsiConsole.MarkupLine("[grey]Scan cancelled.[/]");
            return;
        }

        var startDate = await _scanService.GetLastScanDateAsync(country);
        if (startDate == default)
            startDate = await _scanService.GetEarliestTransactionDateAsync(country);

        AnsiConsole.MarkupLine($"[blue]Scanning will start from:[/] [bold]{startDate:yyyy-MM-dd}[/]");

        var endDate = PromptForDate(startDate, DateTime.Today);
        if (endDate == null)
        {
            AnsiConsole.MarkupLine("[grey]Scan cancelled.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"\n[bold underline green]Scan Summary[/]");
        AnsiConsole.MarkupLine($"[bold]Country:[/] [blue]{country}[/]");
        AnsiConsole.MarkupLine($"[bold]Start date:[/] {startDate:yyyy-MM-dd}");
        AnsiConsole.MarkupLine($"[bold]End date:[/] {endDate:yyyy-MM-dd}");
        AnsiConsole.MarkupLine("\n[grey]Press ENTER to start scanning...[/]");
        Console.ReadLine();

        var result = await AnsiConsole.Status()
            .StartAsync("Scanning transactions...", async ctx =>
            {
                var result = await _scanService.RunScanAsync(country, startDate, endDate.Value);
                ctx.Status("Scan complete!");
                ctx.Spinner(Spinner.Known.Star);
                ctx.SpinnerStyle(Style.Parse("green"));
                return result;
            });

        AnsiConsole.MarkupLine($"\n[bold green]Scan completed![/]");
        AnsiConsole.MarkupLine($"Country: [blue]{result.Country}[/]");
        AnsiConsole.MarkupLine($"From: [blue]{result.StartDate:yyyy-MM-dd}[/] To: [blue]{result.EndDate:yyyy-MM-dd}[/]");
        AnsiConsole.MarkupLine($"Suspicious Transactions: [red]{result.SuspiciousTransactions.Count}[/]");

        if (AnsiConsole.Confirm("Do you want to view flagged transactions now?"))
        {
            ScanDisplay.PrintSuspicious(result.SuspiciousTransactions);
        }
    }

    public static DateTime? PromptForDate(DateTime earliest, DateTime latest)
    {
        var years = Enumerable.Range(earliest.Year, latest.Year - earliest.Year + 1).ToList();
        years.Add(-1); // Cancel option (special value)

        var year = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("[yellow]Select year (or Cancel):[/]")
                .AddChoices(years)
                .UseConverter(i => i == -1 ? "Cancel" : i.ToString()));

        if (year == -1) return null;

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
            return PromptForDate(earliest, latest); // Retry
        }

        return selectedDate;
    }

}
