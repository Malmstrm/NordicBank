using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class CustomerService : ICustomerService
    {
        private readonly NordicBankAppDataContext _dbContext;
        public CustomerService(NordicBankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CustomerDTO?> GetByIdAsyn(int customerId)
        {
            return await _dbContext.Customers
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CustomerDTO()
                {
                    CustomerId = c.CustomerId,
                    Gender = c.Gender,
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    Streetaddress = c.Streetaddress,
                    City = c.City,
                    Zipcode = c.Zipcode,
                    Country = c.Country,
                    CountryCode = c.CountryCode,
                    Birthday = c.Birthday,
                    NationalId = c.NationalId,
                    Telephonecountrycode = c.Telephonecountrycode,
                    Telephonenumber = c.Telephonenumber,
                    Emailaddress = c.Emailaddress,
                    Status = c.CustomerStatus
                })
                .FirstOrDefaultAsync();
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
