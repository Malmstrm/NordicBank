using DataAccessLayer.Data;
using DataAccessLayer.DTO;
using DataAccessLayer.Enums;
using DataAccessLayer.Models;
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

        public async Task<CustomerDTO> CreateAsync(CustomerDTO dto)
        {
            var customer = new Customer
            {
                Gender = dto.Gender,
                Givenname = dto.Givenname,
                Surname = dto.Surname,
                Streetaddress = dto.Streetaddress,
                City = dto.City,
                Zipcode = dto.Zipcode,
                Country = dto.Country,
                CountryCode = dto.CountryCode,
                Birthday = dto.Birthday,
                NationalId = dto.NationalId,
                Telephonecountrycode = dto.Telephonecountrycode,
                Telephonenumber = dto.Telephonenumber,
                Emailaddress = dto.Emailaddress,
                CustomerStatus = DataAccessLayer.Enums.CustomerStatus.Active,
            };

            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();

            var account = new Account()
            {
                Frequency = "Monthly",
                Created = DateOnly.FromDateTime(DateTime.Now),
                Balance = 0,
                AccountStatus = AccountStatus.Inactive,
                Dispositions = new List<Disposition>()
                {
                    new Disposition()
                    {
                        CustomerId = customer.CustomerId,
                        Type = "Owner"
                    }
                }
            };

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();


            dto.CustomerId = customer.CustomerId;
            return dto;
        }
        public async Task<bool> UpdateStatusAsync(int customerId, CustomerStatus newStatus)
        {
            var customer = await _dbContext.Customers.FindAsync(customerId);
            if (customer == null) return false;

            customer.CustomerStatus = newStatus;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<CustomerDTO> UpdateAsync(CustomerDTO dto)
        {
            var customer = await _dbContext.Customers.FindAsync(dto.CustomerId);
            if (customer == null) return null!;

            customer.Gender = dto.Gender;
            customer.Givenname = dto.Givenname;
            customer.Surname = dto.Surname;
            customer.Streetaddress = dto.Streetaddress;
            customer.City = dto.City;
            customer.Zipcode = dto.Zipcode;
            customer.Country = dto.Country;
            customer.CountryCode = dto.CountryCode;
            customer.Birthday = dto.Birthday;
            customer.NationalId = dto.NationalId;
            customer.Telephonecountrycode = dto.Telephonecountrycode;
            customer.Telephonenumber = dto.Telephonenumber;
            customer.Emailaddress = dto.Emailaddress;
            customer.CustomerStatus = dto.Status;

            await _dbContext.SaveChangesAsync();

            return dto;
        }
        public async Task<bool> DeleteAsync(int customerId)
        {
            var customer = await _dbContext.Customers.FindAsync(customerId);
            if (customer == null) return false;

            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();

            return true;
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
