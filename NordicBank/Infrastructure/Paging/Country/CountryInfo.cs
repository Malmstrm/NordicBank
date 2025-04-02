namespace NordicBank.Infrastructure.Paging.Country
{
    public class CountryInfo
    {
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string PhoneCode { get; set; }

        public static List<CountryInfo> All => new List<CountryInfo>
    {
        new() { Name = "Sweden", CountryCode = "SE", PhoneCode = "+46" },
        new() { Name = "Finland", CountryCode = "FI", PhoneCode = "+358" },
        new() { Name = "Denmark", CountryCode = "DK", PhoneCode = "+45" },
        new() { Name = "Norway", CountryCode = "NO", PhoneCode = "+47" }
    };
    }
}
