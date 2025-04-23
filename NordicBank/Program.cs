using DataAccessLayer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Mappings;
using NordicBank.MappingProfiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<NordicBankAppDataContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<NordicBankAppDataContext>();
builder.Services.AddRazorPages();

builder.Services.AddAutoMapper(typeof(EntityToDTOProfile));
builder.Services.AddAutoMapper(typeof(WebMappingProfile));


builder.Services.AddTransient<DataInitializer>();

builder.Services.AddTransient<ICountryOverviewService, CountryOverviewService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<ITransactionService, TransactionService>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddTransient<IAntiMoneyLaunderingService, AntiMoneyLaunderingService>();
builder.Services.AddTransient<ITransactionAnalyzer, TransactionAnalyzer>();
builder.Services.AddTransient<IScanLogRepository, ScanLogRepository>();
builder.Services.AddTransient<IScanResultFactory, ScanResultFactory>();


var app = builder.Build();

// Behövs för Azure!
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.
         GetRequiredService<NordicBankAppDataContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
}
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<NordicBankAppDataContext>();
//    try
//    {
//        if (dbContext.Database.IsRelational())
//        {
//            dbContext.Database.Migrate();
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("🔥 Migration error: " + ex.Message);
//        // Du kan även skriva till en loggfil om du vill se mer
//    }
//}

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<DataInitializer>().MigrateData();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
