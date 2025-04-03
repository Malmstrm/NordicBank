namespace DataAccessLayer.DTO
{
    public class TransactionDTO
    {
        public DateOnly Date { get; set; }
        public string Type { get; set; } = "";
        public string Operation { get; set; } = "";
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string? Description { get; set; }
        public int AccountId { get; set; }
    }
}
