namespace Services.Utility
{
    public class TransactionResult
    {
        public bool Success{ get; set; }
        public string? Message { get; set; }

        public static TransactionResult Failed(string? message) => new TransactionResult { Success = false, Message = message };
        public static TransactionResult Ok() => new TransactionResult { Success = true };
    }
}
