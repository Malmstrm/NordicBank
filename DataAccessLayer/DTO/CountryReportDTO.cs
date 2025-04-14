namespace DataAccessLayer.DTO
{
    public class CountryReportDTO
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public int Clients { get; set; }
        public int Accounts { get; set; }
        public decimal Capital { get; set; }
        public decimal AverageBalance => Accounts > 0 ? Capital / Accounts : 0;
        public string? TopCustomerName { get; set; }
        public decimal? TopCustomerBalance { get; set; }
    }
}
