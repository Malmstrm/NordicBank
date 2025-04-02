using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CustomerService:ICustomerService
    {
        private readonly NordicBankAppDataContext _dbContext;
        public CustomerService(NordicBankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<ViewCustomerDTO>> GetViewAsync()
        {
            return await _dbContext.Customers
                .Select(c => new ViewCustomerDTO()
                {
                    CustomerId = c.CustomerId,
                    Givenname = c.Givenname,
                    Streetaddress = c.Streetaddress,
                    City = c.City,
                    NationalId = c.NationalId,
                    Status = c.CustomerStatus

                }).ToListAsync();
        }
    }
}
