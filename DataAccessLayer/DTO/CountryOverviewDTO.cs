using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class CountryOverviewDTO
    {
        public string Country {  get; set; }
        public string CountryCode { get; set; }
        public int Clients { get; set; }
        public int Accounts { get; set; }
        public decimal Capital { get; set; }
    }
}
