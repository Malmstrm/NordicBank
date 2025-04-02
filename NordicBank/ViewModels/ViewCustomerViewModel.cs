using DataAccessLayer.Enums;

namespace NordicBank.ViewModels
{
    public class ViewCustomerViewModel
    {
        public int CustomerId { get; set; }
        public string Givenname { get; set; }
        public string Streetaddress { get; set; }
        public string City { get; set; }
        public string? NationalId { get; set; }
        public CustomerStatus Status { get; set; }
    }
}
