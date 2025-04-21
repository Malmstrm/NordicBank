using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;
        public CustomerService(NordicBankAppDataContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<CustomerDTO> CreateAsync(CustomerDTO dto)
        {

            var entity = _mapper.Map<Customer>(dto);
            entity.CustomerStatus = CustomerStatus.Active;


            _dbContext.Customers.Add(entity);
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
                        CustomerId = entity.CustomerId,
                        Type = "Owner"
                    }
                }
            };

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CustomerDTO>(entity);
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
            var entity = await _dbContext.Customers.FindAsync(dto.CustomerId);
            if (entity == null) return null!;

            _mapper.Map(dto, entity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CustomerDTO>(entity);
        }
        public async Task<bool> DeleteAsync(int customerId)
        {
            var customer = await _dbContext.Customers.FindAsync(customerId);
            if (customer == null) return false;

            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<CustomerDTO?> GetByIdAsync(int customerId)
        {
            var entity = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
            return entity == null ? null : _mapper.Map<CustomerDTO>(entity);
        }

        public async Task<List<ViewCustomerDTO>> GetViewAsync()
        {
            return await _dbContext.Customers
                .ProjectTo<ViewCustomerDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
