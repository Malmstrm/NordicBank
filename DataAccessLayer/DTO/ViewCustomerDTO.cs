using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class ViewCustomerDTO
    {
        public int CustomerId { get; set; }
        public string Givenname { get; set; }
        public string Streetaddress { get; set; }
        public string City { get; set; }
        public string? NationalId { get; set; }
        public CustomerStatus Status { get; set; }
    }
}
