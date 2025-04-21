using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Models;

namespace Services.Mappings
{
    public class EntityToDTOProfile : Profile
    {

        public EntityToDTOProfile()
        {
            // Customer
            CreateMap<Customer, CustomerDTO>().ReverseMap();

            // Account
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<Account, AccountSummaryDTO>().ReverseMap();

            // Transaction
            CreateMap<Transaction, TransactionDTO>().ReverseMap();

            // Others
            CreateMap<SuspiciousTransaction, SuspiciousTransactionDTO>().ReverseMap();
            CreateMap<ScanLog, ScanHistoryDTO>().ReverseMap();
        }

    }
}
