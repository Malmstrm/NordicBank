using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Mappings;

namespace AntiMoneyLaundry
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Setup DI
            var services = new ServiceCollection();

            services.AddDbContext<NordicBankAppDataContext>(options =>
                options.UseSqlServer("Server=localhost;Database=BankAppData;Trusted_Connection=True;TrustServerCertificate=True"));

            services.AddAutoMapper(typeof(EntityToDTOProfile));

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
