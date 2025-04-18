using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace AntiMoneyLaundry
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Setup DI
            var services = new ServiceCollection();

            services.AddDbContext<NordicBankAppDataContext>(options =>
                options.UseSqlServer("Server=localhost;Database=NordicBankAppData;Trusted_Connection=True;TrustServerCertificate=True"));

            services.AddScoped<IAntiMoneyLaunderingService, AntiMoneyLaunderingService>();
            services.AddScoped<ITransactionAnalyzer, TransactionAnalyzer>();
            services.AddScoped<IScanLogRepository, ScanLogRepository>();
            services.AddScoped<IScanResultFactory, ScanResultFactory>();

            var serviceProvider = services.BuildServiceProvider();

            var menu = new ConsoleMenu(serviceProvider);
            await menu.RunAsync();
        }
    }
}
